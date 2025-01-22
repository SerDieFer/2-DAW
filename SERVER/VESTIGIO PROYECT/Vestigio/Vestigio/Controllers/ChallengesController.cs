using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;

namespace Vestigio.Controllers
{
    public class ChallengesController : Controller
    {
        private readonly VestigioDbContext _context;

        public ChallengesController(VestigioDbContext context)
        {
            _context = context;
        }

        // GET: Challenges
        public async Task<IActionResult> Index(int? pageNumber, string searchTitle, bool? isActive, int? rarityLevel, int pageSize = 5)
        {
            var challengesData = _context.Challenges
                                         .Include(c => c.Product)
                                         .Include(p => p.Images)
                                         .OrderByDescending(c => c.CreationDate)
                                         .AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(searchTitle))
            {
                challengesData = challengesData.Where(c => c.Title.Contains(searchTitle));
            }

            if (isActive.HasValue)
            {
                challengesData = challengesData.Where(c => c.Active == isActive.Value);
            }

            if (rarityLevel.HasValue)
            {
                challengesData = challengesData.Where(c => c.RarityLevel == rarityLevel.Value);
            }

            var paginatedList = await PaginatedList<Challenge>.CreateAsync(
                challengesData.AsNoTracking(), pageNumber ?? 1, pageSize);

            // Asignar valores a ViewData para persistir los filtros
            ViewData["searchTitle"] = searchTitle;
            ViewData["rarityLevel"] = rarityLevel;
            ViewData["isActive"] = isActive;

            return View(paginatedList);
        }

        // GET: Challenges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var challenge = await _context.Challenges
                                          .Include(c => c.Product)
                                          .Include(c => c.Images)
                                          .FirstOrDefaultAsync(m => m.Id == id);

            if (challenge == null) return NotFound();

            return View(challenge);
        }

        // GET: Challenges/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Challenges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Description,Solution,ExpPoints,Coins,RarityLevel,Active,CreationDate,ProductId,ProductLevel")]
            Challenge challenge, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                _context.Add(challenge);
                await _context.SaveChangesAsync();

                await SaveImages(imageFiles, challenge.Id);

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductId);
            return View(challenge);
        }

        // GET: Challenges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductId);
            return View(challenge);
        }

        // POST: Challenges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Description,Solution,ExpPoints,Coins,RarityLevel,Active,CreationDate,ProductId,ProductLevel")]
            Challenge challenge, List<IFormFile> imageFiles)
        {
            if (id != challenge.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(challenge);
                    await _context.SaveChangesAsync();

                    await SaveImages(imageFiles, challenge.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChallengeExists(challenge.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductId);
            return View(challenge);
        }

        // GET: Challenges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var challenge = await _context.Challenges
                                          .Include(c => c.Product)
                                          .FirstOrDefaultAsync(m => m.Id == id);

            if (challenge == null) return NotFound();

            return View(challenge);
        }

        // POST: Challenges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);

            if (challenge != null) _context.Challenges.Remove(challenge);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChallengeExists(int id)
        {
            return _context.Challenges.Any(e => e.Id == id);
        }

        // Método auxiliar para guardar imágenes
        private async Task SaveImages(List<IFormFile> imageFiles, int challengeId)
        {
            if (imageFiles == null || !imageFiles.Any()) return;

            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "challenges");
            if (!Directory.Exists(imageDirectory)) Directory.CreateDirectory(imageDirectory);

            int imageCount = _context.Images.Count(i => i.ChallengeId == challengeId);

            foreach (var file in imageFiles)
            {
                var uniqueFileName = $"challenge{challengeId}-image{++imageCount}{Path.GetExtension(file.FileName)}";
                var imagePath = Path.Combine(imageDirectory, uniqueFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _context.Images.Add(new Image
                {
                    Url = $"/images/challenges/{uniqueFileName}",
                    ChallengeId = challengeId
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
