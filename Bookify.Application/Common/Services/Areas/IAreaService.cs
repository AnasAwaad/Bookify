using Bookify.Domain.Dtos;

namespace Bookify.Application.Common.Services.Areas;
public interface IAreaService
{
    IEnumerable<KeyValuePairDto> GetAreasByCity(int cityId);
}
