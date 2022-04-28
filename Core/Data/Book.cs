
namespace Core.Data
{
    public class Book
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Author Author { get; set; } = new Author();
        public string Categories { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stars { get; set; }
        public int LikeCount { get; set; }
    }
    public class Author
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public Address Address { get; set; } = null!;
    }
    public class Address
    {
        public string City { get; set; }
        public string Country { get; set; }

        public string Plate { get; set; }
        public Address(string city,string country,string plate)
        {
            City = city;
            Country = country;  
            Plate = plate;
        }
    }
}
