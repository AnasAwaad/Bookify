using Bookify.Domain.Dtos;

namespace Bookify.Application.Common.Services.RentalCopies;
public interface IRentalCopiesService
{
    IEnumerable<KeyValuePairDto> GetRentalsPerDay(DateTime? startDate,DateTime? endDate);

    bool CopyIsInRental(int bookCopyId);
}
