using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Models.DTOs;

namespace Vestigio.Services
{
    public class FilterService : IFilterService
    {
        private readonly VestigioDbContext _context;

        public FilterService(VestigioDbContext context)
        {
            _context = context;
        }

        public async Task<FilterResult> GetFilteredResultsAsync(FilterRequest request)
        {
            try
            {
                if (request.ActiveTab.Equals("challenges", StringComparison.OrdinalIgnoreCase))
                {
                    var query = _context.Challenges
                        .Include(c => c.Images)
                        .Include(c => c.Product)
                        .Where(c => c.IsActive);

                    // Aplicar filtros
                    query = ApplyChallengeFilters(
                        query,
                        request.MinLevel,
                        request.MaxLevel,
                        request.SolutionType,
                        request.MinXP,
                        request.MaxXP,
                        request.MinCoins,
                        request.MaxCoins,
                        request.UserLevel,
                        request.SolvedChallenges,
                        request.ShowSolved
                    );

                    // Ordenar
                    query = ApplyChallengeSorting(query, request.ChallengeSort, request.SolvedChallenges);

                    var results = await query.ToListAsync();
                    return new FilterResult(results, "challenges");
                }
                else if (request.ActiveTab.Equals("products", StringComparison.OrdinalIgnoreCase))
                {
                    var query = _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.ProductCategories)
                          .ThenInclude(pc => pc.Category)
                        .Where(p => p.IsActive);

                    query = query.Where(p =>
                        request.UnlockedProductIds.Contains(p.Id) ||
                        request.UnlockedProductLevels.Contains(p.RarityLevel)
                    );

                    // Aplicar otros filtros (precio, categorías)
                    query = ApplyProductFilters(query, request.MinPrice, request.MaxPrice, request.Categories);
                    query = ApplyProductSorting(query, request.ProductSort);

                    var results = await query.ToListAsync();
                    return new FilterResult(results, "products");
                }

                return new FilterResult(new List<object>(), "challenges");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetFilteredResultsAsync: {ex.Message}");
                return new FilterResult(new List<object>(), "challenges");
            }
        }

        // MÉTODOS PRIVADOS DE FILTRADO Y ORDENAMIENTO

        private IQueryable<Challenge> ApplyChallengeFilters(
              IQueryable<Challenge> query,
              int? minLevel,
              int? maxLevel,
              string solutionType,
              int? minXP,
              int? maxXP,
              int? minCoins,
              int? maxCoins,
              int userLevel,
              List<int> solvedChallenges,
              bool? showSolved)
        {
            // Filtro de nivel
            if (minLevel.HasValue) query = query.Where(c => c.RarityLevel >= minLevel.Value);
            if (maxLevel.HasValue) query = query.Where(c => c.RarityLevel <= maxLevel.Value);

            // Filtro de tipo de solución
            if (!string.IsNullOrEmpty(solutionType))
            {
                if (Enum.TryParse<SolutionMode>(solutionType, out var mode))
                {
                    query = query.Where(c => c.SolutionMode == mode);
                }
            }

            // Filtro de estado (resueltos/no resueltos)
            if (showSolved.HasValue)
            {
                query = showSolved.Value
                    ? query.Where(c => solvedChallenges.Contains(c.Id))
                    : query.Where(c => !solvedChallenges.Contains(c.Id));
            }

            return query;
        }

        private IOrderedQueryable<Challenge> ApplyChallengeSorting(
            IQueryable<Challenge> query,
            string sortOrder,
            List<int> solvedChallenges)
        {
            var sortParams = sortOrder.Split('_');
            string sortBy = sortParams[0];
            string direction = sortParams.Length > 1 ? sortParams[1] : "asc";

            // Primero se ordenan los desafíos sin resolver para que queden al principio
            IOrderedQueryable<Challenge> orderedQuery = query.OrderBy(c => solvedChallenges.Contains(c.Id));

            return (sortBy, direction) switch
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
            decimal? minPrice,
            decimal? maxPrice,
            List<int> categories)
        {
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);
            if (categories != null && categories.Any())
                query = query.Where(p => p.ProductCategories.Any(pc => categories.Contains(pc.CategoryId)));

            return query;
        }

        private IQueryable<Product> ApplyProductSorting(
            IQueryable<Product> query,
            string sortOrder)
        {
            var sortParams = sortOrder.Split('_');
            string sortBy = sortParams[0];
            string direction = sortParams.Length > 1 ? sortParams[1] : "asc";

            return sortBy switch
            {
                "price" => direction == "asc" ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "rarity" => direction == "asc" ? query.OrderBy(p => p.RarityLevel) : query.OrderByDescending(p => p.RarityLevel),
                "date" => direction == "asc" ? query.OrderBy(p => p.CreationDate) : query.OrderByDescending(p => p.CreationDate),
                _ => query.OrderBy(p => p.Price)
            };
        }
    }
}
