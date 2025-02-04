    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Vestigio.Data;
    using Vestigio.Models;
    using Vestigio.Utilities;

    namespace Vestigio.Controllers
    {
        [Authorize(Roles = "User")]
        public class ShowcaseController : Controller
        {
            private readonly VestigioDbContext _context;
            private readonly UserManager<User> _userManager;

            public ShowcaseController(VestigioDbContext context, UserManager<User> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task<IActionResult> Index()
            {
                var user = await _userManager.Users
                    .Include(u => u.ChallengesResolutions)
                    .Include(u => u.UnlockedProducts)
                    .Include(u => u.UnlockedProductLevels)
                    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

                if (user == null) return NotFound();

                // Forzar inicialización si es necesario
                user.Level = user.Level < 1 ? 1 : user.Level;

                // Cargar desafíos
                var challenges = await _context.Challenges
                    .Include(c => c.Product)
                    .Where(c => c.IsActive)
                    .ToListAsync();

                // Cargar productos desbloqueados
                var unlockedProductIds = user.UnlockedProducts.Select(up => up.ProductId).ToList();
                var unlockedLevels = user.UnlockedProductLevels.Select(ul => ul.Level).ToList();

                var unlockedProducts = await _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.Sizes)
                    .Where(p => p.IsActive &&
                        (unlockedProductIds.Contains(p.Id) || unlockedLevels.Contains(p.RarityLevel)))
                    .ToListAsync();

                // Pasar datos a la vista
                ViewBag.UserLevel = user.Level;
                ViewBag.SolvedChallenges = user.ChallengesResolutions?.Select(cr => cr.ChallengeId).ToList() ?? new List<int>();
                ViewBag.UnlockedProducts = unlockedProducts;

                return View(challenges);
            }

            // ------------------------ CHALLENGE RESOLUTION METHODS -----------------------------


            // ACTION TO GIVE A SHOWCASE TO THE USER

            // ACTION TO SOLVE CHALLENGE
            [HttpPost]
            public async Task<IActionResult> SolveChallenge(int challengeId, string password)
            {
                var user = await _userManager.Users
                    .Include(u => u.ChallengesResolutions)
                    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

                var challenge = await _context.Challenges
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == challengeId);

                // Validar si el desafío puede ser resuelto
                if (!ValidateChallengeAttempt(user, challenge, password))
                {
                    return View("Error");
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

                // Actualizar el usuario
                user.Coins += challenge.Coins;
                user.GainExp(challenge.ExpPoints);

                // Desbloquear recompensas
                UnlockRewards(user, challenge);

                // Guardar cambios
                _context.ChallengeResolutions.Add(resolution);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // METHOD FOR CHALLENGE VALIDATIONS
            private bool ValidateChallengeAttempt(User user, Challenge challenge, string password)
            {
                if (user == null || challenge == null) return false;

                // Validar nivel
                if (challenge.RarityLevel > user.Level) return false;

                // Validar contraseña
                if (challenge.SolutionMode == SolutionMode.Password && challenge.Password != password) return false;

                // Validar tiempo
                if (challenge.SolutionMode == SolutionMode.TimeBased && !challenge.IsPublic) return false;

                // Validar si ya fue resuelto (con null check)
                if (user.ChallengesResolutions?.Any(cr => cr.ChallengeId == challenge.Id) == true) return false;

                return true;
            }

            // METHOD TO UNLOCK REWARDS TO THE USER
            private void UnlockRewards(User user, Challenge challenge)
            {
                if (challenge.ProductId.HasValue)
                {
                    if (!user.UnlockedProducts.Any(up => up.ProductId == challenge.ProductId.Value))
                    {
                        user.UnlockedProducts
                            .Add(new UserUnlockedProduct { ProductId = challenge.ProductId.Value });
                    }
                }
                else if (challenge.ProductLevel.HasValue)
                {
                    if (!user.UnlockedProductLevels
                        .Any(ul => ul.Level == challenge.ProductLevel.Value))
                    {
                        user.UnlockedProductLevels
                            .Add(new UserUnlockedProductByLevel { Level = challenge.ProductLevel.Value });
                    }
                }
            }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Purchase(int productSizeId, int quantity)
        {
            try
            {
                var productSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == productSizeId);

                // Validación mejorada
                if (productSize == null ||
                    productSize.Stock < quantity ||
                    quantity <= 0 ||
                    !productSize.IsActive)
                {
                    TempData["ErrorMessage"] = "Operación no válida";
                    return RedirectToAction("Index");
                }

                var userId = _userManager.GetUserId(User);

                // Crear orden
                var order = new Order
                {
                    UserId = userId,
                    CreationDate = DateTime.Now,
                    Status = "En Proceso"
                };

                // Crear detalle con ProductSizeId
                var orderDetail = new OrderDetail
                {
                    ProductSizeId = productSize.Id,
                    Quantity = quantity,
                    Price = productSize.Product.Price * quantity
                };

                order.OrderDetails = new List<OrderDetail> { orderDetail };
                _context.Orders.Add(order);

                // Actualizar stock
                productSize.UpdateStock(-quantity);
                if (productSize.Stock <= 0) productSize.IsActive = false;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Orden creada! ID: {order.Id}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la compra";
                // Logger.LogError(ex, "Error en Purchase");
            }

            return RedirectToAction("Index");
        }

    }
}
