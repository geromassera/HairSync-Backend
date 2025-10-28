// Ubicación: TuProyecto.Api/Controllers/TestErrorController.cs

using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Controllers // Asegúrate de usar tu namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestErrorController : ControllerBase
    {
        [HttpGet("error-500")]
        public IActionResult GetError500()
        {
            throw new Exception("Prueba de error 500 (interno)!");
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            throw new NotFoundException("El recurso de prueba solicitado no fue encontrado.");
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            throw new BadRequestException("La solicitud enviada es incorrecta o está malformada.");
        }

        [HttpGet("validation-error")]
        public IActionResult GetValidError()
        {
            throw new ValidException("El campo 'email' no es válido.");
        }

        [HttpGet("forbidden")]
        public IActionResult GetForbidden()
        {
            throw new ForbiddenAccessException("No tienes los permisos necesarios para realizar esta acción.");
        }

        [HttpGet("conflict")]
        public IActionResult GetConflict()
        {
            throw new ConflictException("Ya existe un registro con los datos proporcionados.");
        }
    }
}