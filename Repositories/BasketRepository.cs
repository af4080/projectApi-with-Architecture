using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using projectApiAngular.Data;
using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly Chinese_SalesDbContext _context;
        public BasketRepository(Chinese_SalesDbContext context)
        {
            _context = context;
        }
        //get my basket
        public async Task<IEnumerable<Basket>> GetMyBasket(int userId)
        {
            return await _context.Baskets
                .Where(b => b.UserId == userId)
                .Include(b => b.User)
                .Include(b => b.Gift)
                    .ThenInclude(g => g.Category)
                .Include(b => b.Gift)
                    .ThenInclude(g => g.Doner)
                .ToListAsync();

        }

        //EnterToBasketAsync
        public async Task<Basket> EnterToBasketAsync(Basket basket)
        {
            if (!await _context.Gifts.AnyAsync(b => b.Id == basket.GiftId))
                throw new ArgumentException($" Gift with id {basket.GiftId} does not exsit");
            var existing = await _context.Baskets
                .FirstOrDefaultAsync(b =>
                    b.UserId == basket.UserId &&
                    b.GiftId == basket.GiftId);

            if (existing != null)
            {
                existing.Amount += basket.Amount;
                await _context.SaveChangesAsync();
                return existing;
            }

            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();
            return basket;
        }


        //update amount
        public async Task<Basket?> UpdateBasketAmountAsync(int id, int newAmount)
        {
            var basket = await _context.Baskets.Include(b=>b.User)
                .Include(b=>b.Gift)
                .ThenInclude(g=>g.Category)
                   .Include(b => b.Gift)
                .ThenInclude(g => g.Doner)
                .FirstOrDefaultAsync(b=>b.Id==id);
            if (basket == null)
            {
                return null;
            }
            basket.Amount = newAmount;
            await _context.SaveChangesAsync();
            return basket;
        }
        //delete basket
        public async Task<Basket?> DeleteBasketAsync(int id)
        {
            var basket = await _context.Baskets.FindAsync(id);
            if (basket == null)
            {
                return null;
            }
            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
            return basket;
        }
        //BeginTransactionAsync
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

    }
}
