using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPFI.Data;
using FPFI.Models;

namespace FPFI.Controllers
{
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] Tree tree)
        {
            if (ModelState.IsValid)
            {
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
            return View(tree);
        }

        // POST: Trees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Tree tree)
        {
            if (id != tree.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
