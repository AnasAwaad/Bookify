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
        CreateMap<Book, BookViewModel>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author!.Name))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(b => b.Category!.Name).ToList()));

        // BookCopy
        CreateMap<BookCopy, BookCopyViewModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book!.Title));


        // Users
        CreateMap<ApplicationUser, UserViewModel>();
        CreateMap<UserFormViewModel, ApplicationUser>()
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ReverseMap();

        // Subscripers
        CreateMap<SubscriperFormViewModel, Subscriper>().ReverseMap();
        CreateMap<Subscriper, SubscriperSearchResultViewModel>()
            .ForMember(dest=>dest.FullName,opt=>opt.MapFrom(src=>$"{src.FirstName} {src.LastName}"));
        CreateMap<Subscriper, SubscriberViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area!.Name))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City!.Name));

        CreateMap<Subscription, SubscriptionViewModel>();
        // City & Area
        CreateMap<City, SelectListItem>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
        
        CreateMap<Area, SelectListItem>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

    }
}
