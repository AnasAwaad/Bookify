
using Bookify.Web.Data;
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

        [HttpGet]
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
            return Ok(category);
        }

        public IActionResult Update(int? id)
        {
            if(id == null || id==0)
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

            return View("_UpsertForm",categoryViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(UpsertCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("_UpsertForm",model);
            }
            var category = _context.Categories.Find(model.Id);

            if(category == null)
                return NotFound();

            category.LastUpdatedOn = DateTime.Now;
            category.Name = model.Name;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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
