using Microsoft.AspNetCore.Mvc;
using Vestigio.Models.DTOs;
using Vestigio.Services;
using Vestigio.Utilities;

namespace Vestigio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiShowcaseController : Controller
    {
        private readonly IFilterService _filterService;

        public ApiShowcaseController(IFilterService filterService)  
        {
            _filterService = filterService;
        }

        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] FilterRequest request)
        {
            var result = await _filterService.GetFilteredResultsAsync(request);
            // Si necesitas renderizar una vista parcial a HTML:
            var html = await this.RenderViewToStringAsync(result.ViewName, result.Data);
            return Ok(new { html });
        }
    }
}
