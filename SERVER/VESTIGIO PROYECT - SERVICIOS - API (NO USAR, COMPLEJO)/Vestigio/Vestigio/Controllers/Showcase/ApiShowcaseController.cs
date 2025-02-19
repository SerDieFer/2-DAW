using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Models;
using Vestigio.Services;
using Vestigio.Models.DTOs;
using Vestigio.Utilities;

namespace Vestigio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiShowcaseController : Controller
    {
        private readonly IFilterService _filterService;
        private readonly UserManager<User> _userManager;

        public ApiShowcaseController(IFilterService filterService, UserManager<User> userManager)
        {
            _filterService = filterService;
            _userManager = userManager;
        }

        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] FilterRequest request)
        {
            try
            {

                if (User.Identity.IsAuthenticated)
                {
                    var userId = _userManager.GetUserId(User); // Obtener el ID del usuario autenticado
                    var user = await _userManager.Users
                        .Include(u => u.ChallengesResolutions)
                        .Include(u => u.UnlockedProducts)
                            .ThenInclude(up => up.Product)
                        .Include(u => u.UnlockedProductLevels)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user != null)
                    {
                        // Llenar listas desbloqueadas
                        request.UnlockedProductIds = user.UnlockedProducts.Select(p => p.ProductId).ToList();
                        request.UnlockedProductLevels = user.UnlockedProductLevels.Select(l => l.Level).ToList();
                    }
                }

                var result = await _filterService.GetFilteredResultsAsync(request);

                // Mapear nombres lógicos a rutas físicas
                string viewPath = result.ViewName switch
                {
                    "challenges" => "~/Views/Showcase/_ChallengesPartial.cshtml",
                    "products" => "Views/Showcase/_ProductsPartial.cshtml",
                    _ => throw new InvalidOperationException("Pestaña no válida")
                };

                var html = await this.RenderViewToStringAsync(viewPath, result.Data);
                return Ok(new { html });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la API: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

    }
}
