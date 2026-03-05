
using projectApiAngular.DTO;
using projectApiAngular.Models;
using projectApiAngular.Repositories;
using static projectApiAngular.DTO.GiftDto;

namespace projectApiAngular.Services
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _repository;
        private readonly ILogger<GiftService> _logger;  

        public GiftService(IGiftRepository repository,ILogger<GiftService> logger)
        {
            _repository = repository;
            _logger = logger;
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
            var gifts = await _repository.GetAllGifts();
            var dtos = gifts.Select(d => MapToReadDto(d));
            _logger.LogInformation("Retrieved {Count} gifts from the repository.", dtos.Count());
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
            _logger.LogInformation("Successfully updated gift: {GiftName} in the repository.", updated.Name);
            return MapToReadDto(updated);

        }


        //delete
        public async Task<ReadGiftDto?> DeleteGift(int id)
        {
            _logger.LogWarning("Deleting gift with ID: {GiftId} from the repository.", id);
            var del = await _repository.DeleteGift(id);
            if (del == null) return null;
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
