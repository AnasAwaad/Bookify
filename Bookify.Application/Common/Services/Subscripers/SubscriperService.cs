using Bookify.Application.Common.Interfaces;
using Bookify.Domain.Consts;
using Bookify.Domain.Dtos;
using Bookify.Domain.Entities;
using Bookify.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Common.Services.Subscripers;
internal class SubscriperService : ISubscriperService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubscriperService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public int GetActiveSubscripersCount()
    {
        return _unitOfWork.Subscripers.Count(s=>s.IsActive);
    }

    public Subscriper? GetSubscriperWithRentals(int subscriperId)
    {
        return _unitOfWork.Subscripers.GetQueryable()
            .Include(s => s.Subscriptions)
            .Include(s => s.Rentals)
            .ThenInclude(r => r.RentalCopies)
            .SingleOrDefault(s => s.Id == subscriperId);
    }
    public (string? errorMessage,int? maxAllowedCopies) CanRent(int subscriperId)
    {
        var subscriper = GetSubscriperWithRentals(subscriperId);

        if (subscriper is null)
            return (errorMessage: "Cann't find subscriper", null);

        var availableCopies = subscriper.Rentals
            .SelectMany(r => r.RentalCopies)
            .Count(rc => !rc.ReturnDate.HasValue);

        var maxAllowedCopies = (int)RentalsConfigurations.MaxAllowedCopies - availableCopies;

        if (maxAllowedCopies.Equals(0))
            return (errorMessage: Errors.MaxAllowedCopies, null);

        if (subscriper.IsBlackListed)
            return (errorMessage: Errors.BlackListedSubscriber, null);


        if (subscriper.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalsConfigurations.MaxAllowedCopies))
            return (errorMessage: Errors.InActiveSubscriber, null);

        return (null, maxAllowedCopies: maxAllowedCopies);
    }
    public IEnumerable<KeyValuePairDto> GetSubscripersPerCity()
    {
        var subscribers = _unitOfWork.Subscripers.GetQueryable();
        return subscribers
            .Where(s => s.IsActive)
            .Include(s => s.City)
            .GroupBy(s => new { s.City!.Name })
            .Select(s => new KeyValuePairDto
            (
                s.Key.Name,
                s.Count().ToString()
            )).ToList();
    }

    public Subscriper? AddRentals(int subscriperId, ICollection<RentalCopy> copies, string createdById)
    {
        var subscriper = _unitOfWork.Subscripers.GetById(subscriperId);

        if (subscriper is null)
            return null;

        Rental rental = new()
        {
            RentalCopies = copies,
            CreatedById = createdById
        };

        subscriper.Rentals.Add(rental);
        _unitOfWork.SaveChanges();

        return subscriper;
    }
}
