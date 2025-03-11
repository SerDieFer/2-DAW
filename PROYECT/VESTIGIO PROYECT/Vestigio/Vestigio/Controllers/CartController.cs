using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;

namespace Vestigio.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(VestigioDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Muestra el carrito
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var currentOrderId = HttpContext.Session.GetInt32("CurrentOrderId");

            if (currentOrderId == null)
            {
                return View(new Order());
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductSizes)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSize)
                .FirstOrDefaultAsync(o => o.Id == currentOrderId.Value && o.OrderStatusId == 1);

            if (order == null)
            {
                HttpContext.Session.Remove("CurrentOrderId");
                return View(new Order());
            }

            PrepareOrderViewData(order);
            return View(order);
        }

        // Actualiza el carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCart(int orderId, List<OrderDetail> orderDetails)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductSize)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Pedido no encontrado.";
                    return RedirectToAction("Index");
                }

                foreach (var detail in orderDetails)
                {
                    var existingDetail = order.OrderDetails.FirstOrDefault(od => od.Id == detail.Id);
                    if (existingDetail != null)
                    {
                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.ProductSizeId = detail.ProductSizeId;

                        var productSize = await _context.ProductSizes
                            .Include(ps => ps.Product)
                            .FirstOrDefaultAsync(ps => ps.Id == existingDetail.ProductSizeId);

                        existingDetail.Price = productSize?.Product.Price ?? 0; // Precio unitario
                    }
                }

                order.Total = order.OrderDetails.Sum(od => od.Price * od.Quantity); // Actualizar total
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Carrito actualizado correctamente.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error al actualizar el carrito: {ex.Message}";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int orderDetailId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obtener el usuario actual
                var userId = _userManager.GetUserId(User);

                // Buscar el detalle de la orden
                var orderDetail = await _context.OrderDetails
                    .Include(od => od.Order) // Incluir la orden asociada
                        .ThenInclude(o => o.OrderDetails) // Incluir los detalles de la orden
                    .FirstOrDefaultAsync(od =>
                        od.Id == orderDetailId &&
                        od.Order.UserId == userId); // Solo permite eliminar ítems del usuario actual

                if (orderDetail == null)
                {
                    return Json(new { success = false, message = "Item not found or cannot be deleted." });
                }

                // Obtener la orden asociada
                var order = orderDetail.Order;

                // Eliminar el ítem
                _context.OrderDetails.Remove(orderDetail);

                // Verificar si es el último ítem
                if (order.OrderDetails.Count == 1) // Si solo queda este ítem
                {
                    // Eliminar la orden completa
                    _context.Orders.Remove(order);

                    // Limpiar la sesión si es la orden actual
                    if (HttpContext.Session.GetInt32("CurrentOrderId") == order.Id)
                    {
                        HttpContext.Session.Remove("CurrentOrderId");
                    }
                }
                else
                {
                    // Actualizar el total de la orden
                    order.Total = order.OrderDetails
                        .Where(od => od.Id != orderDetailId) // Excluir el ítem eliminado
                        .Sum(od => od.Price * od.Quantity);
                }

                // Guardar cambios en la base de datos
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Respuesta JSON
                return Json(new
                {
                    success = true,
                    orderDeleted = order.OrderDetails.Count == 1, // Indica si la orden fue eliminada
                    newTotal = order.OrderDetails.Count == 1 ? 0 : order.Total, // Nuevo total (0 si la orden fue eliminada)
                    redirectUrl = order.OrderDetails.Count == 1 ? Url.Action("Index", "Cart") : null // Redirigir si la orden fue eliminada
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = $"Error deleting item: {ex.Message}" });
            }
        }

        // Actualiza la cantidad de un producto
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int orderDetailId, int newQuantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detail = await _context.OrderDetails
                    .Include(od => od.Order)
                    .Include(od => od.ProductSize)
                        .ThenInclude(ps => ps.Product)
                    .FirstOrDefaultAsync(od => od.Id == orderDetailId);

                if (detail == null || detail.Order == null)
                {
                    TempData["ErrorMessage"] = "Detalle no encontrado.";
                    return RedirectToAction("Index");
                }

                if (newQuantity > detail.ProductSize.Stock)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente. Máximo disponible: {detail.ProductSize.Stock}";
                    return RedirectToAction("Index");
                }

                detail.Quantity = newQuantity;
                detail.Order.Total = detail.Order.OrderDetails
                    .Sum(od => od.Price * od.Quantity); // Actualizar total

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Cantidad actualizada correctamente";

            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al actualizar la cantidad";
            }
            return RedirectToAction("Index");
        }

        // Cambia la talla de un producto
        [HttpPost]
        public async Task<IActionResult> ChangeSize(int orderDetailId, int newProductSizeId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detail = await _context.OrderDetails
                    .Include(od => od.Order)
                    .Include(od => od.ProductSize)
                        .ThenInclude(ps => ps.Product)
                    .FirstOrDefaultAsync(od => od.Id == orderDetailId);

                if (detail == null || detail.Order == null)
                {
                    TempData["ErrorMessage"] = "Detalle no encontrado.";
                    return RedirectToAction("Index");
                }

                var newSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == newProductSizeId);

                if (newSize == null || newSize.ProductId != detail.ProductSize.ProductId)
                {
                    TempData["ErrorMessage"] = "Talla no válida.";
                    return RedirectToAction("Index");
                }

                if (detail.Quantity > newSize.Stock)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente. Disponible: {newSize.Stock}";
                    return RedirectToAction("Index");
                }

                detail.ProductSizeId = newSize.Id;
                detail.Price = newSize.Product.Price; // Actualizar precio unitario
                detail.Order.Total = detail.Order.OrderDetails
                    .Sum(od => od.Price * od.Quantity); // Actualizar total

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Talla actualizada correctamente";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al cambiar la talla";
            }
            return RedirectToAction("Index");
        }

        // Confirmar pedido
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.GetUserAsync(User);

                // Verificar que los campos obligatorios estén completos
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

                var currentOrderId = HttpContext.Session.GetInt32("CurrentOrderId");
                if (!currentOrderId.HasValue)
                {
                    TempData["ErrorMessage"] = "No hay pedido activo.";
                    return RedirectToAction("Index");
                }

                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductSize)
                    .FirstOrDefaultAsync(o => o.Id == currentOrderId.Value);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Pedido no encontrado.";
                    return RedirectToAction("Index");
                }

                order.Total = order.OrderDetails.Sum(od => od.Price * od.Quantity); // Última actualización
                order.OrderStatusId = 2; // Confirmado

                foreach (var detail in order.OrderDetails) // Actualizar stock
                {
                    var productSize = await _context.ProductSizes
                        .FromSqlRaw("SELECT * FROM ProductSize WITH (UPDLOCK) WHERE Id = {0}", detail.ProductSizeId)
                        .FirstOrDefaultAsync();

                    productSize.Stock -= detail.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                HttpContext.Session.Remove("CurrentOrderId");
                TempData["SuccessMessage"] = "Compra confirmada correctamente";
                return RedirectToAction("Details", "UserOrders", new { id = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error al confirmar: {ex.Message}";
                return RedirectToAction("Index", "Cart");
            }
        }

        // Métodos auxiliares
        private void PrepareOrderViewData(Order order)
        {
            ViewBag.Total = order.Total;
            ViewBag.IsPending = order.OrderStatusId == 1;
            ViewBag.ItemsCount = order.OrderDetails.Count;
            ViewBag.IsLastItem = ViewBag.ItemsCount == 1;
        }

        // GET: Obtener stock (para la vista)
        public async Task<IActionResult> GetStock(int productSizeId)
        {
            var productSize = await _context.ProductSizes.FindAsync(productSizeId);
            return productSize == null ? NotFound() : Ok(productSize.Stock);
        }
    }
}