using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public interface IGiftRepository
    {
        Task<Gift> AddGift(Gift gift);
        Task<Gift?> DeleteGift(int id);
        Task<IEnumerable<Gift>> GetAllGifts();
        Task<IEnumerable<Gift?>> GetbyNumCustomer(int count);
        Task<IEnumerable<Gift?>> GetGiftByDonnerName(string name);
        Task<Gift?> GetGiftByName(string name);
        Task<Gift?> UpdateGift( Gift gift);

        Task<User?> UpdateGiftWinner(string name, int winnerId);
        Task<string?> GetWinnerByGiftId(int giftId);
       Task<int?> StartNewChineseSale();
        Task<(IEnumerable<Gift> Gifts, int TotalCount)> GetPagedGiftsAsync(int pageNumber, int pageSize);

    }
}