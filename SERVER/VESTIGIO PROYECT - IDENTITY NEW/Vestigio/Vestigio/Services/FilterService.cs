using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            if (request.ActiveTab.ToLower() == "challenges")
            {
                var query = _context.Challenges
                    .Include(c => c.Images)
                    .Include(c => c.Product)
                    .Where(c => c.IsActive);

                // Aplicar filtros de challenges
                query = ApplyChallengeFilters(query,
                                              request.MinLevel,
                                              request.MaxLevel,
                                              request.SolutionType,
                                              request.MinXP,
                                              request.MaxXP,
                                              request.MinCoins,
                                              request.MaxCoins,
                                              request.UserLevel,
                                              request.SolvedChallenges,
                                              request.ShowSolved);

                // Ordenamiento de challenges
                var orderedQuery = ApplyChallengeSorting(query, request.ChallengeSort, request.SolvedChallenges);

                var results = await orderedQuery.ToListAsync();

                return new FilterResult(results, request.ActiveTab);
            }
            else if (request.ActiveTab.ToLower() == "products")
            {
                var query = _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                    .Where(p => p.IsActive &&
                           (request.UnlockedProductIds.Contains(p.Id) ||
                            request.UnlockedProductLevels.Contains(p.RarityLevel)));

                // Aplicar filtros de products
                query = ApplyProductFilters(query, request.MinPrice, request.MaxPrice, request.Categories);

                // Ordenamiento de products
                query = ApplyProductSorting(query, request.ProductSort);

                var results = await query.ToListAsync();

                return new FilterResult(results, request.ActiveTab);
            }

            throw new ArgumentException("Pestaña activa no válida");
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
            System.Collections.Generic.List<int> solvedChallenges,
            bool? showSolved)
        {
            if (minLevel.HasValue)
                query = query.Where(c => c.RarityLevel >= minLevel.Value);
            if (maxLevel.HasValue)
                query = query.Where(c => c.RarityLevel <= maxLevel.Value);
            if (!string.IsNullOrEmpty(solutionType))
            {
                if (Enum.TryParse<SolutionMode>(solutionType, out var mode))
                {
                    query = query.Where(c => c.SolutionMode == mode);
                }
            }
            if (minXP.HasValue)
                query = query.Where(c => c.ExpPoints >= minXP.Value);
            if (maxXP.HasValue)
                query = query.Where(c => c.ExpPoints <= maxXP.Value);
            if (minCoins.HasValue)
                query = query.Where(c => c.Coins >= minCoins.Value);
            if (maxCoins.HasValue)
                query = query.Where(c => c.Coins <= maxCoins.Value);

            if (showSolved.HasValue)
            {
                if (showSolved.Value)
                    query = query.Where(c => solvedChallenges.Contains(c.Id));
                else
                    query = query.Where(c => !solvedChallenges.Contains(c.Id));
            }
            return query;
        }

        private IOrderedQueryable<Challenge> ApplyChallengeSorting(
            IQueryable<Challenge> query,
            string sortOrder,
            System.Collections.Generic.List<int> solvedChallenges)
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
            System.Collections.Generic.List<int> categories)
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
