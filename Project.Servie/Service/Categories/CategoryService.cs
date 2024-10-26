using Microsoft.EntityFrameworkCore;
using Project.Bussiness.Infrastructure;
using Project.Data.Models;
using Project.Model.CategoryModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Servie.Service.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.CategoryRepository.GetQuery()
                .Where(c => c.IsDeleted == false)
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.GetQuery()
                .Where(c => c.CategoryId == categoryId && c.IsDeleted == false)
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .FirstOrDefaultAsync();

            return category;
        }

        public async Task<int> AddCategoryAsync(CategoryCreateModel model)
        {
            var category = new Category
            {
                CategoryName = model.CategoryName,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.SaveChangesAsync();
            return category.CategoryId;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryUpdateModel model)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(model.CategoryId);
            if (category == null || category.IsDeleted == true)
            {
                return false;
            }

            category.CategoryName = model.CategoryName;
            category.UpdatedAt = DateTime.Now;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.IsDeleted == true)
            {
                return false;
            }

            category.IsDeleted = true;
            category.DeletedAt = DateTime.Now;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
