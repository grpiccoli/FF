using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FPFI.Data;
using FPFI.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FPFI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System.Security.Claims;

namespace FPFI.Controllers
{
    [Authorize]
    public class InputsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public InputsController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Inputs
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Inputs.ToListAsync());
        }

        // GET: Inputs/Details/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input = await _context.Inputs
                .SingleOrDefaultAsync(m => m.Id == id);
            if (input == null)
            {
                return NotFound();
            }

            return View(input);
        }

        [HttpPost]
        public JsonResult LoadData(int? id)
        {
            var inputs = _context.Inputs
                        .Where(i => i.EntryId == id);

            var recTot = inputs.Count();

            var inputsPage = inputs.Take(10).ToList();

            return Json(new { draw = 1, recordsFiltered = inputsPage.Count(), recordsTotal = recTot, data = inputsPage });
        }

        // GET: Inputs/Edit/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input = await _context.Inputs.SingleOrDefaultAsync(m => m.Id == id);
            if (input == null)
            {
                return NotFound();
            }
            return View(input);
        }

        // POST: Inputs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Macrostand,Pyear,Age,N,BA,Dg,D100,Hd,Vt,Years,Hp,Hm")] Input input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(input);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (input.Id)
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        // GET: Inputs/Delete/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var input = await _context.Inputs
                .SingleOrDefaultAsync(m => m.Id == id);
            if (input == null)
            {
                return NotFound();
            }

            return View(input);
        }

        // POST: Inputs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var input = await _context.Inputs.SingleOrDefaultAsync(m => m.Id == id);
            _context.Inputs.Remove(input);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InputExists(int id)
        {
            return _context.Inputs.Any(e => e.Id == id);
        }

    }
}
