using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        private readonly ICustomAuthenticationService _customAuthenticationService;
        private readonly IUserService _userService;


        public AuthenticationController(
            ICustomAuthenticationService customAuthenticationService,
            IUserService userService)
        {
            _customAuthenticationService = customAuthenticationService;
            _userService = userService;
        }

        /// <summary>
        /// Endpoint para iniciar sesión y obtener un token JWT.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto loginDto)
        {
            var authResult = await _customAuthenticationService.Autenticar(loginDto);

            return Ok(authResult);
        }

        /// <summary>
        /// Endpoint para registrar un nuevo usuario.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            var newUserDto = await _userService.CreateUserAsync(registerDto);

            return Ok(newUserDto);
        }
    }
}
