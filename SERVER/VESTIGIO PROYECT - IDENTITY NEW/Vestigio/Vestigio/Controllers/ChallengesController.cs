using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;

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
            var challenges = _context.Challenges.Include(c => c.Product).AsQueryable();

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
            ViewData["rarityLevel"] = rarityLevel;
            ViewData["solutionMode"] = solutionMode;
            ViewData["isActive"] = isActive;
            ViewData["startDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["endDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["minExp"] = minExp;
            ViewData["maxExp"] = maxExp;
            ViewData["startReleaseDate"] = startReleaseDate?.ToString("yyyy-MM-dd");
            ViewData["endReleaseDate"] = endReleaseDate?.ToString("yyyy-MM-dd");
            ViewData["pageSize"] = pageSize;

            return View(paginatedChallenges);
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
            ViewData["ProductLevel"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Challenges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsActive,Title,Description,ExpPoints,Coins,RarityLevel,CreationDate,ProductLevel,ProductId,SolutionMode,Password,ReleaseDate")] Challenge challenge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(challenge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductLevel"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductLevel);
            return View(challenge);
        }

        // GET: Challenges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null)
            {
                return NotFound();
            }
            ViewData["ProductLevel"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductLevel);
            return View(challenge);
        }

        // POST: Challenges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsActive,Title,Description,ExpPoints,Coins,RarityLevel,CreationDate,ProductLevel,ProductId,SolutionMode,Password,ReleaseDate")] Challenge challenge)
        {
            if (id != challenge.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(challenge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChallengeExists(challenge.Id))
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
            ViewData["ProductLevel"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductLevel);
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
    }
}
