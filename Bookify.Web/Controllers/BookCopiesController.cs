using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;
public class BookCopiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookCopiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleStatus(int id)
    {
        var model = _context.BookCopies.Find(id);
        if (model == null)
        {
            return BadRequest();
        }
        model.IsActive=!model.IsActive;
        model.LastUpdatedOn= DateTime.Now;
        _context.SaveChanges();
        return Ok();
    }

    public IActionResult Create(int bookId)
    {
        var viewModel=new BookCopyFormViewModel { BookId = bookId };
        return PartialView("Form",viewModel);
    }

    //public IActionResult Update(int id)
    //{
    //    var model = _context.BookCopies.Find(id);
    //    if (model == null)
    //    {
    //        return BadRequest();
    //    }
        
    //}
}
