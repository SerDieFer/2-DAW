using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<IActionResult> Index(int? pageNumber, string searchName, decimal? minPrice, decimal? maxPrice, int? rarityLevel, int? categoryId, int pageSize = 5)
        {
            var productsData = _context.Products
                                       .Include(p => p.Category)
                                       .Include(p => p.Images)
                                       .AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(searchName))
            {
                productsData = productsData.Where(p => p.Name.Contains(searchName));
            }

            if (minPrice.HasValue)
            {
                productsData = productsData.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsData = productsData.Where(p => p.Price <= maxPrice.Value);
            }

            if (rarityLevel.HasValue)
            {
                productsData = productsData.Where(p => p.RarityLevel == rarityLevel.Value);
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {

                productsData = productsData.Where(p => p.CategoryId == categoryId);
            }

            // Paginación
            var paginatedList = await PaginatedList<Product>.CreateAsync(
                productsData.AsNoTracking(),
                pageNumber ?? 1,
                pageSize
            );

            // Obtener todas las categorías para el filtro
            ViewData["Categories"] = await _context.Categories.ToListAsync();

            // Asignamos los valores a ViewData
            ViewData["searchName"] = searchName;
            ViewData["minPrice"] = minPrice;
            ViewData["maxPrice"] = maxPrice;
            ViewData["rarityLevel"] = rarityLevel;
            ViewData["categoryId"] = categoryId ?? 0;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Description,Price,Stock,RarityLevel,CategoryId")] Product product, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {   
                _context.Add(product);
                await _context.SaveChangesAsync();

                // Crear la carpeta de imágenes si no existe
                var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                // Guardar las imágenes asociadas al producto
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    // Obtener el contador de imágenes para este producto
                    int imageCount = _context.Images.Count(i => i.ProductId == product.Id);

                    foreach (var file in imageFiles)
                    {

                        // Crear el nombre de archivo único basado en ProductId y el número de imagen
                        var uniqueFileName = $"product{product.Id}-image{imageCount + 1}{Path.GetExtension(file.FileName)}";
                        var imagePath = Path.Combine(imageDirectory, uniqueFileName);

                        // Guardar la imagen en el directorio
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Crear la entidad de imagen y asociarla al producto
                        var image = new Image
                        {
                            Url = $"/images/products/{uniqueFileName}",
                            ProductId = product.Id
                        };

                        _context.Images.Add(image);
                        imageCount++;
                    }

                    await _context.SaveChangesAsync();
                }

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

            var product = await _context.Products
                                        .FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Description,Price,Stock,RarityLevel,CategoryId")]
            Product product,
            List<IFormFile> imageFiles,
            List<int> imagesToDelete)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizamos el producto
                    _context.Update(product);

                    // Eliminar imágenes seleccionadas
                    if (imagesToDelete != null && imagesToDelete.Any())
                    {
                        var images = _context.Images.Where(i => imagesToDelete.Contains(i.Id)).ToList();

                        foreach (var image in images)
                        {
                            // Eliminamos el archivo físico del servidor
                            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.Url.TrimStart('/'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            // Eliminamos el registro de la base de datos
                            _context.Images.Remove(image);
                        }
                    }

                    // Crear la carpeta de imágenes si no existe
                    var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

                    if (!Directory.Exists(imageDirectory))
                    {
                        Directory.CreateDirectory(imageDirectory);
                    }

                    // Guardar nuevas imágenes si se han subido
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        int imageCount = _context.Images.Count(i => i.ProductId == product.Id);

                        foreach (var file in imageFiles)
                        {
                            var uniqueFileName = $"product{product.Id}-image{imageCount + 1}{Path.GetExtension(file.FileName)}";
                            var imagePath = Path.Combine(imageDirectory, uniqueFileName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var image = new Image
                            {
                                Url = $"/images/products/{uniqueFileName}",
                                ProductId = product.Id
                            };

                            _context.Images.Add(image);
                            imageCount++;
                        }
                    }

                    await _context.SaveChangesAsync();
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
    }
}
