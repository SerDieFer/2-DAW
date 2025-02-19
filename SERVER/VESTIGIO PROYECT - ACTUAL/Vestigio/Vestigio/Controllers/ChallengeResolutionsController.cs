using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vestigio.Data;
using Vestigio.Models;
using Vestigio.Utilities;

namespace Vestigio.Controllers
{
    public class ChallengeResolutionsController : Controller
    {
        private readonly VestigioDbContext _context;

        public ChallengeResolutionsController(VestigioDbContext context)
        {
            _context = context;
        }

        // GET: ChallengeResolutions
        public async Task<IActionResult> Index(int? pageNumber)
        {
            // CHALLENGE RESOLUTION DATA WITH ITS USER
            var challengeResolutionsData = _context.ChallengeResolutions.Include(c => c.Challenge)
                                                                        .Include(c => c.User)
                                                                        .OrderByDescending(c => c.ResolutionDate);
            int pageSize = 3;

            // PAGINATION
            return View(await PaginatedList<ChallengeResolution>.CreateAsync(
                challengeResolutionsData.AsNoTracking(),
                pageNumber ?? 1,
                pageSize
            ));
            //return View(await vestigioDBContext.ToListAsync());
        }

        // GET: ChallengeResolutions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challengeResolution = await _context.ChallengeResolutions
                .Include(c => c.Challenge)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (challengeResolution == null)
            {
                return NotFound();
            }

            return View(challengeResolution);
        }

        // GET: ChallengeResolutions/Create
        public IActionResult Create()
        {
            ViewData["ChallengeId"] = new SelectList(_context.Challenges, "Id", "Solution");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: ChallengeResolutions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChallengeId,UserId,ResolutionDate,CoinsEarned,PointsEarned")] ChallengeResolution challengeResolution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(challengeResolution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChallengeId"] = new SelectList(_context.Challenges, "Id", "Solution", challengeResolution.ChallengeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", challengeResolution.UserId);
            return View(challengeResolution);
        }

        // GET: ChallengeResolutions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challengeResolution = await _context.ChallengeResolutions.FindAsync(id);
            if (challengeResolution == null)
            {
                return NotFound();
            }
            ViewData["ChallengeId"] = new SelectList(_context.Challenges, "Id", "Solution", challengeResolution.ChallengeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", challengeResolution.UserId);
            return View(challengeResolution);
        }

        // POST: ChallengeResolutions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChallengeId,UserId,ResolutionDate,CoinsEarned,PointsEarned")] ChallengeResolution challengeResolution)
        {
            if (id != challengeResolution.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(challengeResolution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChallengeResolutionExists(challengeResolution.Id))
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
            ViewData["ChallengeId"] = new SelectList(_context.Challenges, "Id", "Solution", challengeResolution.ChallengeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", challengeResolution.UserId);
            return View(challengeResolution);
        }

        // GET: ChallengeResolutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challengeResolution = await _context.ChallengeResolutions
                .Include(c => c.Challenge)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (challengeResolution == null)
            {
                return NotFound();
            }

            return View(challengeResolution);
        }

        // POST: ChallengeResolutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var challengeResolution = await _context.ChallengeResolutions.FindAsync(id);
            if (challengeResolution != null)
            {
                _context.ChallengeResolutions.Remove(challengeResolution);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChallengeResolutionExists(int id)
        {
            return _context.ChallengeResolutions.Any(e => e.Id == id);
        }
    }
}
