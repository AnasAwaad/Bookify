namespace Bookify.Domain.Dtos;
public record FilteredDto(int Skip, int PageSize,int OrderColumnIndex,string OrderColumnName,string OrderColumnDirection,string SearchValue);
