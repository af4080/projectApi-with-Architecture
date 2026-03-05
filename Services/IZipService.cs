using projectApiAngular.DTO;

namespace projectApiAngular.Services
{
    public interface IZipService
    {
        void CreateCsvFile(List<GiftWinnerDto> giftWinners, string csvFilePath);
        void CreateZipFile(string csvFilePath, string zipFilePath);
    }
}