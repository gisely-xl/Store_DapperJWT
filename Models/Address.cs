using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models
{
    public class Address
    {
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string UF { get; set; }
        public string Localidade { get; set; }
        public string Bairro { get; set; }
        public string DDD { get; set; }
        public string Complemento { get; set; }
        public string IBGE { get; set; }
        public string GIA { get; set; }
        public string SIAFI { get; set; }

    }
}
