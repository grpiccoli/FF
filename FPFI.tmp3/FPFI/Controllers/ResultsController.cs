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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Diagnostics;
using FPFI.Services;
using System.Text.Encodings.Web;
using FPFI.Models.ViewModels;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FPFI.Controllers
{
    [Authorize]
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string platform;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IViewRenderService _viewRenderService;

        public ResultsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            IEmailSender emailSender,
            IRazorViewEngine viewEngine,
            IViewRenderService viewRenderService,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _viewRenderService = viewRenderService;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _emailSender = emailSender;
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
            platform = RTools.GetPlatform();
        }

        public IConfiguration Configuration { get; }

        // GET: Entries
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context
                .Entries2
                .Include(e => e.ApplicationUser);

            return View(await applicationDbContext.ToListAsync());
        }
        
        public async Task<IActionResult> SubmitDetails(int? id, int? tk)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrySims = _context.Simulations
                    .Where(s => s.EntryId == id);

            if (entrySims == null)
            {
                return NotFound();
            }

            var macrostands = entrySims.Select(c => c.Macrostand).Distinct().ToList();

            if (tk == null)
            {
                tk = macrostands.Count();
            }

            var graph = new List<string> { };
            //EDIT!!!!!!!
            foreach (var ms in macrostands.Take(tk.Value))
            {
                graph.Add("{\"lineColor\": \"purple\",\"balloonText\": \"<div style='margin:5px;'><b>Age:</b>[[x]]<br><b>\"+value+\":</b>[[y]]<br><b>Ms:</b>" + ms + "</div>\",\"bullet\":\"round\",\"minBulletSize\":0,\"bulletSize\":0,\"hideBulletCount\":50,\"xField\":\"Age\",\"yField\":\"" + ms + "y\"}");
            }

            ViewData["Graphs"] = String.Join(",", graph);

            var aged = entrySims.GroupBy(e => e.Age).OrderBy(e => e.Key);

            var layout = new List<string>
            {
                "Hd","N","Dg","Vt","CAI_Dg","CAI_Vt","MAI_Dg","MAI_Vt"
            };

            var data = new Dictionary<string, List<string>>();

            foreach (var dat in layout)
            {
                data.Add(dat, new List<string> { });
            }

            foreach (var age in aged)
            {
                var vars = new Dictionary<string, List<string>>();

                foreach (var var in layout)
                {
                    vars.Add(var, new List<string> { });
                }

                foreach (var sim in age)
                {
                    vars["Dg"].Add('"' + sim.Macrostand + "y\":" + sim.Dg);
                    vars["Hd"].Add('"' + sim.Macrostand + "y\":" + sim.Hd);
                    vars["Vt"].Add('"' + sim.Macrostand + "y\":" + sim.Vt);
                    vars["N"].Add('"' + sim.Macrostand + "y\":" + sim.N);
                    if(sim.CAI_Vt != null) vars["CAI_Vt"].Add('"' + sim.Macrostand + "y\":" + sim.CAI_Vt);
                    if (sim.CAI_Dg != null) vars["CAI_Dg"].Add('"' + sim.Macrostand + "y\":" + sim.CAI_Dg);
                    vars["MAI_Vt"].Add('"' + sim.Macrostand + "y\":" + sim.MAI_Vt);
                    vars["MAI_Dg"].Add('"' + sim.Macrostand + "y\":" + sim.MAI_Dg);
                }

                foreach (var var in layout)
                {
                    data[var].Add("{\"Age\":" + age.Key + ',' + String.Join(',', vars[var]) + '}');
                }
            }

            foreach (var var in layout)
            {
                ViewData[var] = String.Join(",", data[var]);
            }

            return View(await entrySims.ToListAsync());
        }

        public IActionResult SubmitDetails2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrySims = _context.Simulations
                    .Where(s => s.EntryId == id);

            if (entrySims == null)
            {
                return NotFound();
            }

            var aged = entrySims
                            .GroupBy(e => e.Macrostand)
                            .Select(g => new {
                                Name = g.Key,
                                Ages = g.OrderBy(x => x.Age)
                            });

            var layout = new List<string>
            {
                "Hd","N","Dg","Vt","CAI_Dg","CAI_Vt","MAI_Dg","MAI_Vt"
            };

            var data = new Dictionary<string, List<string>>();

            foreach(var dat in layout)
            {
                data.Add(dat,new List<string> { });
            }

            foreach (var macrostand in aged)
            {
                var vars = new Dictionary<string, List<double>>();

                vars.Add("Age",new List<double> { });

                foreach (var var in layout)
                {
                    vars.Add(var, new List<double> { });
                }

                foreach (var sim in macrostand.Ages)
                {
                    vars["Age"].Add(sim.Age);
                    vars["Hd"].Add(sim.Hd);
                    vars["N"].Add(sim.N);
                    vars["Dg"].Add(sim.Dg);
                    vars["Vt"].Add(sim.Vt);
                    if (sim.CAI_Dg.HasValue) vars["CAI_Dg"].Add(sim.CAI_Dg.Value);
                    if (sim.CAI_Vt.HasValue) vars["CAI_Vt"].Add(sim.CAI_Vt.Value);
                    vars["MAI_Dg"].Add(sim.MAI_Dg);
                    vars["MAI_Vt"].Add(sim.MAI_Vt);
                }
                string start = $"{{ x: [{String.Join(",",vars["Age"])}], y: [";
                string end = $"], type: 'scatter', hovermode: 'closest', name: '{macrostand.Name}', mode: 'lines', line: {{color: 'blue',width:1}} }}";

                foreach (var var in layout)
                {
                    data[var].Add(start + String.Join(",", vars[var]) + end);
                }
            }

            foreach(var i in layout)
            {
                ViewData["Layout"+i] = $"{{showlegend: false,xaxis:{{ title: 'Ages'}}, yaxis: {{ title: '{i}' }} }}";
                ViewData["Data"+i] = $"[{String.Join(",", data[i])}];";
            }

            return View();
        }

        [HttpGet]
        public IActionResult Output(int id)
        {
            var entry = _context
                .Entries2
                .FirstOrDefault(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
            return PartialView("_Output", entry);
        }

        [HttpGet]
        public IActionResult Settings(int id)
        {
            var entry = _context
                .Entries2
                .FirstOrDefault(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
            return PartialView("_Settings", entry);
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var model = new DownloadViewModel
            {
                Id = id,
                Xls = false,
                Xlsx = false,
                Xml = false,
                Csv = false
            };
            return PartialView("_Download", model);
        }

        [HttpPost]
        public async Task<IActionResult> Download([Bind("Id,Xml,Csv,Xls,Xlsx,Password")] DownloadViewModel Dl)
        {
            var userId = _userManager.GetUserId(User);

            var entry = await _context
                .Entries2
                .FirstAsync(e => e.Id == Dl.Id && e.ApplicationUserId == userId);

            var user = await _context
                .ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == userId);

            #region Export with R
            //RTools.GenerateFiles(
            //    RTools.GetRformattedConnectionString(_hostingEnvironment, platform),
            //    Regex.Replace(_hostingEnvironment.WebRootPath.ToString(), @"\\", @"\\"),
            //    Dl,
            //    entry,
            //    user);
            #endregion

            var connStr = Exports.GetConnectionString(_hostingEnvironment, platform);

            var connection = new SqlConnection(connStr);

            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

            Task export = Task.Factory.StartNew(() =>
            {
                Exports.Export(Dl,
                    _hostingEnvironment.WebRootPath,
                    connection, 
                    user.Email,
                    _emailSender, 
                    user.Name,
                    baseUrl,_viewEngine,_tempDataProvider,_serviceProvider);
            });

            return RedirectToAction(nameof(Submitted),new { id = Dl.Id, dl = true });
        }

        public async Task<ActionResult> ProgressBar(int id)
        {
            var stage = _context.Entries2.First(i => i.Id == id).Stage;
            var per = (int)Math.Round((double)100 * (int)stage / (Enum.GetNames(typeof(Stage)).Length - 2) );
            var ProgressBarPV = await _viewRenderService.RenderToStringAsync("Shared/_ProgressBar", per);
            var StagePV = await _viewRenderService.RenderToStringAsync("Results/_Stage", stage);

            return Json(new { ProgressBarPV, StagePV });
        }

        // GET: Entries
        public async Task<IActionResult> Submitted(int? id, bool? dl)
        {
            var user = await _userManager.GetUserAsync(User);
            var userEntries = _context
                .Entries2
                .Include(e => e.ApplicationUser)
                .Where(e => e.ApplicationUserId == user.Id);

            if(id != null)
            {
                var runningEntry = await userEntries.SingleOrDefaultAsync(s => s.Id == id);

                string strCmdLine = string.Empty;
                bool empty = RTools.IsRscriptRunning();

                //String.IsNullOrEmpty(result)
                if (runningEntry.Stage != Stage.EmailSent && empty)
                {
                    runningEntry.Stage = Stage.Error;
                    _context.Entries2.Update(runningEntry);
                    _context.SaveChanges();
                }
                ViewData["id"] = id.Value;
            }

            ViewData["email"] = user.Email;
            ViewData["User"] = user.UserName;
            ViewData["Download"] = dl;

            return View(userEntries.OrderByDescending(e => e.ProcessStart));
        }

        // GET: Entries/Details/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entries2
                .Include(e => e.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // GET: Entries/Create
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Entries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Create([Bind("Id,AgeStart,AgeEnd,Distribution,DistributionThinning,Deviation,Species,HeightFunction,Model,VolumeFormula,Way,ApplicationUserId,IP,CreationDate,Submitted")] Entry entry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", entry.ApplicationUserId);
            return View(entry);
        }

        // GET: Entries/Edit/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entries2.SingleOrDefaultAsync(m => m.Id == id);
            if (entry == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", entry.ApplicationUserId);
            return View(entry);
        }

        // POST: Entries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AgeStart,AgeEnd,Distribution,DistributionThinning,Deviation,Species,HeightFunction,Model,VolumeFormula,Way,ApplicationUserId,IP,CreationDate,Submitted")] Entry entry)
        {
            if (id != entry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntryExists(entry.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", entry.ApplicationUserId);
            return View(entry);
        }

        // GET: Entries/Delete/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entries2
                .Include(e => e.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", entry.Id.ToString());
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.Entries2
            .SingleOrDefaultAsync(m => m.Id == id);

            _context.Entries2.Remove(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Submitted));
        }

        private bool EntryExists(int id)
        {
            return _context.Entries2.Any(e => e.Id == id);
        }

    }
}
