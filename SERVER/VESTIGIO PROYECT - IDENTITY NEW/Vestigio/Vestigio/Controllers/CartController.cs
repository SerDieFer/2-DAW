using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vestigio.Data;
using Vestigio.Models;

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

    // Muestra el carrito (pedido pendiente)
    public async Task<IActionResult> Index()
    {
        // Obtener el ID del pedido pendiente desde la sesión
        int? orderId = HttpContext.Session.GetInt32("CartOrderId");
        Order order = null;

        if (orderId.HasValue)
        {
            // Buscar el pedido pendiente
            order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSize)
                .FirstOrDefaultAsync(o => o.Id == orderId.Value && o.Status == "Pendiente");
        }

        // Si no existe un pedido pendiente, mostramos un mensaje
        if (order == null)
        {
            TempData["ErrorMessage"] = "No tienes un carrito activo.";
            return RedirectToAction("Index", "Showcase");
        }

        // Enviar el pedido a la vista
        ViewBag.CartOrder = order;
        return View(order);
    }

    // Eliminar un producto del carrito
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int orderDetailId)
    {
        int? orderId = HttpContext.Session.GetInt32("CartOrderId");
        if (!orderId.HasValue)
        {
            TempData["ErrorMessage"] = "No hay carrito activo.";
            return RedirectToAction("Index");
        }

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == orderId.Value && o.Status == "Pendiente");

        if (order == null)
        {
            TempData["ErrorMessage"] = "Pedido no encontrado.";
            return RedirectToAction("Index");
        }

        var detail = order.OrderDetails.FirstOrDefault(od => od.Id == orderDetailId);
        if (detail != null)
        {
            order.OrderDetails.Remove(detail);  // Eliminar de la colección OrderDetails
            _context.OrderDetails.Remove(detail);  // Eliminar de la base de datos
            await _context.SaveChangesAsync();  // Guardar cambios
            TempData["SuccessMessage"] = "Producto removido del carrito.";
        }
        else
        {
            TempData["ErrorMessage"] = "Detalle de pedido no encontrado.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int orderDetailId, int newQuantity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var detail = await _context.OrderDetails
                .Include(od => od.ProductSize)  // Incluyendo ProductSize
                .ThenInclude(ps => ps.Product) // Incluyendo el Product relacionado
                .FirstOrDefaultAsync(od => od.Id == orderDetailId);

            if (detail == null)
            {
                TempData["ErrorMessage"] = "Detalle del pedido no encontrado";
                return RedirectToAction("Index");
            }

            if (detail.ProductSize == null)
            {
                TempData["ErrorMessage"] = "Producto sin tamaño asociado";
                return RedirectToAction("Index");
            }

            if (detail.ProductSize.Product == null)
            {
                TempData["ErrorMessage"] = "Producto asociado no encontrado";
                return RedirectToAction("Index");
            }

            // Verificar que el stock no sea negativo
            if (detail.ProductSize.Stock < 0)
            {
                TempData["ErrorMessage"] = "El stock del producto es inválido";
                return RedirectToAction("Index");
            }

            var stockDifference = newQuantity - detail.Quantity;

            // Verificar si hay suficiente stock
            if (detail.ProductSize.Stock < stockDifference)
            {
                TempData["ErrorMessage"] = "Stock insuficiente para actualizar la cantidad";
                return RedirectToAction("Index");
            }

            // Actualizar el stock y la cantidad
            detail.ProductSize.Stock -= stockDifference;
            detail.Quantity = newQuantity;

            // Verificación de producto antes de acceder al precio
            decimal price = detail.ProductSize.Product.Price;  // Esto debería estar seguro ahora
            detail.Price = price * newQuantity;

            // Guardar cambios y confirmar la transacción
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Cantidad actualizada correctamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Capturamos y mostramos el detalle del error
            TempData["ErrorMessage"] = $"Error al actualizar la cantidad: {ex.Message}\n{ex.StackTrace}";
            return RedirectToAction("Index");
        }
    }


    [HttpPost]
    public async Task<IActionResult> ChangeSize(int orderDetailId, int newProductSizeId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var detail = await _context.OrderDetails
                .Include(od => od.ProductSize)
                .FirstOrDefaultAsync(od => od.Id == orderDetailId);

            var newSize = await _context.ProductSizes
                .FirstOrDefaultAsync(ps => ps.Id == newProductSizeId);

            if (detail == null || newSize == null || newSize.Stock < detail.Quantity)
            {
                TempData["ErrorMessage"] = "No se puede cambiar la talla";
                return RedirectToAction("Index");
            }

            // Restaurar stock talla anterior
            detail.ProductSize.Stock += detail.Quantity;

            // Actualizar nueva talla
            newSize.Stock -= detail.Quantity;
            detail.ProductSizeId = newSize.Id;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Talla actualizada correctamente";
            return RedirectToAction("Index");
        }
        catch
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Error al cambiar la talla";
            return RedirectToAction("Index");
        }
    }

    // Confirmar la compra
    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        int? orderId = HttpContext.Session.GetInt32("CartOrderId");
        if (!orderId.HasValue)
        {
            TempData["ErrorMessage"] = "No hay carrito activo.";
            return RedirectToAction("Index");
        }

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == orderId.Value && o.Status == "Pendiente");

        if (order == null)
        {
            TempData["ErrorMessage"] = "Pedido no encontrado.";
            return RedirectToAction("Index");
        }

        // Cambiar el estado del pedido a "Confirmado"
        order.Status = "Confirmado";
        await _context.SaveChangesAsync();

        // Eliminar el carrito de la sesión
        HttpContext.Session.Remove("CartOrderId");

        TempData["SuccessMessage"] = "Compra confirmada.";
        return RedirectToAction("Index");
    }
}
