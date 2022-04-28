using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Dto
{
    public class BookIdNameDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BookIdNameDto(string id,string name)
        {
            Id = id;
            Name = name;
        }
    }
}
