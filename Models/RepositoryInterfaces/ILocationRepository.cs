using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models.RepositoryInterfaces
{
    public interface ILocationRepository
    {
        IEnumerable<Location> GetLocations();
        Location GetLocation(int id);
        LocationDb Create(LocationDb location);
        LocationDb Update(LocationDb location);
        Task Delete(int id);
    }
}
