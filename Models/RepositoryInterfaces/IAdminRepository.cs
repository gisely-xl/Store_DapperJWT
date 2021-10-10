using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models.RepositoryInterfaces
{
    public interface IAdminRepository
    {
        Admin GetAdmin(string name, string password);
    }
}
