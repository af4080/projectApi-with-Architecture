using projectApiAngular.DTO;
using static projectApiAngular.DTO.GiftDto;

namespace projectApiAngular.Services
{
    public interface IGiftService
    {
        Task<ReadGiftDto> AddGift(CreateGiftDto gift);
        Task<ReadGiftDto?> DeleteGift(int id);
        Task<IEnumerable<ReadGiftDto>> GetAllGifts();
        Task<IEnumerable<ReadGiftDto?>> GetbyNumCustomer(int count);
        Task<IEnumerable<ReadGiftDto?>> GetGiftByDonnerName(string name);
        Task<ReadGiftDto?> GetGiftByName(string name);
        Task<ReadGiftDto?> UpdateGift(string name, UpdateGiftDto gift);
        Task<PagedResponse<ReadGiftDto>> GetPagedGifts(int pageNumber, int pageSize);
        Task<string?> GetWinnerByGiftId(int giftId);
    }
}