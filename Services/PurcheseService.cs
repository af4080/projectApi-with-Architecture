using projectApiAngular.DTO;
using projectApiAngular.Models;
using projectApiAngular.Repositories;
using static projectApiAngular.DTO.PurcheseDto;

namespace projectApiAngular.Services
{
    public class PurcheseServicecs : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ILogger<PurcheseServicecs> _logger;
        private readonly IGiftRepository _giftRepository;
        public PurcheseServicecs(IPurchaseRepository purchaseRepository, ILogger<PurcheseServicecs> logger, IGiftRepository giftRepository)
        {
            _purchaseRepository = purchaseRepository;
            _logger = logger;
            _giftRepository = giftRepository;
        }

        private static ReadPurcheseDto Map(Purchase p) =>
         new ReadPurcheseDto
         {
             Id = p.Id,
             CustomerId = p.CustomerId,
             CustomerName = p.Castomer?.Name,
             CustomerEmail = p.Castomer?.Email,
             GiftId = p.GiftId,
             GiftName = p.Gift?.Name,
             GiftPrice = p.Gift?.Price,
             PurchDate = p.PurchaseDate
         };
        //add purchase
        public async Task<ReadPurcheseDto> AddPurchaseAsync(CreatePurcheseDto dto)
        {
            _logger.LogInformation("Adding a new purchase for CustomerId: {CustomerId}, GiftId: {GiftId}", dto.CustomerId, dto.GiftId);
            try
            {

                if (dto.PurchDate > DateTime.Now)
                    throw new ArgumentException("Purchase date cannot be in the future.");

                var winnerName = await _giftRepository.GetWinnerByGiftId(dto.GiftId);
                if (winnerName != null)
                    throw new GiftAlreadyAssignedException(winnerName);

                var entity = new Purchase
                {
                    CustomerId = dto.CustomerId,
                    GiftId = dto.GiftId,
                    PurchaseDate = dto.PurchDate
                };

                var saved = await _purchaseRepository.AddPurchase(entity);
                _logger.LogInformation("Purchase added successfully with Id: {PurchaseId}", saved.Id);
                return Map(saved);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new purchase for CustomerId: {CustomerId}, GiftId: {GiftId}", dto.CustomerId, dto.GiftId);
                throw new Exception( ex.Message);
            }


        }
        //get buyers details
        public async Task<IEnumerable<ReadPurcheseDto>> GetBuyersDetails()
        {
            var items = await _purchaseRepository.GetBuyersDetails();
            _logger.LogInformation("Retrieved {Count} purchase records with buyer details.", items.Count());
            return items.Select(Map);
        }
        //get by gift
        public async Task<IEnumerable<ReadPurcheseDto>> GetPurchasesByGiftAsync(string name)
        {
            var items = await _purchaseRepository.GetPurchasesByGiftAsync(name);
            _logger.LogInformation("Retrieved {Count} purchase records for gift: {GiftName}.", items.Count(), name);
            return items.Select(Map);
        }
        //sort by sellings
        public async Task<IEnumerable<ReadPurcheseDto>> GetGiftsSortedBySalesAsync()
        {
            var items = await _purchaseRepository.GetGiftsSortedBySalesAsync();
            _logger.LogInformation("Retrieved {Count} gifts sorted by sales.", items.Count());
            return items.Select(Map);
        }
        //get purchases ordered by price
        public async Task<IEnumerable<ReadPurcheseDto>> GetPurchasesOrderedByPriceAsync()
        {
            var items = await _purchaseRepository.GetPurchasesOrderedByPriceAsync();
            _logger.LogInformation("Retrieved {Count} purchases ordered by price.", items.Count());
            return items.Select(Map);
        }
        public async Task<decimal> GetTotalSalesRevenue()
        {
            _logger.LogInformation("Calculating total sales revenue.");

            var purchases = await _purchaseRepository.GetAll(); // יש לוודא שיש פונקציה כזו במאגר
            if (purchases == null || !purchases.Any())
            {
                _logger.LogWarning("No purchases found.");
                return 0;
            }

            decimal totalRevenue = purchases.Sum(purchase => purchase.Gift.Price);  // נניח שיש שדה בשם Amount
            return totalRevenue;
        }

    }
}
