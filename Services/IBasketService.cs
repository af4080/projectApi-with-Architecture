using projectApiAngular.DTO;
using static projectApiAngular.DTO.BasketDto;

namespace projectApiAngular.Services
{
    public interface IBasketService
    {
        Task<int?> DeleteBasketAsync(int id);
        Task<ReadBasketDto?> EnterToBasketAsync(CreateBasketDto basketDto);
        Task<IEnumerable<ReadBasketDto>> GetMyBasket();
        Task<ReadBasketDto?> UpdateBasketAmountAsync(int id, int newAmount);
        Task<bool> BuyAllBasket();
    }
}