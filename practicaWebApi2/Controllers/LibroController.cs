using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practicaWebApi2.Models;

namespace practicaWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {

        private readonly BibliotecaContext _librosContexto;

        public LibroController(BibliotecaContext librosContexto)
        {
            _librosContexto = librosContexto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Libro> libros = (from l in _librosContexto.Libro
                                  select l).ToList();
            if (libros.Count() == 0)
            {
                return NotFound();

            }

            return Ok(libros);
        }

        [HttpGet]
        [Route("GetById")]
        public IActionResult GetById(int id)
        {
            var libros = (from l in _librosContexto.Libro
                          join a in _librosContexto.Autor
                          on l.AutorId equals a.Id
                          where l.Id == id
                          select new
                          {
                              l.Titulo,
                              a.Nombre,
                          }).ToList();


            if (libros.Count() == 0)
            {
                return NotFound();

            }

            return Ok(libros);
        }

        [HttpPost]
        [Route("AddLibro")]
        public IActionResult AddLibro([FromBody] Libro libro)
        {
            try
            {
                _librosContexto.Libro.Add(libro);
                _librosContexto.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("Actualizar/{id}")]
        public IActionResult Actualiar(int id, [FromBody] Libro libroActualizar)
        {
            Libro? libroActual = (from l in _librosContexto.Libro
                                  where l.Id == id
                                  select l).FirstOrDefault();

            if (libroActual == null)
            {
                return NotFound();
            }

            libroActual.Titulo = libroActualizar.Titulo;
            libroActual.Anyopublicacion = libroActualizar.Anyopublicacion;
            libroActual.AutorId = libroActualizar.AutorId;
            libroActual.CategoriaId = libroActualizar.CategoriaId;
            libroActual.Resumen = libroActualizar.Resumen;


            _librosContexto.Entry(libroActual).State = EntityState.Modified;
            _librosContexto.SaveChanges();

            return Ok(libroActualizar);

        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            Libro? libro = (from l in _librosContexto.Libro
                            where l.Id == id
                            select l).FirstOrDefault();
            /* var libro = _librosContexto.Libro.Find(id); // Busca directamente por la clave primaria*/

            if (libro == null)
            {
                return NotFound();
            }

            _librosContexto.Attach(libro); // podria omitirse esto
            _librosContexto.Libro.Remove(libro);
            _librosContexto.SaveChanges();

            return Ok(libro);

        }

        [HttpGet]
        [Route("GetLibrosYear")]
        public IActionResult GetLibrosYear()
        {
            List<Libro> libros = (from l in _librosContexto.Libro
                                  where l.Anyopublicacion > 2000
                                  select l).ToList();

            if (!libros.Any())
            {
                return NotFound();

            }

            return Ok(libros);
        }

        [HttpGet]
        [Route("GetCantidadLibros/{id}")]
        public IActionResult GetCantidadLibros(int id)
        {
            var cantidadLibros = (from a in _librosContexto.Autor
                                  join l in _librosContexto.Libro
                                  on a.Id equals l.AutorId
                                  where a.Id == id
                                  group l by a.Nombre into g
                                  select new
                                  {
                                      Nombre = g.Key,
                                      Total = g.Count()
                                  }).ToList();

            if (cantidadLibros == null)
            {
                return NotFound();

            }

            return Ok(cantidadLibros);
        }


        [HttpGet]
        [Route("GetLibrosPaginacion")]
        public IActionResult GetLibrosPaginacion()
        {
            var libros = (from l in _librosContexto.Libro
                          join a in _librosContexto.Autor
                          on l.AutorId equals a.Id
                          select new
                          {
                              l.Titulo,
                              l.Anyopublicacion,
                              l.Resumen,
                              a.Nombre
                          })
                          .Skip(2)
                          .Take(4)
                          .ToList();

            return Ok(libros);
        }

        [HttpGet]
        [Route("GetLibrosFind{titulo}")]
        public IActionResult GetLibrosFind(string titulo)
        {
            var libros = (from l in _librosContexto.Libro
                          join a in _librosContexto.Autor
                          on l.AutorId equals a.Id
                          where l.Titulo.Contains(titulo)
                          select new
                          {
                              l.Titulo,
                              l.Anyopublicacion,
                              l.Resumen,
                              a.Nombre
                          }).ToList();

            return Ok(libros);
        }
        //
    }
}
