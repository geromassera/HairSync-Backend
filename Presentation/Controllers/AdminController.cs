using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Domain.Enums;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICurriculumService _curriculumService;

        public AdminController(IUserService userService, ICurriculumService curriculumService)
        {
            _userService = userService;
            _curriculumService = curriculumService;
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPatch("users/{userId}/role")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangeUserRole(int userId, [FromBody] ChangeRoleRequestDto request)
        {
            await _userService.ChangeUserRoleAsync(userId, request.NewRole);
            return NoContent();
        }

        [HttpPatch("users/{userId}/assign-branch/{branchId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AssignBranchToBarber(int userId, int branchId)
        {
            await _userService.AssignBranchToBarberAsync(userId, branchId);
            return NoContent();
        }

        /// <summary>
        /// Permite a un Admin descargar un CV por el ID de la postulación.
        /// Endpoint: GET /api/admin/curriculum/{curriculumId}/download
        /// </summary>
        [HttpGet("curriculum/{curriculumId}/download")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DownloadCurriculum(int curriculumId)
        {
            var result = await _curriculumService.GetCvFileAsync(curriculumId);

            var contentDisposition = new ContentDisposition
            {
                FileName = result.FileName,
                DispositionType = "attachment"
            };

            Response.Headers["Content-Disposition"] = contentDisposition.ToString(); 

            return new FileStreamResult(result.FileStream, result.ContentType);
        }

        /// <summary>
        /// Endpoint para que el Admin vea una lista de todas las postulaciones de CV.
        /// Endpoint: GET /api/admin/curriculums
        /// </summary>
        [HttpGet("curriculums")]
        [ProducesResponseType(typeof(IEnumerable<CurriculumListDto>), 200)]
        public async Task<IActionResult> GetAllCurriculums()
        {
            var curriculums = await _curriculumService.GetAllCurriculumsAsync();
            return Ok(curriculums);
        }
    }
}
