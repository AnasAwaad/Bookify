using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModels;
using Bookify.Web.Data;
using Microsoft.AspNetCore.Mvc;

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
			return View(_context.Categories.ToList());
		}

        [HttpGet]
        public IActionResult Create()
        {
            return View("UpsertForm");
        }


        [HttpPost]
        public IActionResult Create(UpsertCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UpsertForm",model);
            }
            var category = new Category
            {
                Name = model.Name,
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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

            return View("UpsertForm",categoryViewModel);
        }


        [HttpPost]
        public IActionResult Update(UpsertCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UpsertForm",model);
            }
            var category = new Category
            {
                Id=model.Id,
                Name = model.Name,
            };
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
