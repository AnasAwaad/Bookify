using Bookify.Domain.Dtos;

namespace Bookify.Application.Common.Services.Subscriper;
public interface ISubscriperService
{
    int GetActiveSubscripersCount();
    IEnumerable<KeyValuePairDto> GetSubscripersPerCity();
}
