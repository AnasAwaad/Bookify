using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Mapping;

public class DomainProfile : Profile
{
    public DomainProfile()
    {
        // Category
        CreateMap<Category, CategoryViewModel>().ReverseMap();
        CreateMap<UpsertCategoryViewModel, Category>().ReverseMap();
		CreateMap<Category, SelectListItem>()
			.ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
		
        //Author
		CreateMap<Author, AuthorViewModel>().ReverseMap();
        CreateMap<UpsertAuthorViewModel, Author>().ReverseMap();
		CreateMap<Author, SelectListItem>()
			.ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));


		// Book
		CreateMap<BookFormViewModel, Book>().ReverseMap();
        

    }
}
