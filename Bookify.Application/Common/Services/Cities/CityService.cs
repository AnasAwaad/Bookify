
using Bookify.Application.Common.Interfaces;
using Bookify.Domain.Consts;

namespace Bookify.Application.Common.Services.Cities;
internal class CityService : ICityService
{
    private readonly IUnitOfWork _unitOfWork;

    public CityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<City> GetActiveCities()
    {
        return _unitOfWork.Cities.FindAll(c => c.IsActive,orderBy:c=>c.Name,orderByDirection:OrderBy.Ascending);
    }
}
