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
                .Include(p => p.ProductSizes)
                .Where(p => p.IsActive &&
                    (unlockedProductIds.Contains(p.Id) || unlockedLevels.Contains(p.RarityLevel)))
                .ToListAsync();

            // Pasar datos a la vista mediante ViewBag
            ViewBag.UserLevel = user.Level;
            ViewBag.SolvedChallenges = user.ChallengesResolutions?.Select(cr => cr.ChallengeId).ToList() ?? new List<int>();
            ViewBag.UnlockedProducts = unlockedProducts;
            ViewBag.Challenges = challenges;

            return View(challenges);
        }


        // ------------------------ CHALLENGE RESOLUTION METHODS -----------------------------

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

            // Actualizar el usuario
            user.Coins += challenge.Coins;
            user.GainExp(challenge.ExpPoints);

            // Desbloquear recompensas
            UnlockRewards(user, challenge);

            // Guardar cambios
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
        public async Task<IActionResult> AddToCart(int productSizeId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null) return Redirect("/Identity/Account/Login");

                var productSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == productSizeId);

                if (productSize == null || !productSize.IsActive || productSize.Product == null || !productSize.Product.IsActive)
                {
                    TempData["ErrorMessage"] = "Producto no disponible";
                    return RedirectToAction("Index");
                }

                // Obtener orden pendiente del usuario
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pendiente");

                // Calcular cantidad existente en el carrito
                int existingQuantityInCart = order?.OrderDetails
                    .Where(od => od.ProductSizeId == productSizeId)
                    .Sum(od => od.Quantity) ?? 0;

                int totalDesired = existingQuantityInCart + quantity;
                if (totalDesired > productSize.Stock)
                {
                    TempData["ErrorMessage"] = $"Stock insuficiente. Disponible: {productSize.Stock}";
                    return RedirectToAction("Index");
                }

                // Crear orden si no existe
                if (order == null)
                {
                    order = new Order
                    {
                        UserId = userId,
                        CreationDate = DateTime.Now,
                        Status = "Pendiente",
                        OrderDetails = new List<OrderDetail>()
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                return RedirectToAction("Index");
            }
        }

    }
}
