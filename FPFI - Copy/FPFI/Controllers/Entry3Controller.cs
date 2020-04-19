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
using System.Text.Encodings.Web;

namespace FPFI.Controllers
{
    public class Entry3Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IHostingEnvironment _hostingEnvironment;

        public Entry3Controller(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
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
                .Tree.Include(t => t.Species).Include(t => t.Region)
                .Single(t => t.Region.Command == "uruguay_guanare"
                && t.Species.Command == "eucalyptus_grandis").Id;

            ViewData["TreeId"] = new SelectList(
                from Tree t in _context.Tree.Include(t => t.Species).Include(t => t.Region)
                select new { TreeId = t.Id,
                    Name = t.Species.Name + " - " + t.Region.Name },
                "TreeId", "Name", selectedId);

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
            //"Include_Thinning," +
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
                select new { Id = t.Id, Name = t.Species.Name + " " + t.Region.Name },
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

        public async Task<IActionResult> Step2(int? id, bool? err)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry3.SingleAsync(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));

            ViewData["Error"] = err;

            if (entry != null) return View(entry);

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Step2([Bind("Id,Excel,AgeStart,AgeEnd")] Entry entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context.Entry3
                    .SingleAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Excel = entrada.Excel;

                if (entry.Excel == null) return View(entry);

                var contentDisposition = ContentDispositionHeaderValue.Parse(entry.Excel.ContentDisposition);

                var filename = contentDisposition.FileName.Trim('"');

                var stream = entry.Excel.OpenReadStream();

                var inputs = new List<Input3> { };

                var prods = new List<Product3> { };

                try
                {
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["initial"];
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;

                        var colNames = new string[]
                        {
                        "id",
                        "macrostand",
                        "pyear",
                        "Age",
                        "N",
                        "BA",
                        "Dg",
                        "d100",
                        "Hd",
                        "Vt",
                        "years",
                        "thinningAge",
                        "n.afterThin",
                        "thinTypes",
                        "thin_coeff",
                        "hp",
                        "hm",
                        "DBH_sd",
                        "DBH_max",
                        "random_SI",
                        "random_BA"
                        };

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var range = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column];
                            if (!range.Any(c => !string.IsNullOrEmpty(c.Text)))
                            {
                                break;
                            }
                            var item = new Input3
                            {
                                Id_ = Convert.ToInt32(worksheet.Cells[row, worksheet.GetColumnByName(colNames[0])].Value),
                                Macrostand = worksheet.Cells[row, worksheet.GetColumnByName(colNames[1])].Value.ToString(),

                                Pyear = Convert.ToInt16(worksheet.Cells[row, worksheet.GetColumnByName(colNames[2])].Value),

                                Age = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[3])].Value.ToString()),
                                N = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[4])].Value.ToString()),
                                BA = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[5])].Value.ToString()),
                                Dg = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[6])].Value.ToString()),

                                DBH_sd = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[17])].Value.ToString()),
                                DBH_max = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[18])].Value.ToString()),

                                D100 = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[7])].Value.ToString()),
                                Hd = Double.Parse(worksheet.Cells[row, worksheet.GetColumnByName(colNames[8])].Value.ToString()),
                                Vt = Convert.ToInt32(worksheet.Cells[row, worksheet.GetColumnByName(colNames[9])].Value),
                                Years = Convert.ToInt32(worksheet.Cells[row, worksheet.GetColumnByName(colNames[10])].Value),

                                Hp = Convert.ToInt32(worksheet.Cells[row, worksheet.GetColumnByName(colNames[15])].Value),
                                Hm = Convert.ToInt32(worksheet.Cells[row, worksheet.GetColumnByName(colNames[16])].Value),
                            };
                            var thinningAge = worksheet.Cells[row, worksheet.GetColumnByName(colNames[11])].Value;
                            item.ThinningAges = (thinningAge != null) ? thinningAge.ToString() : "";
                            var nAfterThin = worksheet.Cells[row, worksheet.GetColumnByName(colNames[12])].Value;
                            item.NAfterThins = (nAfterThin != null) ? nAfterThin.ToString() : "";
                            var thinType = worksheet.Cells[row, worksheet.GetColumnByName(colNames[13])].Value;
                            item.ThinTypes = (thinType != null) ? thinType.ToString() : "";
                            var thinCoef = worksheet.Cells[row, worksheet.GetColumnByName(colNames[14])].Value;
                            item.ThinCoefs = (thinCoef != null) ? thinCoef.ToString() : "";
                            var randomSI = worksheet.Cells[row, worksheet.GetColumnByName(colNames[19])].Value;
                            item.Random_SI = (randomSI != null) ? randomSI.ToString() : "";
                            var randomBA = worksheet.Cells[row, worksheet.GetColumnByName(colNames[20])].Value;
                            item.Random_BA = (randomBA != null) ? randomBA.ToString() : "";
                            inputs.Add(item);
                        }

                        colNames = new string[]
                        {
                        "name","diameter","length","value","log_type","class"
                        };

                        worksheet = package.Workbook.Worksheets["products"];
                        rowCount = worksheet.Dimension.Rows;
                        ColCount = worksheet.Dimension.Columns;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var range = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column];
                            if (!range.Any(c => !string.IsNullOrEmpty(c.Text)))
                            {
                                break;
                            }
                            var item = new Product3
                            {
                                Diameter = Convert.ToInt16(worksheet.Cells[row, worksheet.GetColumnByName(colNames[1])].Value),
                                Length = Convert.ToInt16(worksheet.Cells[row, worksheet.GetColumnByName(colNames[2])].Value),
                                Value = Convert.ToInt16(worksheet.Cells[row, worksheet.GetColumnByName(colNames[3])].Value),
                                LogType = (LogType)Enum.Parse(typeof(LogType), worksheet.Cells[row, worksheet.GetColumnByName(colNames[4])].Value.ToString()),
                                Class = (Class)Enum.Parse(typeof(Class), worksheet.Cells[row, worksheet.GetColumnByName(colNames[5])].Value.ToString())
                            };
                            var name = worksheet.Cells[row, worksheet.GetColumnByName(colNames[0])].Value;
                            item.X_1 = (name != null) ? name.ToString() : "";

                            prods.Add(item);
                        }

                        worksheet = package.Workbook.Worksheets["params"];
                        rowCount = worksheet.Dimension.Rows;
                        ColCount = worksheet.Dimension.Columns;

                        var par = new Parameter3
                        {
                            Beta1 = Double.Parse(worksheet.Cells[2, 1].Value.ToString()),
                            Beta2 = Double.Parse(worksheet.Cells[2, 2].Value.ToString()),
                            Beta3 = Double.Parse(worksheet.Cells[2, 3].Value.ToString()),
                            Beta4 = Double.Parse(worksheet.Cells[2, 4].Value.ToString()),
                            Alpha1 = Double.Parse(worksheet.Cells[2, 5].Value.ToString()),
                            Alpha2 = Double.Parse(worksheet.Cells[2, 6].Value.ToString())
                        };
                        entry.Parameter = par;
                    }
                }
                catch
                {
                    return RedirectToAction(nameof(Step2), new { id = entry.Id, err = true });
                }
                entry.Inputs = inputs;
                entry.Products = prods;
                entry.IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                _context.Entry3.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step3), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step3(int? id, int? pg, int? rpp, string srt, bool? asc)
        {
            if (id == null) return NotFound();

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
            if (String.IsNullOrEmpty(srt)) srt = "Id_";
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

                var platform = RTools.GetPlatform();

                var connStr = RTools.GetRformattedConnectionString(_hostingEnvironment, platform);

                var tmp = platform == "Unix" ? ", tmpdir = '/tmp'" : string.Empty;

                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

                var prodsList = new List<string> { };

                foreach(var e in Enum.GetValues(typeof(LogType)))
                {
                    prodsList.Add($@"prods$log_type[prods$log_type =={(int)e}] = ""{e.ToString()}""");
                }

                var prods = String.Join(Environment.NewLine,prodsList);

                var batch =
$@"
#install and load required packages
list.of.packages <- c(""data.table"",
                    ""matrixStats"",
                    ""plyr"",
                    ""RODBC"",
                    ""stringr"",
                    ""rlist"",
                    ""jsonlite"",
                    ""sendmailR"",
                    ""httr"",
                    ""fpfi3"",
                    ""readxl"")

new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
if (length(new.packages)) install.packages(new.packages,repos=""https://cran.us.r-project.org"")
lapply(list.of.packages, require, character.only = TRUE)

#open database and read entry
conn <- odbcDriverConnect(""{connStr}"")

df <- rename(subset(data.table(sqlQuery(conn, str_c(""select * from Input3 where Entry3Id = {entry.Id} ""))), select = -c(Id,Entry3Id)),
        c(""Id_"" = ""id"",
        ""Macrostand"" = ""macrostand"",
        ""Pyear""=""pyear"",
        ""D100"" = ""d100"",
        ""Years""=""years"",
        ""ThinningAges""=""thinningAge"",
        ""NAfterThins""=""n.afterThin"",
        ""ThinTypes""=""thinTypes"",
        ""ThinCoefs""=""thin_coeff"",
        ""Hp""=""hp"",
        ""Hm""=""hm"",
        ""Random_SI""=""random_SI"",
        ""Random_BA""=""random_BA""))

df <- df[,c(""id"",
            ""macrostand"",
            ""pyear"",
            ""Age"",
            ""N"",
            ""BA"",
            ""Dg"",
            ""DBH_sd"",
            ""DBH_max"",
            ""d100"",
            ""Hd"",
            ""Vt"",
            ""years"",
            ""thinningAge"",
            ""n.afterThin"",
            ""thinTypes"",
            ""thin_coeff"",
            ""hp"",
            ""hm"",
            ""random_SI"",
            ""random_BA"")]

df[df=='']<-NA

conn <- odbcDriverConnect(""{connStr}"")

prods <- rename(subset(data.table(sqlQuery(conn, str_c(""select * from Product3 where Entry3Id = {entry.Id}""))), select = -c(Id,Entry3Id)),
        c(""X_1""=""name"",
        ""Diameter""=""diameter"",
        ""Length""=""length"",
        ""Value""=""value"",
        ""LogType""=""log_type"",
        ""Class""=""class""))

prods <- prods[,c(""diameter"",""length"",""value"",""name"",""log_type"",""class"")]

prods$class <- sub(""^"",""VP"",prods$class)

{prods}

prods$name <- as.character(prods$name)

prods <- prods[order(-rank(diameter),-rank(value))]

conn <- odbcDriverConnect(""{connStr}"")

params <- list.remove(unlist(sqlQuery(conn, str_c(""select * from Parameter3 where Entry3Id = {entry.Id}""))), c(""Id"",""Entry3Id""))

names(params) <- c(""a1"", ""a2"", ""beta1"", ""beta2"", ""beta3"", ""beta4"")

params <- params[c(""beta1"", ""beta2"", ""beta3"", ""beta4"",""a1"",""a2"")]

df$thinTypes <- as.character(df$thinTypes)

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry3 set Stage=2 where Id={entry.Id}"", sep=""""))

#run simulation
sims <- FullSimulation(in_data = list(df = df, params = params, prods = prods),
                age_range = c({entry.AgeStart},{entry.AgeEnd}),
                distribution = ""{entry.Distribution}"",
                distribution_thinning = ""{entry.DistributionThinning}"",
                include_thinning = {entry.Include_Thinning.ToString()[0]},
                species = ""{entry.Tree.Species.Command}"",
                model = ""{entry.Model}"",
                volform = ""{entry.VolumeFormula}"",
                byClass = {entry.ByClass.ToString()[0]},
                way = ""{entry.Way}"",
                region = ""{entry.Tree.Region.Command}"",
                stump = {entry.Stump},
                mg_disc = {entry.MgDisc},
                length_disc = {entry.LengthDisc})

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry3 set Stage=3 where Id={entry.Id}"", sep=""""))

#parse and save simulations
sims$simulation <- rename(sims$simulation,
        c(""id""=""Id_"",
        ""macrostand""=""Macrostand"",
        ""sd""=""Sd"",
        ""thin_trees""=""Thin_trees"",
        ""thinaction""=""Thinaction"",
        ""thin_type""=""ThinTypes"",
        ""thin_coef""=""ThinCoefs"",
        ""distr""=""Distr"",
        ""idg""=""Idg""))

sims$simulation$Entry3Id <- rep({entry.Id},nrow(sims$simulation))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from Simulation3"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
sims$simulation$Id <- start[[1]]:(start+nrow(sims$simulation)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")

ColumnsOfTable       <- sqlColumns(conn, ""Simulation3"")
varTypes             <- as.character(ColumnsOfTable$TYPE_NAME) 
names(varTypes)      <- as.character(ColumnsOfTable$COLUMN_NAME) 

setcolorder(sims$simulation, as.character(ColumnsOfTable$COLUMN_NAME))

colnames(sims$simulation) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(sims$simulation, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)
conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT Simulation3 FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"", sep=""""))

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry3 set Stage=4 where Id={entry.Id}"", sep=""""))

#parse and save tapers
sims$taper$stand_level$harvest <- rename(sims$taper$stand_level$harvest,
        c(""id""=""Id_"",
        ""macrostand""=""Macrostand"",
        ""pyear""=""Pyear"",
        ""idg""=""Idg""))

sims$taper$stand_level$thinning <- rename(sims$taper$stand_level$thinning,
        c(""id""=""Id_"",
        ""macrostand""=""Macrostand"",
        ""pyear""=""Pyear"",
        ""idg""=""Idg"",
        ""thin_year""=""Thin_year"",
        ""thin.name""=""Thin_name""))

list.of.packages <- c(""dplyr"")
new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
if (length(new.packages)) install.packages(new.packages,repos=""https://cran.us.r-project.org"")
library(""dplyr"")

vp_harvest <- sims$taper$stand_level$harvest %>% dplyr:: select(grep(""VP"", names(sims$taper$stand_level$harvest)))
vp_thinning <-sims$taper$stand_level$thinning %>% dplyr:: select(grep(""VP"", names(sims$taper$stand_level$thinning)))
taper_stand_harvest <- sims$taper$stand_level$harvest %>% dplyr:: select(setdiff(seq(1,length(sims$taper$stand_level$harvest)),grep(""VP"", names(sims$taper$stand_level$harvest))))
taper_stand_thinning <- sims$taper$stand_level$thinning %>% dplyr:: select(setdiff(seq(1,length(sims$taper$stand_level$thinning)),grep(""VP"", names(sims$taper$stand_level$thinning))))
detach(""package:dplyr"", unload=TRUE)

taper_log_thinning <- sims$taper$log_level$thinning
taper_log_harvest <- sims$taper$log_level$harvest

taper_log_thinning$Type <- rep({(int)TypeLog.Thinning},nrow(taper_log_thinning))
taper_log_harvest$Type <- rep({(int)TypeLog.Harvest},nrow(taper_log_harvest))
taper_log <- data.frame(rbind(as.matrix(taper_log_harvest),as.matrix(taper_log_thinning)))

taper_log <- rename(taper_log,
        c(""idseq""=""Idseq"",
        ""grade""=""Grade"",
        ""log_type""=""LogType"",
        ""log""=""Log"",
        ""diameter""=""Diameter"",
        ""volume""=""Volume"",
        ""product""=""Product"",
        ""value""=""Value"",
        ""class""=""Class"",
        ""id""=""Id_"",
        ""idg""=""Idg"",
        ""dbh""=""Dbh"",
        ""ht""=""Ht"",
        ""freq""=""Freq"",
        ""idgu""=""Idgu"",
        ""hp""=""Hp"",
        ""hm""=""Hm""))

taper_log$Entry3Id <- rep({entry.Id},nrow(taper_log))

taper_stand_thinning$Entry3Id <- rep({entry.Id},nrow(taper_stand_thinning))
taper_stand_harvest$Entry3Id <- rep({entry.Id},nrow(taper_stand_harvest))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from TaperLog"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
taper_log$Id <- start[[1]]:(start+nrow(taper_log)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from TaperStandHarvest"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
taper_stand_harvest$Id <- start[[1]]:(start+nrow(taper_stand_harvest)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from TaperStandThinning"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
taper_stand_thinning$Id <- start[[1]]:(start+nrow(taper_stand_thinning)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")
ColumnsOfTable <- sqlColumns(conn, ""TaperLog"")
varTypes <- as.character(ColumnsOfTable$TYPE_NAME)
names(varTypes) <- as.character(ColumnsOfTable$COLUMN_NAME)
setcolorder(taper_log, as.character(ColumnsOfTable$COLUMN_NAME))
colnames(taper_log) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(taper_log, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)

conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT TaperLog FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"",sep=""""))


conn <- odbcDriverConnect(""{connStr}"")
ColumnsOfTable <- sqlColumns(conn, ""TaperStandHarvest"")
varTypes <- as.character(ColumnsOfTable$TYPE_NAME)
names(varTypes) <- as.character(ColumnsOfTable$COLUMN_NAME)
setcolorder(taper_stand_harvest, as.character(ColumnsOfTable$COLUMN_NAME))
colnames(taper_stand_harvest) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(taper_stand_harvest, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)

conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT TaperStandHarvest FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"",sep=""""))


conn <- odbcDriverConnect(""{connStr}"")
ColumnsOfTable <- sqlColumns(conn, ""TaperStandThinning"")
varTypes <- as.character(ColumnsOfTable$TYPE_NAME)
names(varTypes) <- as.character(ColumnsOfTable$COLUMN_NAME)
setcolorder(taper_stand_thinning, as.character(ColumnsOfTable$COLUMN_NAME))
colnames(taper_stand_thinning) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(taper_stand_thinning, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)

conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT TaperStandThinning FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"",sep=""""))


#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry3 set Stage=5 where Id={entry.Id}"", sep=""""))

#parse and save diameters
conn <- odbcDriverConnect(""{connStr}"")
start < -sqlQuery(conn, ""select max(Id) from VP"")
if (is.na(start)) {{ start = 1}} else {{ start = start + 1}}

vp_harvest$Idg <- taper_stand_harvest$Idg
vp_thinning$Idg <- taper_stand_thinning$Idg
vp <- data.frame(rbind(as.matrix(vp_harvest),as.matrix(vp_thinning)))

tmp <- as.data.frame(list(start[[1]]:(start+nrow(vp)-1)[[1]],
                        vp[,c(1:length(vp) == 1)],
                        rep(names(vp)[[1]],nrow(vp)),
                        c(rep(0,nrow(vp_harvest)),rep(1,nrow(vp_thinning))),
                        vp$Idg))
rownames(tmp) <- NULL
names(tmp) <- c(""Id"",""Value"",""Class"",""Type"",""Idg"")

for (i in 2:(length(vp)-1)){{
    tmp2 <- as.data.frame(list((start+nrow(vp)*(i-1))[[1]]:(start+nrow(vp)*i-1)[[1]],
                                vp[,c(1:length(vp) == i)],
                                rep(names(vp)[[i]],nrow(vp)),
                                c(rep(0,nrow(vp_harvest)),rep(1,nrow(vp_thinning))),
                                vp$Idg))
    rownames(tmp2) <- NULL
    names(tmp2) <- c(""Id"",""Value"",""Class"",""Type"",""Idg"")
    tmp <- data.frame(rbind(as.matrix(tmp),as.matrix(tmp2)))
}}

tmp$Entry3Id <- rep({entry.Id},nrow(tmp))

tmp$Class <- gsub(""VP"","""",tmp$Class)

conn <- odbcDriverConnect(""{connStr}"")

ColumnsOfTable <- sqlColumns(conn, ""VP"")
varTypes <- as.character(ColumnsOfTable$TYPE_NAME)
names(varTypes) <- as.character(ColumnsOfTable$COLUMN_NAME)
setcolorder(tmp, as.character(ColumnsOfTable$COLUMN_NAME))
colnames(tmp) <- as.character(ColumnsOfTable$COLUMN_NAME)

#bulk insert is 10x faster than sqlsave for long entries
tmp <- sapply(tmp, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)
conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT VP FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"",sep=""""))

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry3 set Stage=6 where Id={entry.Id}"", sep=""""))

#save time and close
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry3 set ProcessTime="", proc.time()[[3]], "" where Id={entry.Id}"", sep=""""))

#send email
from <- ""<no-reply@fpfi.cl>""
to <- ""<{entry.ApplicationUser.Email}>""
subject <- ""FPFI Entry Results are Ready""
body <- paste(""
<h3>Dear {entry.ApplicationUser.Name},</h3>
<br/>
The results for your entry submition number <strong>{entry.Id}</strong> to FPFI3 algorith can be viewed on the following links:

<h4>SVG:</h4>
<a href='{HtmlEncoder.Default.Encode($"{baseUrl}/Results/SvgGraphs/" + entry.Id + "?v=3")}'>
    {HtmlEncoder.Default.Encode($"{baseUrl}/Results/SvgGraphs/" + entry.Id + "?v=3")}
</a>
<h4>CANVAS:</h4>
<a href='{HtmlEncoder.Default.Encode($"{baseUrl}/Results/CanvasGraphs/" + entry.Id + "?v=3")}'>
    {HtmlEncoder.Default.Encode($"{baseUrl}/Results/CanvasGraphs/" + entry.Id + "?v=3")}
</a>
<br/>
<br/>
Kind Regards,
<br/>
<h4>FPFI Team</h4>
<br/>
<p style='color:#f9f9f9'>
"", Sys.time(), ""</p>"")
body <- gsub(""\n"","""",body)

key1 = ""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""

msg = sprintf('{{\""personalizations\"":
        [{{\""to\"": [{{\""email\"": \""%s\""}}]}}],
          \""from\"": {{\""email\"": \""%s\""}},
          \""subject\"": \""%s"",
          \""content\"": [{{\""type\"": \""text/html\"",
          \""value\"": \""%s\""}}]}}', to, from, subject, body)

pp = POST(""https://api.sendgrid.com/v3/mail/send"",
                body = msg,
        config = add_headers(""Authorization"" = sprintf(""Bearer %s"", key1),
                        ""Content-Type"" = ""application/json""),
        verbose())

conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry3 set Stage=7 where Id={entry.Id}"", sep=""""))
close(conn)
";

                entry.ProcessStart = DateTime.Now;

                entry.Stage = Stage.Submitted;

                _context.Entry3.Update(entry);

                _context.SaveChanges();

                REngineRunner.RunFromCmd(batch, platform, $"> {_hostingEnvironment.WebRootPath}/../Logs/_3{entry.Id}.cshtml 2>&1");

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
        public async Task<IActionResult> Create([Bind("Include_Thinning,ByClass,Stump,MgDisc,LengthDisc,Id,ApplicationUserId,AgeStart,AgeEnd,Distribution,DistributionThinning,Model,VolumeFormula,Way,IP,ProcessStart,ProcessTime,Stage")] Entry3 entry3)
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
        public async Task<IActionResult> Edit(int id, [Bind("Include_Thinning,ByClass,Stump,MgDisc,LengthDisc,Id,ApplicationUserId,AgeStart,AgeEnd,Distribution,DistributionThinning,Model,VolumeFormula,Way,IP,ProcessStart,ProcessTime,Stage")] Entry3 entry3)
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
