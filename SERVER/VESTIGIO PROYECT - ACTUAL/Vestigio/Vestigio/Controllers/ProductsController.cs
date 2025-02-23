using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;
using System.Globalization;

namespace Vestigio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly VestigioDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(VestigioDbContext context, UserManager<User> userManager, ILogger<ProductsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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
                .Include(p => p.ProductSizes)
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
                productsQuery = productsQuery.Where(p => p.ProductSizes.Sum(ps => ps.Stock) >= minStock.Value);

            if (maxStock.HasValue)
                productsQuery = productsQuery.Where(p => p.ProductSizes.Sum(ps => ps.Stock) <= maxStock.Value);

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
                .Include(p => p.ProductSizes)
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

            ViewData["ProductSizes"] = ClothingSizes.Sizes.Keys.ToList();
            return View();
        }

        // POST: Products/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,IsActive,Name,Description,PriceInput,RarityLevel,CreationDate")]
            Product product,
            List<int> categoryIds, // Cambiar a List<int> para recibir los IDs de las categorías
            Dictionary<string, int> sizes, // Cambiar a Dictionary para recibir tamaños y stock
            List<IFormFile> imageFiles) // Cambiar a List<IFormFile> para recibir las imágenes
        {
            if (ModelState.IsValid)
            {
                try
                {

                    // Convertir PriceInput a Price (manualmente)
                    if (!string.IsNullOrEmpty(product.PriceInput))
                    {
                        if (decimal.TryParse(product.PriceInput.Replace(',', '.'),
                                           NumberStyles.Any,
                                           CultureInfo.InvariantCulture,
                                           out var parsedPrice))
                        {
                            product.Price = parsedPrice;
                        }
                        else
                        {
                            ModelState.AddModelError("PriceInput", "Formato inválido");
                        }
                    }

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
                                product.ProductSizes.Add(new ProductSize
                                {
                                    Size = size.Key,
                                    Stock = size.Value,
                                    ProductId = product.Id // Esto se asignará automáticamente al guardar el producto
                                });
                            }
                            else
                            {
                                ModelState.AddModelError("ProductSizes", $"Invalid size: {size.Key}");
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

            ViewData["ProductSizes"] = ClothingSizes.Sizes.Keys.ToList();

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
                .Include(p => p.ProductSizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Configurar ViewBag.Categories
            ViewBag.Categories = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewData["ProductSizes"] = ClothingSizes.Sizes.Keys.ToList();
            ViewData["SelectedCategories"] = product.ProductCategories.Select(pc => pc.CategoryId).ToList();
            ViewData["SelectedSizes"] = product.ProductSizes.ToDictionary(ps => ps.Size, ps => ps.Stock);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,IsActive,Name,Description,PriceInput,RarityLevel,CreationDate")] Product product,
            List<int> categoryIds,
            Dictionary<string, int> sizes,
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

                    // Convertir PriceInput a Price (manualmente)
                    if (!string.IsNullOrEmpty(product.PriceInput))
                    {
                        if (decimal.TryParse(product.PriceInput.Replace(',', '.'),
                                           NumberStyles.Any,
                                           CultureInfo.InvariantCulture,
                                           out var parsedPrice))
                        {
                            product.Price = parsedPrice;
                        }
                        else
                        {
                            ModelState.AddModelError("PriceInput", "Formato inválido");
                        }
                    }

                    // Obtener el producto existente junto a sus relaciones
                    var existingProduct = await _context.Products
                        .Include(p => p.ProductCategories)
                        .Include(p => p.ProductSizes)
                        .Include(p => p.Images)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Actualizar las propiedades básicas del producto
                    _context.Entry(existingProduct).CurrentValues.SetValues(product);

                    // Actualizar las categorías:
                    // Se eliminan las existentes y se agregan las nuevas
                    existingProduct.ProductCategories.Clear();
                    foreach (var categoryId in categoryIds)
                    {
                        existingProduct.ProductCategories.Add(new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = categoryId
                        });
                    }

                    // Actualizar los tamaños (ProductSizes)
                    // Actualizar los existentes, agregar los nuevos y eliminar los que ya no existan

                    // Actualizar o agregar tallas según el diccionario 'sizes'
                    foreach (var newSize in sizes)
                    {
                        // Buscar si la talla ya existe
                        var existingSize = existingProduct.ProductSizes
                            .FirstOrDefault(ps => ps.Size == newSize.Key);

                        if (existingSize != null)
                        {
                            // Actualizar la talla existente
                            existingSize.Stock = newSize.Value;
                            existingSize.IsActive = newSize.Value > 0;
                        }
                        else
                        {
                            // Agregar nueva talla
                            existingProduct.ProductSizes.Add(new ProductSize
                            {
                                ProductId = product.Id,
                                Size = newSize.Key,
                                Stock = newSize.Value,
                                IsActive = newSize.Value > 0
                            });
                        }
                    }

                    // Eliminar las tallas que ya no se encuentran en la nueva selección
                    var sizesToRemove = existingProduct.ProductSizes
                        .Where(ps => !sizes.ContainsKey(ps.Size))
                        .ToList();

                    foreach (var sizeToRemove in sizesToRemove)
                    {
                        existingProduct.ProductSizes.Remove(sizeToRemove);
                    }

                    // Manejar imágenes
                    if (imageFiles != null && imageFiles.Any())
                    {
                        await DeleteImages(existingProduct.Images);
                        existingProduct.Images.Clear();
                        await SaveImages(imageFiles, product.Id);
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                    _logger.LogError(ex, "Error al editar el producto ID {ProductId}", product.Id);
                }
            }

            // Recargar datos para la vista en caso de error
            ViewBag.Categories = await _context.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();
            ViewData["ProductSizes"] = ClothingSizes.Sizes.Keys.ToList();
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
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Buscar el producto con todas sus relaciones
            var product = await _context.Products
                .Include(p => p.ProductSizes) // Tamaños
                .Include(p => p.ProductCategories) // Categorías
                .Include(p => p.OrderDetails) // Detalles de pedido
                .Include(p => p.Images) // Imágenes
                .Include(p => p.Challenges) // Desafíos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Eliminar los tamaños relacionados
            _context.ProductSizes.RemoveRange(product.ProductSizes);

            // Eliminar las categorías relacionadas
            _context.ProductCategories.RemoveRange(product.ProductCategories);

            // Eliminar los detalles de pedido relacionados
            _context.OrderDetails.RemoveRange(product.OrderDetails);

            // Eliminar las imágenes relacionadas
            _context.Images.RemoveRange(product.Images);

            // Eliminar los desafíos relacionados
            _context.Challenges.RemoveRange(product.Challenges);

            // Eliminar el producto
            _context.Products.Remove(product);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkInactive(int id)
        {
            // Buscar el producto incluyendo sus desafíos asociados
            var product = await _context.Products
                .Include(p => p.Challenges)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Marcar el producto como inactivo
            product.IsActive = false;

            // Marcar también los desafíos asociados como inactivos
            if (product.Challenges != null && product.Challenges.Any())
            {
                foreach (var challenge in product.Challenges)
                {
                    challenge.IsActive = false;
                    _context.Update(challenge);
                }
            }

            _context.Update(product);
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

        private async Task DeleteImages(ICollection<Image> images)
        {
            if (images == null || !images.Any()) return;

            foreach (var image in images)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.Url.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Images.RemoveRange(images);
            await _context.SaveChangesAsync();
        }

    }
}