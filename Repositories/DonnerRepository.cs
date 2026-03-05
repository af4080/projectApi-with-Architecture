using Microsoft.EntityFrameworkCore;
using projectApiAngular.Data;
using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public class DonnerRepository: IDonnerRepository
    {
        private readonly Chinese_SalesDbContext _context;
        public DonnerRepository(Chinese_SalesDbContext context)
        {
            _context = context;
        }
        //get
        public async Task<IEnumerable<Donner>> GetAllDonners()
        {

            return await _context.Doners.ToListAsync();
        }

        //get by id
        public async Task<Donner?> GetDonnerById(int id)
        {
            return await _context.Doners
                .Include(d => d.Gifts)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        //get by name
        public async Task<Donner?> GetDonnerByName(string name)
        {
            return await _context.Doners
                .FirstOrDefaultAsync(p => p.Name == name);
        }
        //get by email
        public async Task<Donner?> GetDonnerByEmail(string email)
        {
            return await _context.Doners
                .FirstOrDefaultAsync(p => p.Email == email);
        }
        //get by gift id
        public async Task<Donner?> GetDonnerByGiftId(int giftId)
        {
            return await _context.Doners
                .Include(d => d.Gifts)
             .FirstOrDefaultAsync(d => d.Gifts != null && d.Gifts.Any(g => g.Id == giftId));

        }
        //add donner
        public async Task<Donner> AddDonner(Donner donner)
        {
            _context.Doners.Add(donner);
            await _context.SaveChangesAsync();
            return donner;
        }
        //update donner
        public async Task<Donner?> UpdateDonner( Donner donner)
        {
          
            await _context.SaveChangesAsync();
            return donner;
        }
        //delete donner
        public async Task<Donner?> DeleteDonner(int id)
        {
            var existingDonner = await _context.Doners.FindAsync(id);
            if (existingDonner == null)
            {
                return null;
            }
            _context.Doners.Remove(existingDonner);
            await _context.SaveChangesAsync();
            return existingDonner;
        }
    }


}
