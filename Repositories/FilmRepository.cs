using Dapper;
using Microsoft.Extensions.Configuration;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Repositories
{
    public class FilmRepository : IFilmRepository
    {
        private readonly string _connectString;
        public FilmRepository(IConfiguration configuration)
        {
            _connectString = configuration.GetConnectionString("Default");
        }

        public Film Create(Film film)
        {
            using var connection = new SqlConnection(_connectString);
            var createQuery = $"INSERT INTO MOVIES (Title, Direction, Year) VALUES (@Title, @Direction, @Year)";
            connection.Execute(createQuery, new
            {
                film.Title,
                film.Direction,
                film.Year
            });

            //output with the Id from database
            var queryId = $"SELECT max(Id) as last_id from MOVIES";
            var ids = connection.Query(queryId);
            film.Id = ids.ToList()[0].last_id;

            return film;

        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectString);
            var deletQuery = $"DELETE FROM MOVIES WHERE Id = {id}";
            await connection.ExecuteAsync(deletQuery, new { Id = id });
        }

        public Film GetFilm(int id)
        {
            using var conneciton = new SqlConnection(_connectString);
            var getByIdQuery = $"SELECT * FROM MOVIES WHERE Id = {id}";
            var film = conneciton.QueryFirstOrDefault<Film>(getByIdQuery, new { Id = id });
            return film;
        }

        public IEnumerable<Film> GetFilms()
        {
            using var conneciton = new SqlConnection(_connectString);
            var getQuery = "SELECT * FROM MOVIES";
            var movies = SqlMapper.Query<Film>(conneciton, getQuery);
            return movies;
        }

        public Film Update(Film film)
        {
            using var conneciton = new SqlConnection(_connectString);
            var updateQuery = $"UPDATE MOVIES SET Title=@Title, Direction=@Direction, Year=@Year WHERE Id={film.Id}";
            conneciton.Execute(updateQuery, new { 
                film.Id,
                film.Title,
                film.Direction,
                film.Year
            });
            return film;
        }
    }
}
