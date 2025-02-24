using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Migrations;
using Vestigio.Models;
using Vestigio.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Vestigio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly VestigioDbContext _context;

        public OrdersController(VestigioDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(
            DateTime? startDate, DateTime? endDate,
            decimal? minTotal, decimal? maxTotal,
            int? statusId, int? pageNumber, string? userEmail, int pageSize = 6)
        {
            // ORDER DATA WITH THEIR USER
            var ordersData = _context.Orders.AsQueryable();

            // SORT ORDERS BY DESCENDING CREATION DATE
            ordersData = ordersData.OrderByDescending(s => s.CreationDate)
                                   .Include(o => o.User)
                                   .Include(o => o.OrderStatus);

            // FILTERS
            if (startDate.HasValue)
                ordersData = ordersData.Where(o => o.CreationDate >= startDate.Value.Date);
            if (endDate.HasValue)
                ordersData = ordersData.Where(o => o.CreationDate <= endDate.Value.Date.AddDays(1).AddTicks(-1));
            if (statusId.HasValue)
                ordersData = ordersData.Where(o => o.OrderStatusId == statusId.Value);
            if (minTotal.HasValue)
                ordersData = ordersData.Where(o => o.Total >= minTotal.Value);
            if (maxTotal.HasValue)
                ordersData = ordersData.Where(o => o.Total <= maxTotal.Value);
            if (!string.IsNullOrEmpty(userEmail))
                ordersData = ordersData.Where(o => o.User.Email.Contains(userEmail));

            var paginatedOrders = await PaginatedList<Order>.CreateAsync(
                ordersData.AsNoTracking(),
                pageNumber ?? 1,
                pageSize
            );

            ViewBag.Statuses = await _context.OrderStatuses.ToListAsync();
            ViewBag.Filters = new { startDate, endDate, minTotal, maxTotal, statusId, userEmail };

            // PAGINATION
            return View(paginatedOrders);
        }

        public async Task<IActionResult> Details(int id)
        {

            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSize)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction("Index");
            }

            ViewBag.Total = order.OrderDetails.Sum(d => d.Price * d.Quantity);
            ViewBag.Statuses = await _context.OrderStatuses.ToListAsync();

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

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,CreationDate,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CreationDate,Status")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {         
                var order = await _context.Orders
                    .Include(o => o.OrderStatus)
                    .Include(o => o.OrderDetails)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == id && o.UserId == o.User.Id);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("Index");
                }
                if (order.OrderStatus.StatusName != "Pending" && order.OrderStatus.StatusName != "Cancelled")
                {
                    TempData["ErrorMessage"] = "Only pending or cancelling orders can be deleted.";
                    return RedirectToAction("Details", new { id });
                }

                _context.OrderDetails.RemoveRange(order.OrderDetails);

                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Order deleted successfully.";

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error deleting order: {ex.Message}";
                return Json(new
                {
                    success = false,
                    message = "Error deleting order: " + ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int newStatusId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Buscar el pedido por id
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Pedido no encontrado.";
                    return RedirectToAction("Index");
                }

                // Validar que el nuevo estado exista en la base de datos
                var status = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Id == newStatusId);
                if (status == null)
                {
                    TempData["ErrorMessage"] = "Estado no válido.";
                    return RedirectToAction("Details", new { id });
                }

                // Actualizar el estado del pedido
                order.OrderStatusId = newStatusId;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Estado del pedido actualizado correctamente.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al actualizar el estado del pedido: " + ex.Message;
            }
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detail = await _context.OrderDetails
                    .Include(d => d.Order)
                        .ThenInclude(o => o.OrderDetails)
                    .FirstOrDefaultAsync(d => d.Id == itemId);

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

                return Json(new { success = true, newTotal = order.Total });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Error al eliminar el ítem: " + ex.Message });
            }
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

        public async Task<IActionResult> GetStock(int productSizeId)
        {
            var stock = await _context.ProductSizes
                .Where(ps => ps.Id == productSizeId)
                .Select(ps => ps.Stock)
                .FirstOrDefaultAsync();

            return Json(stock > 0 ? stock : 0);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
