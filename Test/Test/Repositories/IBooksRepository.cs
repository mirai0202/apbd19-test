using Test.Models.Dtos;

namespace Test.Repositories;

public interface IBooksRepository
{
    Task<bool> DoesBookExist(int id);
    Task<int?> GetBookIdByTitle(string title);
    Task<List<BooksDto>> GetBookEditions(int id);
    Task AddBook(BooksPostDto booksPostDto);
}