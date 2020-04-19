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
using Microsoft.AspNetCore.Hosting;
using FPFI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text.Encodings.Web;
using System.ComponentModel;
using OfficeOpenXml;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using FPFI.Hubs;

namespace FPFI.Controllers
{
    [Authorize(Policy = "Apps")]
    public class Entry2Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<EntryHub> _hubContext;

        public Entry2Controller(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IHubContext<EntryHub> hubContext,
            IServiceProvider serviceProvider,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // GET: Entry2
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Entry2.Include(e => e.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Step1(int? id)
        {
            ViewData["Dist"] = new SelectList(
                from Dist e in Enum.GetValues(typeof(Dist))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Species"] = new SelectList(
                from Sp e in Enum.GetValues(typeof(Sp))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Height"] = new SelectList(
                from Hfunc e in Enum.GetValues(typeof(Hfunc))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Model"] = new SelectList(
                from Model e in Enum.GetValues(typeof(Model))
                select new { Id = e, Name = String.Join(" - ", e.ToString(), e.GetDisplayName()) },
                "Id", "Name");

            ViewData["Vform"] = new SelectList(
                from VolF e in Enum.GetValues(typeof(VolF))
                select new { Id = e, Name = String.Join(" - ", e.ToString(), e.GetDisplayName()) },
                "Id", "Name");

            ViewData["Way"] = new SelectList(
                from Way e in Enum.GetValues(typeof(Way))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            if (id == null)
            {
                var defaultEntry = new Entry2
                {
                    //Default Values
                    AgeStart = 15,
                    AgeEnd = 30,
                    Deviation = 0
                };
                return View(defaultEntry);
            };
            var model = await _context.Entry2
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
            "Deviation," +
            "Species," +
            "HeightFunction," +
            "Model," +
            "VolumeFormula," +
            "Way")] Entry2 entry)
        {
            if (ModelState.IsValid)
            {
                if (entry.Id > 0)
                {
                    var old = await _context.Entry2
                        .FirstOrDefaultAsync(e => e.Id == entry.Id && e.ApplicationUserId == _userManager.GetUserId(User));
                    if (old == null) return NotFound();
                    old.AgeStart = entry.AgeStart;
                    old.AgeEnd = entry.AgeEnd;
                    old.Distribution = entry.Distribution;
                    old.DistributionThinning = entry.DistributionThinning;
                    old.Deviation = entry.Deviation;
                    old.Species = entry.Species;
                    old.HeightFunction = entry.HeightFunction;
                    old.Model = entry.Model;
                    old.VolumeFormula = entry.VolumeFormula;
                    old.Way = entry.Way;
                    _context.Update(old);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Step2), new { id = old.Id });
                }
                entry.ApplicationUserId = _userManager.GetUserId(User);

                var failed = _context.Entry2
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
                                _context.Input2.Remove(inp);
                            }
                        }
                        if (ent.Products.Any())
                        {
                            foreach (var prod in ent.Products)
                            {
                                _context.Product2.Remove(prod);
                            }
                        }
                        if (ent.Parameter != null) _context.Parameter2.Remove(ent.Parameter);
                        _context.Entry2.Remove(ent);
                    }
                }
                _context.Entry2.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step2), new { id = entry.Id });
            }
            ViewData["Dist"] = new SelectList(
                from Dist e in Enum.GetValues(typeof(Dist))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Species"] = new SelectList(
                from Sp e in Enum.GetValues(typeof(Sp))
                select new { Id = e, Name = e.GetDisplayName() },
                "Id", "Name");

            ViewData["Height"] = new SelectList(
                from Hfunc e in Enum.GetValues(typeof(Hfunc))
                select new { Id = e, Name = e.GetDisplayName() },
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

            var entry = await _context.Entry2.FirstOrDefaultAsync(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));

            ViewData["Error"] = err;

            if (entry != null) return View(entry);

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Step2([Bind("Id,Excel,AgeStart,AgeEnd")] Entry entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context.Entry2.FirstOrDefaultAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Excel = entrada.Excel;

                if (entry.Excel == null) return View(entry);

                var contentDisposition = ContentDispositionHeaderValue.Parse(entry.Excel.ContentDisposition);

                var filename = contentDisposition.FileName.Trim('"');

                var stream = entry.Excel.OpenReadStream();

                var inputs = new List<Input2> { };

                var prods = new List<Product2> { };

                try
                {
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["initial"];
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var range = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column];
                            if (!range.Any(c => !string.IsNullOrEmpty(c.Text)))
                            {
                                break;
                            }
                            var item = new Input2
                            {
                                Id_ = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                                Macrostand = worksheet.Cells[row, 2].Value.ToString(),

                                Pyear = Convert.ToInt16(worksheet.Cells[row, 3].Value),

                                Age = double.Parse(worksheet.Cells[row, 4].Value.ToString()),
                                N = double.Parse(worksheet.Cells[row, 5].Value.ToString()),
                                BA = double.Parse(worksheet.Cells[row, 6].Value.ToString()),
                                Dg = double.Parse(worksheet.Cells[row, 7].Value.ToString()),
                                D100 = double.Parse(worksheet.Cells[row, 8].Value.ToString()),
                                Hd = double.Parse(worksheet.Cells[row, 9].Value.ToString()),
                                Vt = Convert.ToInt32(worksheet.Cells[row, 10].Value),
                                Years = Convert.ToInt32(worksheet.Cells[row, 11].Value),

                                ThinningAges = (worksheet.Cells[row, 12].Value != null) ? worksheet.Cells[row, 12].Value.ToString() : "",
                                NAfterThins = (worksheet.Cells[row, 13].Value != null) ? worksheet.Cells[row, 13].Value.ToString() : "",
                                ThinTypes = (worksheet.Cells[row, 14].Value != null) ? worksheet.Cells[row, 14].Value.ToString() : "",
                                ThinCoefs = (worksheet.Cells[row, 15].Value != null) ? worksheet.Cells[row, 15].Value.ToString() : "",

                                Hp = Convert.ToInt32(worksheet.Cells[row, 16].Value),
                                Hm = Convert.ToInt32(worksheet.Cells[row, 17].Value)
                            };
                            inputs.Add(item);
                        }

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
                            var item = new Product2
                            {
                                X_1 = (worksheet.Cells[row, 1].Value != null) ? worksheet.Cells[row, 1].Value.ToString() : "",
                                Diameter = Convert.ToInt16(worksheet.Cells[row, 2].Value),
                                Length = Convert.ToInt16(worksheet.Cells[row, 3].Value),
                                Priority = Convert.ToInt16(worksheet.Cells[row, 4].Value)
                            };
                            prods.Add(item);
                        }

                        worksheet = package.Workbook.Worksheets["params"];
                        rowCount = worksheet.Dimension.Rows;
                        ColCount = worksheet.Dimension.Columns;

                        var par = new Parameter2
                        {
                            Beta1 = double.Parse(worksheet.Cells[2, 1].Value.ToString()),
                            Beta2 = double.Parse(worksheet.Cells[2, 2].Value.ToString()),
                            Beta3 = double.Parse(worksheet.Cells[2, 3].Value.ToString()),
                            Beta4 = double.Parse(worksheet.Cells[2, 4].Value.ToString()),
                            Alpha1 = double.Parse(worksheet.Cells[2, 5].Value.ToString()),
                            Alpha2 = double.Parse(worksheet.Cells[2, 6].Value.ToString())
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

                _context.Entry2.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step3), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step3(int? id, int? pg, int? rpp, string srt, bool? asc)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry2
            .Include(s => s.Inputs)
            .SingleOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == _userManager.GetUserId(User));

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
                PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Input2)).Find(srt, false);

                if (asc.Value)
                {
                    entry.Inputs = entry.Inputs.OrderBy(x => prop.GetValue(x)).ToList();
                }
                else
                {
                    entry.Inputs = entry.Inputs.OrderByDescending(x => prop.GetValue(x)).ToList();
                    //entry.Inputs = entry.Inputs.OrderByDescending(i => i.GetType().GetProperty(srt));
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
        public async Task<IActionResult> Step3([Bind("Id,Inputs,AgeStart,AgeEnd")] Entry2 entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context.Entry2.FirstOrDefaultAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Inputs = entrada.Inputs;

                _context.Entry2.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step4), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step4(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry2
            .Include(s => s.Products)
            .SingleOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == _userManager.GetUserId(User));

            if (entry == null) return NotFound();

            entry.Products = entry.Products.OrderBy(p => p.Diameter).ToList();

            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Step4([Bind("Id,Products,AgeStart,AgeEnd")] Entry2 entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context
                    .Entry2
                    .FirstOrDefaultAsync(e => e.Id == entrada.Id
                    && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Products = entrada.Products;

                _context.Entry2.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step5), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step5(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entry2
            .Include(s => s.Parameter)
            .SingleOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == _userManager.GetUserId(User));

            if (entry == null) return NotFound();

            return View(entry);
        }

        [HttpPost]
        [RequestFormSizeLimit(valueCountLimit: 200000, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
        public async Task<IActionResult> Step5([Bind("Id,Parameter,AgeStart,AgeEnd")] Entry2 entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context.Entry2
                    .Include(e => e.Parameter)
                    .Include(e => e.Products)
                    .Include(e => e.ApplicationUser)
                    .FirstOrDefaultAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Parameter = entrada.Parameter;

                var platform = RTools.GetPlatform();

                var connStr = RTools.GetRformattedConnectionString(_hostingEnvironment, platform);

                var tmp = platform == "Unix" ? ", tmpdir = '/tmp'" : string.Empty;

                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

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
                    ""fpfi2"")

new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
if (length(new.packages)) install.packages(new.packages,repos=""https://cran.us.r-project.org"")
lapply(list.of.packages, require, character.only = TRUE)

#open database and read entry
conn <- odbcDriverConnect(""{connStr}"")

df <- rename(subset(data.table(sqlQuery(conn, str_c(""select * from Input2 where Entry2Id = {entry.Id} ""))), select = -c(Id,Entry2Id)),
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
        ""Hm""=""hm""))

df <- df[,c(""id"",
            ""macrostand"",
            ""pyear"",
            ""Age"",
            ""N"",
            ""BA"",
            ""Dg"",
            ""d100"",
            ""Hd"",
            ""Vt"",
            ""years"",
            ""thinningAge"",
            ""n.afterThin"",
            ""thinTypes"",
            ""thin_coeff"",
            ""hp"",
            ""hm"")]

df[df=='']<-NA

conn <- odbcDriverConnect(""{connStr}"")

prods <- rename(subset(data.table(sqlQuery(conn, str_c(""select * from Product2 where Entry2Id = {entry.Id}""))), select = -c(Id,Entry2Id)),
        c(""X_1""=""X__1"",
        ""Diameter""=""diameter"",
        ""Length""=""length"",
        ""Priority""=""priority""))

prods <- prods[order(prods$priority)]

prods <- prods[,c(""X__1"",""diameter"",""length"",""priority"")]

conn <- odbcDriverConnect(""{connStr}"")

params <- list.remove(unlist(sqlQuery(conn, str_c(""select * from Parameter2 where Entry2Id = {entry.Id}""))), c(""Id"",""Entry2Id""))

names(params) <- c(""a1"", ""a2"", ""beta1"", ""beta2"", ""beta3"", ""beta4"")

params <- params[c(""beta1"", ""beta2"", ""beta3"", ""beta4"",""a1"",""a2"")]

df$thinTypes <- as.character(df$thinTypes)

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry2 set Stage=2 where Id={entry.Id}"", sep=""""))

#run simulation
sims <-fullSim(in_data = list(df = df, params = params, prods = prods),
                age_range = c({entry.AgeStart},{entry.AgeEnd}),
                distribution = ""{entry.Distribution}"",
                distribution_thinning = ""{entry.DistributionThinning}"",
                deviation = {entry.Deviation},
                species = ""{entry.Species}"",
                height_function = ""{entry.HeightFunction}"",
                model = ""{entry.Model}"",
                volform = ""{entry.VolumeFormula}"",
                way = ""{entry.Way}"")

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry2 set Stage=3 where Id={entry.Id}"", sep=""""))

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

sims$simulation$Entry2Id <- rep({entry.Id},nrow(sims$simulation))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from Simulation2"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
sims$simulation$Id <- start[[1]]:(start+nrow(sims$simulation)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")

ColumnsOfTable       <- sqlColumns(conn, ""Simulation2"")
varTypes             <- as.character(ColumnsOfTable$TYPE_NAME) 
names(varTypes)      <- as.character(ColumnsOfTable$COLUMN_NAME) 

setcolorder(sims$simulation, as.character(ColumnsOfTable$COLUMN_NAME))

colnames(sims$simulation) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(sims$simulation, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)
conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT Simulation2 FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"", sep=""""))

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry2 set Stage=4 where Id={entry.Id}"", sep=""""))

#parse and save tapers
sims$taper <- rename(sims$taper,
        c(""id""=""Id_"",
        ""dbh""=""Dbh"",
        ""ht""=""Ht"",
        ""freq""=""Freq"",
        ""idgu""=""Idgu"",
        ""hp""=""Hp"",
        ""hm""=""Hm"",
        ""merchvol""=""MerchVol"",
        ""idg""=""Idg""))

list.of.packages <- c(""dplyr"")
new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
if (length(new.packages)) install.packages(new.packages,repos=""https://cran.us.r-project.org"")
library(""dplyr"")

diams <- sims$taper %>% dplyr:: select(grep(""diam"", names(sims$taper)))

taper <- sims$taper %>% dplyr:: select(setdiff(seq(1,length(sims$taper)),grep(""diam"", names(sims$taper))))
detach(""package:dplyr"", unload=TRUE)

taper$Entry2Id <- rep({entry.Id},nrow(taper))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from Taper2"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
taper$Id <- start[[1]]:(start+nrow(taper)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")

ColumnsOfTable       <- sqlColumns(conn, ""Taper2"")
varTypes <- as.character(ColumnsOfTable$TYPE_NAME)
names(varTypes) <- as.character(ColumnsOfTable$COLUMN_NAME)

setcolorder(taper, as.character(ColumnsOfTable$COLUMN_NAME))

colnames(taper) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(taper, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)
conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT Taper2 FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"",sep=""""))

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry2 set Stage=5 where Id={entry.Id}"", sep=""""))

#parse and save diameters #first step could be optimized
#df = as.data.frame(t(as.data.frame(replicate(nrow(diams),names(diams),FALSE))))
#rownames(df) <- NULL
#names(df) <- rep(""Name"",length(df))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from Diams"")
if(is.na(start)){{start = 1}}else{{start=start+1}}

#tmp <- as.data.frame(list(start:nrow(taper),taper$Idg,diams[,c(1:length(diams) == 1),with=FALSE],df[,c(1:length(df) == 1)]))
tmp <- as.data.frame(list(start[[1]]:(start+nrow(diams)-1)[[1]],taper$Idg,diams[,c(1:length(diams) == 1),with=FALSE],rep(names(diams)[[1]],nrow(diams)) ))
rownames(tmp) <- NULL
names(tmp) <- c(""Id"",""Idg"",""Value"",""Name"")

for (i in 2:length(diams)){{
    #tmp2 <- as.data.frame(list((start+nrow(taper)*(i-1))[[1]]:(nrow(taper)*i)[[1]],taper$Idg,diams[,c(1:length(diams) == i),with=FALSE],df[,c(1:length(df) == i)] ))
    tmp2 <- as.data.frame(list((start+nrow(diams)*(i-1))[[1]]:(start+nrow(diams)*i-1)[[1]],taper$Idg,diams[,c(1:length(diams) == i),with=FALSE],rep(names(diams)[[i]],nrow(diams)) ))
    rownames(tmp2) <- NULL
    names(tmp2) <- c(""Id"",""Idg"",""Value"",""Name"")
    tmp <- data.frame(rbind(as.matrix(tmp2),as.matrix(tmp)))
}}

tmp$Entry2Id <- rep({entry.Id},nrow(tmp))

conn <- odbcDriverConnect(""{connStr}"")

ColumnsOfTable <- sqlColumns(conn, ""Diams"")
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
sqlQuery(conn, paste(""BULK INSERT Diams FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"",sep=""""))

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry2 set Stage=6 where Id={entry.Id}"", sep=""""))

#save time and close
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entry2 set ProcessTime="", proc.time()[[3]], "" where Id={entry.Id}"", sep=""""))

#send email
from <- ""<no-reply@fpfi.cl>""
to <- ""<{entry.ApplicationUser.Email}>""
subject <- ""FPFI Entry Results are Ready""
body <- paste(""
<h3>Dear {entry.ApplicationUser.Name},</h3>
<br/>
The results for your entry submition number <strong>{entry.Id}</strong> to FPFI2 algorith can be viewed on the following links:

<h4>SVG:</h4>
<a href='{HtmlEncoder.Default.Encode($"{baseUrl}/Results/SvgGraphs/" + entry.Id + "?v=2")}'>
    {HtmlEncoder.Default.Encode($"{baseUrl}/Results/SvgGraphs/" + entry.Id + "?v=2")}
</a>
<h4>CANVAS:</h4>
<a href='{HtmlEncoder.Default.Encode($"{baseUrl}/Results/CanvasGraphs/" + entry.Id + "?v=2")}'>
    {HtmlEncoder.Default.Encode($"{baseUrl}/Results/CanvasGraphs/" + entry.Id + "?v=2")}
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
sqlQuery(conn, paste(""update Entry2 set Stage=7 where Id={entry.Id}"", sep=""""))
close(conn)
";

                entry.ProcessStart = DateTime.Now;

                entry.Stage = Stage.Submitted;

                _context.Entry2.Update(entry);

                _context.SaveChanges();

                var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>()
                            .CreateScope();

                var hub = serviceScope.ServiceProvider.GetService<IHubContext<EntryHub>>();

                REngineRunner.RunFromCmd(batch, _hostingEnvironment.ContentRootPath, entry.Id, 2, _context, hub);

                return RedirectToAction("Index", "Results", new { id = entry.Id, v = 2 });
            }
            return RedirectToAction(nameof(Step1));
        }

        // GET: Entry2/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry2 = await _context.Entry2
                .Include(e => e.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entry2 == null)
            {
                return NotFound();
            }

            return View(entry2);
        }

        // GET: Entry2/Create
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Entry2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Deviation,Species,HeightFunction,Id,ApplicationUserId,AgeStart,AgeEnd,Distribution,DistributionThinning,Model,VolumeFormula,Way,IP,ProcessStart,ProcessTime,Stage")] Entry2 entry2)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entry2);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", entry2.ApplicationUserId);
            return View(entry2);
        }

        // GET: Entry2/Edit/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry2 = await _context.Entry2.SingleOrDefaultAsync(m => m.Id == id);
            if (entry2 == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", entry2.ApplicationUserId);
            return View(entry2);
        }

        // POST: Entry2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Edit(int id, [Bind("Deviation,Species,HeightFunction,Id,ApplicationUserId,AgeStart,AgeEnd,Distribution,DistributionThinning,Model,VolumeFormula,Way,IP,ProcessStart,ProcessTime,Stage")] Entry2 entry2)
        {
            if (id != entry2.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Entry2Exists(entry2.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", entry2.ApplicationUserId);
            return View(entry2);
        }

        // GET: Entry2/Delete/5
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry2 = await _context.Entry2
                .Include(e => e.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entry2 == null)
            {
                return NotFound();
            }

            return View(entry2);
        }

        // POST: Entry2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry2 = await _context.Entry2.SingleOrDefaultAsync(m => m.Id == id);
            _context.Entry2.Remove(entry2);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Entry2Exists(int id)
        {
            return _context.Entry2.Any(e => e.Id == id);
        }
    }
}
