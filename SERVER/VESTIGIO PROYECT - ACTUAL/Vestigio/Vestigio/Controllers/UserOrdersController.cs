using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;

namespace Vestigio.Controllers
{
    [Authorize(Roles = "User")]
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
            int? statusId, int? pageNumber, int pageSize = 6)
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSize)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            // Aplicar filtros
            if (startDate.HasValue)
                query = query.Where(o => o.CreationDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(o => o.CreationDate <= endDate.Value.Date.AddDays(1).AddTicks(-1));

            if (statusId.HasValue)
                query = query.Where(o => o.OrderStatusId == statusId.Value);

            if (minTotal.HasValue)
                query = query.Where(o => o.Total >= minTotal.Value);

            if (maxTotal.HasValue)
                query = query.Where(o => o.Total <= maxTotal.Value);

            var paginatedOrders = await PaginatedList<Order>.CreateAsync(
                query.OrderByDescending(o => o.CreationDate),
                pageNumber ?? 1,
                pageSize
            );

            ViewBag.Statuses = await _context.OrderStatuses.ToListAsync();
            ViewBag.Filters = new { startDate, endDate, minTotal, maxTotal, statusId };

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

            ViewBag.Total = order.OrderDetails.Sum(d => d.Price * d.Quantity);

            ViewBag.Sizes = await _context.ProductSizes
                 .Select(ps => new SelectListItem
                 {
                     Value = ps.Id.ToString(),
                     Text = ps.Size
                 })
                 .ToListAsync();

            foreach (var detail in order.OrderDetails)
            {
                await _context.Entry(detail.Product)
                    .Collection(p => p.ProductSizes)
                    .LoadAsync();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, List<OrderDetail> orderDetails)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                            .ThenInclude(p => p.ProductSizes)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Pedido no encontrado." });
                }

                foreach (var detail in orderDetails)
                {
                    var existingDetail = order.OrderDetails.FirstOrDefault(od => od.Id == detail.Id);
                    if (existingDetail != null)
                    {
                        var isValidSize = existingDetail.Product.ProductSizes.Any(ps => ps.Id == detail.ProductSizeId);
                        if (!isValidSize)
                        {
                            return Json(new { success = false, message = "Talla inválida para el producto" });
                        }

                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.ProductSizeId = detail.ProductSizeId;
                        existingDetail.Price = existingDetail.Product.Price; // Precio unitario
                    }
                }

                order.Total = order.OrderDetails.Sum(od => od.Price * od.Quantity); // Actualizar total
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, newTotal = order.Total });
            }
            catch
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Error al actualizar el pedido" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = _userManager.GetUserId(User);
                var detail = await _context.OrderDetails
                    .Include(d => d.Order)
                        .ThenInclude(o => o.OrderDetails)
                    .FirstOrDefaultAsync(d => d.Id == itemId && d.Order.UserId == userId);

                if (detail == null)
                    return Json(new { success = false, message = "Ítem no encontrado." });

                var order = detail.Order;
                _context.OrderDetails.Remove(detail);

                // Actualizar el total del pedido
                order.Total = order.OrderDetails
                    .Where(od => od.Id != itemId)
                    .Sum(od => od.Price * od.Quantity);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, newTotal = order.Total }); // Devolver el nuevo total
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Error al eliminar el ítem: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.GetUserAsync(User);

                // Validar datos del usuario
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
                    .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

                if (order == null || order.OrderStatusId != 1 || !order.OrderDetails.Any())
                {
                    TempData["ErrorMessage"] = "Pedido no válido para confirmar";
                    return RedirectToAction("Index");
                }

                order.Total = order.OrderDetails.Sum(od => od.Price * od.Quantity); // Actualizar total final

                foreach (var detail in order.OrderDetails)
                {
                    var productSize = await _context.ProductSizes
                        .FromSqlRaw("SELECT * FROM ProductSize WITH (UPDLOCK) WHERE Id = {0}", detail.ProductSizeId)
                        .FirstOrDefaultAsync();

                    if (productSize == null || productSize.Stock < detail.Quantity)
                    {
                        TempData["ErrorMessage"] = $"Stock insuficiente para {productSize?.Product.Name}";
                        await transaction.RollbackAsync();
                        return RedirectToAction("Details", new { id });
                    }
                    productSize.Stock -= detail.Quantity;
                }

                order.OrderStatusId = 2; // Confirmado
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Pedido confirmado correctamente.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al confirmar el pedido";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = _userManager.GetUserId(User);

                var order = await _context.Orders
                    .Include(o => o.OrderStatus) // Asegurar que OrderStatus esté incluido
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Pedido no encontrado.";
                    return RedirectToAction("Index");
                }

                if (order.OrderStatus.StatusName != "Pending")
                {
                    TempData["ErrorMessage"] = "Solo se pueden eliminar pedidos pendientes.";
                    return RedirectToAction("Details", new { id });
                }

                // Eliminar detalles primero
                _context.OrderDetails.RemoveRange(order.OrderDetails);

                // Eliminar el pedido
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Pedido eliminado correctamente.";
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error al eliminar el pedido: {ex.Message}";
                return Json(new
                {
                    success = false,
                    message = "Error al eliminar el pedido: " + ex.Message
                });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetStock(int productSizeId)
        {
            var stock = await _context.ProductSizes
                .Where(ps => ps.Id == productSizeId)
                .Select(ps => ps.Stock)
                .FirstOrDefaultAsync();

            return Json(stock > 0 ? stock : 0);
        }

        public async Task<IActionResult> ProductModal(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound();

            return PartialView("~/Views/UserOrders/_ProductInfo.cshtml", product);
        }
    }
}