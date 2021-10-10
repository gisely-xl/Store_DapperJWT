using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models
{
    public class LocationDb
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int FilmId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ReturnDate { get; set; }

    }
}
