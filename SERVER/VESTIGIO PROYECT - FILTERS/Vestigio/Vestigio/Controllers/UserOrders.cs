using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;

namespace Vestigio.Controllers
{
    [Authorize]
    public class UserOrdersController : Controller
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserOrdersController(VestigioDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Lista todos los pedidos realizados por el usuario
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Include(o => o.OrderStatus)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreationDate)
                .ToListAsync();

            return View(orders);
        }

        // 2. Muestra el detalle de un pedido en particular
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSize)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // 3. Confirma el pedido pendiente (actualiza stock y cambia estado)
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userId = _userManager.GetUserId(User);

                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductSize)
                            .ThenInclude(ps => ps.Product)
                    .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Pedido no encontrado.";
                    return RedirectToAction("Index");
                }

                if (order.OrderStatusId != 1) // Estado pendiente
                {
                    TempData["ErrorMessage"] = "El pedido no se encuentra en estado pendiente.";
                    return RedirectToAction("Details", new { id = id });
                }

                foreach (var detail in order.OrderDetails)
                {
                    var productSize = await _context.ProductSizes
                        .FromSqlRaw("SELECT * FROM ProductSizes WITH (UPDLOCK) WHERE Id = {0}", detail.ProductSizeId)
                        .FirstOrDefaultAsync();

                    if (productSize == null || productSize.Stock < detail.Quantity)
                    {
                        TempData["ErrorMessage"] = $"Stock insuficiente para {productSize?.Product.Name ?? "el producto"}. Disponible: {productSize?.Stock}";
                        await transaction.RollbackAsync();
                        return RedirectToAction("Details", new { id = id });
                    }

                    productSize.UpdateStock(-detail.Quantity);
                }

                order.OrderStatusId = 2; // Estado confirmado
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Pedido confirmado correctamente.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al confirmar el pedido.";
            }

            return RedirectToAction("Details", new { id = id });
        }
    }
}
