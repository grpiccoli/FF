using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPFI.Data;
using FPFI.Models;
using Microsoft.AspNetCore.Authorization;

namespace FPFI.Controllers
{
    [Authorize(Roles = "Administrator", Policy = "Data")]
    public class TreesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TreesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tree.ToListAsync());
        }

        // GET: Trees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tree = await _context.Tree
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tree == null)
            {
                return NotFound();
            }

            return View(tree);
        }

        // GET: Trees/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Species"] = new SelectList(
                from Species s in _context.Species 
                select new { s.Id, s.Name }, "Id", "Name");

            ViewData["Regions"] = new SelectList(
                from Region r in _context.Region
                select new { r.Id, r.Name }, "Id", "Name");

            var def = await _context.Tree.SingleOrDefaultAsync(t => t.Default == true);

            ViewData["Default"] = $"{def.Region} {def.Species}";

            return View();
        }

        // POST: Trees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, SpeciesId, RegionId, Default")] Tree tree)
        {
            if (ModelState.IsValid)
            {
                if (tree.Default)
                {
                    await _context.Tree.ForEachAsync(t => t.Default = false);
                }
                _context.Add(tree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tree);
        }

        // GET: Trees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tree = await _context.Tree.SingleOrDefaultAsync(m => m.Id == id);
            if (tree == null)
            {
                return NotFound();
            }

            ViewData["Species"] = new SelectList(
                from Species s in _context.Species
                select new { s.Id, s.Name }, "Id", "Name");

            ViewData["Regions"] = new SelectList(
                from Region r in _context.Region
                select new { r.Id, r.Name }, "Id", "Name");

            var def = await _context.Tree.SingleOrDefaultAsync(t => t.Default == true);

            ViewData["Default"] = $"{def.Region} {def.Species}";

            return View(tree);
        }

        // POST: Trees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, SpeciesId, RegionId")] Tree tree)
        {
            if (id != tree.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (tree.Default)
                    {
                        await _context.Tree.ForEachAsync(t => t.Default = false);
                    }
                    _context.Update(tree);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreeExists(tree.Id))
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
            return View(tree);
        }

        // GET: Trees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tree = await _context.Tree
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tree == null)
            {
                return NotFound();
            }

            return View(tree);
        }

        // POST: Trees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tree = await _context.Tree.SingleOrDefaultAsync(m => m.Id == id);
            _context.Tree.Remove(tree);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreeExists(int id)
        {
            return _context.Tree.Any(e => e.Id == id);
        }
    }
}
