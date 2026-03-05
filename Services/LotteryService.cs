using CsvHelper;
using projectApiAngular.DTO;
using projectApiAngular.Models;
using projectApiAngular.Repositories;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using static projectApiAngular.DTO.UserDto;

namespace projectApiAngular.Services
{
    public class LotteryService : ILotteryService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IGiftRepository _giftRepository;
        private readonly ILogger<LotteryService> _logger;
        private readonly IUserRepository _userRepository;
        private static int countLotteries = 0;
        public LotteryService(IPurchaseRepository purchaseRepository, IGiftRepository giftRepository, ILogger<LotteryService> logger, IUserRepository userRepository)
        {
            _purchaseRepository = purchaseRepository;
            _giftRepository = giftRepository;
            _logger = logger;
            _userRepository = userRepository;
            _userRepository = userRepository;
        }
        public async Task<IEnumerable<ReadUserDto>> RunLottery()
        {
            _logger.LogInformation("Starting lottery run number {countLotteries}", countLotteries);
            List<ReadUserDto?> winners = new List<ReadUserDto?>();
            var gifts = await _giftRepository.GetAllGifts();
            foreach (var gift in gifts)
            {

                int? winnerId = await GetWinnerOfGift(gift.Name);
                if (winnerId == null)
                {
                    _logger.LogWarning("No winner selected for gift:{GiftName} because there were no purchases.",gift.Name);
                    continue;
                }

                var winner = await _giftRepository.UpdateGiftWinner(gift.Name, winnerId.Value);

                if (winner == null)
                    throw new InvalidOperationException("Error updating gift winner.");

                winners.Add(new ReadUserDto
                {
                    Id = winner.Id,
                    Name = winner.Name,
                    Email = winner.Email,
                    Phone = winner.Phone,
                });

            }

            return winners;
        }

        private async Task<int?> GetWinnerOfGift(string giftName)
        {
            _logger.LogInformation("Selecting winner for gift: {GiftName}", giftName);
            var purchases = await _purchaseRepository.GetPurchasesByGiftAsync(giftName);
            if (purchases == null || !purchases.Any())
                return null;
            var randomIndex = new Random().Next(0, purchases.Count());
            var winnerId = purchases.ElementAt(randomIndex).CustomerId;
            _logger.LogInformation("Winner selected: CustomerId {WinnerId} for gift: {GiftName}", winnerId, giftName);
            return winnerId;

        }
        public async Task<List<GiftWinnerDto>> GetAllGiftWinners()
        {
            _logger.LogInformation("Fetching all gifts and their winners.");

            // קבלת כל המתנות מהמאגר
            var gifts = await _giftRepository.GetAllGifts();  // יש לוודא שזו פונקציה חוקית במאגר
            if (gifts == null || !gifts.Any())
            {
                _logger.LogWarning("No gifts found.");
                throw new InvalidOperationException("No gifts found.");
            }

            var giftWinners = new List<GiftWinnerDto>();


            foreach (var gift in gifts)
            {
                if (gift.WinnerId != null)
                {
                    // אם המתנה כבר יש לה זוכה, מקבלים את פרטי הזוכה
                    var winner = await _userRepository.GetUserById(gift.WinnerId.Value);  // יש לוודא שיש פונקציה כזו במאגר
                    if (winner == null)
                    {
                        _logger.LogWarning("winner with id {winnerId} not found for gift {giftName}.", gift.WinnerId, gift.Name);
                        throw new InvalidOperationException($"winner with id {gift.WinnerId.Value} not found for gift {gift.Name}");
                    }
                    giftWinners.Add(new GiftWinnerDto
                    {
                        GiftName = gift.Name,
                        WinnerName = winner.Name,
                        WinnerEmail = winner.Email,
                        WinnerPhone = winner.Phone
                    });
                }
            }

            return giftWinners;
        }


        public async Task<int> StartNewChineseSale()
        {
            _logger.LogInformation("Starting new lottery round");
            await _giftRepository.StartNewChineseSale();
            return ++countLotteries;
        }

        public async Task<ReadUserDto?> RunLottery(string giftName)
        {

            _logger.LogInformation("Starting lottery for gift: {GiftName}", giftName);
            var gift = await _giftRepository.GetGiftByName(giftName);
            if (gift == null)
            {
                _logger.LogWarning("Gift with name {GiftName} not found.", giftName);
                throw new ArgumentException($"Gift with name {giftName} does not exist.");
            }
            if (gift.WinnerId != null)
            {
                _logger.LogWarning("Lottery already executed for gift: {GiftName}", giftName);
                throw new InvalidOperationException("Lottery already executed for this gift.");
            }



            var winnerId = await GetWinnerOfGift(gift.Name);
            if (winnerId == null)
            {
                return null;
            }
            var winner = await _giftRepository.UpdateGiftWinner(gift.Name, winnerId.Value);
            if (winner == null)
            {
                _logger.LogWarning("Failed to update winner for gift: {GiftName}", giftName);
                throw new InvalidOperationException("Error updating gift winner.");
            }
            return new ReadUserDto
            {
                Id = winner.Id,
                Name = winner.Name,
                Email = winner.Email,
                Phone = winner.Phone,
            };
        

        }
    }



}