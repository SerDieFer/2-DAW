using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;

namespace Vestigio.Controllers
{
    public class ShowcaseController : Controller
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ShowcaseController(VestigioDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            // Cargar los desafíos que estarán visibles para todos
            var challenges = await _context.Challenges
                .Include(c => c.Product)
                .Where(c => c.IsActive)
                .ToListAsync();

            // Valores por defecto para la vista
            int userLevel = 1;

            // Si el usuario está autenticado, se cargan datos específicos
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.Users
                    .Include(u => u.ChallengesResolutions)
                    .Include(u => u.UnlockedProducts)
                    .Include(u => u.UnlockedProductLevels)
                    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

                if (user != null)
                {
                    user.Level = user.Level < 1 ? 1 : user.Level;
                    userLevel = user.Level;

                    // Cargar productos desbloqueados
                    var unlockedProductIds = user.UnlockedProducts.Select(up => up.ProductId).ToList();
                    var unlockedLevels = user.UnlockedProductLevels.Select(ul => ul.Level).ToList();

                    var unlockedProducts = await _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.ProductSizes)
                        .Where(p => p.IsActive &&
                            (unlockedProductIds.Contains(p.Id) || unlockedLevels.Contains(p.RarityLevel)))
                        .ToListAsync();

                    ViewBag.SolvedChallenges = user.ChallengesResolutions?.Select(cr => cr.ChallengeId).ToList() ?? new List<int>();
                    ViewBag.UnlockedProducts = unlockedProducts;
                    ViewBag.UserLevel = userLevel;
                }
            }

            ViewBag.Challenges = challenges;

            return View(challenges);
        }

        [HttpPost]
        public async Task<IActionResult> SolveChallenge(int challengeId, string password)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var user = await _userManager.Users
                .Include(u => u.ChallengesResolutions)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction("Index");
            }

            // Obtener el desafío
            var challenge = await _context.Challenges
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == challengeId);

            if (challenge == null)
            {
                TempData["ErrorMessage"] = "Desafío no encontrado.";
                return RedirectToAction("Index");
            }

            // Validar si el desafío puede ser resuelto
            if (!ValidateChallengeAttempt(user, challenge, password))
            {
                TempData["ErrorMessage"] = "No se pudo resolver el desafío";
                return RedirectToAction("Index");
            }

            // Crear la resolución del desafío
            var resolution = new ChallengeResolution
            {
                UserId = user.Id,
                ChallengeId = challengeId,
                ResolutionDate = DateTime.UtcNow,
                CoinsEarned = challenge.Coins,
                PointsEarned = challenge.ExpPoints
            };

            // Actualizar la EXP y el nivel del usuario
            // Esto maneja la subida de nivel y el progreso automáticamente
            user.GainExp(challenge.ExpPoints);

            // Actualizar al usuario en la base de datos
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                TempData["ErrorMessage"] = $"Error al actualizar el usuario: {errors}";
                return RedirectToAction("Index");
            }

            // Refrescar la cookie de autenticación
            await _signInManager.RefreshSignInAsync(user);

            // Desbloquear recompensas
            UnlockRewards(user, challenge);

            // Guardar resolución del desafío
            _context.ChallengeResolutions.Add(resolution);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Desafío resuelto!";
            return RedirectToAction(nameof(Index));
        }

        private bool ValidateChallengeAttempt(User user, Challenge challenge, string password)
        {
            if (user == null || challenge == null) return false;

            // Validar nivel
            if (challenge.RarityLevel > user.Level) return false;

            // Validar contraseña
            if (challenge.SolutionMode == SolutionMode.Password && challenge.Password != password) return false;

            // Validar tiempo
            if (challenge.SolutionMode == SolutionMode.TimeBased && !challenge.IsPublic) return false;

            // Validar si ya fue resuelto
            if (user.ChallengesResolutions?.Any(cr => cr.ChallengeId == challenge.Id) == true) return false;

            return true;
        }

        // Método para desbloquear recompensas al usuario
        private void UnlockRewards(User user, Challenge challenge)
        {
            if (challenge.ProductId.HasValue)
            {
                if (!user.UnlockedProducts.Any(up => up.ProductId == challenge.ProductId.Value))
                {
                    user.UnlockedProducts.Add(new UserUnlockedProduct { ProductId = challenge.ProductId.Value });
                }
            }
            else if (challenge.ProductLevel.HasValue)
            {
                if (!user.UnlockedProductLevels.Any(ul => ul.Level == challenge.ProductLevel.Value))
                {
                    user.UnlockedProductLevels.Add(new UserUnlockedProductByLevel { Level = challenge.ProductLevel.Value });
                }
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productSizeId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null) return Redirect("/Identity/Account/Login");

                // Verificar si existe un pedido activo en la sesión actual
                if (!HttpContext.Session.TryGetValue("CurrentOrderId", out byte[] orderIdBytes))
                {
                    // Crear nuevo pedido si no existe
                    var newOrder = new Order
                    {
                        UserId = userId,
                        CreationDate = DateTime.Now,
                        OrderStatusId = 1,
                        OrderDetails = new List<OrderDetail>()
                    };
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                    // Guardar el ID del pedido en la sesión
                    HttpContext.Session.SetInt32("CurrentOrderId", newOrder.Id);
                }

                // Obtener el ID del pedido actual desde la sesión
                var currentOrderId = HttpContext.Session.GetInt32("CurrentOrderId").Value;

                // Cargar el producto y validar stock
                var productSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == productSizeId);

                if (productSize.Stock < quantity)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente. Disponible: {productSize.Stock}";
                    return RedirectToAction("Index");
                }

                // Cargar el pedido actual
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == currentOrderId);

                // Calcular cantidad existente en el carrito
                int existingQuantity = order.OrderDetails
                    .Where(od => od.ProductSizeId == productSizeId)
                    .Sum(od => od.Quantity);

                if (existingQuantity + quantity > productSize.Stock)
                {
                    TempData["ErrorMessage"] = $"Límite de stock alcanzado. Disponible: {productSize.Stock - existingQuantity}";
                    return RedirectToAction("Index");
                }

                // Agregar o actualizar detalle sin modificar stock
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
    }
}
