using APBD19.Models;

namespace APBD19.Dtos;

public class TripDto
{
    public String Name { get; set; }

    public String Description { get; set; }
    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<CountryDto> Countries { get; set; }
    public IEnumerable<ClientDto> Clients { get; set; }
}

public class PageTripDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<TripDto> Trips { get; set; }
}
