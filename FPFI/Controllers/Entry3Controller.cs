using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPFI.Data;
using FPFI.Models;
using FPFI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using OfficeOpenXml;
using FPFI.Extensions;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FPFI.Hubs;

namespace FPFI.Controllers
{
    [Authorize(Policy = "Apps")]
    public class Entry3Controller : Controller
    {
        private readonly IHubContext<EntryHub> _hubContext;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Entry3Controller(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IServiceProvider serviceProvider,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IHubContext<EntryHub> hubContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _serviceProvider = serviceProvider;
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task<IActionResult> Step1(int? id)
        {
            ViewData["Dist"] = new SelectList(
                from Dist e in Enum.GetValues(typeof(Dist))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            var selectedId = _context
                .Tree.SingleOrDefault(t => t.Default == true).Id;

            ViewData["TreeId"] = new SelectList(
                from Tree t in _context.Tree.Include(t => t.Species).Include(t => t.Region)
                select new
                {
                    TreeId = t.Id,
                    Name = t.Species.Name + " - " + t.Region.Name
                },
                "TreeId", "Name", selectedId);

            ViewData["Model"] = new SelectList(
                from Model e in Enum.GetValues(typeof(Model))
                select new { Id = e, Name = string.Join(" - ", e.ToString(), e.GetDisplayName()) },
                "Id", "Name");

            ViewData["Vform"] = new SelectList(
                from VolF e in Enum.GetValues(typeof(VolF))
                select new { Id = e, Name = string.Join(" - ", e.ToString(), e.GetDisplayName()) },
                "Id", "Name");

            ViewData["Way"] = new SelectList(
                from Way e in Enum.GetValues(typeof(Way))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            if (id == null)
            {
                var defaultEntry = new Entry3
                {
                    //Default Values
                    AgeStart = 15,
                    AgeEnd = 30,
                    Stump = 0.15,
                    MgDisc = 0,
                    LengthDisc = 0,
                    Include_Thinning = true,
                    ByClass = true
                };
                return View(defaultEntry);
            };
            var model = await _context.Entry3
                .SingleOrDefaultAsync(e => e.Id == id);
            if (model == null) return NotFound();
            if (model.ApplicationUserId ==
                _userManager.GetUserId(User)) return View(model);
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step1(
    [Bind(
            "Id," +
            "AgeStart," +
            "AgeEnd," +
            "Distribution," +
            "DistributionThinning," +
            "Description," +
            "TreeId," +
            "Model," +
            "VolumeFormula," +
            //"ByClass," +
            "Way,"+
            "Stump,"+
            "MgDisc,"+
            "LengthDisc")] Entry3 entry)
        {
            if (ModelState.IsValid)
            {
                entry.Include_Thinning = true;
                entry.ByClass = true;
                if (entry.Id > 0)
                {
                    var old = await _context.Entry3
                        .SingleAsync(e => e.Id == entry.Id && e.ApplicationUserId == _userManager.GetUserId(User));
                    if (old == null) return NotFound();
                    old.AgeStart = entry.AgeStart;
                    old.AgeEnd = entry.AgeEnd;
                    old.Distribution = entry.Distribution;
                    old.Description = entry.Description;
                    old.DistributionThinning = entry.DistributionThinning;
                    old.Include_Thinning = entry.Include_Thinning;
                    old.TreeId = entry.TreeId;
                    old.Model = entry.Model;
                    old.VolumeFormula = entry.VolumeFormula;
                    old.ByClass = entry.ByClass;
                    old.Way = entry.Way;
                    old.Stump = entry.Stump;
                    old.MgDisc = entry.MgDisc;
                    old.LengthDisc = entry.LengthDisc;
                    _context.Update(old);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Step2), new { id = old.Id });
                }
                entry.ApplicationUserId = _userManager.GetUserId(User);

                var failed = _context.Entry3
                        .Include(e => e.Inputs)
                        .Include(e => e.Parameter)
                        .Include(e => e.Products)
                        .Where(s => s.ApplicationUserId ==
                        entry.ApplicationUserId && (int)s.Stage == 0);

                if (failed.Any())
                {
                    foreach (var ent in failed)
                    {
                        if (ent.Inputs.Any())
                        {
                            foreach (var inp in ent.Inputs)
                            {
                                _context.Input3.Remove(inp);
                            }
                        }
                        if (ent.Products.Any())
                        {
                            foreach (var prod in ent.Products)
                            {
                                _context.Product3.Remove(prod);
                            }
                        }
                        if (ent.Parameter != null) _context.Parameter3.Remove(ent.Parameter);
                        _context.Entry3.Remove(ent);
                    }
                }
                _context.Entry3.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step2), new { id = entry.Id });
            }
            ViewData["Dist"] = new SelectList(
                from Dist e in Enum.GetValues(typeof(Dist))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Species"] = new SelectList(
                from Tree t in _context.Tree.Include(t => t.Species).Include(t => t.Region)
                select new { t.Id, Name = t.Species.Name + " " + t.Region.Name },
                "Id", "Name");

            ViewData["Model"] = new SelectList(
                from Model e in Enum.GetValues(typeof(Model))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Vform"] = new SelectList(
                from VolF e in Enum.GetValues(typeof(VolF))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Way"] = new SelectList(
                from Way e in Enum.GetValues(typeof(Way))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            return View(entry);
        }

        public async Task<IActionResult> Step_2(int? id, bool? err)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry3.SingleAsync(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));

