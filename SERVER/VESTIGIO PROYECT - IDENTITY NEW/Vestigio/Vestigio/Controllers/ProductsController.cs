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
                .Include(p => p.Category)
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
                productsQuery = productsQuery.Where(p => p.Stock >= minStock.Value);

            if (rarityLevel.HasValue)
                productsQuery = productsQuery.Where(p => p.RarityLevel == rarityLevel.Value);

            if (isActive.HasValue)
                productsQuery = productsQuery.Where(p => p.IsActive == isActive.Value);

            if (categoryId.HasValue && categoryId.Value > 0)
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);

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
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,IsActive,Name,Description,Price,Stock,RarityLevel,CreationDate,CategoryId")] 
            Product product, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                await SaveImages(imageFiles, product.Id);

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, 
            [Bind("Id,IsActive,Name,Description,Price,Stock,RarityLevel,CreationDate,CategoryId")]
            Product product, List<IFormFile> imageFiles)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
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
                .Include(p => p.Category)
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
