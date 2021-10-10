using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models
{
    public class Location
    {
        public int Id { get; set; }
        public Client Customer { get; set; }
        public Film Film { get; set; }
        public string LoanDate { get; set; }
        public string ReturnDate { get; set; }
    }
}
