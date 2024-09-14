using Bookify.Application.Common.Interfaces;
using Bookify.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Common.Services.Subscriper;
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
}
