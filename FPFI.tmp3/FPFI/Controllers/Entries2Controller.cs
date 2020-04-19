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
using FPFI.Services;
using System.Net.Http.Headers;
using OfficeOpenXml;
using System.ComponentModel;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace FPFI.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IHostingEnvironment _hostingEnvironment;

        public EntriesController(ApplicationDbContext context,
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

        // GET: Entries
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context
                .Entries2
                .Include(e => e.ApplicationUser);

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
                var defaultEntry = new Entry2
                {
                    //Default Values
                    AgeStart = 15,
                    AgeEnd = 30,
                    Deviation = 0
                };
                return View(defaultEntry);
            };
            var model = await _context.Entries2
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
                if(entry.Id > 0)
                {
                    var old = await _context.Entries2
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

                var failed = _context.Entries2
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
                                _context.Inputs.Remove(inp);
                            }
                        }
                        if (ent.Products.Any())
                        {
                            foreach (var prod in ent.Products)
                            {
                                _context.Products2.Remove(prod);
                            }
                        }
                        if (ent.Parameter != null) _context.Parameters.Remove(ent.Parameter);
                        _context.Entries2.Remove(ent);
                    }
                }
                _context.Entries2.Add(entry);
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

        public async Task<IActionResult> Step2(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entries2.FirstOrDefaultAsync(e => e.Id == id && e.ApplicationUserId == _userManager.GetUserId(User));

            if(entry != null) return View(entry);

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Step2([Bind("Id,Excel,AgeStart,AgeEnd")] Entry entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context.Entries2.FirstOrDefaultAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Excel = entrada.Excel;

                if(entry.Excel == null) return View(entry);

                var contentDisposition = ContentDispositionHeaderValue.Parse(entry.Excel.ContentDisposition);

                var filename = contentDisposition.FileName.Trim('"');

                var stream = entry.Excel.OpenReadStream();

                var inputs = new List<Input> { };

                var prods = new List<Product2> { };

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
                        var item = new Input
                        {
                            Id_ = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                            Macrostand = worksheet.Cells[row, 2].Value.ToString(),

                            Pyear = Convert.ToInt16(worksheet.Cells[row, 3].Value),

                            Age = Double.Parse(worksheet.Cells[row, 4].Value.ToString()),
                            N = Double.Parse(worksheet.Cells[row, 5].Value.ToString()),
                            BA = Double.Parse(worksheet.Cells[row, 6].Value.ToString()),
                            Dg = Double.Parse(worksheet.Cells[row, 7].Value.ToString()),
                            D100 = Double.Parse(worksheet.Cells[row, 8].Value.ToString()),
                            Hd = Double.Parse(worksheet.Cells[row, 9].Value.ToString()),
                            Vt = Convert.ToInt32(worksheet.Cells[row, 10].Value),
                            Years = Convert.ToInt32(worksheet.Cells[row, 11].Value),

                            ThinningAges = (worksheet.Cells[row, 12].Value != null) ? worksheet.Cells[row, 12].Value.ToString() : "",
                            NAfterThins = (worksheet.Cells[row, 13].Value != null) ? worksheet.Cells[row, 13].Value.ToString() : "",
                            ThinTypes = (worksheet.Cells[row, 14].Value != null) ? worksheet.Cells[row, 14].Value.ToString() : "",
                            ThinCoefs = (worksheet.Cells[row, 12].Value != null) ? worksheet.Cells[row, 15].Value.ToString() : "",

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

                    var par = new Parameter
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
                entry.Inputs = inputs;
                entry.Products = prods;
                entry.IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                _context.Entries2.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step3), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step3(int? id, int? pg, int? rpp, string srt, bool? asc)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entries2
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
                PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Input)).Find(srt, false);

                if (asc.Value)
                {
                    entry.Inputs = entry.Inputs.OrderBy(x => prop.GetValue(x));
                }
                else
                {
                    entry.Inputs = entry.Inputs.OrderByDescending(x => prop.GetValue(x));
                    //entry.Inputs = entry.Inputs.OrderByDescending(i => i.GetType().GetProperty(srt));
                }
            }
            catch
            {
                return NotFound();
            }

            if(rpp.HasValue) ViewData["last"] = (Math.Ceiling((double)(entry.Inputs.Count() / rpp.Value))).ToString();

            entry.Inputs = entry.Inputs.Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value);

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
                var entry = await _context.Entries2.FirstOrDefaultAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Inputs = entrada.Inputs;

                _context.Entries2.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step4), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step4(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entries2
            .Include(s => s.Products)
            .SingleOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == _userManager.GetUserId(User));

            if (entry == null) return NotFound();

            entry.Products = entry.Products.OrderBy(p => p.Diameter);

            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Step4([Bind("Id,Products,AgeStart,AgeEnd")] Entry2 entrada)
        {
            if (ModelState.IsValid && entrada.Id != 0)
            {
                var entry = await _context
                    .Entries2
                    .FirstOrDefaultAsync(e => e.Id == entrada.Id
                    && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Products = entrada.Products;

                _context.Entries2.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Step5), new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        public async Task<IActionResult> Step5(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.Entries2
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
                var entry = await _context.Entries2
                    .Include(e => e.Parameter)
                    .Include(e => e.Products)
                    .Include(e => e.ApplicationUser)
                    .FirstOrDefaultAsync(e => e.Id == entrada.Id && e.ApplicationUserId == _userManager.GetUserId(User));

                if (entry == null) return NotFound();

                entry.Parameter = entrada.Parameter;

                var platform = RTools.GetPlatform();

                var connStr = RTools.GetRformattedConnectionString(_hostingEnvironment,platform);

                var tmp = platform == "Unix"? ", tmpdir = '/tmp'" : string.Empty ;

                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

                var batch = 
$@"
#save output to variable
sink(tt <- textConnection(""results"",""w""),split=TRUE)

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

df <- rename(subset(data.table(sqlQuery(conn, str_c(""select * from Inputs where EntryId = {entry.Id} ""))), select = -c(Id,EntryId)),
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

prods <- rename(subset(data.table(sqlQuery(conn, str_c(""select * from Products where EntryId = {entry.Id}""))), select = -c(Id,EntryId)),
        c(""X_1""=""X__1"",
        ""Diameter""=""diameter"",
        ""Length""=""length"",
        ""Priority""=""priority""))

prods <- prods[order(prods$priority)]

prods <- prods[,c(""X__1"",""diameter"",""length"",""priority"")]

conn <- odbcDriverConnect(""{connStr}"")

params <- list.remove(unlist(sqlQuery(conn, str_c(""select * from Parameters where EntryId = {entry.Id}""))), c(""Id"",""EntryId""))

names(params) <- c(""a1"", ""a2"", ""beta1"", ""beta2"", ""beta3"", ""beta4"")

params <- params[c(""beta1"", ""beta2"", ""beta3"", ""beta4"",""a1"",""a2"")]

df$thinTypes <- as.character(df$thinTypes)

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set Stage=2, Output='"", paste(warnings(), collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))

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
sqlQuery(conn, paste(""update Entries set Stage=3, Output='"", paste(warnings(), collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))

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

sims$simulation$EntryId <- rep({entry.Id},nrow(sims$simulation))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from Simulations"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
sims$simulation$Id <- start[[1]]:(start+nrow(sims$simulation)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")

ColumnsOfTable       <- sqlColumns(conn, ""Simulations"")
varTypes             <- as.character(ColumnsOfTable$TYPE_NAME) 
names(varTypes)      <- as.character(ColumnsOfTable$COLUMN_NAME) 

setcolorder(sims$simulation, as.character(ColumnsOfTable$COLUMN_NAME))

colnames(sims$simulation) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(sims$simulation, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)
conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT Simulations FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"", sep=""""))

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set Stage=4, Output='"", paste(warnings(), collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))

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

taper$EntryId <- rep({entry.Id},nrow(taper))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from Tapers"")
if(is.na(start)){{start = 1}}else{{start=start+1}}
taper$Id <- start[[1]]:(start+nrow(taper)-1)[[1]]

conn <- odbcDriverConnect(""{connStr}"")

ColumnsOfTable       <- sqlColumns(conn, ""Tapers"")
varTypes <- as.character(ColumnsOfTable$TYPE_NAME)
names(varTypes) <- as.character(ColumnsOfTable$COLUMN_NAME)

setcolorder(taper, as.character(ColumnsOfTable$COLUMN_NAME))

colnames(taper) <- as.character(ColumnsOfTable$COLUMN_NAME)

tmp <- sapply(taper, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)
conn < -odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""BULK INSERT Tapers FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"",sep=""""))

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set Stage=5, Output='"", paste(warnings(), collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))

#parse and save diameters #first step could be optimized
#df = as.data.frame(t(as.data.frame(replicate(nrow(diams),names(diams),FALSE))))
#rownames(df) <- NULL
#names(df) <- rep(""Name"",length(df))

conn <- odbcDriverConnect(""{connStr}"")
start <- sqlQuery(conn,""select max(Id) from Diams"")
if(is.na(start)){{start = 1}}else{{start=start+1}}

#tmp <- as.data.frame(list(start:nrow(taper),taper$Id,diams[,c(1:length(diams) == 1),with=FALSE],df[,c(1:length(df) == 1)]))
tmp <- as.data.frame(list(start[[1]]:(start+nrow(diams)-1)[[1]],taper$Id,diams[,c(1:length(diams) == 1),with=FALSE],rep(names(diams)[[1]],nrow(diams)) ))
rownames(tmp) <- NULL
names(tmp) <- c(""Id"",""TaperId"",""Value"",""Name"")

for (i in 2:length(diams)){{
    #tmp2 <- as.data.frame(list((start+nrow(taper)*(i-1))[[1]]:(nrow(taper)*i)[[1]],taper$Id,diams[,c(1:length(diams) == i),with=FALSE],df[,c(1:length(df) == i)] ))
    tmp2 <- as.data.frame(list((start+nrow(diams)*(i-1))[[1]]:(start+nrow(diams)*i-1)[[1]],taper$Id,diams[,c(1:length(diams) == i),with=FALSE],rep(names(diams)[[i]],nrow(diams)) ))
    rownames(tmp2) <- NULL
    names(tmp2) <- c(""Id"",""TaperId"",""Value"",""Name"")
    tmp <- data.frame(rbind(as.matrix(tmp2),as.matrix(tmp)))
}}

tmp$EntryId <- rep({entry.Id},nrow(tmp))

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
sqlQuery(conn, paste(""update Entries set Stage=6, Output='' where Id={entry.Id}"", sep=""""))

#save time and close
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set ProcessTime="", proc.time()[[3]], "" where Id={entry.Id}"", sep=""""))

#send email
from <- ""<no-reply@fpfi.cl>""
to <- ""<{entry.ApplicationUser.Email}>""
subject <- ""FPFI Entry Results are Ready""
body <- paste(""
<h3>Dear {entry.ApplicationUser.Name},</h3>
<br/>
The results for your entry submition number <strong>{entry.Id}</strong> can be viewed on the following links:

<h4>SVG:</h4>
<a href='{HtmlEncoder.Default.Encode($"{baseUrl}/Results/SubmitDetails/" + entry.Id)}'>
    {HtmlEncoder.Default.Encode($"{baseUrl}/Results/SubmitDetails/" + entry.Id)}
</a>
<h4>CANVAS:</h4>
<a href='{HtmlEncoder.Default.Encode($"{baseUrl}/Results/SubmitDetails2/" + entry.Id)}'>
    {HtmlEncoder.Default.Encode($"{baseUrl}/Results/SubmitDetails2/" + entry.Id)}
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

#pp = POST(""https://api.sendgrid.com/v3/mail/send"",
#                body = msg,
#        config = add_headers(""Authorization"" = sprintf(""Bearer %s"", key1),
#                        ""Content-Type"" = ""application/json""),
#        verbose())

sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set Stage=7, Output='' where Id={entry.Id}"", sep=""""))
close(conn)
close(tt)
";

                entry.ProcessStart = DateTime.Now;

                entry.Stage = Stage.Submitted;

                _context.Entries2.Update(entry);

                _context.SaveChanges();

                REngineRunner.RunFromCmd(batch, platform, "");

                return RedirectToAction("Submitted", "Results", new { id = entry.Id });
            }
            return RedirectToAction(nameof(Step1));
        }

        // GET: Entries/Details/5
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

            return View(entry);
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator", Policy = "Data")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.Entries2.SingleOrDefaultAsync(m => m.Id == id);
            _context.Entries2.Remove(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntryExists(int id)
        {
            return _context.Entries2.Any(e => e.Id == id);
        }

    }
}
