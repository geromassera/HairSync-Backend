using Application.Interfaces.External;
using Application.Models.External;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JokesController : Controller
    {
        private readonly IJokeService _jokeService;

        public JokesController(IJokeService jokeService)
        {
            _jokeService = jokeService;
        }

        [HttpGet("random")]
        [AllowAnonymous]
        public async Task<ActionResult<JokeDto>> GetRandomJoke()
        {
            var joke = await _jokeService.GetRandomJokeAsync();

            return Ok(joke);
        }
    }
}
