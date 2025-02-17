﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index(
            int? minLevel = null, int? maxLevel = null,
            int? minXP = null, int? maxXP = null,
            int? minCoins = null, int? maxCoins = null,
            decimal? minPrice = null, decimal? maxPrice = null,
            string solutionType = null, bool? showSolved = null,
            List<int> categories = null, string activeTab = "challenges",
            string challengeSort = "level_asc", string productSort = "price_asc" )
        {
            // RESET PARAMETERS FOR FILTERING
            if (activeTab == "challenges")
            {
                minPrice = null;
                maxPrice = null;
                categories = null;
                productSort = "price_asc";
            }
            else if (activeTab == "products")
            {
                minLevel = null;
                maxLevel = null;
                solutionType = null;
                minXP = null;
                maxXP = null;
                minCoins = null;
                maxCoins = null;
                challengeSort = "level_asc";
            }

            // BASE DATA
            int userLevel = 1;
            List<int> solvedChallenges = new List<int>();
            List<Product> unlockedProducts = new List<Product>();
            List<Category> validCategories = new List<Category>();
            bool hasUnlockedProducts = false;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.Users
                    .Include(u => u.ChallengesResolutions)
                    .Include(u => u.UnlockedProducts)
                        .ThenInclude(up => up.Product)
                    .Include(u => u.UnlockedProductLevels)
                    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

                if (user != null)
                {
                    userLevel = Math.Clamp(user.Level, 1, 10);
                    solvedChallenges = user.ChallengesResolutions.Select(cr => cr.ChallengeId).ToList();

                    // GET ALWAYS UNLOCKED PRODUCTS
                    var unlockedProductIds = user.UnlockedProducts.Select(up => up.ProductId).ToList();
                    var unlockedLevels = user.UnlockedProductLevels.Select(ul => ul.Level).ToList();

                    var productsQuery = _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.ProductSizes)
                        .Include(p => p.ProductCategories)
                            .ThenInclude(pc => pc.Category)
                        .Where(p => p.IsActive &&
                            (unlockedProductIds.Contains(p.Id) || unlockedLevels.Contains(p.RarityLevel)));

                    // ONLY APPLU FILTERS IS THE TAB IS ACTIVE
                    if (activeTab == "products")
                    {
                        productsQuery = ApplyProductFilters(productsQuery, minPrice, maxPrice, categories);
                        productsQuery = ApplyProductSorting(productsQuery, productSort);
                    }

                    unlockedProducts = await productsQuery.ToListAsync();
                    hasUnlockedProducts = unlockedProducts.Any();
                    validCategories = await productsQuery
                        .SelectMany(p => p.ProductCategories.Select(pc => pc.Category))
                        .Distinct()
                        .ToListAsync();
                }
            }

            // IF NO PRODUCTS ARE UNLOCKED ONLY CHALLENGE TAB WILL BE ACTIVE
            if (activeTab == "products" && !hasUnlockedProducts)
            {
                activeTab = "challenges";
            }

            // FILTER CHALLENGES IF THE ACTIVE TAB IS THE CHALLENGES ONE
            List<Challenge> filteredChallenges = new List<Challenge>();
            if (activeTab == "challenges")
            {
                var challengesQuery = _context.Challenges
                    .Include(c => c.Images)
                    .Include(c => c.Product)
                    .Where(c => c.IsActive);

                challengesQuery = ApplyChallengeFilters(challengesQuery, minLevel, maxLevel, solutionType,
                                                      minXP, maxXP, minCoins, maxCoins,
                                                      userLevel, solvedChallenges, showSolved);

                filteredChallenges = ApplyChallengeSorting(challengesQuery, challengeSort, solvedChallenges).ToList();
            }

            ViewBag.Challenges = filteredChallenges;
            ViewBag.UnlockedProducts = unlockedProducts;
            ViewBag.ValidCategories = validCategories;
            ViewBag.SolvedChallenges = solvedChallenges;
            ViewBag.UserLevel = userLevel;
            ViewBag.HasUnlockedProducts = hasUnlockedProducts;
            ViewBag.ActiveTab = activeTab;

            ViewBag.Filters = new
            {
                MinLevel = activeTab == "challenges" ? minLevel : null,
                MaxLevel = activeTab == "challenges" ? maxLevel : null,
                SolutionType = activeTab == "challenges" ? solutionType : null,
                MinXP = activeTab == "challenges" ? minXP : null,
                MaxXP = activeTab == "challenges" ? maxXP : null,
                MinCoins = activeTab == "challenges" ? minCoins : null,
                MaxCoins = activeTab == "challenges" ? maxCoins : null,
                ChallengeSort = activeTab == "challenges" ? challengeSort : "level_asc",
                MinPrice = activeTab == "products" ? minPrice : null,
                MaxPrice = activeTab == "products" ? maxPrice : null,
                Categories = activeTab == "products" ? categories : null,
                ProductSort = activeTab == "products" ? productSort : "price_asc",
                ShowSolved = activeTab == "challenges" ? showSolved : null
            };

            // AJAX HANDLE
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return activeTab switch
                {
                    "challenges" => PartialView("_ChallengesPartial", filteredChallenges),
                    "products" => PartialView("_ProductsPartial", unlockedProducts),
                    _ => BadRequest()
                };
            }

            return View(filteredChallenges);
        }


        // CHALLENGES FILTERING METHOD
        private IQueryable<Challenge> ApplyChallengeFilters(
            IQueryable<Challenge> query,
            int? minLevel, int? maxLevel, string solutionType,
            int? minXP, int? maxXP, int? minCoins, int? maxCoins,
            int userLevel, List<int> solvedChallenges, bool? showSolved)
        {
            // User level filter
            // query = query.Where(c => c.RarityLevel <= userLevel);

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
            IQueryable<Product> query, decimal? minPrice, decimal? maxPrice, List<int> categories)
        {
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice);
            if (categories != null && categories.Any())
                query = query.Where(p => p.ProductCategories.Any(pc => categories.Contains(pc.CategoryId)));

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
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var challenge = await _context.Challenges
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == challengeId);

            if (challenge == null) return NotFound();

            if (!ValidateChallengeAttempt(user, challenge, password))
            {
                TempData["ErrorMessage"] = "No se pudo resolver el desafío";
                return RedirectToAction("Index");
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

            TempData["SuccessMessage"] = "¡Desafío resuelto!";
            return RedirectToAction("Index");
        }

        // CHALLENGE ATTEMPT VALIDATION
        private bool ValidateChallengeAttempt(User user, Challenge challenge, string password)
        {
            if (user == null || challenge == null) return false;
            if (challenge.RarityLevel > user.Level) return false;
            if (challenge.SolutionMode == SolutionMode.Password && challenge.Password != password) return false;
            if (challenge.SolutionMode == SolutionMode.TimeBased && !challenge.IsPublic) return false;
            if (user.ChallengesResolutions.Any(cr => cr.ChallengeId == challenge.Id)) return false;

            return true;
        }

        // REWARDS UNLOCKING METHOD
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


    // CART ADD METHOD
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
        return RedirectToAction("Index", "Cart");
    }

    // ORDER HANDLING METHOD
    private async Task<Order> GetOrCreateOrder(string userId)
        {
            var orderId = HttpContext.Session.GetInt32("CurrentOrderId");
            if (orderId.HasValue)
            {
                return await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
            }

            var newOrder = new Order
            {
                UserId = userId,
                CreationDate = DateTime.Now,
                OrderStatusId = 1,
                OrderDetails = new List<OrderDetail>()
            };
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetInt32("CurrentOrderId", newOrder.Id);
            return newOrder;
        }
    }
}