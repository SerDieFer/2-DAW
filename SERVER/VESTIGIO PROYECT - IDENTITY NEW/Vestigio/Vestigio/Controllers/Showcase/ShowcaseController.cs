using Microsoft.AspNetCore.Authorization;
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
using Vestigio.Services;
using Vestigio.Models.DTOs;

namespace Vestigio.Controllers.Showcase
{
    public class ShowcaseController : Controller
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IFilterService _filterService;

        public ShowcaseController(VestigioDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IFilterService filterService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _filterService = filterService;
        }

        public async Task<IActionResult> Index(
            [FromQuery] List<int> categories,
            [FromQuery] string productSort = "price_asc",
            int? minLevel = null,
            int? maxLevel = null,
            int? minXP = null,
            int? maxXP = null,
            int? minCoins = null,
            int? maxCoins = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string solutionType = null,
            bool? showSolved = null,
            string activeTab = "challenges",
            string challengeSort = "level_asc")
        {
            // Obtener datos del usuario autenticado
            int userLevel = 1;
            List<int> solvedChallenges = new List<int>();
            List<int> unlockedProductIds = new List<int>();
            List<int> unlockedProductLevels = new List<int>();

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
                    unlockedProductIds = user.UnlockedProducts.Select(up => up.ProductId).ToList();
                    unlockedProductLevels = user.UnlockedProductLevels.Select(ul => ul.Level).ToList();
                }
            }

            // Si se solicita la pestaña de "products" pero no hay productos desbloqueados, se fuerza a "challenges"
            if (activeTab.ToLower() == "products" &&
                (!unlockedProductIds.Any() && !unlockedProductLevels.Any()))
            {
                activeTab = "challenges";
            }

            // Construir el objeto FilterRequest
            var filterRequest = new FilterRequest
            {
                ActiveTab = activeTab,

                // Filtros para challenges
                MinLevel = activeTab == "challenges" ? minLevel : null,
                MaxLevel = activeTab == "challenges" ? maxLevel : null,
                SolutionType = activeTab == "challenges" ? solutionType : null,
                MinXP = activeTab == "challenges" ? minXP : null,
                MaxXP = activeTab == "challenges" ? maxXP : null,
                MinCoins = activeTab == "challenges" ? minCoins : null,
                MaxCoins = activeTab == "challenges" ? maxCoins : null,
                ChallengeSort = activeTab == "challenges" ? challengeSort : "level_asc",
                ShowSolved = activeTab == "challenges" ? showSolved : null,

                // Filtros para products
                MinPrice = activeTab == "products" ? minPrice : null,
                MaxPrice = activeTab == "products" ? maxPrice : null,
                Categories = activeTab == "products" ? categories : new List<int>(),
                ProductSort = activeTab == "products" ? productSort : "price_asc",

                // Datos del usuario
                UserLevel = userLevel,
                SolvedChallenges = solvedChallenges,
                UnlockedProductIds = unlockedProductIds,
                UnlockedProductLevels = unlockedProductLevels
            };

            // Obtener resultados filtrados mediante el servicio
            var filterResult = await _filterService.GetFilteredResultsAsync(filterRequest);

            // Si es una petición AJAX, devolver la vista parcial adecuada
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(filterResult.ViewName, filterResult.Data);
            }

            return View(filterResult.Data);
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