// using BookstoreCatalog.Mvc.Models;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using System;

// namespace BookstoreCatalog.Mvc.Repositories;

// public class FakeBookRepository : IBookRepository
// {
//     public Task<List<Book>> GetAllReadOnlyAsync()
//     {
//         var data = new List<Book>
//         {
//             new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Price = 150000, Stock = 10 },
//             new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", Price = 120000, Stock = 0 }
//         };
//         return Task.FromResult(data);
//     }
//     public Task<List<Book>> GetAllAsync() 
//         => Task.FromResult(new List<Book>());
//     public Task<Book?> GetByIdAsync(int id) 
//         => Task.FromResult<Book?>(null);
//     public Task AddAsync(Book book) 
//         => Task.CompletedTask;
//     public Task SaveChangesAsync() 
//         => Task.CompletedTask;
// }