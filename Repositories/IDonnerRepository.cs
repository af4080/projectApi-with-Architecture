using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public interface IDonnerRepository
    {
        Task<Donner> AddDonner(Donner donner);
        Task<Donner?> DeleteDonner(int id);
        Task<IEnumerable<Donner>> GetAllDonners();
        Task<Donner?> GetDonnerByEmail(string email);
        Task<Donner?> GetDonnerByGiftId(int giftId);
        Task<Donner?> GetDonnerById(int id);
        Task<Donner?> GetDonnerByName(string name);
        Task<Donner?> UpdateDonner(Donner donner);
    }
}