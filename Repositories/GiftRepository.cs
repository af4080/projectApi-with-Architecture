using Microsoft.EntityFrameworkCore;
using projectApiAngular.Data;
using projectApiAngular.Models;
using static projectApiAngular.Repositories.GiftRepository;

namespace projectApiAngular.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        private readonly Chinese_SalesDbContext _context;

        public GiftRepository(Chinese_SalesDbContext context)
        {
            _context = context;
        }

        //get
        public async Task<IEnumerable<Gift>> GetAllGifts()
        {
            return await _context.Gifts.Include(g => g.Category).Include(d => d.Doner).ToListAsync();
        }
        //get by name
        public async Task<Gift?> GetGiftByName(string name)
        {
            return await _context.Gifts.Include(name => name.Category).Include(d => d.Doner)
                .FirstOrDefaultAsync(p => p.Name == name);
        }
        //get by doner
        public async Task<IEnumerable<Gift?>> GetGiftByDonnerName(string name)
        {
            return await _context.Gifts.Include(d => d.Doner).Include(n => n.Category)
                .Where(p => p.Doner.Name == name).ToListAsync();
        }
        //get by num customer
        public async Task<IEnumerable<Gift?>> GetbyNumCustomer(int count)
        {
            return await _context.Gifts
                 .Include(g => g.Purchases)
                 .Include(g => g.Category)
                 .Include(g => g.Doner)
                 .Where(g => g.Purchases.Count == count)
                 .ToListAsync();

        }

        //post

        public async Task<Gift> AddGift(Gift gift)
        {
            if (!await _context.Categories.AnyAsync(c => c.Id == gift.CategoryId))
                throw new ArgumentException($"Category with id {gift.CategoryId} does not exist.");

            if (!await _context.Doners.AnyAsync(d => d.Id == gift.DonerId))
                throw new ArgumentException($"Doner with id {gift.DonerId} does not exist.");
            if (await _context.Gifts.AnyAsync(g => g.Name == gift.Name))
                throw new ArgumentException($"A gift with the name '{gift.Name}' already exists.");

            _context.Gifts.Add(gift);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error saving Gift to database. See inner exception for details.", ex);
            }

            await _context.Entry(gift).Reference(g => g.Category).LoadAsync();
            await _context.Entry(gift).Reference(g => g.Doner).LoadAsync();

            return gift;
        }
        //update
        public async Task<Gift?> UpdateGift(Gift gift)
        {
            // Repository assumes entity is already tracked and validated

            await _context.SaveChangesAsync();
            return gift;
        }


        //delete
        public async Task<Gift?> DeleteGift(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null)
                return null;

            bool hasPurchases = await _context.Purchases
                .AnyAsync(p => p.GiftId == id);

            if (hasPurchases)
                throw new InvalidOperationException(
                    "Cannot delete gift that has purchases");
            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();
            await _context.Entry(gift).Reference(g => g.Category).LoadAsync();
            await _context.Entry(gift).Reference(g => g.Doner).LoadAsync();

            return gift;
        }
        //get winner name by gift id
        public async Task<string?> GetWinnerByGiftId(int giftId)
        {
            var gift = await _context.Gifts
                .Include(g => g.Winner)
                .FirstOrDefaultAsync(g => g.Id == giftId);
            return gift?.Winner?.Name;
        }

        //update winner
        public async Task<User?> UpdateGiftWinner(string name, int winnerId)
        {
            var existingGift = await _context.Gifts
           .FirstOrDefaultAsync(g => g.Name == name);
            if (existingGift == null)
            {
                return null;
            }
            if (existingGift.WinnerId != null)
            {
                throw new InvalidOperationException("it's immposible to do duplicate lotteries");
            }
            existingGift.WinnerId = winnerId;
            await _context.SaveChangesAsync();
            var winner = await _context.Users.FindAsync(winnerId);
            return winner;
        }

        //start a new chinese_sale
        public async Task<int?> StartNewChineseSale()
        {
            var gifts = await _context.Gifts.ToListAsync();
            foreach (var gift in gifts)
            {
                gift.WinnerId = null;
            }
            await _context.SaveChangesAsync();
            return null;
        }




        public async Task<(IEnumerable<Gift> Gifts, int TotalCount)> GetPagedGiftsAsync(int pageNumber, int pageSize)
        {
            var query = _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Doner)
                .AsNoTracking();

            // שליפת סך כל הרשומות
            var totalCount = await query.CountAsync();

            // ביצוע העימוד ללא תגיות מיותרות
            var gifts = await query
                .OrderBy(g => g.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (gifts, totalCount);
        }
    }
}


