namespace Bookify.Application.Common.Services.RentalService;
public interface IRentalService
{
    int? RemoveRental(int id, string lastUpdatedById);
}
