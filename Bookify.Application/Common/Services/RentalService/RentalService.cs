using Bookify.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Common.Services.RentalService;
internal class RentalService : IRentalService
{
    private readonly IUnitOfWork _unitOfWork;

    public RentalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public int? RemoveRental(int id,string lastUpdatedById)
    {
        //var rental = _unitOfWork.Rentals.Include(r => r.RentalCopies).SingleOrDefault(r => r.Id == id);
        var rental=_unitOfWork.Rentals.Find(r=>r.Id==id,r=>r.Include(x=>x.RentalCopies));

        if (rental is null || rental.CreatedOn.Date != DateTime.Today)
            return null;

        rental.IsActive = false;
        rental.LastUpdatedOn = DateTime.Today;
        rental.LastUpdatedById = lastUpdatedById;

        _unitOfWork.SaveChanges();

        return rental.RentalCopies.Count;
    }
}
