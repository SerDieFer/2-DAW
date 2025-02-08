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

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSize)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pendiente");

        if (order == null)
        {
            TempData["ErrorMessage"] = "No tienes un carrito activo.";
            return RedirectToAction("Index", "Showcase");
        }

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int orderDetailId)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pendiente");

        if (order == null)
        {
            TempData["ErrorMessage"] = "Pedido no encontrado.";
            return RedirectToAction("Index");
        }

        var detail = order.OrderDetails.FirstOrDefault(od => od.Id == orderDetailId);
        if (detail != null)
        {
            // Eliminar restauración de stock
            order.OrderDetails.Remove(detail);
            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Producto removido del carrito.";
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
                .Include(od => od.ProductSize)
                .ThenInclude(ps => ps.Product)
                .FirstOrDefaultAsync(od => od.Id == orderDetailId);

            // Validación mejorada
            if (newQuantity > detail.ProductSize.Stock)
            {
                TempData["ErrorMessage"] = $"Stock insuficiente. Máximo disponible: {detail.ProductSize.Stock}";
                return RedirectToAction("Index");
            }

            // Actualizar cantidad sin modificar stock
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
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.Id == newProductSizeId);

            // Validación mejorada de stock
            if (detail.Quantity > newSize.Stock)
            {
                TempData["ErrorMessage"] = $"Stock insuficiente en nueva talla. Disponible: {newSize.Stock}";
                return RedirectToAction("Index");
            }

            // Cambiar talla sin modificar stock
            detail.ProductSizeId = newSize.Id;

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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ConfirmOrder()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSize)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pendiente");

            // Bloquear registros para evitar race conditions
            foreach (var detail in order.OrderDetails)
            {
                var productSize = await _context.ProductSizes
                    .FromSqlRaw("SELECT * FROM ProductSizes WITH (UPDLOCK) WHERE Id = {0}", detail.ProductSizeId)
                    .FirstOrDefaultAsync();

                if (productSize.Stock < detail.Quantity)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente para {productSize.Product.Name}. Disponible: {productSize.Stock}";
                    await transaction.RollbackAsync();
                    return RedirectToAction("Index");
                }

                productSize.UpdateStock(-detail.Quantity);
            }

            order.Status = "Confirmada";
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["SuccessMessage"] = "Compra confirmada correctamente";
        }
        catch
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Error al confirmar la compra";
        }
        return RedirectToAction("Index", "Orders");
    }

}
