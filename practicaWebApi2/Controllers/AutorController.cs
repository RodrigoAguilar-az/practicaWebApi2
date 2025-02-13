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
    }
}
