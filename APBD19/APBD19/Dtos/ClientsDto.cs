namespace APBD19.Dtos;

public class ClientDto
{
    public String FirstName { get; set; }
    public String LastName { get; set; }
}

public class PostClientDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telephone { get; set; } = null!;
    public string Pesel { get; set; } = null!;
    public int IdTrip { get; set; }
    public String TripName { get; set; }
    public DateTime PaymentDate { get; set; }
}
