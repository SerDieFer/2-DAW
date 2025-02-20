using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;

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
        public async Task<IActionResult> Index(
         DateTime? startDate, DateTime? endDate,
         decimal? minTotal, decimal? maxTotal,
         int? statusId, int? pageNumber, int pageSize = 5)
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                .Where(o => o.UserId == userId)
                .Select(o => new {
                    Order = o,
                    Total = o.OrderDetails.Sum(d => d.Price * d.Quantity)
                })
                .AsQueryable();

            // Aplicar filtros
            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(x => x.Order.CreationDate >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.Order.CreationDate <= end);
            }

            if (statusId.HasValue)
            {
                query = query.Where(x => x.Order.OrderStatusId == statusId.Value);
            }

            if (minTotal.HasValue)
            {
                query = query.Where(x => x.Total >= minTotal.Value);
            }

            if (maxTotal.HasValue)
            {
                query = query.Where(x => x.Total <= maxTotal.Value);
            }

            // Proyección final
            var finalQuery = query.Select(x => x.Order);

            var paginatedOrders = await PaginatedList<Order>.CreateAsync(
                finalQuery.OrderByDescending(o => o.CreationDate),
                pageNumber ?? 1,
                pageSize
            );

            // Calcular totales
            var ordersWithTotal = paginatedOrders.Select(o => {
                o.Total = o.OrderDetails.Sum(d => d.Price * d.Quantity);
                return o;
            }).ToList();

            ViewBag.Statuses = await _context.OrderStatuses.ToListAsync();
            ViewBag.Filters = new
            {
                startDate,
                endDate,
                minTotal,
                maxTotal,
                statusId
            };

            return View(paginatedOrders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
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
                var user = await _userManager.GetUserAsync(User);

                if (string.IsNullOrWhiteSpace(user.DNI) ||
                    string.IsNullOrWhiteSpace(user.Address) ||
                    string.IsNullOrWhiteSpace(user.City) ||
                    string.IsNullOrWhiteSpace(user.PostalCode) ||
                    string.IsNullOrWhiteSpace(user.FirstName) ||
                    string.IsNullOrWhiteSpace(user.LastName) ||
                    string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    TempData["ErrorMessage"] = "Debes completar tu información personal antes de realizar un pedido.";
                    return Redirect("/Identity/Account/Manage");
                }

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
                        .FromSqlRaw("SELECT * FROM ProductSize WITH (UPDLOCK) WHERE Id = {0}", detail.ProductSizeId)
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
