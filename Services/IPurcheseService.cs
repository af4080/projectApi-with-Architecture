using projectApiAngular.DTO;

namespace projectApiAngular.Services
{
    public interface IPurchaseService
    {
        Task<PurcheseDto.ReadPurcheseDto> AddPurchaseAsync(PurcheseDto.CreatePurcheseDto dto);
        Task<IEnumerable<PurcheseDto.ReadPurcheseDto>> GetBuyersDetails();
        Task<IEnumerable<PurcheseDto.ReadPurcheseDto>> GetGiftsSortedBySalesAsync();
        Task<IEnumerable<PurcheseDto.ReadPurcheseDto>> GetPurchasesByGiftAsync(string name);
        Task<IEnumerable<PurcheseDto.ReadPurcheseDto>> GetPurchasesOrderedByPriceAsync();
        Task<decimal> GetTotalSalesRevenue();

    }
}


