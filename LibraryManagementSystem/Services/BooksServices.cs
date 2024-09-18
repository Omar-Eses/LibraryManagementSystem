﻿// using LibraryManagementSystem.Data;
// using LibraryManagementSystem.Interfaces;
// using LibraryManagementSystem.Models;
// using Microsoft.EntityFrameworkCore;
//
// namespace LibraryManagementSystem.Services;
//
// public class BooksServices(LMSContext context) : IBooksService
// {
//     public async Task<Book> AddBookAsync(Book book)
//     {
//         context.Books.Add(book);
//         await context.SaveChangesAsync();
//         return book;
//     }
//
//     public bool BookExists(long id)
//     {
//         return context.Books.Any(e => e.Id == id);
//     }
//
//     public async Task<bool> DeleteBookAsync(long id)
//     {
//         var oldBook = await context.Books.FindAsync(id);
//         if (oldBook == null)
//             return false;
//         context.Books.Remove(oldBook);
//         await context.SaveChangesAsync();
//         return true;
//     }
//
//     public async Task<IEnumerable<Book>> GetAllBooksAsync()
//     {
//         return await context.Books.ToListAsync();
//     }
//
//     public async Task<Book?> GetBookByIdAsync(long id)
//     {
//         return await context.Books.FindAsync(id);
//     }
//
//     public async Task<bool> UpdateBookAsync(long id, Book book)
//     {
//         if (id != book.Id)
//             return await Task.FromResult(false);
//         try
//         {
//             var oldBook = await context.Books.FirstOrDefaultAsync(b => b.Id == id);
//             if (oldBook == null)
//                 return await Task.FromResult(false);
//             oldBook.BookTitle = book.BookTitle;
//             oldBook.BookDescription = book.BookDescription;
//             oldBook.ISBN = book.ISBN;
//             oldBook.BookGenre = book.BookGenre;
//             oldBook.BookPublisher = book.BookPublisher;
//             oldBook.BookPublishedDate = book.BookPublishedDate;
//             oldBook.BorrowedStatus = book.BorrowedStatus;
//             oldBook.UpdatedAt = DateTimeOffset.Now;
//             context.Books.Update(oldBook);
//             await context.SaveChangesAsync();
//             return await Task.FromResult(true);
//
//         }
//         catch (Exception e)
//         {
//             if (!BookExists(id))
//             {
//                 Console.WriteLine(e.Message);
//                 return await Task.FromResult(false);
//             }
//             throw;
//         }
//     }
// }
