using Bookify.Application.Common.Services.Categories;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class CategoriesController : Controller
    {
        private readonly IMapper mapper;
        private readonly ICategoryService _categoryService;

        public CategoriesController(IMapper mapper, ICategoryService categoryService)
        {
            this.mapper = mapper;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var category = _categoryService.GetAll();

            return View(mapper.Map<IEnumerable<CategoryViewModel>>(category));
        }

        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_UpsertForm");
        }


        [HttpPost]
        public IActionResult Create(UpsertCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var category = _categoryService.Add(model.Name, User.GetUserId());

            var cateogryVM = mapper.Map<CategoryViewModel>(category);
            return PartialView("_CategoryRow", cateogryVM);
        }

        [AjaxOnly]
        public IActionResult Update(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
                return NotFound();

            var categoryVM = mapper.Map<UpsertCategoryViewModel>(category);

            return PartialView("_UpsertForm", categoryVM);
        }


        [HttpPost]
        public IActionResult Update(UpsertCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("_UpsertForm", model);
            }

            var category = _categoryService.Update(model.Id, model.Name, User.GetUserId());
            if (category is null)
                return NotFound();

            return PartialView("_CategoryRow", mapper.Map<CategoryViewModel>(category));
        }


        public IActionResult IsCategoryAllowed(UpsertCategoryViewModel model)
        {
            return Json(_categoryService.IsCategoryAllowed(model.Id, model.Name));
        }

        #region Ajax Request Handles

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var category = _categoryService.ToggleStatus(id,User.GetUserId());
            if (category == null)
                return NotFound();

            return Json(new { lastUpdatedOn = category.LastUpdatedOn.ToString() });
        }
        #endregion

    }
}
