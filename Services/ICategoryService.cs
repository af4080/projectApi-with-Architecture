using projectApiAngular.DTO;
using static projectApiAngular.DTO.CategoryDto;

namespace projectApiAngular.Services
{
    public interface ICategoryService
    {
        Task<ReadCategoryDto> AddCategory(CreateCategoryDto category);
        Task<ReadCategoryDto?> DeleteCategory(int id);
        Task<IEnumerable<ReadCategoryDto>> GetAllCategories();
        Task<ReadCategoryDto?> UpdateCategory(int id, UpdateCategoryDto category);
    }
}