using projectApiAngular.DTO;
using static projectApiAngular.DTO.UserDto;

namespace projectApiAngular.Services
{
    public interface ILotteryService
    {
        Task<IEnumerable<ReadUserDto>> RunLottery();
        Task<List<GiftWinnerDto>> GetAllGiftWinners();

        Task<int> StartNewChineseSale();

        Task<ReadUserDto?> RunLottery(string giftName);


    }
}