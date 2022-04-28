using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Dto
{
    public class BookDto
    {
        public List<Book> Books { get; set; } = new List<Book>();
        public long Total { get; set; }
    }
}
