using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;

namespace Vestigio.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(VestigioDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Muestra el carrito: si no hay "CurrentOrderId" en sesión, se muestra un carrito vacío.
        public async Task<IActionResult> Index()
        {
            var currentOrderId = HttpContext.Session.GetInt32("CurrentOrderId");
            if (currentOrderId == null)
            {
                // No hay pedido en la sesión, se muestra un carrito vacío.
                return View(new Order());
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductSizes)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSize)
                .FirstOrDefaultAsync(o => o.Id == currentOrderId.Value && o.OrderStatusId == 1);

            if (order == null)
            {
                // Si por alguna razón el pedido no se encuentra (por ejemplo, ya fue confirmado)
                // se elimina la variable de sesión y se muestra un carrito vacío.
                HttpContext.Session.Remove("CurrentOrderId");
                return View(new Order());
            }

            return View(order);
        }

        // Agrega un producto al carrito
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productSizeId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Redirect("/Identity/Account/Login");

                // Se obtiene el pedido actual desde la sesión
                var currentOrderId = HttpContext.Session.GetInt32("CurrentOrderId");
                Order order;
                if (currentOrderId == null)
                {
                    // No existe pedido en la sesión: se crea uno nuevo
                    order = new Order
                    {
                        UserId = userId,
                        CreationDate = DateTime.Now,
                        OrderStatusId = 1, // 1 = Pending
                        OrderDetails = new List<OrderDetail>()
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Se guarda el ID del nuevo pedido en la sesión
                    HttpContext.Session.SetInt32("CurrentOrderId", order.Id);
                }
                else
                {
                    // Se intenta cargar el pedido pendiente de la sesión
                    order = await _context.Orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefaultAsync(o => o.Id == currentOrderId.Value && o.OrderStatusId == 1);

                    if (order == null)
                    {
                        // En caso de que exista la variable de sesión pero el pedido ya no esté pendiente,
                        // se crea un nuevo pedido.
                        order = new Order
                        {
                            UserId = userId,
                            CreationDate = DateTime.Now,
                            OrderStatusId = 1,
                            OrderDetails = new List<OrderDetail>()
                        };
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetInt32("CurrentOrderId", order.Id);
                    }
                }

                // Se carga la talla del producto y se valida el stock
                var productSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == productSizeId);

                if (productSize == null)
                {
                    TempData["ErrorMessage"] = "Producto no encontrado.";
                    return RedirectToAction("Index");
                }

                if (productSize.Stock < quantity)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente. Disponible: {productSize.Stock}";
                    return RedirectToAction("Index");
                }

                // Se calcula la cantidad ya agregada al carrito para esta talla
                int existingQuantity = order.OrderDetails
                    .Where(od => od.ProductSizeId == productSizeId)
                    .Sum(od => od.Quantity);

                if (existingQuantity + quantity > productSize.Stock)
                {
                    TempData["ErrorMessage"] = $"Límite de stock alcanzado. Disponible: {productSize.Stock - existingQuantity}";
                    return RedirectToAction("Index");
                }

                // Se agrega el detalle o se actualiza si ya existe
                var existingDetail = order.OrderDetails.FirstOrDefault(od => od.ProductSizeId == productSizeId);
                if (existingDetail != null)
                {
                    existingDetail.Quantity += quantity;
                    existingDetail.Price = productSize.Product.Price * existingDetail.Quantity;
                }
                else
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductSizeId = productSizeId,
                        ProductId = productSize.Product.Id,
                        Quantity = quantity,
                        Price = productSize.Product.Price * quantity
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Producto agregado al carrito";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
            }
            return RedirectToAction("Index");
        }

        // Remueve un producto del carrito
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int orderDetailId)
        {
            var currentOrderId = HttpContext.Session.GetInt32("CurrentOrderId");
            if (currentOrderId == null)
            {
                TempData["ErrorMessage"] = "No hay pedido activo.";
                return RedirectToAction("Index");
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == currentOrderId.Value && o.OrderStatusId == 1);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                HttpContext.Session.Remove("CurrentOrderId");
                return RedirectToAction("Index");
            }

            var detail = order.OrderDetails.FirstOrDefault(od => od.Id == orderDetailId);
            if (detail != null)
            {
                order.OrderDetails.Remove(detail);
                _context.OrderDetails.Remove(detail);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Producto removido del carrito.";
            }

            return RedirectToAction("Index");
        }

        // Actualiza la cantidad de un producto en el carrito
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int orderDetailId, int newQuantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detail = await _context.OrderDetails
                    .Include(od => od.ProductSize)
                    .ThenInclude(ps => ps.Product)
                    .FirstOrDefaultAsync(od => od.Id == orderDetailId);

                if (detail == null)
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
                detail.Price = detail.ProductSize.Product.Price * newQuantity;

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

        // Cambia la talla de un producto en el carrito
        [HttpPost]
        public async Task<IActionResult> ChangeSize(int orderDetailId, int newProductSizeId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detail = await _context.OrderDetails
                    .Include(od => od.ProductSize)
                        .ThenInclude(ps => ps.Product)
                    .FirstOrDefaultAsync(od => od.Id == orderDetailId);

                if (detail == null)
                {
                    TempData["ErrorMessage"] = "Detalle no encontrado.";
                    return RedirectToAction("Index");
                }

                var newSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == newProductSizeId);

                // Validaciones
                if (newSize == null)
                {
                    TempData["ErrorMessage"] = "Talla no encontrada.";
                    return RedirectToAction("Index");
                }

                if (newSize.ProductId != detail.ProductSize.ProductId)
                {
                    TempData["ErrorMessage"] = "No puedes cambiar a un producto diferente.";
                    return RedirectToAction("Index");
                }

                if (newSize.Stock < detail.Quantity)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente. Disponible: {newSize.Stock}";
                    return RedirectToAction("Index");
                }

                // Actualizar talla
                detail.ProductSizeId = newSize.Id;
                detail.ProductId = newSize.ProductId;
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

        // Confirma el pedido: utiliza el pedido pendiente que se encuentre en la sesión y luego elimina la variable de sesión.
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
                    TempData["ErrorMessage"] = "No hay pedido activo para confirmar.";
                    return RedirectToAction("Index");
                }

                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSize)
                    .ThenInclude(ps => ps.Product)
                    .FirstOrDefaultAsync(o => o.Id == currentOrderId.Value && o.OrderStatusId == 1);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Pedido no encontrado o ya confirmado.";
                    HttpContext.Session.Remove("CurrentOrderId");
                    return RedirectToAction("Index");
                }

                // Se actualiza el stock con un bloqueo (UPDLOCK) para evitar race conditions
                foreach (var detail in order.OrderDetails)
                {
                    var productSize = await _context.ProductSizes
                        .FromSqlRaw("SELECT * FROM ProductSize WITH (UPDLOCK) WHERE Id = {0}", detail.ProductSizeId)
                        .FirstOrDefaultAsync();

                    if (productSize == null || productSize.Stock < detail.Quantity)
                    {
                        TempData["ErrorMessage"] = $"Stock insuficiente para {productSize?.Product?.Name ?? "el producto"}. Disponible: {productSize?.Stock ?? 0}";
                        await transaction.RollbackAsync();
                        return RedirectToAction("Index");
                    }

                    productSize.UpdateStock(-detail.Quantity);
                }

                order.OrderStatusId = 2; // Se asume que 2 representa un pedido confirmado
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Se elimina la variable de sesión para que, al iniciar una nueva sesión, no se muestre este pedido pendiente.
                HttpContext.Session.Remove("CurrentOrderId");
                TempData["SuccessMessage"] = "Compra confirmada correctamente";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al confirmar la compra: " + ex.Message;
            }

            return RedirectToAction("Index", "Orders");
        }

    }
}
