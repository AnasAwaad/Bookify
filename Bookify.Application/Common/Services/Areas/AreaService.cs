using Bookify.Application.Common.Interfaces;
using Bookify.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Common.Services.Areas;
internal class AreaService : IAreaService
{
    private readonly IUnitOfWork _unitOfWork;

    public AreaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<AreaItemDto> GetAreasByCity(int cityId)
    {
        return _unitOfWork.Areas.GetQueryable()
                .Where(a => a.CityId == cityId && a.IsActive)
                .Select(a => new AreaItemDto(a.Id, a.Name))
                .ToList();

    }
}
