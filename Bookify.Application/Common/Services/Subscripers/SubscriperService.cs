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

    public Subscriper AddSubscriper(Subscriper subscriper, string createdById)
    {

        var subscription = new Subscription
        {
            CreatedById = subscriper.CreatedById,
            CreatedOn = subscriper.CreatedOn,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1),
        };

        subscriper.CreatedOn = DateTime.Now;
        subscriper.CreatedById = createdById;

        subscriper.Subscriptions.Add(subscription);

        _unitOfWork.Subscripers.Add(subscriper);
        _unitOfWork.SaveChanges();

        return subscriper;
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

    public IQueryable<Subscriper>? GetDatails()
    {
        return _unitOfWork.Subscripers.GetQueryable()
            .Include(s => s.Area)
            .Include(s => s.City)
            .Include(s => s.Subscriptions)
            .Include(s => s.Rentals)
            .ThenInclude(r => r.RentalCopies);

    }

    public Subscriper? GetById(int id)
    {
        return _unitOfWork.Subscripers.GetById(id);
    }

    public Subscriper Update(Subscriper subscriper, string updatedById)
    {
        subscriper.LastUpdatedOn = DateTime.Now;
        subscriper.LastUpdatedById = updatedById;

        _unitOfWork.Subscripers.Update(subscriper);

        _unitOfWork.SaveChanges();
        return subscriper;
    }

    public Subscriper? SearchForSubscriper(string value)
    {
        var subscriper = _unitOfWork.Subscripers
            .Find(s => s.MobileNumber == value || s.Email == value || s.NationalId == value);

        return subscriper;
    }

    public bool IsAllowedEmail(int id, string email)
    {
        var subscriper = _unitOfWork.Subscripers.Find(s => s.Email == email);
        return subscriper is null || subscriper.Id.Equals(id);
    }

    public bool IsAllowedMobileNumber(int id, string mobileNumber)
    {
        var subscriper = _unitOfWork.Subscripers.Find(s => s.MobileNumber == mobileNumber);
        return subscriper is null || subscriper.Id.Equals(id);
    }

    public bool IsAllowedNationalId(int id, string nationalId)
    {
        var subscriper = _unitOfWork.Subscripers.Find(s => s.NationalId == nationalId);
        return subscriper is null || subscriper.Id.Equals(id);
    }

    public Subscriper? GetSubscriperWithSubscription(int subscriperId)
    {
        return _unitOfWork.Subscripers.Find(s => s.Id == subscriperId, include: s => s.Include(x => x.Subscriptions));
    }

    public Subscription RenewSubscription(int subscriperId,string createdById)
    {
        var subscriper = GetSubscriperWithSubscription(subscriperId);
        
        var lastSubscription = subscriper!.Subscriptions.Last();
        var startDate = lastSubscription.EndDate > DateTime.Today ? lastSubscription.EndDate.AddDays(1) : DateTime.Today;
        var newSubscription = new Subscription()
        {
            CreatedById = createdById,
            CreatedOn = DateTime.Now,
            StartDate = startDate,
            EndDate = startDate.AddYears(1)
        };

        subscriper.Subscriptions.Add(newSubscription);
        _unitOfWork.SaveChanges();

        return newSubscription;
    }
}
