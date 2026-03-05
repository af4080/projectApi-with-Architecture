using projectApiAngular.DTO;
using static projectApiAngular.DTO.DonnerDto;

namespace projectApiAngular.Services
{
    public interface IDonnerService
    {
        Task<ReadDonnerDto> AddDonner(CreateDonnerDto dto);
        Task<ReadDonnerDto?> DeleteDonner(int id);
        Task<IEnumerable<ReadDonnerDto>> GetAllDonners();
        Task<ReadDonnerDto?> GetDonnerByEmail(string email);
        Task<ReadDonnerDto?> GetDonnerByGiftId(int giftId);
        Task<ReadDonnerDto?> GetDonnerById(int id);
        Task<ReadDonnerDto?> GetDonnerByName(string name);
        Task<ReadDonnerDto?> UpdateDonner(int id, UpdateDonnerDto dto);



    }
}