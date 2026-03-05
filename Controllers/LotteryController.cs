using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectApiAngular.DTO;
using projectApiAngular.Services;
using static projectApiAngular.DTO.UserDto;

namespace projectApiAngular.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LotteryController : ControllerBase
    {
        private readonly ILotteryService _lotteryService;
        private readonly IZipService _zipService;

        public LotteryController(ILotteryService lotteryService, IZipService zipService)
        {
            _lotteryService = lotteryService;
            _zipService = zipService;
        }
        [HttpPost]
        public async Task<IActionResult> RunLottery()
        {
            try
            {
                var result = await _lotteryService.RunLottery();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
     
            [HttpPost("{giftName}")]
            public async Task<IActionResult> RunLotteryForGift(string giftName)
            {
                try
                {
                    var result = await _lotteryService.RunLottery(giftName);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        [HttpGet]
        public async Task<IActionResult> GetAllGiftWinners()
        {
            try
            {
                var result = await _lotteryService.GetAllGiftWinners();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("newSale")]
        public async Task<IActionResult> StartNewChineseSale()
        {
            try
            {
                await _lotteryService.StartNewChineseSale();
                return Ok("New sale started successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("downled-winner-zip")]
        public async Task<IActionResult> DownloadGiftWinnersAsZip()
        {

            var giftWinners = await _lotteryService.GetAllGiftWinners(); // קבלת רשימת הזוכים

            if (giftWinners == null || !giftWinners.Any())

                return NotFound("No winners data to download.");


            // יצירת קובץ CSV
            var csvFileName = "gift_winners.csv";
            var csvFilePath = Path.Combine(Path.GetTempPath(), csvFileName);
            _zipService.CreateCsvFile(giftWinners, csvFilePath);

            // יצירת קובץ ZIP
            var zipFileName = "gift_winners.zip";
            var zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);
            _zipService.CreateZipFile(csvFilePath, zipFilePath);

            // מחיקת הקובץ CSV הזמני
            System.IO.File.Delete(csvFilePath);

            // קריאת קובץ ה-ZIP לבייטים
            var fileBytes = await System.IO.File.ReadAllBytesAsync(zipFilePath);

            // שליחת קובץ ה-ZIP להורדה
            return File(fileBytes, "application/zip", zipFileName);
        }
    }
}
