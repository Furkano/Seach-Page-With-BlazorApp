using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Dto
{
    public class CategoriesDto
    {
        public string CategoryName { get; set; }
        public long BookCount { get; set; }
        public CategoriesDto(string categoryName,long bookCount)
        {
            CategoryName = categoryName;
            BookCount = bookCount;
        }
    }
}
