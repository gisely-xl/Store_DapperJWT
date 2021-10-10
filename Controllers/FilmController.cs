using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Store_Dapper.WebAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly IFilmRepository _filmRepository;
        public FilmController(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }
        // GET: api/<FilmController>
        /// <summary>
        /// List all movies on database
        /// </summary>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '404'>Not found: Movies not found</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Film), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Get()
        {
            try
            {
                var films = _filmRepository.GetFilms();
                if (films == null)
                    return NotFound(new { message = "Movies not found" });
                return Ok(_filmRepository.GetFilms());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<FilmController>/5
        /// <summary>
        /// Show a movie by Id
        /// </summary>
        /// <remarks>
        /// 
        /// EXAMPLE:
        ///         id = 2
        /// 
        /// </remarks>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>BadRequest: Exception catched</response>
        /// <response code = '404'>Not found</response>
        /// <param name="id">Film Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Film), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Get(int id)
        {
            try
            {
                var film = _filmRepository.GetFilm(id);
                if (film == null)
                    return NotFound(film);
                return Ok(film);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // POST api/<FilmController>
        /// <summary>
        /// Create a new film
        /// </summary>
        /// <remarks>
        ///     Example:
        ///     
        ///         POST
        ///             {
        ///                 "id":"1",
        ///                 "title":"FROZEN",
        ///                 "direction": "Jennifer Lee",
        ///                 "year": 2019
        ///             }
        /// </remarks>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Read the message</response>
        /// <param name="film"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Film), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Post([FromBody] Film film)
        {
            try
            {
                //if film exists do not create another
                var getFilm = _filmRepository.GetFilms();
                var button = false;
                foreach (var item in getFilm)
                {
                    var bTitle = item.Title.Equals(film.Title);
                    var bDirection = item.Direction.Equals(film.Direction);
                    var bYear = item.Year.Equals(film.Year);

                    if (bTitle == true && bDirection == true && bYear == true)
                    {
                        button = true;
                    }
                }
                if (button == true)
                    return BadRequest($"\t Film: {film.Title}| {film.Direction}| {film.Year}| already exist on database");

                _filmRepository.Create(film);
                return CreatedAtAction(nameof(Get), new { Id = film.Id }, film);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<FilmController>/5
        /// <summary>
        /// Update film
        /// </summary>
        /// <remarks>
        ///     Example:
        ///     
        ///         PUT
        ///             {
        ///                 "id":"1",
        ///                 "title":"new film",
        ///                 "direction": "director",
        ///                 "year": 2000
        ///             }
        /// </remarks>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Read the message</response>
        /// <response code = '404'> Not found: Film not found to be updated</response>
        /// <param name="film">Film Updates</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(Film), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Put([FromBody] Film film)
        {
            try
            {
                //film need to exist on database
                var getFilm = _filmRepository.GetFilms();
                var button = false;
               
                foreach (var item in getFilm)
                {
                    var bId = item.Id.Equals(film.Id);
                    var bTitle = item.Title.Equals(film.Title);
                    var bDirection = item.Direction.Equals(film.Direction);
                    var bYear = item.Year.Equals(film.Year);

                    if (bId == true)
                    {
                        button = true;
                    }
                    
                }
                if (button != true)
                    return NotFound(new { message = "Film not found" });
                
                var upFilm = _filmRepository.Update(film);
                return Ok(upFilm);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<FilmController>/5
        /// <summary>
        /// Delete film by Id
        /// </summary>
        /// <remarks>
        /// Example:
        ///         Id = 2 
        /// </remarks>
        /// <response code = '204'>Success in Delete</response>
        /// <response code = '400'>BadRequest: Exception catched</response>
        /// <reponse code = '404'>Not found: Film not found to be deleted</reponse>
        /// <param name="id">Film Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Delete(int id)
        {
            try
            {
                var film = _filmRepository.GetFilm(id);
                if (film == null)
                    return NotFound(film);
                //locations nao permite que o filme seja apagado

                _filmRepository.Delete(id);
                return NoContent();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
