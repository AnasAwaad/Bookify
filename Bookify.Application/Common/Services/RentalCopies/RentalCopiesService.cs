using Bookify.Application.Common.Interfaces;
using Bookify.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Common.Services.RentalCopies;
internal class RentalCopiesService : IRentalCopiesService
{
    private readonly IUnitOfWork _unitOfWork;

    public RentalCopiesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<KeyValuePairDto> GetRentalsPerDay(DateTime? startDate,DateTime? endDate)
    {
        var rentals = _unitOfWork.RentalCopies.GetQueryable();

        return rentals.Where(r=>r.RentalDate >=startDate && r.RentalDate <=endDate)
            .GroupBy(r => new { r.RentalDate })
            .Select(r => new KeyValuePairDto(
                r.Key.RentalDate.ToString("d MMM"),
                r.Count().ToString()
            ))
            .ToList();

    }

    public bool CopyIsInRental(int bookCopyId)
    {
        return _unitOfWork.RentalCopies.IsExists(c => c.BookCopyId == bookCopyId && !c.ReturnDate.HasValue);
    }
}