            ViewData["Error"] = err;

            if (entry != null) return View(entry);

            return NotFound();
        }

        public
    (
        Dictionary<string, Dictionary<string, object>>, string
    )
    Analyze<T>
    (
        BindingFlags bindingFlags
    )
        {
            var data = typeof(T).GetFields(bindingFlags).Concat(typeof(T).BaseType.GetFields(bindingFlags)).ToArray();

            var ddata = new Dictionary<string, Dictionary<string, object>> { };

            foreach (var dt in data)
            {
                var name = Regex.Replace(dt.Name, "<([a-zA-Z0-9_]+)>.*", "$1");
                var sn = typeof(T)
                    .GetMember(name)
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DisplayAttribute>(false)
                    ?.ShortName ?? null;
                if (sn == null) continue;
                var tmp = new Dictionary<string, object>
                {
                    { "type", dt.FieldType },
                };
                tmp.Add("var", sn);
                ddata.Add(name, tmp);
            }

            return (ddata, null);
        }

        public async Task<ICollection<T>> Read<T>
            (BindingFlags bindingFlags, ExcelWorksheet worksheet, string userId) where T : Indexed
        {
            int rowCount = worksheet.Dimension.Rows;
            int ColCount = worksheet.Dimension.Columns;

            (Dictionary<string, Dictionary<string, object>> tdata, string error)
                = Analyze<T>(bindingFlags);

            var items = new List<T> { };

            if (error != null)
            {
                return items;
            }

            double cnt = (double)100 / rowCount;
            double pgr = 0;

            for (int row = 2; row <= rowCount; row++)
            {
                var range = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column];
                if (!range.Any(c => !string.IsNullOrEmpty(c.Text)))
                {
                    break;
                }

                var item = Activator.CreateInstance<T>();
                foreach (var d in tdata)
                {
                    MethodInfo method = typeof(Entry3Controller).GetMethod("GetFromExcel")
                        .MakeGenericMethod(new Type[] { (Type)d.Value["type"] });
                    object value = null;
                    value = method.Invoke(value, new object[] { worksheet, (string)d.Value["var"], row, null });
                    System.Diagnostics.Debug.WriteLine($"row:{row} key:{d.Key} value:{value}");
                    item[d.Key] = value;
                }
                System.Diagnostics.Debug.WriteLine($"row:{row} done");
                pgr += cnt;
                await _hubContext.Clients.Client(userId).SendAsync("Update", "progress", pgr);
                items.Add(item);
            }
            System.Diagnostics.Debug.WriteLine($"document done");
            await _hubContext.Clients.Client(userId).SendAsync("Update", "progress", 100);

