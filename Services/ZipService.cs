using CsvHelper;
using projectApiAngular.DTO;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace projectApiAngular.Services
{
    public class ZipService : IZipService
    {
        public void CreateZipFile(string csvFilePath, string zipFilePath)
        {
            using (var zipToCreate = new FileStream(zipFilePath, FileMode.Create))
            using (var archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
            {
                // יצירת הערך ה-ZIP לקובץ CSV
                var zipEntry = archive.CreateEntry(Path.GetFileName(csvFilePath), CompressionLevel.Fastest);
                using (var zipStream = zipEntry.Open())
                using (var fileStream = new FileStream(csvFilePath, FileMode.Open))
                {
                    fileStream.CopyTo(zipStream);
                }
            }
        }

        public void CreateCsvFile(List<GiftWinnerDto> giftWinners, string csvFilePath)
        {
            using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // כותב את הרשומות (כל הנתונים)
                csv.WriteRecords(giftWinners);
            }
        }
    }
}
