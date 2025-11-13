using Application.Exceptions;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("me")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto dto)
        {
            var userId = GetCurrentUserId();

            var updatedUser = await _userService.UpdateUserAsync(userId, dto);

            return Ok(updatedUser);
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();

            var user = await _userService.GetUserByIdAsync(userId);

            if (user is null)
            {
                return NotFound(new { Message = "No se pudo encontrar el perfil del usuario" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Endpoint para eliminar la cuenta del usuario autenticado.
        /// </summary>
        [HttpDelete("me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userId = GetCurrentUserId();

            var success = await _userService.DeleteUserAsync(userId);

            if (!success)
            {
                return NotFound(new { Message = "Usuario no encontrado." });
            }

            return NoContent();
        }

        [HttpGet("barbers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBarbers()
        {
            var barbers = await _userService.GetAllBarbersAsync();
            return Ok(barbers);
        }



        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedException("El token de autenticacion no contiene el id del usuario.");
            }

            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedException("El Id del usuario en el token no es un numero entero valido.");
        }
    }
}
