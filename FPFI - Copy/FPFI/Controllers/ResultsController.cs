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

        public async Task<IActionResult> Index(int? id, int? v, bool? dl)
        {
            var user = await _userManager.GetUserAsync(User);
            var userEntries2 = _context
                .Entry2
                .Include(e => e.ApplicationUser)
                .Where(e => e.ApplicationUserId == user.Id);

            var userEntries3 = _context
                .Entry3
                .Include(e => e.ApplicationUser)
                .Where(e => e.ApplicationUserId == user.Id);

            var vm2 = from Entry2 e in userEntries2
            select new EntryViewModel
            {
                Id = e.Id,
                Stage = e.Stage,
                ProcessStart = e.ProcessStart,
                IP = e.IP,
                ProcessTime = e.ProcessTime,
                Version = 2
            };

            var vm3 = from Entry3 e in userEntries3
            select new EntryViewModel
            {
                Id = e.Id,
                Stage = e.Stage,
                ProcessStart = e.ProcessStart,
                IP = e.IP,
                ProcessTime = e.ProcessTime,
                Version = 3
            };

            var model = new ResultsViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                Dl = dl,
                I = id,
                V = v,
                Entries = vm2.Concat(vm3).OrderByDescending(s => s.ProcessStart),
            };

            if (id != null && v != null)
            {
                bool empty = RTools.IsRscriptRunning();

                if (empty)
                {
                    if (v == 2)
                    {
                        var runningEntry2 = await userEntries2.SingleAsync(s => s.Id == id);

                        if (runningEntry2.Stage != Stage.EmailSent)
                        {
                            runningEntry2.Stage = Stage.Error;
                            _context.Entry2.Update(runningEntry2);
                            _context.SaveChanges();
                        }
                        model.Running2 = runningEntry2;
                        model.ATD = runningEntry2.ProcessStart;
                        model.ProgressPercentage = Math.Round((double)100 * (int)runningEntry2.Stage / ( Enum.GetNames(typeof(Stage)).Length - 1 ) );
                    } else if (v == 3)
                    {
                        var runningEntry3 = await userEntries3.SingleAsync(s => s.Id == id);

                        if (runningEntry3.Stage != Stage.EmailSent)
                        {
                            runningEntry3.Stage = Stage.Error;
                            _context.Entry3.Update(runningEntry3);
                            _context.SaveChanges();
                        }
                        model.Running3 = runningEntry3;
                        model.ATD = runningEntry3.ProcessStart;
                        model.ProgressPercentage = Math.Round((double)100 * (int)runningEntry3.Stage / (Enum.GetNames(typeof(Stage)).Length - 1) );
                    }
                    model.ETA = model.ATD.Value
                        .AddMinutes((DateTime.Now - model.ATD.Value).TotalMinutes / (model.ProgressPercentage / 100));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> SvgGraphs(int? id, int? v, int? tk)
        {
            if (id == null || v == null)
            {
                return NotFound();
            }

            var entrySims = Enumerable.Empty<Simulation>().AsQueryable();

            if(v == 2)
            {
                var sims = from Simulation2 s in _context.Simulation2.Where(s => s.Entry2Id == id)
                           select new Simulation
                           {
                               Id = s.Id,
                               Id_ = s.Id_,
                               Macrostand = s.Macrostand,
                               Age = s.Age,
                               N = s.N,
                               BA = s.BA,
                               Dg = s.Dg,
                               Hd = s.Hd,
                               Vt = s.Vt,
                               Sd = s.Sd,
                               Thin_trees = s.Thin_trees,
                               Thinaction = s.Thinaction,
                               ThinTypes = s.ThinTypes,
                               ThinCoefs = s.ThinCoefs,
                               Distr = s.Distr,
                               Idg = s.Idg,
                               CAI_Dg = s.CAI_Dg,
                               CAI_Vt = s.CAI_Vt,
                               MAI_Dg = s.MAI_Dg,
                               MAI_Vt = s.MAI_Vt
                           };

                entrySims = sims;
            }else if(v == 3)
            {
                var sims = from Simulation3 s in _context.Simulation3.Where(s => s.Entry3Id == id)
                           select new Simulation
                           {
                               Id = s.Id,
                               Id_ = s.Id_,
                               Macrostand = s.Macrostand,
                               Age = s.Age,
                               N = s.N,
                               BA = s.BA,
                               Dg = s.Dg,
                               Hd = s.Hd,
                               Vt = s.Vt,
                               Sd = s.Sd,
                               Thin_trees = s.Thin_trees,
                               Thinaction = s.Thinaction,
                               ThinTypes = s.ThinTypes,
                               ThinCoefs = s.ThinCoefs,
                               Distr = s.Distr,
                               Idg = s.Idg,
                               CAI_Dg = s.CAI_Dg,
                               CAI_Vt = s.CAI_Vt,
                               MAI_Dg = s.MAI_Dg,
                               MAI_Vt = s.MAI_Vt
                           };

                entrySims = sims;
            }

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

        public IActionResult CanvasGraphs(int? id, int? v)
        {
            if (id == null || v == null)
            {
                return NotFound();
            }

            var entrySims = Enumerable.Empty<Simulation>().AsQueryable();

            if (v == 2)
            {
                var sims = from Simulation2 s in _context.Simulation2.Where(s => s.Entry2Id == id)
                           select new Simulation
                           {
                               Id = s.Id,
                               Id_ = s.Id_,
                               Macrostand = s.Macrostand,
                               Age = s.Age,
                               N = s.N,
                               BA = s.BA,
                               Dg = s.Dg,
                               Hd = s.Hd,
                               Vt = s.Vt,
                               Sd = s.Sd,
                               Thin_trees = s.Thin_trees,
                               Thinaction = s.Thinaction,
                               ThinTypes = s.ThinTypes,
                               ThinCoefs = s.ThinCoefs,
                               Distr = s.Distr,
                               Idg = s.Idg,
                               CAI_Dg = s.CAI_Dg,
                               CAI_Vt = s.CAI_Vt,
                               MAI_Dg = s.MAI_Dg,
                               MAI_Vt = s.MAI_Vt
                           };

                entrySims = sims;
            }
            else if (v == 3)
            {
                var sims = from Simulation3 s in _context.Simulation3.Where(s => s.Entry3Id == id)
                           select new Simulation
                           {
                               Id = s.Id,
                               Id_ = s.Id_,
                               Macrostand = s.Macrostand,
                               Age = s.Age,
                               N = s.N,
                               BA = s.BA,
                               Dg = s.Dg,
                               Hd = s.Hd,
                               Vt = s.Vt,
                               Sd = s.Sd,
                               Thin_trees = s.Thin_trees,
                               Thinaction = s.Thinaction,
                               ThinTypes = s.ThinTypes,
                               ThinCoefs = s.ThinCoefs,
                               Distr = s.Distr,
                               Idg = s.Idg,
                               CAI_Dg = s.CAI_Dg,
                               CAI_Vt = s.CAI_Vt,
                               MAI_Dg = s.MAI_Dg,
                               MAI_Vt = s.MAI_Vt
                           };

                entrySims = sims;
            }

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
                var vars = new Dictionary<string, List<double>>() { { "Age", new List<double> { } } };

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
        public IActionResult Output(int id, int ver)
        {
            if (ver == 2)
            {
                return PartialView("_Output", new OutputViewModel
                {
                    Id = id,
                    Version = ver
                });
            }
            else if (ver == 3)
            {
                return PartialView("_Output", new OutputViewModel
                {
                    Id = id,
                    Version = ver
                });
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Settings(int id, int ver)
        {
            if (ver == 2)
            {
                var entry = _context
                    .Entry2
                    .First(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
                return PartialView("_Settings2", entry);
            }
            else if (ver == 3)
            {
                var entry = _context
                    .Entry3
                    .Include(e => e.Tree)
                    .ThenInclude(t => t.Species)
                    .Include(e => e.Tree)
                    .ThenInclude(t => t.Region)
                    .First(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));
                return PartialView("_Settings3", entry);
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Download(int id, int ver)
        {
            var model = new DownloadViewModel
            {
                Id = id,
                Xlsx = false,
                Xml = false,
                Csv = false,
                Version = ver
            };
            return PartialView("_Download", model);
        }

        [HttpPost]
        public async Task<IActionResult> Download([Bind("Id,Xml,Csv,Xlsx,Password,Version")] DownloadViewModel Dl)
        {
            var userId = _userManager.GetUserId(User);

            var connStr = Exports.GetConnectionString(_hostingEnvironment, platform);

            var connection = new SqlConnection(connStr);

            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

            var user = await _context
                .ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == userId);

            Task export = Task.Factory.StartNew(() =>
            {
                Exports.Export(Dl,
                    _hostingEnvironment.WebRootPath,
                    connection,
                    user.Email,
                    _emailSender,
                    user.Name,
                    baseUrl,
                    _viewEngine,
                    _tempDataProvider,
                    _serviceProvider);
            });
            #region Export with R
            //var entry = await _context
            //    .Entry2
            //    .FirstAsync(e => e.Id == Dl.Id && e.ApplicationUserId == userId);

            //RTools.GenerateFiles(
            //    RTools.GetRformattedConnectionString(_hostingEnvironment, platform),
            //    Regex.Replace(_hostingEnvironment.WebRootPath.ToString(), @"\\", @"\\"),
            //    Dl,
            //    entry,
            //    user);
            #endregion

            return RedirectToAction(nameof(Index),new { id = Dl.Id, v = Dl.Version, dl = true });
        }

        public async Task<ActionResult> ProgressBar(int id, int v)
        {
            var stage = v == 2 ? _context.Entry2.First(i => i.Id == id).Stage : _context.Entry3.First(i => i.Id == id).Stage;
            var per = (int)Math.Round((double)100 * (int)stage / (Enum.GetNames(typeof(Stage)).Length - 2) );
            var ProgressBarPV = await _viewRenderService.RenderToStringAsync("Shared/_ProgressBar", per);
            var StagePV = await _viewRenderService.RenderToStringAsync("Results/_Stage", stage);

            return Json(new { ProgressBarPV, StagePV });
        }

        // GET: Entries/Details/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entry2
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

            var entry = await _context.Entry2.SingleOrDefaultAsync(m => m.Id == id);
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
        public IActionResult Delete(int id, int ver)
        {
            return PartialView("_Delete", new DeleteViewModel{ Id = id, Ver = ver });
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> DeleteConfirmed(int id, int ver)
        {
            var userId = _userManager.GetUserId(User);

            if(ver == 2)
            {
                var entry = await _context.Entry2
                .SingleAsync(m => m.Id == id && m.ApplicationUserId == userId);

                _context.Entry2.Remove(entry);
            }else if(ver == 3)
            {
                var entry = await _context.Entry3
                .SingleAsync(m => m.Id == id && m.ApplicationUserId == userId);

                _context.Entry3.Remove(entry);
            }
            try
            {
                System.IO.File.Delete($"~/Logs/_{ver}{id}.cshtml");
            }
            catch
            {

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntryExists(int id)
        {
            return _context.Entry2.Any(e => e.Id == id);
        }

    }
}
