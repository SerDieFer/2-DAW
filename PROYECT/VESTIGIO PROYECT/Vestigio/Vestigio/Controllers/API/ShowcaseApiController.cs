using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.DTO_s;
using System.ComponentModel.DataAnnotations;

namespace Vestigio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowcaseApiController : ControllerBase
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ShowcaseApiController> _logger;

        public ShowcaseApiController(
            VestigioDbContext context,
            UserManager<User> userManager,
            ILogger<ShowcaseApiController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("challenges")]
        public async Task<ActionResult<ChallengeResponseDto>> GetChallenges(
            [FromQuery] ChallengeFilterDto filter)
        {
            try
            {
                var user = await GetAuthenticatedUser();
                var userLevel = user?.Level ?? 1;
                var solvedChallenges = user?.ChallengesResolutions.Select(cr => cr.ChallengeId).ToList() ?? new List<int>();

                var query = _context.Challenges
                    .Include(c => c.Images)
                    .Include(c => c.Product)
                    .Where(c => c.IsActive)
                    .AsQueryable();

                query = ApplyChallengeFilters(query, filter, userLevel, solvedChallenges);
                query = ApplyChallengeSorting(query, filter.Sort, solvedChallenges);

                var challenges = await query.ToListAsync();

                return new ChallengeResponseDto
                {
                    Challenges = challenges.Select(c => new ChallengeDto(c, solvedChallenges.Contains(c.Id))),
                    Count = challenges.Count,
                    UserLevel = userLevel
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo desafíos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("products")]
        public async Task<ActionResult<ProductResponseDto>> GetProducts(
            [FromQuery] ProductFilterDto filter)
        {
            try
            {
                var user = await GetAuthenticatedUser();
                var response = new ProductResponseDto();

                if (user != null)
                {
                    var unlockedProductIds = user.UnlockedProducts.Select(up => up.ProductId).ToList();
                    var unlockedLevels = user.UnlockedProductLevels.Select(ul => ul.Level).ToList();

                    var query = _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.ProductSizes)
                        .Include(p => p.ProductCategories)
                            .ThenInclude(pc => pc.Category)
                        .Where(p => p.IsActive &&
                            (unlockedProductIds.Contains(p.Id) || unlockedLevels.Contains(p.RarityLevel)))
                        .AsQueryable();

                    query = ApplyProductFilters(query, filter);
                    query = ApplyProductSorting(query, filter.Sort);

                    response.Products = await query
                        .Select(p => new ProductDto(p))
                        .ToListAsync();

                    response.Categories = await query
                        .SelectMany(p => p.ProductCategories.Select(pc => pc.Category))
                        .Distinct()
                        .ToListAsync();

                    response.Sizes = await query
                        .SelectMany(p => p.ProductSizes
                            .Where(ps => ps.Stock > 0)
                            .Select(ps => new ProductSizeDto { Id = ps.Id, Size = ps.Size }))
                        .GroupBy(ps => ps.Size)
                        .Select(g => g.First())
                        .ToListAsync();
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [Authorize]
        [HttpPost("challenges/{challengeId}/solve")]
        public async Task<IActionResult> SolveChallenge(int challengeId, [FromBody] ChallengeSolveRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await GetAuthenticatedUser();
                if (user == null) return Unauthorized();

                var challenge = await _context.Challenges
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == challengeId);

                if (challenge == null) return NotFound("Desafío no encontrado");

                if (!ValidateChallengeAttempt(user, challenge, request.Password))
                    return BadRequest("Intento de solución inválido");

                if (user.ChallengesResolutions.Any(cr => cr.ChallengeId == challengeId))
                    return Conflict("Desafío ya resuelto");

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

                UnlockRewards(user, challenge);
                await _context.ChallengeResolutions.AddAsync(resolution);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new ChallengeSolveResultDto
                {
                    NewLevel = user.Level,
                    EarnedCoins = challenge.Coins,
                    EarnedXP = challenge.ExpPoints
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error resolviendo desafío");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [Authorize]
        [HttpPost("cart")]
        public async Task<ActionResult<CartDto>> AddToCart([FromBody] CartItemRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await GetAuthenticatedUser();
                if (user == null) return Unauthorized();

                var productSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == request.ProductSizeId);

                if (productSize == null) return NotFound("Producto no encontrado");
                if (productSize.Stock < request.Quantity) return BadRequest("Stock insuficiente");

                var order = await GetOrCreateCartOrder(user.Id);

                var existingItem = order.OrderDetails
                    .FirstOrDefault(od => od.ProductSizeId == request.ProductSizeId);

                if (existingItem != null)
                {
                    if (existingItem.Quantity + request.Quantity > productSize.Stock)
                        return BadRequest("Cantidad excede el stock disponible");

                    existingItem.Quantity += request.Quantity;
                }
                else
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductSizeId = request.ProductSizeId,
                        ProductId = productSize.Product.Id,
                        Quantity = request.Quantity,
                        Price = productSize.Product.Price
                    });
                }

                order.Total = order.OrderDetails.Sum(od => od.Price * od.Quantity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CartDto(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error agregando al carrito");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private async Task<User> GetAuthenticatedUser()
        {
            if (!User.Identity.IsAuthenticated) return null;

            return await _userManager.Users
                .Include(u => u.ChallengesResolutions)
                .Include(u => u.UnlockedProducts)
                .Include(u => u.UnlockedProductLevels)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
        }

        private IQueryable<Challenge> ApplyChallengeFilters(
            IQueryable<Challenge> query,
            ChallengeFilterDto filter,
            int userLevel,
            List<int> solvedChallenges)
        {
            if (filter.MinLevel.HasValue) query = query.Where(c => c.RarityLevel >= filter.MinLevel);
            if (filter.MaxLevel.HasValue) query = query.Where(c => c.RarityLevel <= filter.MaxLevel);

            if (!string.IsNullOrEmpty(filter.SolutionType) &&
                Enum.TryParse<SolutionMode>(filter.SolutionType, out var solutionMode))
            {
                query = query.Where(c => c.SolutionMode == solutionMode);
            }

            if (filter.MinXP.HasValue) query = query.Where(c => c.ExpPoints >= filter.MinXP);
            if (filter.MaxXP.HasValue) query = query.Where(c => c.ExpPoints <= filter.MaxXP);
            if (filter.MinCoins.HasValue) query = query.Where(c => c.Coins >= filter.MinCoins);
            if (filter.MaxCoins.HasValue) query = query.Where(c => c.Coins <= filter.MaxCoins);

            if (filter.ShowSolved.HasValue)
            {
                query = filter.ShowSolved.Value
                    ? query.Where(c => solvedChallenges.Contains(c.Id))
                    : query.Where(c => !solvedChallenges.Contains(c.Id));
            }

            return query;
        }

        private IQueryable<Challenge> ApplyChallengeSorting(
            IQueryable<Challenge> query,
            string sortOrder,
            List<int> solvedChallenges)
        {
            var sortParams = sortOrder.Split('_');
            var (sortBy, direction) = (sortParams[0], sortParams.Length > 1 ? sortParams[1] : "asc");

            var orderedQuery = query.OrderBy(c => solvedChallenges.Contains(c.Id));

            return (sortBy.ToLower(), direction.ToLower()) switch
            {
                ("level", "asc") => orderedQuery.ThenBy(c => c.RarityLevel),
                ("level", "desc") => orderedQuery.ThenByDescending(c => c.RarityLevel),
                ("date", "asc") => orderedQuery.ThenBy(c => c.CreationDate),
                ("date", "desc") => orderedQuery.ThenByDescending(c => c.CreationDate),
                _ => orderedQuery.ThenBy(c => c.RarityLevel)
            };
        }

        private IQueryable<Product> ApplyProductFilters(
            IQueryable<Product> query,
            ProductFilterDto filter)
        {
            if (filter.MinPrice.HasValue) query = query.Where(p => p.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(p => p.Price <= filter.MaxPrice);

            if (filter.Categories?.Any() == true)
                query = query.Where(p => p.ProductCategories.Any(pc => filter.Categories.Contains(pc.CategoryId)));

            if (filter.MinLevel.HasValue) query = query.Where(p => p.RarityLevel >= filter.MinLevel);
            if (filter.MaxLevel.HasValue) query = query.Where(p => p.RarityLevel <= filter.MaxLevel);

            if (filter.Sizes?.Any() == true)
                query = query.Where(p => p.ProductSizes.Any(ps => filter.Sizes.Contains(ps.Size) && ps.Stock > 0));

            return query;
        }

        private IQueryable<Product> ApplyProductSorting(IQueryable<Product> query, string sort)
        {
            var sortParams = sort.Split('_');
            var (sortBy, direction) = (sortParams[0], sortParams.Length > 1 ? sortParams[1] : "asc");

            return (sortBy.ToLower(), direction.ToLower()) switch
            {
                ("price", "asc") => query.OrderBy(p => p.Price),
                ("price", "desc") => query.OrderByDescending(p => p.Price),
                ("rarity", "asc") => query.OrderBy(p => p.RarityLevel),
                ("rarity", "desc") => query.OrderByDescending(p => p.RarityLevel),
                ("date", "asc") => query.OrderBy(p => p.CreationDate),
                ("date", "desc") => query.OrderByDescending(p => p.CreationDate),
                _ => query.OrderBy(p => p.Price)
            };
        }

        private async Task<Order> GetOrCreateCartOrder(string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderStatusId == 1);

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    CreationDate = DateTime.UtcNow,
                    OrderStatusId = 1,
                    OrderDetails = new List<OrderDetail>()
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }

            return order;
        }

        private bool ValidateChallengeAttempt(User user, Challenge challenge, string password)
        {
            if (user == null || challenge == null) return false;
            if (challenge.RarityLevel > user.Level) return false;
            if (challenge.SolutionMode == SolutionMode.Password && challenge.Password != password) return false;
            if (challenge.SolutionMode == SolutionMode.TimeBased && !challenge.IsPublic) return false;

            return true;
        }

        private void UnlockRewards(User user, Challenge challenge)
        {
            if (challenge.ProductId.HasValue &&
                !user.UnlockedProducts.Any(up => up.ProductId == challenge.ProductId))
            {
                user.UnlockedProducts.Add(new UserUnlockedProduct { ProductId = challenge.ProductId.Value });
            }
            else if (challenge.ProductLevel.HasValue &&
                     !user.UnlockedProductLevels.Any(ul => ul.Level == challenge.ProductLevel))
            {
                user.UnlockedProductLevels.Add(new UserUnlockedProductByLevel { Level = challenge.ProductLevel.Value });
            }
        }
    }
}