            return items;
        }

        public async Task<IActionResult> Step2(int? id, bool? err)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry3.SingleAsync(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));

            ViewData["Error"] = err;

            if (entry != null) return View(entry);

            return NotFound();
        }

        [HttpPost]
        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step2(string qquuid, string qqfilename, int id, string userId, int qqtotalfilesize, IFormFile qqfile)
        {
            string error = null;

            if (qqfile.Length > 0)
            {
                try
                {
                    var sheets = new Dictionary<Type, string>
                    {
                        { typeof(Input3), "initial" },
                        { typeof(Product3), "products" },
                        { typeof(Parameter3), "params" }
                    };

                var entry = await _context.Entry3
                .SingleAsync(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));

                using (var stream = qqfile.OpenReadStream())
                {
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

                        foreach (var sheet in sheets)
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[sheet.Value];
                            if (sheet.Key == typeof(Input3))
                            {
                                var name = typeof(Input3)
                                    .GetMember("Id").FirstOrDefault()
                                    ?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? typeof(Input3).Name;
                                await _hubContext.Clients.Client(userId).SendAsync("Update", "log", $"Reading sheet {name} from file {qqfilename}");
                                var value = await Read<Input3>(bindingFlags, worksheet, userId);
                                entry.Inputs = value;
                            }
                            else if (sheet.Key == typeof(Product3))
                            {
                                var name = typeof(Product3)
                                    .GetMember("Id").FirstOrDefault()
                                    ?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? typeof(Product3).Name;
                                await _hubContext.Clients.Client(userId).SendAsync("Update", "log", $"Reading sheet {name} from file {qqfilename}");                                //await hub.Send("log", $"Reading {name} from {qqfilename}");

                                var value = await Read<Product3>(bindingFlags, worksheet,userId);
                                entry.Products = value;
                            }
                            else if (sheet.Key == typeof(Parameter3))
                            {
                                var name = typeof(Parameter3)
                                    .GetMember("Id").FirstOrDefault()
                                    ?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? typeof(Parameter3).Name;
                                await _hubContext.Clients.Client(userId).SendAsync("Update", "log", $"Reading sheet {name} from file {qqfilename}");

                                var value = await Read<Parameter3>(bindingFlags, worksheet,userId);
                                entry.Parameter = value.First();
                            }
                        }
                    }
                }

                entry.IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                _context.Entry3.Update(entry);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.Client(userId).SendAsync("Update", "log", $"File {qqfilename} uploaded to FPFI database");

                }
                catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }

            return Ok(new { success = string.IsNullOrWhiteSpace(error), error });
        }

        [HttpPost]
        public async Task<IActionResult> Step_2([Bind("Id,Excel,AgeStart,AgeEnd")] Entry entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var sheets = new Dictionary<Type, string>
                {
                    { typeof(Input3), "initial" },
                    { typeof(Product3), "products" },
                    { typeof(Parameter3), "params" }
                };

                var entry = await _context.Entry3
                    .SingleAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Excel = entrada.Excel;

                if (entry.Excel == null) return View(entry);

                var contentDisposition = ContentDispositionHeaderValue.Parse(entry.Excel.ContentDisposition);

                var filename = contentDisposition.FileName.Trim('"');

                var stream = entry.Excel.OpenReadStream();

                //try
                //{
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

                    foreach (var sheet in sheets)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[sheet.Value];
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;

                        if (sheet.Key == typeof(Input3))
                        {
                            var value = await Read<Input3>(bindingFlags, worksheet,"");
                            entry.Inputs = value;
                        }
                        else if (sheet.Key == typeof(Product3))
                        {
                            var value = await Read<Product3>(bindingFlags, worksheet,"");
                            entry.Products = value;
                        }
                        else if (sheet.Key == typeof(Parameter3))
                        {
                            var value = await Read<Parameter3>(bindingFlags, worksheet,"");
                            entry.Parameter = value.First();
                        }
                    }
                }
                //}
                //catch
                //{
                //    return RedirectToAction(nameof(Step2), new { id = entry.Id, err = true });
                //}
                entry.IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                _context.Entry3.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step3), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public static object GetFromExcel<T>(ExcelWorksheet worksheet, string var, int? row, int? col)
        {
            object value = null;
            var type = typeof(T);
            if (string.IsNullOrWhiteSpace(var)) return null;

            object val;
            if (row.HasValue)
            {
                try
                {
                    col = worksheet.GetColumnByName(var);
                    val = worksheet.Cells[row.Value, col.Value].Value;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                val = worksheet.Cells[var].Value;
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                if (val != null)
                {
                    var num = Regex.Replace(Regex.Replace(val.ToString(), @"(-.*|\.+)$", ""), @"[^0-9]", "");
                    if (!string.IsNullOrWhiteSpace(num))
                    {
                        value = Convert.ToInt32(num);
                    }
                }
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                if (val != null)
                {
                    var num = Regex.Replace(Regex.Replace(val.ToString(), @"(?<=^.+)(-.*|\.+)$", ""), @"[^0-9\.,-]", "");
                    if (!string.IsNullOrWhiteSpace(num))
                    {
                        var culture = CultureInfo.CreateSpecificCulture(num.Contains(',') ? "es-CL" : "en-GB");
                        value = Convert.ToDouble(num, culture);
                    }
                }
            }
            else if (type == typeof(string))
            {
                value = val != null ?
                                val.ToString() : "";
            }
            else if (type == typeof(bool))
            {
                value = val != null &&
                    val.ToString() == "Si" || val.ToString() == "Yes";
            }
            else if (type == typeof(DateTime))
            {
                var sDate = val.ToString();
                var formats = new string[] { "yyyyMMdd", "yyyy-MM-dd", "dd-MM-yyyy hh:mm", "dd-MM-yyyy", "dd-MM-yyyy hh:mm'&nbsp;'" };
                DateTime.TryParseExact(sDate, formats, new CultureInfo("es-CL"), DateTimeStyles.None, out DateTime date);
                //DateTime.TryParse(sDate, out DateTime date);
                value = date;
            }
            else if (type == typeof(Enum))
            {
                value = Enum.Parse(DataTableExtensions.GetEnumType("FPFI.Models." + typeof(T).ToString()), val.ToString());
            }
            else if (type == typeof(LogType))
            {
                Enum.TryParse(val.ToString(), out LogType log);
                value = log;
            }

            return (T)value;
        }

        public async Task<IActionResult> Step3(int? id, int? pg, int? rpp, string srt, bool? asc)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entry = await _context.Entry3
            .Include(s => s.Inputs)
            .SingleAsync(s => s.Id == id && s.ApplicationUserId == _userManager.GetUserId(User));

            if (entry == null) return NotFound();

            ViewData["Years"] = new SelectList(
                from num in Enumerable.Range(1970, 2100)
                select new { Id = num, Name = num },
                "Id", "Name");

            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (string.IsNullOrEmpty(srt)) srt = "Id_";
            if (asc == null) asc = true;

            try
            {
                PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Input3)).Find(srt, false);

                if (asc.Value)
                {
                    entry.Inputs = entry.Inputs.OrderBy(x => prop.GetValue(x)).ToList();
                }
                else
                {
                    entry.Inputs = entry.Inputs.OrderByDescending(x => prop.GetValue(x)).ToList();
                }
            }
            catch
            {
                return NotFound();
            }

            if (rpp.HasValue) ViewData["last"] = (Math.Ceiling((double)(entry.Inputs.Count() / rpp.Value))).ToString();

            entry.Inputs = entry.Inputs.Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value).ToList();

            ViewData["srt"] = srt;
            ViewData["asc"] = asc.Value.ToString();
            ViewData["pg"] = Convert.ToString(pg.Value);
            ViewData["rpp"] = rpp.Value.ToString();

            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Step3([Bind("Id,Inputs,AgeStart,AgeEnd")] Entry3 entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context.Entry3
                    .SingleAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Inputs = entrada.Inputs;

                _context.Entry3.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step4), new { id = entry.Id, v = 2 });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step4(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry3
            .Include(s => s.Products)
            .SingleAsync(s => s.Id == id && s.ApplicationUserId == _userManager.GetUserId(User));

            if (entry == null) return NotFound();

            entry.Products = entry.Products.OrderBy(p => p.Diameter).ToList();

            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Step4([Bind("Id,Products,AgeStart,AgeEnd")] Entry3 entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context
                    .Entry3
                    .SingleAsync(e => e.Id == entrada.Id
                    && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Products = entrada.Products;

                _context.Entry3.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step5), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step5(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry3
            .Include(s => s.Parameter)
            .SingleAsync(s => s.Id == id && s.ApplicationUserId == _userManager.GetUserId(User));

            if (entry == null) return NotFound();

            return View(entry);
        }

        [HttpPost]
        [RequestFormSizeLimit(valueCountLimit: 200000, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
        public async Task<IActionResult> Step5([Bind("Id,Parameter,AgeStart,AgeEnd")] Entry3 entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context.Entry3
                    .Include(e => e.Parameter)
                    .Include(e => e.Products)
                    .Include(e => e.ApplicationUser)
                    .Include(e => e.Tree)
                    .ThenInclude(t => t.Region)
                    .Include(e => e.Tree)
                    .ThenInclude(t => t.Species)
                    .SingleAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Parameter = entrada.Parameter;
                entry.ProcessStart = DateTime.Now;
                entry.Stage = Stage.Submitted;
                _context.Entry3.Update(entry);
                _context.SaveChanges();

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                string batch = RTools.MakeScript(_hostingEnvironment, entry, baseUrl);

                var root = _hostingEnvironment.ContentRootPath;

                Task import = Task.Factory.StartNew(() =>
                {
                    using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>()
                            .CreateScope())
                    {
                        var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                        var hub = serviceScope.ServiceProvider.GetService<IHubContext<EntryHub>>();
                        REngineRunner.RunFromCmd(batch, root, entry.Id, 3, context, hub);
                    }
                });

                return RedirectToAction("Index", "Results", new { id = entry.Id, v = 3 });
            }
            return RedirectToAction(nameof(Step1));
        }

        // GET: Entry3
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Entry3.Include(e => e.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Entry3/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry3 = await _context.Entry3
                .Include(e => e.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entry3 == null)
            {
                return NotFound();
            }

            return View(entry3);
        }

        // GET: Entry3/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Entry3/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Include_Thinning,ByClass,Stump,MgDisc,LengthDisc,Id,ApplicationUserId,AgeStart,AgeEnd," +
            "Distribution,DistributionThinning,Model,VolumeFormula,Way,IP,ProcessStart,ProcessTime,Stage")] Entry3 entry3)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entry3);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", entry3.ApplicationUserId);
            return View(entry3);
        }

        // GET: Entry3/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry3 = await _context.Entry3.SingleOrDefaultAsync(m => m.Id == id);
            if (entry3 == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", entry3.ApplicationUserId);
            return View(entry3);
        }

        // POST: Entry3/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [
            Bind("Include_Thinning,ByClass,Stump,MgDisc,LengthDisc,Id,ApplicationUserId,AgeStart,AgeEnd,Distribution," +
            "DistributionThinning,Model,VolumeFormula,Way,IP,ProcessStart,ProcessTime,Stage")] Entry3 entry3)
        {
            if (id != entry3.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Entry3Exists(entry3.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", entry3.ApplicationUserId);
            return View(entry3);
        }

        // GET: Entry3/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry3 = await _context.Entry3
                .Include(e => e.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entry3 == null)
            {
                return NotFound();
            }

            return View(entry3);
        }

        // POST: Entry3/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry3 = await _context.Entry3.SingleOrDefaultAsync(m => m.Id == id);
            _context.Entry3.Remove(entry3);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Entry3Exists(int id)
        {
            return _context.Entry3.Any(e => e.Id == id);
        }
    }
}
