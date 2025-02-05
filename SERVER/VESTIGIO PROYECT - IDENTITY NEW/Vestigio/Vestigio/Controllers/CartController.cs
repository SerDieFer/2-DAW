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
            // Restaurar stock
            var productSize = await _context.ProductSizes.FindAsync(detail.ProductSizeId);
            if (productSize != null)
            {
                productSize.UpdateStock(detail.Quantity);
                await _context.SaveChangesAsync();
            }

            order.OrderDetails.Remove(detail);
            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();

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
                .Include(od => od.ProductSize)
                .ThenInclude(ps => ps.Product)
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

            int stockDifference = newQuantity - detail.Quantity;
            if (detail.ProductSize.Stock < stockDifference)
            {
                TempData["ErrorMessage"] = "Stock insuficiente para actualizar la cantidad";
                return RedirectToAction("Index");
            }

            // Usar UpdateStock para ajustar el stock
            detail.ProductSize.UpdateStock(-stockDifference);
            detail.Quantity = newQuantity;
            detail.Price = detail.ProductSize.Product.Price * newQuantity;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Cantidad actualizada correctamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = $"Error al actualizar la cantidad: {ex.Message}";
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
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.Id == newProductSizeId);

            if (detail == null || newSize == null ||
                newSize.Stock < detail.Quantity ||
                detail.ProductId != newSize.ProductId) // Validar mismo producto
            {
                TempData["ErrorMessage"] = "No se puede cambiar la talla";
                return RedirectToAction("Index");
            }

            // Restaurar stock talla anterior
            detail.ProductSize.UpdateStock(detail.Quantity);

            // Actualizar nueva talla
            newSize.UpdateStock(-detail.Quantity);
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ConfirmOrder()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Redirect("/Identity/Account/Login");

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSize)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pendiente");

            if (order == null || !order.OrderDetails.Any())
            {
                TempData["ErrorMessage"] = "No tienes productos en el carrito";
                return RedirectToAction("Index", "Cart");
            }

            // Verificar stock antes de confirmar
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Quantity > detail.ProductSize.Stock)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente para {detail.ProductSize.Product.Name}. Disponible: {detail.ProductSize.Stock}";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Restar stock
            foreach (var detail in order.OrderDetails)
            {
                detail.ProductSize.UpdateStock(-detail.Quantity);
            }

            // Cambiar estado de la orden a "Confirmada"
            order.Status = "Confirmada";
            //order.ConfirmationDate = DateTime.Now;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Compra confirmada";
            return RedirectToAction("Index", "Orders");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Error al procesar la compra";
            return RedirectToAction("Index", "Cart");
        }
    }

}
