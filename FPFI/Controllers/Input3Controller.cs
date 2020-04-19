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
    [Authorize(Policy = "Data")]
    public class Input3Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Input3Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Input3
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Input3.Include(i => i.Entry);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Input3/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input3 = await _context.Input3
                .Include(i => i.Entry)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (input3 == null)
            {
                return NotFound();
            }

            return View(input3);
        }

        // GET: Input3/Create
        public IActionResult Create()
        {
            ViewData["Entry3Id"] = new SelectList(_context.Entry3, "Id", "Discriminator");
            return View();
        }

        // POST: Input3/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Entry3Id,DBH_sd,DBH_max,Random_SI,Random_BA,Id,Id_,Macrostand,Pyear,Age,N,BA,Dg," +
            "D100,Hd,Vt,Years,ThinningAges,NAfterThins,ThinTypes,ThinCoefs,Hp,Hm")] Input3 input3)
        {
            if (ModelState.IsValid)
            {
                _context.Add(input3);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Entry3Id"] = new SelectList(_context.Entry3, "Id", "Discriminator", input3.Entry3Id);
            return View(input3);
        }

        // GET: Input3/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input3 = await _context.Input3.SingleOrDefaultAsync(m => m.Id == id);
            if (input3 == null)
            {
                return NotFound();
            }
            ViewData["Entry3Id"] = new SelectList(_context.Entry3, "Id", "Discriminator", input3.Entry3Id);
            return View(input3);
        }

        // POST: Input3/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Entry3Id,DBH_sd,DBH_max,Random_SI,Random_BA,Id,Id_," +
            "Macrostand,Pyear,Age,N,BA,Dg,D100,Hd,Vt,Years,ThinningAges,NAfterThins,ThinTypes,ThinCoefs,Hp,Hm")] Input3 input3)
        {
            if (id != input3.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(input3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Input3Exists(input3.Id))
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
            ViewData["Entry3Id"] = new SelectList(_context.Entry3, "Id", "Discriminator", input3.Entry3Id);
            return View(input3);
        }

        // GET: Input3/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input3 = await _context.Input3
                .Include(i => i.Entry)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (input3 == null)
            {
                return NotFound();
            }

            return View(input3);
        }

        // POST: Input3/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var input3 = await _context.Input3.SingleOrDefaultAsync(m => m.Id == id);
            _context.Input3.Remove(input3);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Input3Exists(int id)
        {
            return _context.Input3.Any(e => e.Id == id);
        }
    }
}
