using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models.RepositoryInterfaces
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetClients();
        Client GetClient(int id);
        ClientDb Create(ClientDb newClient);
        ClientDb Update(ClientDb client);
        Task Delete(int id);
    }
}
