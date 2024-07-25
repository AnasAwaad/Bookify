using AutoMapper;

namespace Bookify.Web.Mapping;

public class DomainProfile : Profile
{
    public DomainProfile()
    {
        // Category
        CreateMap<Category,CategoryViewModel>().ReverseMap();
        CreateMap<UpsertCategoryViewModel, Category>().ReverseMap();

        //Author
        CreateMap<Author, AuthorViewModel>().ReverseMap();
        CreateMap<UpsertAuthorViewModel, Author>().ReverseMap();

    }
}
