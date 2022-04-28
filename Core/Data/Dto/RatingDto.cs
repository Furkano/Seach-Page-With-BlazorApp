

namespace Core.Data.Dto
{
    public class RatingDto
    {
        public int RatingStar { get; set; }
        public long BookCount { get; set; }
        public RatingDto(int ratingStar,long bookCount)
        {
            RatingStar = ratingStar;
            BookCount = bookCount;
        }
    }
}
