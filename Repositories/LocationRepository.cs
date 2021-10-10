using Dapper;
using Microsoft.Extensions.Configuration;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly string _connectString;
        private readonly IFilmRepository _filmRepository;
        private readonly IClientRepository _clientRepository;

        public LocationRepository(IConfiguration configuration, IFilmRepository filmRepository, IClientRepository clientRepository)
        {
            _connectString = configuration.GetConnectionString("Default");
            _filmRepository = filmRepository;
            _clientRepository = clientRepository;
        }

        public IEnumerable<Location> GetLocations()
        {
            using var connection = new SqlConnection(_connectString);
            var getQuery = "SELECT * FROM LOCATIONS";
            var locationsDb = SqlMapper.Query<LocationDb>(connection, getQuery);
            var locations = SqlMapper.Query<Location>(connection, getQuery);

            for (int i = 0; i < locations.AsList().Count; i++)
            {
                //Assigning Client
                int clientId = locationsDb.AsList()[i].ClientId;
                locations.AsList()[i].Customer = _clientRepository.GetClient(clientId);

                //Assigning Film
                int filmId = locationsDb.AsList()[i].FilmId;
                locations.AsList()[i].Film = _filmRepository.GetFilm(filmId);

                //Assigning LoanDate formated
                locations.AsList()[i].LoanDate = locationsDb.AsList()[i].LoanDate.ToString("D", CultureInfo.CreateSpecificCulture("en-Us"));

                //Assigning ReturnDate formated
                locations.AsList()[i].ReturnDate = locationsDb.AsList()[i].ReturnDate.ToString("D", CultureInfo.CreateSpecificCulture("en-Us"));
            }
            return locations;
        }
        public Location GetLocation(int id)
        {
            using var connection = new SqlConnection(_connectString);
            var getByIdQuery = $"SELECT * FROM LOCATIONS WHERE Id = {id}";
            var locationDb = connection.QueryFirstOrDefault<LocationDb>(getByIdQuery, new { Id = id });
            var location = new Location();

            //assigning Id
            location.Id = id;

            //assigning Client
            location.Customer = _clientRepository.GetClient(locationDb.ClientId);

            //assigning Film
            location.Film = _filmRepository.GetFilm(locationDb.FilmId);

            //formating and assigning LoanDate
            location.LoanDate = locationDb.LoanDate.ToString("D", CultureInfo.CreateSpecificCulture("en-US"));

            //formating and assgning ReturnDate                   
            location.ReturnDate = locationDb.ReturnDate.ToString("D", CultureInfo.CreateSpecificCulture("en-US"));

            return location;
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectString);
            var deletQuery = $"DELETE FROM LOCATIONS WHERE Id = {id}";
            await connection.ExecuteAsync(deletQuery, new { Id = id });
        }

        public LocationDb Create(LocationDb location)
        {   
            using var connection = new SqlConnection(_connectString);
            var createQuery = $"INSERT INTO LOCATIONS (ClientId, FilmId, LoanDate, ReturnDate) VALUES (@ClientId, @FilmId, @LoanDate, @ReturnDate)";
            connection.Execute(createQuery, new
            {
                location.Id,
                location.ClientId,
                location.FilmId,
                location.LoanDate,
                location.ReturnDate
            });

            var queryId = $"SELECT max(Id) as last_id from LOCATIONS";
            var ids = connection.Query(queryId);
            location.Id = ids.AsList()[0].last_id;

            return location;
        }

        public LocationDb Update(LocationDb location)
        {
            using var connection = new SqlConnection(_connectString);
            var updateQuery = $"UPDATE LOCATIONS SET ClientId=@ClientId, FilmId=@FilmId, LoanDate=@LoanDate, ReturnDate=@ReturnDate WHERE Id={location.Id}";
            connection.Execute(updateQuery, new { 
                location.Id,
                location.ClientId,
                location.FilmId,
                location.LoanDate,
                location.ReturnDate
            });
            return location;
        }
    }
}
