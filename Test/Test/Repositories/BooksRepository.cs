using System.Data;
using Microsoft.Data.SqlClient;
using Test.Models.Dtos;

namespace Test.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;

    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM books WHERE PK = @PK";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<int?> GetBookIdByTitle(string title)
    {
        var query = "SELECT 1 FROM books WHERE title = @title";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@title", title);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            return reader.GetInt32(reader.GetOrdinal("PK"));
        }

        return null;
    }

    public async Task<List<BooksDto>> GetBookEditions(int id)
    {
        var query = @"SELECT
                        books.PK AS BookId,
                        books.title AS BookTitle,
                        books_editions.edition_title AS EditionTitle,
                        publishing_houses.name AS PublishingHouseName,
                        books_editions.release_date AS ReleaseDate
                    FROM books
                             JOIN books_editions ON books_editions.FK_BOOK = books.PK
                             JOIN publishing_houses ON publishing_houses.PK = books_editions.FK_publishing_house
                    WHERE books.PK = @PK;";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));

        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var bookIdOrdinal = reader.GetOrdinal("BookId");
        var bookTitleOrdinal = reader.GetOrdinal("BookTitle");
        var editionTitleOrdinal = reader.GetOrdinal("EditionTitle");
        var publishingHouseNameOrdinal = reader.GetOrdinal("PublishingHouseName");
        var releaseDateOrdinal = reader.GetOrdinal("ReleaseDate");
        
        List<BooksDto> booksDtos = [];

        while (await reader.ReadAsync())
        {
            booksDtos.Add(
                new BooksDto
                {
                    Id = reader.GetInt32(bookIdOrdinal),
                    BookTitle = reader.GetString(bookTitleOrdinal),
                    EditionTitle = reader.GetString(editionTitleOrdinal),
                    PublishingHouseName = reader.GetString(publishingHouseNameOrdinal),
                    ReleaseDate = reader.GetDateTime(releaseDateOrdinal)
                }
            );
        }

        return booksDtos;
    }

    public async Task AddBook(BooksPostDto booksPostDto)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        
        //----------------------------------------------------------
        var addBookQuery = "INSERT INTO books VALUES(@title)";
        
        await using SqlCommand addBookCommand = new SqlCommand();
        addBookCommand.Connection = connection;
        addBookCommand.CommandText = addBookQuery;
        addBookCommand.Parameters.AddWithValue("@title", booksPostDto.BookTitle);
        
        //----------------------------------------------------------
        var addEditionQuery = @"INSERT INTO books_editions VALUES(
                                @FK_publishing_house, 
                                @FK_book,
                                @edition_title,
                                @release_date
                              )";
        
        await using SqlCommand addEditionCommand = new SqlCommand();
        addEditionCommand.Connection = connection;
        addEditionCommand.CommandText = addEditionQuery;
        addEditionCommand.Parameters.AddWithValue("@FK_publishing_house", booksPostDto.PublishingHouseId);
        addEditionCommand.Parameters.AddWithValue("@FK_book", await GetBookIdByTitle(booksPostDto.BookTitle));
        addEditionCommand.Parameters.AddWithValue("@edition_title", booksPostDto.EditionTitle);
        addEditionCommand.Parameters.AddWithValue("@release_date", booksPostDto.ReleaseDate);
        
        await connection.OpenAsync();

        await addBookCommand.ExecuteNonQueryAsync();
        await addEditionCommand.ExecuteNonQueryAsync();
    }
}