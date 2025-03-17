using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;
using System.Text.Json;

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

        // PRINCIPAL METHOD TO RENDER THE VIEW WITH REACT
        public IActionResult Index()
        {
            return View();
        }

        // API TO GET CHALLENGES WITH FILTERING
        [HttpGet]
        public async Task<IActionResult> GetChallenges(
            int? minLevel = null, int? maxLevel = null,
            int? minXP = null, int? maxXP = null,
            int? minCoins = null, int? maxCoins = null,
            string solutionType = null, bool? showSolved = null,
            string challengeSort = "level_asc")
        {
            // BASE DATA
            int userLevel = 1;
            List<int> solvedChallenges = new List<int>();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.Users
                    .Include(u => u.ChallengesResolutions)
                    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

                if (user != null)
                {
                    userLevel = Math.Clamp(user.Level, 1, 10);
                    solvedChallenges = user.ChallengesResolutions.Select(cr => cr.ChallengeId).ToList();
                }
            }

            var challengesQuery = _context.Challenges
                .Include(c => c.Images)
                .Include(c => c.Product)
                .Where(c => c.IsActive);

            challengesQuery = ApplyChallengeFilters(challengesQuery, minLevel, maxLevel, solutionType,
                                                  minXP, maxXP, minCoins, maxCoins,
                                                  userLevel, solvedChallenges, showSolved);

            var filteredChallenges = ApplyChallengeSorting(challengesQuery, challengeSort, solvedChallenges).ToList();

            // ADD INFO ABOUT IF THE CHALLENGE IS SOLVED
            var result = filteredChallenges.Select(c => new
            {
                c.Id,
                c.Title,
                c.Description,
                c.RarityLevel,
                c.ExpPoints,
                c.Coins,
                c.SolutionMode,
                c.IsActive,
                c.CreationDate,
                Images = c.Images.Select(i => i.ImagePath),
                Product = c.Product != null ? new { c.Product.Id, c.Product.Name } : null,
                IsSolved = solvedChallenges.Contains(c.Id),
                IsUnlockable = c.RarityLevel <= userLevel,
                IsPublic = c.IsPublic
            });

            return Json(result);
        }

        // API TO GET PRODUCTS WITH FILTERING
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            int? minProductLevel = null, int? maxProductLevel = null,
            decimal? minPrice = null, decimal? maxPrice = null,
            List<int> categories = null, List<string> sizes = null,
            string productSort = "price_asc")
        {
            // BASE DATA
            List<Product> unlockedProducts = new List<Product>();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.Users
                    .Include(u => u.UnlockedProducts)
                        .ThenInclude(up => up.Product)
                    .Include(u => u.UnlockedProductLevels)
                    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

                if (user != null)
                {
                    // GET UNLOCKED PRODUCTS
                    var unlockedProductIds = user.UnlockedProducts.Select(up => up.ProductId).ToList();
                    var unlockedLevels = user.UnlockedProductLevels.Select(ul => ul.Level).ToList();

                    var productsQuery = _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.ProductSizes)
                        .Include(p => p.ProductCategories)
                            .ThenInclude(pc => pc.Category)
                        .Where(p => p.IsActive &&
                            (unlockedProductIds.Contains(p.Id) || unlockedLevels.Contains(p.RarityLevel)));

                    productsQuery = ApplyProductFilters(productsQuery, minPrice, maxPrice, minProductLevel, maxProductLevel, categories, sizes);
                    productsQuery = ApplyProductSorting(productsQuery, productSort);

                    unlockedProducts = await productsQuery.ToListAsync();
                }
            }

            // FORMAT DATA TO JSON RESPONSE
            var result = unlockedProducts.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.RarityLevel,
                p.CreationDate,
                Images = p.Images.Select(i => i.ImagePath),
                Sizes = p.ProductSizes.Where(ps => ps.Stock > 0).Select(ps => new
                {
                    ps.Id,
                    ps.Size,
                    ps.Stock
                }),
                Categories = p.ProductCategories.Select(pc => new
                {
                    pc.Category.Id,
                    pc.Category.Name
                })
            });

            return Json(result);
        }

        // API TO GET CATEGORIES
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            List<Category> categories = new List<Category>();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var unlockedProductIds = await _context.UserUnlockedProducts
                        .Where(up => up.UserId == user.Id)
                        .Select(up => up.ProductId)
                        .ToListAsync();

                    var unlockedLevels = await _context.UserUnlockedProductByLevels
                        .Where(ul => ul.UserId == user.Id)
                        .Select(ul => ul.Level)
                        .ToListAsync();

                    categories = await _context.Categories
                        .Where(c => c.ProductCategories.Any(pc => 
                            pc.Product.IsActive && (unlockedProductIds.Contains(pc.ProductId) || 
                                                   unlockedLevels.Contains(pc.Product.RarityLevel))))
                        .ToListAsync();
                }
            }

            return Json(categories);
        }

        // API TO GET AVAILABLE SIZES
        [HttpGet]
        public async Task<IActionResult> GetSizes()
        {
            List<string> sizes = new List<string>();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var unlockedProductIds = await _context.UserUnlockedProducts
                        .Where(up => up.UserId == user.Id)
                        .Select(up => up.ProductId)
                        .ToListAsync();

                    var unlockedLevels = await _context.UserUnlockedProductByLevels
                        .Where(ul => ul.UserId == user.Id)
                        .Select(ul => ul.Level)
                        .ToListAsync();

                    sizes = await _context.ProductSizes
                        .Where(ps => ps.Stock > 0 && ps.Product.IsActive && 
                               (unlockedProductIds.Contains(ps.ProductId) || 
                                unlockedLevels.Contains(ps.Product.RarityLevel)))
                        .Select(ps => ps.Size)
                        .Distinct()
                        .ToListAsync();
                }
            }

            return Json(sizes);
        }

        // API TO GET USER INFO
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { isAuthenticated = false, level = 1 });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { isAuthenticated = false, level = 1 });

            return Json(new { 
                isAuthenticated = true, 
                level = user.Level,
                hasUnlockedProducts = await _context.UserUnlockedProducts.AnyAsync(up => up.UserId == user.Id) ||
                                    await _context.UserUnlockedProductByLevels.AnyAsync(ul => ul.UserId == user.Id)
            });
        }

        // CHALLENGES FILTERING METHOD
        private IQueryable<Challenge> ApplyChallengeFilters(
            IQueryable<Challenge> query,
            int? minLevel, int? maxLevel, string solutionType,
            int? minXP, int? maxXP, int? minCoins, int? maxCoins,
            int userLevel, List<int> solvedChallenges, bool? showSolved)
        {
            // CUSTOM FILTERS
            if (minLevel.HasValue) query = query.Where(c => c.RarityLevel >= minLevel);
            if (maxLevel.HasValue) query = query.Where(c => c.RarityLevel <= maxLevel);

            if (!string.IsNullOrEmpty(solutionType))
            {
                if (Enum.TryParse<SolutionMode>(solutionType, out var solutionMode))
                {
                    query = query.Where(c => c.SolutionMode == solutionMode);
                }
            }

            if (minXP.HasValue) query = query.Where(c => c.ExpPoints >= minXP);
            if (maxXP.HasValue) query = query.Where(c => c.ExpPoints <= maxXP);
            if (minCoins.HasValue) query = query.Where(c => c.Coins >= minCoins);
            if (maxCoins.HasValue) query = query.Where(c => c.Coins <= maxCoins);

            if (showSolved.HasValue)
            {
                if (showSolved.Value)
                {
                    query = query.Where(c => solvedChallenges.Contains(c.Id));
                }
                else
                {
                    query = query.Where(c => !solvedChallenges.Contains(c.Id));
                }
            }

            return query;
        }

        // CHALLENGES SORTING METHOD
        private IOrderedQueryable<Challenge> ApplyChallengeSorting(
            IQueryable<Challenge> query, string sortOrder, List<int> solvedChallenges)
        {
            var sortParams = sortOrder.Split('_');
            string sortBy = sortParams[0];
            string direction = sortParams.Length > 1 ? sortParams[1] : "asc";

            // FIRST UNSOLVED ONES, THE SOLVED TO THE END
            IOrderedQueryable<Challenge> orderedQuery = query
                .OrderBy(c => solvedChallenges.Contains(c.Id));

            // SORTING
            return (sortBy, direction) switch
            {
                ("level", "asc") => orderedQuery.ThenBy(c => c.RarityLevel),
                ("level", "desc") => orderedQuery.ThenByDescending(c => c.RarityLevel),
                ("date", "asc") => orderedQuery.ThenBy(c => c.CreationDate),
                ("date", "desc") => orderedQuery.ThenByDescending(c => c.CreationDate),
                _ => orderedQuery.ThenBy(c => c.RarityLevel)
            };
        }

        // PRODUCTS FILTERING METHOD
        private IQueryable<Product> ApplyProductFilters(
            IQueryable<Product> query,
            decimal? minPrice, decimal? maxPrice,
            int? minProductLevel, int? maxProductLevel,
            List<int> categories, List<string> sizes)
        {
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice);
            if (categories != null && categories.Any())
                query = query.Where(p => p.ProductCategories.Any(pc => categories.Contains(pc.CategoryId)));

            if (minProductLevel.HasValue) query = query.Where(c => c.RarityLevel >= minProductLevel);
            if (maxProductLevel.HasValue) query = query.Where(c => c.RarityLevel <= maxProductLevel);

            if (sizes != null && sizes.Any())
            {
                query = query.Where(p => p.ProductSizes.Any(ps => sizes.Contains(ps.Size) && ps.Stock > 0));
            }

            return query;
        }

        // PRODUCTS SORTING METHOD
        private IQueryable<Product> ApplyProductSorting(IQueryable<Product> query, string sort)
        {
            var sortParams = sort.Split('_');
            string sortBy = sortParams[0];
            string direction = sortParams.Length > 1 ? sortParams[1] : "asc";

            return sortBy switch
            {
                "price" => direction == "asc"
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price),
                "rarity" => direction == "asc"
                    ? query.OrderBy(p => p.RarityLevel)
                    : query.OrderByDescending(p => p.RarityLevel),
                "date" => direction == "asc"
                    ? query.OrderBy(p => p.CreationDate)
                    : query.OrderByDescending(p => p.CreationDate),
                _ => query.OrderBy(p => p.Price)
            };
        }

        // CHALLENGE SOLVING METHOD
        [HttpPost]
        public async Task<IActionResult> SolveChallenge(int challengeId, string password)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Identity/Account/Login");
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var challenge = await _context.Challenges
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == challengeId);

            if (challenge == null) return NotFound();

            if (!ValidateChallengeAttempt(user, challenge, password))
            {
                return Json(new { success = false, message = "No se pudo resolver el desafío" });
            }

            var resolution = new ChallengeResolution
            {
                UserId = user.Id,
                ChallengeId = challengeId,
                ResolutionDate = DateTime.UtcNow,
                CoinsEarned = challenge.Coins,
                PointsEarned = challenge.ExpPoints
            };

            user.GainExp(challenge.ExpPoints);
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            UnlockRewards(user, challenge);
            _context.ChallengeResolutions.Add(resolution);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "¡Desafío resuelto!", userLevel = user.Level });
        }

        // CHALLENGE ATTEMPT VALIDATION - Mantener igual
        private bool ValidateChallengeAttempt(User user, Challenge challenge, string password)
        {
            if (user == null || challenge == null) return false;
            if (challenge.RarityLevel > user.Level) return false;
            if (challenge.SolutionMode == SolutionMode.Password && challenge.Password != password) return false;
            if (challenge.SolutionMode == SolutionMode.TimeBased && !challenge.IsPublic) return false;
            if (user.ChallengesResolutions.Any(cr => cr.ChallengeId == challenge.Id)) return false;

            return true;
        }

        // REWARDS UNLOCKING METHOD - Mantener igual
        private void UnlockRewards(User user, Challenge challenge)
        {
            if (challenge.ProductId.HasValue && !user.UnlockedProducts.Any(up => up.ProductId == challenge.ProductId))
            {
                user.UnlockedProducts.Add(new UserUnlockedProduct { ProductId = challenge.ProductId.Value });
            }
            else if (challenge.ProductLevel.HasValue && !user.UnlockedProductLevels.Any(ul => ul.Level == challenge.ProductLevel))
            {
                user.UnlockedProductLevels.Add(new UserUnlockedProductByLevel { Level = challenge.ProductLevel.Value });
            }
        }

        // CART ADD METHOD - REACT
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productSizeId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { success = false, message = "Usuario no autenticado", redirect = "/Identity/Account/Login" });

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
                        OrderStatusId = 1, // Pending
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
                    return Json(new { success = false, message = "Producto no encontrado." });
                }

                if (productSize.Stock < quantity)
                {
                    return Json(new { success = false, message = $"Stock insuficiente. Disponible: {productSize.Stock}" });
                }

                // Se calcula la cantidad ya agregada al carrito para esta talla
                int existingQuantity = order.OrderDetails
                    .Where(od => od.ProductSizeId == productSizeId)
                    .Sum(od => od.Quantity);

                if (existingQuantity + quantity > productSize.Stock)
                {
                    return Json(new { success = false, message = $"Límite de stock alcanzado. Disponible: {productSize.Stock - existingQuantity}" });
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
                        Price = productSize.Product.Price
                    });
                }
                order.Total = order.OrderDetails.Sum(od => od.Price * od.Quantity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Json(new { success = true, message = "Producto agregado al carrito", redirect = "/Cart/Index" });
            }
            catch
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Error al procesar la solicitud" });
            }
        }
    }
}