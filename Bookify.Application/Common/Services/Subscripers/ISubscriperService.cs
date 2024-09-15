using Bookify.Domain.Dtos;
using Bookify.Domain.Entities;

namespace Bookify.Application.Common.Services.Subscripers;
public interface ISubscriperService
{
    Subscriper AddSubscriper(Subscriper subscriper,string createdById);
    Subscriper? AddRentals(int subscriperId, ICollection<RentalCopy> copies, string createdById);
    Subscriper? GetById(int id);
    int GetActiveSubscripersCount();
    IEnumerable<KeyValuePairDto> GetSubscripersPerCity();
    Subscriper? GetSubscriperWithRentals(int subscriperId);
    Subscriper? GetSubscriperWithSubscription(int subscriperId);
    (string? errorMessage, int? maxAllowedCopies) CanRent(int subscriperId);
    IQueryable<Subscriper>? GetDatails();
    Subscriper? SearchForSubscriper(string value);
    Subscriper Update(Subscriper subscriper, string updatedById);
    Subscription RenewSubscription(int subscriperId, string createdById);
    bool IsAllowedEmail(int id, string email);
    bool IsAllowedMobileNumber(int id, string mobileNumber);
    bool IsAllowedNationalId(int id, string nationalId);

}
