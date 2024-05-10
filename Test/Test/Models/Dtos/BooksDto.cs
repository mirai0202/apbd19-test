namespace Test.Models.Dtos;

public class BooksDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; }
    public string EditionTitle { get; set; }
    public string PublishingHouseName { get; set; }
    public DateTime ReleaseDate { get; set; }
}

public class BooksPostDto
{
    public string BookTitle { get; set; }
    public string EditionTitle { get; set; }
    public int PublishingHouseId { get; set; }
    public DateTime ReleaseDate { get; set; }
}