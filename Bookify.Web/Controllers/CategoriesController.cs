
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            var category = _context.Categories.AsNoTracking().ToList();

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

            var category = mapper.Map<Category>(model);

            category.CreatedOn = DateTime.Now;
            category.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.Categories.Add(category);
            _context.SaveChanges();

            var cateogryVM = mapper.Map<CategoryViewModel>(category);
            return PartialView("_CategoryRow", cateogryVM);
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
            var category = _context.Categories.Find(model.Id);

            if (category == null)
                return NotFound();

            category = mapper.Map(model, category);

            category.LastUpdatedOn = DateTime.Now;
            category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.SaveChanges();
            return PartialView("_CategoryRow", mapper.Map<CategoryViewModel>(category));
        }


        public IActionResult IsCategoryAllowed(UpsertCategoryViewModel model)
        {
            var category = _context.Categories.SingleOrDefault(c => c.Name == model.Name);
            // for new category null 
            // for update category without change the name => category will be filled 
            // check for id of the category with the same name equal model.Id
            if (category == null || category.Id == model.Id)
                return Json(true);
            return Json(false);
        }








        #region Ajax Request Handles

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            category.LastUpdatedOn = DateTime.Now;
            category.IsActive = !category.IsActive;
            category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.SaveChanges();
            return Json(new { lastUpdatedOn = category.LastUpdatedOn.ToString() });
        }
        #endregion

    }
}
