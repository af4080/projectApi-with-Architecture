
using Microsoft.Extensions.Options;
using projectApiAngular.Configurations;
using projectApiAngular.DTO;
using projectApiAngular.Models;
using projectApiAngular.Repositories;
using static projectApiAngular.DTO.GiftDto;

namespace projectApiAngular.Services
{
    public class GiftService : IGiftService
    {
        private const string GiftListCacheKey = "gifts:all";

        private readonly IGiftRepository _repository;
        private readonly ILogger<GiftService> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly TimeSpan _giftListCacheTtl;

        public GiftService(
            IGiftRepository repository,
            ILogger<GiftService> logger,
            IRedisCacheService cacheService,
            IOptions<RedisSettings> redisSettings)
        {
            _repository = repository;
            _logger = logger;
            _cacheService = cacheService;
            var ttlSeconds = redisSettings.Value.GiftCacheTtlSeconds;
            _giftListCacheTtl = TimeSpan.FromSeconds(ttlSeconds > 0 ? ttlSeconds : 60);
        }

        private static ReadGiftDto MapToReadDto(Gift g)
        {
            return new ReadGiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Price = g.Price,
                ImagePath = g.ImagePath,
                CategoryId = g.CategoryId,
                CategoryName = g.Category?.Name ?? "",
                DonerId = g.DonerId,
                DonerName = g.Doner.Name
            };
        }


        //get
        public async Task<IEnumerable<ReadGiftDto>> GetAllGifts()
        {
            var cachedGifts = await _cacheService.GetAsync<List<ReadGiftDto>>(GiftListCacheKey);
            if (cachedGifts != null && cachedGifts.Count > 0)
            {
                _logger.LogInformation("Returned {Count} gifts from Redis cache.", cachedGifts.Count);
                return cachedGifts;
            }

            var gifts = await _repository.GetAllGifts();
            var dtos = gifts.Select(d => MapToReadDto(d)).ToList();

            await _cacheService.SetAsync(GiftListCacheKey, dtos, _giftListCacheTtl);
            _logger.LogInformation("Cached {Count} gifts in Redis for {TtlSeconds} seconds.", dtos.Count, _giftListCacheTtl.TotalSeconds);
            return dtos;
        }

        //get by name
        public async Task<ReadGiftDto?> GetGiftByName(string name)
        {

            var g = await _repository.GetGiftByName(name);
            if (g == null) return null;
            _logger.LogInformation("Retrieved gift: {GiftName} from the repository.", g.Name);
            return MapToReadDto(g);
        }
        //get by doner
        public async Task<IEnumerable<ReadGiftDto?>> GetGiftByDonnerName(string name)
        {
            var g = await _repository.GetGiftByDonnerName(name);
            _logger.LogInformation("Retrieved {Count} gifts for doner: {DonerName} from the repository.", g.Count(), name);

            return g.Select(d => MapToReadDto(d));
        }
        //get by num customer
        public async Task<IEnumerable<ReadGiftDto?>> GetbyNumCustomer(int count)
        {
            var g = await _repository.GetbyNumCustomer(count);
            _logger.LogInformation("Retrieved {Count} gifts for customers count: {CustomerCount} from the repository.", g.Count(), count);
            return g.Select(d => MapToReadDto(d));
        }
        //post
        public async Task<ReadGiftDto> AddGift(CreateGiftDto gift)
        {
            _logger.LogInformation("Adding a new gift: {GiftName} to the repository.", gift.Name);
            var entity = new Gift
            {
                Name = gift.Name,
                Price = gift.Price,
                CategoryId = gift.CategoryId,
                ImagePath = gift.ImagePath,
                Description = gift.Description,
                DonerId = gift.DonerId
            };

            var createdGift = await _repository.AddGift(entity);
            await _cacheService.RemoveAsync(GiftListCacheKey);
            _logger.LogInformation("Successfully added gift: {GiftName} with ID: {GiftId} to the repository.", createdGift.Name, createdGift.Id);

            return MapToReadDto(createdGift);


        }

        //update
        public async Task<ReadGiftDto?> UpdateGift(string name, UpdateGiftDto dto)
        {
            _logger.LogInformation("Updating gift: {GiftName} in the repository.", name);
            var existingGift = await _repository.GetGiftByName(name);
            if (existingGift == null)
                return null;

            if (dto.Name != null)
                existingGift.Name = dto.Name;

            if (dto.Price.HasValue)
                existingGift.Price = dto.Price.Value;

            if (dto.CategoryId.HasValue)
                existingGift.CategoryId = dto.CategoryId.Value;
            if (dto.ImagePath != null)
                existingGift.ImagePath = dto.ImagePath;

            if (dto.Description != null)
                existingGift.Description = dto.Description;

            if (existingGift.WinnerId != null)
                throw new InvalidOperationException("Cannot update gift after lottery");


            // DonerId לא משתנה

            var updated = await _repository.UpdateGift(existingGift);
            if (updated == null)
                return null;

            await _cacheService.RemoveAsync(GiftListCacheKey);
            _logger.LogInformation("Successfully updated gift: {GiftName} in the repository.", updated.Name);
            return MapToReadDto(updated);

        }


        //delete
        public async Task<ReadGiftDto?> DeleteGift(int id)
        {
            _logger.LogWarning("Deleting gift with ID: {GiftId} from the repository.", id);
            var del = await _repository.DeleteGift(id);
            if (del == null) return null;

            await _cacheService.RemoveAsync(GiftListCacheKey);
            _logger.LogWarning("Successfully deleted gift: {GiftName} with ID: {GiftId} from the repository.", del.Name, del.Id);
            return MapToReadDto(del);


        }
        //get winner by gift id
        public async Task<string?> GetWinnerByGiftId(int giftId)
        {
           
            var gift = await _repository.GetWinnerByGiftId(giftId);
            _logger.LogInformation("get winner for gift {giftId}", giftId);
            return gift;   

        }


        //pagination
        public async Task<PagedResponse<ReadGiftDto>> GetPagedGifts(int pageNumber, int pageSize)
        {
            var (gifts, totalCount) = await _repository.GetPagedGiftsAsync(pageNumber, pageSize);

            var dtos = gifts.Select(g => MapToReadDto(g));

            _logger.LogInformation("Retrieved page {PageNumber} of gifts.", pageNumber);

            return new PagedResponse<ReadGiftDto>(dtos, pageNumber, pageSize, totalCount);
        }
    }
}
