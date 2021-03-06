using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models
{
    public class ClientDb
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Contact { get; set; }
        [Required]
        public string Cep_C { get; set; }
        public Address Address { get; set; }
    }
}
