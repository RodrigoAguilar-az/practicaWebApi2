using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practicaWebApi2.Models;

namespace practicaWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {

        private readonly BibliotecaContext _autoresContexto;

        public AutorController(BibliotecaContext autoresContexto)
        {
            _autoresContexto = autoresContexto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Autor> listadoAutor = (from a in _autoresContexto.Autor
                                        select a).ToList();
            if (listadoAutor.Count() == 0)
            {
                return NotFound();

            }

            return Ok(listadoAutor);
        }

        [HttpGet]
        [Route("GetAllByAutor")]
        public IActionResult GetAllByAutor(int id)
        {

            var listadoAutor = (from a in _autoresContexto.Autor
                                join l in _autoresContexto.Libro
                                on a.Id equals l.AutorId
                                where a.Id == id
                                select new
                                {
                                    a.Id,
                                    a.Nombre,
                                    a.Nacionalidad,
                                    l.Titulo,
                                    l.Resumen
                                }).ToList();

            if (listadoAutor.Count() == 0)
            {
                return NotFound();

            }

            return Ok(listadoAutor);
        }

        [HttpPost]
        [Route("AddAutor")]
        public IActionResult AddAutor([FromBody] Autor autor)
        {
            try
            {
                _autoresContexto.Autor.Add(autor);
                _autoresContexto.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPut]
        [Route("Actualizar/{id}")]
        public IActionResult Actualiar(int id, [FromBody] Autor autorActualizar)
        {
            Autor? autorActual = (from a in _autoresContexto.Autor
                                  where a.Id == id
                                  select a).FirstOrDefault();

            if (autorActual == null)
            {
                return NotFound();
            }

            autorActual.Nombre = autorActualizar.Nombre;
            autorActual.Nacionalidad = autorActualizar.Nacionalidad;

            _autoresContexto.Entry(autorActual).State = EntityState.Modified;
            _autoresContexto.SaveChanges();

            return Ok(autorActualizar);

        }

        [HttpDelete]
        [Route("eliminar{id}")]
        public IActionResult Eliminar(int id)
        {
            Autor? autor = (from a in _autoresContexto.Autor
                            where a.Id == id
                            select a).FirstOrDefault();


            if (autor == null)
            {
                return NotFound();
            }

            _autoresContexto.Attach(autor);
            _autoresContexto.Autor.Remove(autor);
            _autoresContexto.SaveChanges();

            return Ok(autor);
        }

        [HttpGet]
        [Route("GetCantidadLibros/{id}")]
        public IActionResult GetCantidadLibros(int id)
        {
            var cantidadLibros = (from a in _autoresContexto.Autor
                                  join l in _autoresContexto.Libro
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
        [Route("GetAutoresTop")]
        public IActionResult GetAutoresTop()
        {
            /*var cantidadLibros = (from a in _autoresContexto.Autor
                                  join l in _autoresContexto.Libro
                                  on a.Id equals l.AutorId
                                  group l by a.Nombre into g
                                  select new
                                  {
                                      Nombre = g.Key,
                                      Total = g.Count()
                                  }).OrderByDescending(tot => tot.Total).Take(3).ToList();*/

            var cantidadLibros = (from a in _autoresContexto.Autor
                                  join l in _autoresContexto.Libro
                                  on a.Id equals l.AutorId
                                  group l by a.Nombre into g
                                  orderby g.Count() descending
                                  select new
                                  {
                                      Nombre = g.Key,
                                      Total = g.Count()
                                  }).Take(3).ToList();

            if (cantidadLibros == null)
            {
                return NotFound();

            }

            return Ok(cantidadLibros);
        }

        [HttpGet]
        [Route("TieneLibros/{id}")]
        public IActionResult TieneLibros(int id)
        {
            // Verificar si el autor existe
            var autorExiste = (from a in _autoresContexto.Autor
                               where a.Id == id
                               select a).FirstOrDefault();

            if (autorExiste == null)
            {
                return NotFound(new { Mensaje = "Autor no encontrado" });
            }

            var cantidadLibros = (from l in _autoresContexto.Libro
                                  where l.AutorId == id
                                  select l).ToList();


            if (cantidadLibros.Count > 0)
            {
                return Ok(new { AutorId = id, TieneLibros = true });
            }
            else
            {
                return Ok(new { AutorId = id, TieneLibros = false });
            }
        }

        [HttpGet]
        [Route("PrimerLibro/{id}")]
        public IActionResult PrimerLibro(int id)
        {
            // Verificar si el autor existe
            var autorExiste = (from a in _autoresContexto.Autor
                               where a.Id == id
                               select a).FirstOrDefault();

            if (autorExiste == null)
            {
                return NotFound(new { Mensaje = "Autor no encontrado" });
            }

            var primerLibro = (from l in _autoresContexto.Libro
                               where l.AutorId == id
                               orderby l.Anyopublicacion ascending
                               select new
                               {
                                   l.Titulo,
                                   l.Anyopublicacion,
                                   l.AutorId,
                               }).FirstOrDefault();


            if (primerLibro != null)
            {
                return Ok(primerLibro);
            }
            else
            {
                return Ok(new { AutorId = id, TieneLibros = false });
            }
        }


    }
}
