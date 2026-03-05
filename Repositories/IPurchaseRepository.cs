using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase> AddPurchase(Purchase purchase);
        Task<IEnumerable<Purchase>> GetBuyersDetails();
        Task<IEnumerable<Purchase>> GetGiftsSortedBySalesAsync();
        Task<IEnumerable<Purchase>> GetPurchasesByGiftAsync(string name);
        Task<IEnumerable<Purchase>> GetPurchasesOrderedByPriceAsync();
        Task<IEnumerable<Purchase>> GetAll();
    }
}