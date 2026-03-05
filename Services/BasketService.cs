using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using projectApiAngular.Models;
using projectApiAngular.Repositories;
using System.Security.Claims;
using static projectApiAngular.DTO.BasketDto;
using static projectApiAngular.DTO.GiftDto;
using static projectApiAngular.DTO.PurcheseDto;
using static projectApiAngular.DTO.UserDto;

namespace projectApiAngular.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BasketService> _logger;
        private readonly IPurchaseService _purchaseService;
        private readonly IGiftRepository _giftRepository;
        public BasketService(IBasketRepository basketRepository, IHttpContextAccessor httpContextAccessor, ILogger<BasketService> logger,IPurchaseService purchaseService, IGiftRepository giftRepository)
        {
            _basketRepository = basketRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _purchaseService = purchaseService;
            _giftRepository = giftRepository;

        }
        //GetCurrentUserId

        private int GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity!.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userIdClaim = user.FindFirst("id");
            if (userIdClaim == null)
                throw new Exception("User Id claim missing");

            return int.Parse(userIdClaim.Value);
        }


        //map to dto

        private static ReadBasketDto Map(Basket b)
        {

            return new ReadBasketDto
            {
                Id = b.Id,
                Amount = b.Amount,
                UserId = b.UserId,
                User = new ReadUserDto
                {
                    Id = b.User.Id,
                    Name = b.User.Name,
                    Email = b.User.Email,
                    Phone = b.User.Phone,
                    Role = Role.user.GetDisplayName()

                },
                GiftId = b.GiftId,
                Gift = new ReadGiftDto
                {
                    Name = b.Gift.Name,
                    Description = b.Gift.Description,
                    Id = b.Gift.Id,
                    Price = b.Gift.Price,
                    ImagePath = b.Gift.ImagePath,
                    CategoryName = b.Gift.Category.Name,
                    DonerName = b.Gift.Doner.Name,
                    DonerId = b.Gift.DonerId,
                    CategoryId = b.Gift.CategoryId

                }
            };
        }



        //get my basket
        public async Task<IEnumerable<ReadBasketDto>> GetMyBasket()
        {
            int userId = GetCurrentUserId();
            _logger.LogInformation(
         "Fetching basket for user {UserId}",userId);

            var baskets = await _basketRepository.GetMyBasket(userId);
            _logger.LogInformation("User {UserId} has {Count} basket items", userId, baskets.Count());
            return baskets.Select(Map);
        }


        //EnterToBasketAsync
        public async Task<ReadBasketDto?> EnterToBasketAsync(CreateBasketDto basketDto)
        {
            int userId = GetCurrentUserId();
            _logger.LogInformation(
            "User {UserId} adding gift {GiftId} amount {Amount} to basket",
            userId,
            basketDto.GiftId,
            basketDto.Amount
);

            var winnerName = await _giftRepository.GetWinnerByGiftId(basketDto.GiftId);
            if (winnerName != null)
            {
                throw new GiftAlreadyAssignedException(winnerName);
            }

            var entity = new Basket
            {
                UserId = userId,
                GiftId = basketDto.GiftId,
                Amount = basketDto.Amount
            };

            var basket = await _basketRepository.EnterToBasketAsync(entity);
            _logger.LogInformation(
            "Basket item {BasketId} created for user {UserId}",
            basket.Id,
            userId
);
            
            return  new ReadBasketDto {  Id=basket.Id ,Amount=basket.Amount , GiftId =basket.GiftId ,UserId=userId };
        }

        //update amount
        public async Task<ReadBasketDto?> UpdateBasketAmountAsync(int id, int newAmount)
        {
            if (newAmount <= 0 || newAmount > 1000)
            {
                _logger.LogWarning(
                "Invalid basket amount {Amount} for basket {BasketId}",
                newAmount,
                id);
                throw new ArgumentException("Amount must be greater than zero and cannot exceed 1000.");
            }
            var basket = await _basketRepository.UpdateBasketAmountAsync(id, newAmount);
            if (basket == null)
            {
                        _logger.LogWarning(
             "Basket {BasketId} not found for update", id);
                return null;
            }
            _logger.LogInformation(
             "Basket {BasketId} updated to amount {Amount}",id,newAmount);
            return Map(basket);
        }
        //delete basket
        public async Task<int?> DeleteBasketAsync(int id)
        {
            _logger.LogInformation(
           "Deleting basket {BasketId}",id);
            var basket = await _basketRepository.DeleteBasketAsync(id);
            if (basket == null)
            {
                _logger.LogWarning(
                "Basket {BasketId} not found for deletion", id );
                return null;
            }
            _logger.LogInformation(
            "Basket {BasketId} deleted successfully",id);
            return basket.Id;
        }
        //puorchese
        public async Task<bool> BuyAllBasket()
        {
            int userId = GetCurrentUserId();
            _logger.LogInformation("Starting purchase process for user {UserId}", userId);

            var baskets = await _basketRepository.GetMyBasket(userId);
            if(!baskets.Any())
            {
                _logger.LogWarning("No basket item founds for user {userId} to puorchese",userId);
                return false;
            }
            using var transaction = await _basketRepository.BeginTransactionAsync();
            try
            {
                foreach (var basket in baskets)
                {
                    for (int i = 0; i < basket.Amount; i++)
                    {
                        var p = await _purchaseService.AddPurchaseAsync(new CreatePurcheseDto
                        {
                            CustomerId = userId,
                            GiftId = basket.GiftId,
                            PurchDate = DateTime.Now
                        });
                        if (p == null) throw new Exception($"Fail to create purchase for gift {basket.GiftId}");

                    }
                    var deletedId = await DeleteBasketAsync(basket.Id);
                    if (deletedId == null) throw new Exception($"Fail to delete basket item {basket.Id}");
                }
                await transaction.CommitAsync();
                _logger.LogInformation("All basket items purchased and cleared successfully for {userId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Purchase process failed for user {userId}. Transaction rolled back.", userId);
                return false;
            }

        }
    }
}
