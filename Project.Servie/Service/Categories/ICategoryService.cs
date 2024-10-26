using Project.Model.CategoryModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Servie.Service.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllCategoriesAsync();
        Task<CategoryViewModel> GetCategoryByIdAsync(int categoryId);
        Task<int> AddCategoryAsync(CategoryCreateModel model);
        Task<bool> UpdateCategoryAsync(CategoryUpdateModel model);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
