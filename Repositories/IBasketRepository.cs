using Microsoft.EntityFrameworkCore.Storage;
using projectApiAngular.Models;

namespace projectApiAngular.Repositories
{
    public interface IBasketRepository
    {
        Task<Basket> EnterToBasketAsync(Basket basket);
        Task<Basket?> DeleteBasketAsync(int id);
        Task<Basket?> UpdateBasketAmountAsync(int id, int newAmount);
        Task<IEnumerable<Basket>> GetMyBasket(int userId);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}