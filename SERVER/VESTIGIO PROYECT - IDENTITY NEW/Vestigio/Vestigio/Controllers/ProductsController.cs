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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Vestigio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProductsController(VestigioDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index(
            string searchName, decimal? minPrice, decimal? maxPrice, int? pageNumber,
            int? rarityLevel, int? categoryId, bool? isActive, int? minStock, int? maxStock,
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
                productsQuery = productsQuery.Where(p => p.Sizes.Sum(ps => ps.Stock) >= minStock.Value);

            if (maxStock.HasValue)
                productsQuery = productsQuery.Where(p => p.Sizes.Sum(ps => ps.Stock) <= maxStock.Value);

            if (rarityLevel.HasValue)
                productsQuery = productsQuery.Where(p => p.RarityLevel == rarityLevel.Value);

            if (isActive.HasValue)
                productsQuery = productsQuery.Where(p => p.IsActive == isActive.Value);

            if (categoryId.HasValue && categoryId.Value > 0)
                productsQuery = productsQuery.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));

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
            ViewData["maxStock"] = maxStock;
            ViewData["startDate"] = startDate?.ToString("dd-MM-yyyy"); ;
            ViewData["endDate"] = endDate?.ToString("dd-MM-yyyy"); ;

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
            var categories = await _context.Categories.ToListAsync();
            // Pasar las categorías correctamente formateadas para JS
            ViewBag.Categories = categories.Select(c => new { Value = c.Id, Text = c.Name }).ToList();

            ViewData["Sizes"] = ClothingSizes.Sizes.Keys.ToList();
            return View();
        }

        // POST: Products/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,IsActive,Name,Description,Price,RarityLevel,CreationDate")] 
            Product product,
            List<int> categoryIds, // Cambiar a List<int> para recibir los IDs de las categorías
            Dictionary<string, int> sizes, // Cambiar a Dictionary para recibir tamaños y stock
            List<IFormFile> imageFiles) // Cambiar a List<IFormFile> para recibir las imágenes
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Procesar las categorías seleccionadas
                    if (categoryIds != null && categoryIds.Any())
                    {
                        foreach (var categoryId in categoryIds)
                        {

                            product.ProductCategories.Add(new ProductCategory
                            {
                                CategoryId = categoryId,
                                ProductId = product.Id // Esto se asignará automáticamente al guardar el producto
                            });
                        }
                    }

                    // Procesar los tamaños y stock
                    if (sizes != null && sizes.Any())
                    {
                        foreach (var size in sizes)
                        {
                            if (ClothingSizes.Sizes.ContainsKey(size.Key))
                            {
                                product.Sizes.Add(new ProductSize
                                {
                                    Size = size.Key,
                                    Stock = size.Value,
                                    ProductId = product.Id // Esto se asignará automáticamente al guardar el producto
                                });
                            }
                            else
                            {
                                ModelState.AddModelError("Sizes", $"Invalid size: {size.Key}");
                                return ViewWithErrors(product);
                            }
                        }
                    }

                    // Guardar el producto en la base de datos
                    _context.Products.Add(product);

                    await _context.SaveChangesAsync();

                    // Procesar las imágenes (si hay)
                    if (imageFiles != null && imageFiles.Any())
                    {
                        await SaveImages(imageFiles, product.Id);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                    return ViewWithErrors(product);
                }
            }

            return ViewWithErrors(product);
        }

        private IActionResult ViewWithErrors(Product product)
        {
            // Recargar los datos necesarios en la vista
            ViewBag.Categories = _context.Categories
                .Select(c => new { Value = c.Id, Text = c.Name }).ToList();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,IsActive,Name,Description,Price,RarityLevel,CreationDate")] Product product,
        List<int> categoryIds, // Cambiar a List<int> para recibir los IDs de las categorías
        Dictionary<string, int> sizes, // Recibir los tamaños y stock como un diccionario
        List<IFormFile> imageFiles)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el producto existente con sus relaciones
                    var existingProduct = await _context.Products
                        .Include(p => p.ProductCategories)
                        .Include(p => p.Sizes)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Actualizar propiedades básicas del producto
                    _context.Entry(existingProduct).CurrentValues.SetValues(product);

                    // Actualizar categorías
                    existingProduct.ProductCategories.Clear(); // Eliminar categorías existentes
                    foreach (var categoryId in categoryIds)
                    {
                        existingProduct.ProductCategories.Add(new ProductCategory
                        {
                            ProductId = product.Id, // Asignar el ID del producto
                            CategoryId = categoryId // Asignar el ID de la categoría
                        });
                    }

                    // Actualizar tamaños y stock
                    existingProduct.Sizes.Clear(); // Eliminar tamaños existentes
                    foreach (var size in sizes)
                    {
                        existingProduct.Sizes.Add(new ProductSize
                        {
                            ProductId = product.Id, // Asignar el ID del producto
                            Size = size.Key, // Asignar el tamaño
                            Stock = size.Value // Asignar el stock
                        });
                    }

                    // Guardar cambios en la base de datos
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    // Guardar imágenes (si hay)
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

            // Si el modelo no es válido, recargar los datos necesarios en la vista
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