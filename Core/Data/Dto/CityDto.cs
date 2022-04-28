
namespace Core.Data.Dto
{
    public class CityDto
    {
        public string Name { get; set; }
        public long BookCount { get; set; }
        public CityDto(string name,long bookCount)
        {
            Name = name;
            BookCount = bookCount;
        }
    }
}
