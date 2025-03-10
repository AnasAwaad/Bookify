﻿
using Bookify.Infrastructure.Persistence.Repositories;

namespace Bookify.Infrastructure.Persistence;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
     
    public IGenericRepository<Author> Authors => new GenericRepository<Author>(_context);
    public IGenericRepository<Area> Areas => new GenericRepository<Area>(_context);
    public IBookRepository Books => new BookRepository(_context);
    public IGenericRepository<BookCopy> BookCopies => new GenericRepository<BookCopy>(_context);
    public IGenericRepository<Category> Categories => new GenericRepository<Category>(_context);
    public IGenericRepository<City> Cities => new GenericRepository<City>(_context);
    public IGenericRepository<Rental> Rentals => new GenericRepository<Rental>(_context);
    public IGenericRepository<RentalCopy> RentalCopies => new GenericRepository<RentalCopy>(_context);
    public IGenericRepository<Subscriper> Subscripers => new GenericRepository<Subscriper>(_context);

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}
