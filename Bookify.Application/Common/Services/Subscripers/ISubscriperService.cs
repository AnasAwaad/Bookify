using Bookify.Domain.Dtos;
using Bookify.Domain.Entities;

namespace Bookify.Application.Common.Services.Subscripers;
public interface ISubscriperService
{
    int GetActiveSubscripersCount();
    IEnumerable<KeyValuePairDto> GetSubscripersPerCity();
    Subscriper? GetSubscriperWithRentals(int subscriperId);
    (string? errorMessage, int? maxAllowedCopies) CanRent(int subscriperId);

    Subscriper? AddRentals(int subscriperId, ICollection<RentalCopy> copies, string createdById);
}
