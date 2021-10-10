using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Models.RepositoryInterfaces
{
    public interface IFilmRepository
    {
        IEnumerable<Film> GetFilms();
        Film GetFilm(int id);
        Film Create(Film film);
        Film Update(Film film);
        Task Delete(int id);

    }
}
