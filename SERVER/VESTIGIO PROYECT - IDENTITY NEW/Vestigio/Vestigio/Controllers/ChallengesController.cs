using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Vestigio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ChallengesController : Controller
    {
        private readonly VestigioDbContext _context;

        public ChallengesController(VestigioDbContext context)
        {
            _context = context;
        }

        // GET: Challenges
        public async Task<IActionResult> Index(
            int? pageNumber, string searchTitle, int? minCoins, int? maxCoins,
            int? rarityLevel, SolutionMode? solutionMode, bool? isActive,
            DateTime? startDate, DateTime? endDate, int? minExp, int? maxExp,
            DateTime? startReleaseDate, DateTime? endReleaseDate, int pageSize = 5)
        {
            var challenges = _context.Challenges
                .Include(c => c.Images)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTitle))
            {
                challenges = challenges.Where(c => c.Title.Contains(searchTitle));
            }

            if (minCoins.HasValue)
            {
                challenges = challenges.Where(c => c.Coins >= minCoins.Value);
            }

            if (maxCoins.HasValue)
            {
                challenges = challenges.Where(c => c.Coins <= maxCoins.Value);
            }

            if (rarityLevel.HasValue)
            {
                challenges = challenges.Where(c => c.RarityLevel == rarityLevel);
            }

            if (solutionMode.HasValue)
            {
                challenges = challenges.Where(c => c.SolutionMode == solutionMode.Value);
            }

            if (isActive.HasValue)
            {
                challenges = challenges.Where(c => c.IsActive == isActive.Value);
            }

            if (startDate.HasValue)
            {
                challenges = challenges.Where(c => c.CreationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                challenges = challenges.Where(c => c.CreationDate <= endDate.Value);
            }

            if (minExp.HasValue)
            {
                challenges = challenges.Where(c => c.ExpPoints >= minExp.Value);
            }

            if (maxExp.HasValue)
            {
                challenges = challenges.Where(c => c.ExpPoints <= maxExp.Value);
            }

            if (startReleaseDate.HasValue)
            {
                challenges = challenges.Where(c => c.ReleaseDate >= startReleaseDate.Value);
            }

            if (endReleaseDate.HasValue)
            {
                challenges = challenges.Where(c => c.ReleaseDate <= endReleaseDate.Value);
            }

            // Pagination
            var paginatedChallenges = await PaginatedList<Challenge>.CreateAsync(
                challenges.OrderBy(c => c.CreationDate), pageNumber ?? 1, pageSize);

            // Pass filters to ViewData
            ViewData["searchTitle"] = searchTitle;
            ViewData["minCoins"] = minCoins;
            ViewData["maxCoins"] = maxCoins;
            ViewData["isActive"] = isActive;
            ViewData["startDate"] = startDate?.ToString("dd-MM-yyyy");
            ViewData["endDate"] = endDate?.ToString("dd-MM-yyyy");
            ViewData["minExp"] = minExp;
            ViewData["maxExp"] = maxExp;
            ViewData["startReleaseDate"] = startReleaseDate?.ToString("dd-MM-yyyy");
            ViewData["endReleaseDate"] = endReleaseDate?.ToString("dd-MM-yyyy");
            ViewData["pageSize"] = pageSize;

            ViewData["SolutionModes"] = new SelectList(Enum.GetValues(typeof(SolutionMode)));
            ViewData["RarityLevels"] = Enumerable.Range(1, 10).Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = $"{i} - {LevelsNaming.GetLevelName(i)}"
            });

            return View(await PaginatedList<Challenge>
                .CreateAsync(challenges.OrderBy(c => c.CreationDate), pageNumber ?? 1, pageSize));
        }

        // GET: Challenges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challenge = await _context.Challenges
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (challenge == null)
            {
                return NotFound();
            }

            return View(challenge);
        }

        // GET: Challenges/Create
        public IActionResult Create()
        {
            PrepareDropdowns();
            return View(new Challenge
            {
                IsActive = true,
                CreationDate = DateTime.Now,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,ExpPoints,Coins,RarityLevel,SolutionMode," +
            "Password,ReleaseDate,ProductLevel,ProductId,IsActive")]
            Challenge challenge,
            List<IFormFile> imageFiles)
        {

            ValidateChallenge(challenge);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(challenge);
                    await _context.SaveChangesAsync();

                    if (imageFiles != null && imageFiles.Any())
                    {
                        await SaveImages(imageFiles, challenge.Id);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                }
            }

            PrepareDropdowns();
            return View(challenge);
        }

        // GET: Challenges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var challenge = await _context.Challenges
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (challenge == null) return NotFound();

            PrepareDropdowns();
            return View(challenge);
        }

        // POST: Challenges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,IsActive,Title,Description,ExpPoints,Coins,RarityLevel," +
            "CreationDate,ProductLevel,ProductId,SolutionMode,Password,ReleaseDate")]
            Challenge challenge,
            List<IFormFile> imageFiles)
        {
            if (id != challenge.Id) return NotFound();

            ValidateChallenge(challenge);

            if (ModelState.IsValid)
            {
                try
                {
                    // Cargar entidad existente
                    var existingChallenge = await _context.Challenges
                        .Include(c => c.Images)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    _context.Entry(existingChallenge).CurrentValues.SetValues(challenge);

                    await SaveImages(imageFiles, challenge.Id);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChallengeExists(challenge.Id)) return NotFound();
                    throw;
                }
            }

            PrepareDropdowns();
            return View(challenge);
        }

        // GET: Challenges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challenge = await _context.Challenges
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (challenge == null)
            {
                return NotFound();
            }

            return View(challenge);
        }

        // POST: Challenges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge != null)
            {
                _context.Challenges.Remove(challenge);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChallengeExists(int id)
        {
            return _context.Challenges.Any(e => e.Id == id);
        }

        private void ValidateChallenge(Challenge challenge)
        {
            // Validación de modo de solución
            if (challenge.SolutionMode == SolutionMode.Password)
            {
                if (string.IsNullOrWhiteSpace(challenge.Password))
                    ModelState.AddModelError("Password", "The password is required!");
                
                challenge.ReleaseDate = null;
            }
            else if (challenge.SolutionMode == SolutionMode.TimeBased)
            {
                if (!challenge.ReleaseDate.HasValue)
                    ModelState.AddModelError("ReleaseDate", "The release date is required!");
                else if (challenge.ReleaseDate <= DateTime.Now)
                    ModelState.AddModelError("ReleaseDate", "The release date must be in the future! ");
                
                challenge.Password = null;
            }

            // Validación de asociación
            if (challenge.ProductLevel.HasValue && challenge.ProductId.HasValue)
                ModelState.AddModelError("", "Select only one association method!");
            else if (!challenge.ProductLevel.HasValue && !challenge.ProductId.HasValue)
                ModelState.AddModelError("", "You must select an association method!");
        }

        private void PrepareDropdowns()
        {
            ViewBag.ProductLevels = Enumerable.Range(1, 10)
                .Select(i => new SelectListItem 
                { 
                    Value = i.ToString(),
                    Text = $"{i} - {LevelsNaming.GetLevelName(i)}" 
                });

            ViewBag.Products = new SelectList(_context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToList(), "Id", "Name");
        }

        // AUX METHOD TO SAVE IMAGES
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
