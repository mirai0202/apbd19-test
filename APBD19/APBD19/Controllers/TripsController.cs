using APBD19.Data;
using APBD19.Dtos;
using APBD19.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD19.Controllers;

[Route("api/trips")]
[ApiController]
public class TripsController(MasterContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBooks(int page, int pageSize = 10)
    {
        var trips = context.Trips.Select(e => new TripDto
            {
                Name = e.Name,
                Description = e.Description,
                DateFrom = e.DateFrom,
                MaxPeople = e.MaxPeople,
                Countries = e.IdCountries.Select(e => new CountryDto
                {
                    Name = e.Name
                }),
                Clients = e.ClientTrips.Select(e => new ClientDto
                {
                    FirstName = e.IdClientNavigation.FirstName,
                    LastName = e.IdClientNavigation.LastName
                })
            })
            .OrderBy(e => e.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(page)
            .ToListAsync();;

        var pageTripDto = new PageTripDto
        {
            Page = page,
            PageSize = pageSize,
            Trips = await trips
        };

        return Ok(pageTripDto);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, PostClientDto client)
    {
        if (await context.Clients.AnyAsync(e => e.Pesel == client.Pesel))
        {
            return BadRequest("Client with the following pesel already exists");
        }
        
        var trip = await context.Trips
            .Include(t => t.ClientTrips)
            .FirstOrDefaultAsync(t => t.IdTrip == idTrip);

        if (trip == null)
        {
            return NotFound("Trip not found");
        }

        if (trip.DateFrom < DateTime.Now)
        {
            return BadRequest("Trip has already finished");
        }

        if (trip.ClientTrips.Any(ct => client.Pesel == context.Clients.FirstOrDefaultAsync(c => c.IdClient == ct.IdClient).Result?.Pesel))
        {
            return BadRequest("Client is already registered for this trip.");
        }

        return Ok();
    }
}