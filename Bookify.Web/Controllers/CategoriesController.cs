
using Bookify.Web.Data;
using Bookify.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Categories.AsNoTracking().ToList());
        }

        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_UpsertForm");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UpsertCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var category = new Category
            {
                Name = model.Name,
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return PartialView("_CategoryRow", category);
        }

        [AjaxOnly]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            var categoryViewModel = new UpsertCategoryViewModel()
            {
                Id = category.Id,
                Name = category.Name,
            };

            return PartialView("_UpsertForm", categoryViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(UpsertCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("_UpsertForm", model);
            }
            var category = _context.Categories.Find(model.Id);

            if (category == null)
                return NotFound();

            category.LastUpdatedOn = DateTime.Now;
            category.Name = model.Name;

            _context.SaveChanges();
            return PartialView("_CategoryRow", category);
        }


        [HttpPost]
        public IActionResult IsCategoryAllowed(UpsertCategoryViewModel model)
        {
            var category = _context.Categories.SingleOrDefault(c => c.Name == model.Name);
            // for new category null 
            // for update category without change the name => category will be filled 
            // check for id of the category with the same name equal model.Id
            if (category == null || category.Id==model.Id)
                return Json(true);
            return Json(false);
        }








        #region Ajax Request Handles

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            category.LastUpdatedOn = DateTime.Now;
            category.IsActive=!category.IsActive;
            _context.SaveChanges();
            return Json(new { lastUpdatedOn = category.LastUpdatedOn.ToString() } );
        }
        #endregion

    }
}
