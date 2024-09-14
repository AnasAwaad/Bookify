﻿using Bookify.Domain.Dtos;

namespace Bookify.Application.Common.Services.Books;
public interface IBookService
{
    Book? GetById(int id);
    Book? GetWithCategories(int id);
    IQueryable<Book> GetDetails();
    (IQueryable<Book> books, int count) GetFiltered(FilteredDto dto);
    Book Add(Book book,IList<int> selectedCategories,string createdById);
    Book Update(Book book,IList<int> selectedCategories,string LastUpdatedById);
    bool IsBookAllowed(int id,string bookTitle,int authorId);
    Book? ToggleStatus(int id, string lastUpdatedById);
}
