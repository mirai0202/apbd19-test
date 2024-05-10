using Microsoft.AspNetCore.Mvc;
using Test.Models.Dtos;
using Test.Repositories;

namespace Test.Controllers;

[Route("/api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpGet("{id}/editions")]
    public async Task<IActionResult> GetBookEditions(int id)
    {
        if (!await _booksRepository.DoesBookExist(id))
        {
            return NotFound($"Animal with given id {id} was not found!");
        }
        
        return Ok(await _booksRepository.GetBookEditions(id));
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(BooksPostDto booksPostDto)
    {
        await _booksRepository.AddBook(booksPostDto);
        return Ok("Book has been added!");
    }
}