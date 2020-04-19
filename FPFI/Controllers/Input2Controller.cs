using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPFI.Data;
using FPFI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FPFI.Controllers
{
    [Authorize(Policy = "Data")]
    public class Input2Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Input2Controller(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Input2
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Input2.Include(i => i.Entry);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Input2/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input2 = await _context.Input2
                .Include(i => i.Entry)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (input2 == null)
            {
                return NotFound();
            }

            return View(input2);
        }

        // GET: Input2/Create
        public IActionResult Create()
        {
            ViewData["Entry2Id"] = new SelectList(_context.Entry2, "Id", "Discriminator");
            return View();
        }

        // POST: Input2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Entry2Id,Id,Id_,Macrostand,Pyear,Age,N,BA,Dg,D100,Hd,Vt,Years,ThinningAges,NAfterThins,ThinTypes,ThinCoefs,Hp,Hm")] Input2 input2)
        {
            if (ModelState.IsValid)
            {
                _context.Add(input2);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Entry2Id"] = new SelectList(_context.Entry2, "Id", "Discriminator", input2.Entry2Id);
            return View(input2);
        }

        // GET: Input2/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input2 = await _context.Input2.SingleOrDefaultAsync(m => m.Id == id);
            if (input2 == null)
            {
                return NotFound();
            }
            ViewData["Entry2Id"] = new SelectList(_context.Entry2, "Id", "Discriminator", input2.Entry2Id);
            return View(input2);
        }

        // POST: Input2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Entry2Id,Id,Id_,Macrostand,Pyear,Age,N,BA,Dg,D100,Hd,Vt,Years,ThinningAges,NAfterThins,ThinTypes,ThinCoefs,Hp,Hm")] Input2 input2)
        {
            if (id != input2.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(input2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Input2Exists(input2.Id))
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
            ViewData["Entry2Id"] = new SelectList(_context.Entry2, "Id", "Discriminator", input2.Entry2Id);
            return View(input2);
        }

        // GET: Input2/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input2 = await _context.Input2
                .Include(i => i.Entry)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (input2 == null)
            {
                return NotFound();
            }

            return View(input2);
        }

        // POST: Input2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var input2 = await _context.Input2.SingleOrDefaultAsync(m => m.Id == id);
            _context.Input2.Remove(input2);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Input2Exists(int id)
        {
            return _context.Input2.Any(e => e.Id == id);
        }
    }
}
