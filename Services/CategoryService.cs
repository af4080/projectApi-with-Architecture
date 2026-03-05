using projectApiAngular.Models;
using projectApiAngular.Repositories;
using static projectApiAngular.DTO.CategoryDto;

namespace projectApiAngular.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ICategoryRepository categoryRepository,ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        //get
        public async Task<IEnumerable<ReadCategoryDto>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            if (categories == null)
            {
                _logger.LogWarning("Categories repository returned null");
                return Enumerable.Empty<ReadCategoryDto>();
            }
            var dtos = categories.Select(c => new ReadCategoryDto { Id = c.Id, Name = c.Name });
            _logger.LogDebug("Fetched {Count} categories", dtos.Count());
            return dtos;

        }
        //post
        public async Task<ReadCategoryDto> AddCategory(CreateCategoryDto category)
        {
            _logger.LogInformation("Adding new category with name {Name}", category.Name);
            try
            {
             var entity = new Category
            {
                Name = category.Name
            };
            var addedCategory = await _categoryRepository.AddCategory(entity);
                _logger.LogInformation("Category {CategoryId} created successfully",addedCategory.Id );

                return new ReadCategoryDto { Id = addedCategory.Id, Name = addedCategory.Name };
            }
            catch (Exception ex)
            {
                _logger.LogError( ex,"Failed to add category with name {Name} ", category.Name);
                throw ;
            }

        }

        //update
        public async Task<ReadCategoryDto?> UpdateCategory(int id, UpdateCategoryDto category)
        {
            var entity = new Category
            {
                Name = category.Name 
            };
            var updatedCategory = await _categoryRepository.UpdateCategory(id, entity);
            if (updatedCategory == null)
            {
                _logger.LogWarning("Category {CategoryId} not found for update",id);
                return null;
            }
            _logger.LogInformation("Category {CategoryId} updated", id);
            return new ReadCategoryDto { Id = updatedCategory.Id, Name = updatedCategory.Name };
        }
        //delete
        public async Task<ReadCategoryDto?> DeleteCategory(int id)
        {
            _logger.LogInformation("Deleting category {CategoryId}", id);
            var deletedCategory = await _categoryRepository.DeleteCategory(id);
            if (deletedCategory == null)
            {
                _logger.LogWarning("Category {CategoryId} not found for deletion", id);
                return null;
            }

            _logger.LogInformation("Category {CategoryId} deleted", id);
            return new ReadCategoryDto { Id = deletedCategory.Id, Name = deletedCategory.Name };
        }

    }
}
