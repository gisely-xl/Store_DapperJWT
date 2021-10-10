using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Direction { get; set; }
        public int Year { get; set; }
    }
}
