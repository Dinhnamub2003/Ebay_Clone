using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.CategoryModel;
using Project.Servie.Service.Categories;

namespace Project.RazorWeb.Pages.Auth.Home
{
    public class homeModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public homeModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<CategoryViewModel> Categories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Categories = await _categoryService.GetAllCategoriesAsync();
            return Page();
        }
    }
}
