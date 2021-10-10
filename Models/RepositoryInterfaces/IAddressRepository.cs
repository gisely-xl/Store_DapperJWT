using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models.RepositoryInterfaces
{
    public interface IAddressRepository
    {
        
        IEnumerable<Address> GetAddresses();
        Address GetAddress(string cep);
        Address Create(long cep);
        Address Update(string oldCep, long newCep);
        Task Delete(string cep);
    }
}
