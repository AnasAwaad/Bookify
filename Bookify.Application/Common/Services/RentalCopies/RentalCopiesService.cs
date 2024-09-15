using Bookify.Application.Common.Interfaces;
using Bookify.Application.Common.Models;
using Bookify.Domain.Dtos;
using Bookify.Domain.Entities;
using Bookify.Domain.Enums;
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

    public PaginatedList<RentalCopy> GetPaginatedList(int pageNumber,DateTime startDate, DateTime endDate)
    {
        var rentals= _unitOfWork.RentalCopies.GetQueryable()
                .Include(r => r.Rental)
                .ThenInclude(r => r!.Subscriper)
                .Include(r => r.BookCopy)
                .ThenInclude(c => c!.Book)
                .ThenInclude(b => b!.Author)
                .OrderBy(r => r.RentalDate)
                .Where(r => r.RentalDate >= startDate && r.RentalDate <= endDate);

        return PaginatedList<RentalCopy>.Create(rentals, pageNumber, (int)ReportsConfiguration.pageSize);
    }

    public IQueryable<RentalCopy> GetQurableRowData(DateTime startDate, DateTime endDate)
    {
        return _unitOfWork.RentalCopies.GetQueryable()
            .Include(r => r.Rental)
            .ThenInclude(r => r!.Subscriper)
            .Include(r => r.BookCopy)
            .ThenInclude(c => c!.Book)
            .ThenInclude(b => b!.Author)
            .OrderBy(r => r.RentalDate)
            .Where(r => r.RentalDate >= startDate && r.RentalDate <= endDate);
    }
}
