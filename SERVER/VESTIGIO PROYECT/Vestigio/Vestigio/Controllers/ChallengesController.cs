using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Index(int? pageNumber)
        {
            // CHALLENGE DATA WITH PRODUCT 
            var challengesData = _context.Challenges.Include(c => c.Product)
                                                    .OrderByDescending(s => s.CreationDate);

            int pageSize = 3;

            // PAGINATION
            return View(await PaginatedList<Challenge>.CreateAsync(
                challengesData.AsNoTracking(),
                pageNumber ?? 1,
                pageSize
            ));



            //return View(await vestigioDBContext.ToListAsync());
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
                .Include(p => p.Images)
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Challenges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Solution,ExpPoints,Coins,RarityLevel,Active,CreationDate,ProductId,ProductLevel")] Challenge challenge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(challenge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductId);
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductId);
            return View(challenge);
        }

        // POST: Challenges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Solution,ExpPoints,Coins,RarityLevel,Active,CreationDate,ProductId,ProductLevel")] Challenge challenge)
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", challenge.ProductId);
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
