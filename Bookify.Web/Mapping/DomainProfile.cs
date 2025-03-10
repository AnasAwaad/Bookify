﻿using Bookify.Domain.Dtos;
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

        CreateMap<LatestBookDto, BookViewModel>();
        CreateMap<TopBookDto, BookViewModel>();
        CreateMap<Book, BookSearchResaultViewModel>();


        CreateMap<Book, BookRowViewModel>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author!.Name))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(b => b.Category!.Name).ToList()));


        // BookCopy
        CreateMap<BookCopy, BookCopyViewModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book!.Title))
            .ForMember(dest => dest.ImageThumbnailUrl, opt => opt.MapFrom(src => src.Book!.ImageThumbnailUrl));

        CreateMap<BookCopy, BookCopyFormViewModel>();
            

        // Users
        CreateMap<ApplicationUser, UserViewModel>();
        CreateMap<UserFormViewModel, ApplicationUser>()
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ReverseMap();

        CreateMap<UserFormViewModel, CreateUserDto>();

		// Subscripers
		CreateMap<SubscriperFormViewModel, Subscriper>().ReverseMap();
        CreateMap<Subscriper, SubscriperSearchResultViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
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

        // Rentals
        CreateMap<Rental, RentalViewModel>();
        CreateMap<RentalCopy, RentalCopyViewModel>();

        CreateMap<RentalCopy, RentalReportRowViewModel>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.BookCopy!.Book!.Author!.Name))
            .ForMember(dest => dest.SubscriperId, opt => opt.MapFrom(src => src.Rental!.Subscriper!.Id))
            .ForMember(dest => dest.SubscriberName, opt => opt.MapFrom(src => $"{src.Rental!.Subscriper!.FirstName} {src.Rental!.Subscriper!.LastName}"))
            .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.Rental!.Subscriper!.MobileNumber))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.BookCopy!.Book!.Title));


    }
}
