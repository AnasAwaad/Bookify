using Bookify.Application.Common.Models;
using Bookify.Domain.Dtos;
using System.Linq.Dynamic.Core;

namespace Bookify.Application.Common.Services.RentalCopies;
public interface IRentalCopiesService
{
    IEnumerable<KeyValuePairDto> GetRentalsPerDay(DateTime? startDate,DateTime? endDate);
    PaginatedList<RentalCopy> GetPaginatedList(int pageNumber, DateTime startDate, DateTime endDate);
    IQueryable<RentalCopy> GetQurableRowData(DateTime startDate, DateTime endDate);
    bool CopyIsInRental(int bookCopyId);
}
