using Microsoft.EntityFrameworkCore;
using projectApiAngular.Data;
using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly Chinese_SalesDbContext _context;
        public PurchaseRepository(Chinese_SalesDbContext context)
        {
            _context = context;
        }
        //get by gift
        public async Task<IEnumerable<Purchase>> GetPurchasesByGiftAsync(string name)
        {
            var purchases = await _context.Purchases.Include(g => g.Gift).Where(g => g.Gift.Name.Equals(name)).ToListAsync();
            return purchases;
        }
        //get customer details
        public async Task<IEnumerable<Purchase>> GetBuyersDetails()
        {
            var purchases = await _context.Purchases.Include(p => p.Castomer).Include(p=>p.Gift).ToListAsync();
            return purchases;
        }

        //sort by sellings
        public async Task<IEnumerable<Purchase>> GetGiftsSortedBySalesAsync()
        {
            var purchases = await _context.Purchases
                .GroupBy(p => p.GiftId)
                .Select(g => new
                {
                    GiftId = g.Key,
                    TotalSold = g.Count(),
                    Purchases = g.Select(p => p)
                })
                .OrderByDescending(g => g.TotalSold)
                .ToListAsync();

            var puchaseList = purchases.SelectMany(g => g.Purchases).ToList();

            var finalList = await _context.Purchases
                  .Where(p => puchaseList.Select(pl => pl.Id).Contains(p.Id))
                .Include(p => p.Gift)
                .Include(p => p.Castomer)
                .ToListAsync();
            return finalList;
        }
        //order by price
        public async Task<IEnumerable<Purchase>> GetPurchasesOrderedByPriceAsync()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Gift)
                .Include(p => p.Castomer)
                .OrderByDescending(p => p.Gift.Price)
                .ToListAsync();
            return purchases;
        }

        //add purchase  
        public async Task<Purchase> AddPurchase(Purchase purchase)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == purchase.CustomerId))
                throw new ArgumentException($"User with id {purchase.CustomerId} does not exist.");
            if (!await _context.Gifts.AnyAsync(g => g.Id == purchase.GiftId))
                throw new ArgumentException($"Gift with id {purchase.GiftId} does not exist.");
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }
        //get all
        public async Task<IEnumerable<Purchase>> GetAll()
        {
            var purcheses = await _context.Purchases.Include(p=>p.Castomer).Include(p=>p.Gift).ToListAsync();
            return purcheses;
        }

    }
}
