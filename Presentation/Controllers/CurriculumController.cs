using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumService _curriculumService;

        public CurriculumController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
        }

        /// <summary>
        /// Permite a cualquier usuario subir un CV (anónimo).
        /// Endpoint: POST /api/curriculum
        /// Content-Type MUST be multipart/form-data
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CurriculumSuccessDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> PostCv([FromForm] CurriculumDto applicationDto)
        {
            var result = await _curriculumService.ProcessCurriculumApplicationAsync(applicationDto);

            return StatusCode(201, result); 
        }
    }
}
