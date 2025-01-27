using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Vestigio.Controllers
{
    public class ProductsController : Controller
    {
        private readonly VestigioDbContext _context;

        public ProductsController(VestigioDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(
            string searchName, decimal? minPrice, decimal? maxPrice, int? pageNumber,
            int? rarityLevel, int? categoryId, bool? isActive, int? minStock,
            DateTime? startDate, DateTime? endDate, int pageSize = 5)
        {
            // BASE QUERY
            var productsQuery = _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .AsQueryable();

            // APPLY FILTERS
            if (!string.IsNullOrWhiteSpace(searchName))
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchName));

            if (minPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            if (minStock.HasValue)
                productsQuery = productsQuery.Where(p => p.TotalStock >= minStock.Value);

            if (rarityLevel.HasValue)
                productsQuery = productsQuery.Where(p => p.RarityLevel == rarityLevel.Value);

            if (isActive.HasValue)
                productsQuery = productsQuery.Where(p => p.IsActive == isActive.Value);

            if (categoryId.HasValue && categoryId.Value > 0)
                productsQuery = productsQuery.Where(p => p.ProductCategories
                .Any(pc => pc.CategoryId == categoryId.Value));

            if (startDate.HasValue)
                productsQuery = productsQuery.Where(p => p.CreationDate >= startDate.Value);

            if (endDate.HasValue)
                productsQuery = productsQuery.Where(p => p.CreationDate <= endDate.Value);

            // PAGINATION
            var paginatedList = await PaginatedList<Product>.CreateAsync(
                productsQuery.AsNoTracking(),
                pageNumber ?? 1,
                pageSize
            );

            // FETCH CATEGORIES FOR DROPDOWNS
            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Challenges"] = await _context.Challenges.ToListAsync();

            // PASS FILTER VALUES BACK TO THE VIEW
            ViewData["searchName"] = searchName;
            ViewData["minPrice"] = minPrice;
            ViewData["maxPrice"] = maxPrice;
            ViewData["rarityLevel"] = rarityLevel;
            ViewData["categoryId"] = categoryId ?? 0;
            ViewData["isActive"] = isActive;
            ViewData["minStock"] = minStock;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;

            return View(paginatedList);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewData["Sizes"] = ClothingSizes.Sizes.Keys.ToList();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,IsActive,Name,Description,Price,RarityLevel,CreationDate")]
            Product product, List<int> categoryIds, Dictionary<string, int> sizes, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                // Add selected categories
                foreach (var categoryId in categoryIds)
                {
                    product.ProductCategories.Add(new ProductCategory { CategoryId = categoryId });
                }

                // Add sizes with stock
                foreach (var size in sizes)
                {
                    product.AddSize(size.Key, size.Value);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();

                await SaveImages(imageFiles, product.Id);

                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewData["Sizes"] = ClothingSizes.Sizes.Keys.ToList();
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewData["Sizes"] = ClothingSizes.Sizes.Keys.ToList();
            ViewData["SelectedCategories"] = product.ProductCategories.Select(pc => pc.CategoryId).ToList();
            ViewData["SelectedSizes"] = product.Sizes.ToDictionary(ps => ps.Size, ps => ps.Stock);

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,IsActive,Name,Description,Price,RarityLevel,CreationDate")]
            Product product, List<int> categoryIds, Dictionary<string, int> sizes, List<IFormFile> imageFiles)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products
                        .Include(p => p.ProductCategories)
                        .Include(p => p.Sizes)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // UPDATE PRODUCT PROPERTIES
                    _context.Entry(existingProduct).CurrentValues.SetValues(product);

                    // UPDATE CATEGORIES
                    existingProduct.ProductCategories.Clear();
                    foreach (var categoryId in categoryIds)
                    {
                        existingProduct.ProductCategories.Add(new ProductCategory { CategoryId = categoryId });
                    }

                    // UPDATE SIZES
                    foreach (var size in sizes)
                    {
                        existingProduct.UpdateSizeStock(size.Key, size.Value);
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    await SaveImages(imageFiles, product.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewData["Sizes"] = ClothingSizes.Sizes.Keys.ToList();
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Sizes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // AUX METHOD TO SAVE IMAGES
        private async Task SaveImages(List<IFormFile> imageFiles, int productId)
        {
            if (imageFiles == null || !imageFiles.Any()) return;

            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            if (!Directory.Exists(imageDirectory)) Directory.CreateDirectory(imageDirectory);

            int imageCount = _context.Images.Count(i => i.ProductId == productId);

            foreach (var file in imageFiles)
            {
                var uniqueFileName = $"product{productId}-image{++imageCount}{Path.GetExtension(file.FileName)}";
                var imagePath = Path.Combine(imageDirectory, uniqueFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _context.Images.Add(new Image
                {
                    Url = $"/images/products/{uniqueFileName}",
                    ProductId = productId
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}