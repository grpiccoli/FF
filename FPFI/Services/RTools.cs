using FPFI.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace FPFI.Services
{
    public class RTools
    {
        public static string SetId<T>(string conn, string col)
        {
            return $@"{Start<T>(conn)}
{col}$Id <- start[[1]]:(start+nrow({col})-1)[[1]]";
        }

        public static string Start<T>(string conn)
        {
            return $@"{conn}
start <- sqlQuery(conn,""select max(Id) from {typeof(T).Name}"")
odbcClose(conn)
if(is.na(start)){{start = 1}}else{{start=start+1}}";
        }

        public static string BulkInsert<T>(string conn, string tmp, string col)
        {
            var name = typeof(T)
.GetMember("Id").FirstOrDefault()
?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? typeof(T).Name;
            return $@"{conn}
ColumnsOfTable       <- sqlColumns(conn, ""{typeof(T).Name}"")
odbcClose(conn)
ColumnsOfTable$COLUMN_NAME[is.na(ColumnsOfTable$COLUMN_NAME)] <- ""NA""
setcolorder({col}, as.character(ColumnsOfTable$COLUMN_NAME))
tmp <- sapply({col}, as.character)
tmp[is.na(tmp)] <- ''
tmpFile <- tempfile('tmpScript'{tmp})
write.table(tmp,tmpFile, quote=FALSE, sep="","", row.names=FALSE, col.names = FALSE, append=FALSE)
{conn}
bulk<-sqlQuery(conn, paste(""BULK INSERT {typeof(T).Name} FROM '"",tmpFile,""' WITH (FIELDTERMINATOR = ',',ROWTERMINATOR = '\n')"", sep=""""))
odbcClose(conn)
if(identical(bulk,character(0))){{write(""Resulting {typeof(T).Name} successfully added to database"", stdout())}}else{{bulk}}";
        }

        public static string SetStage<T>(string conn, int id, int stg)
        {
            var name = ((Stage)stg).GetType()
.GetMember(((Stage)stg).ToString()).FirstOrDefault()
?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? ((Stage)stg).ToString();
            return $@"{conn}
stage<-sqlQuery(conn, paste(""update {typeof(T).Name} set Stage={stg} where Id={id}"", sep=""""))
odbcClose(conn)
if(identical(stage,character(0))){{write(""Stage {stg} {name} success"", stdout())}}else{{stage}}
write(""stage {stg} {name}"", stdout())";
        }

        public static string ReName<T>(string var, string conn, int? id)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var data = typeof(T).GetFields(bindingFlags).Concat(typeof(T).BaseType.GetFields(bindingFlags)).ToArray();

            var result = new List<string>();
            var sort = new Dictionary<int, string>();

            var i = id.HasValue;

            foreach (var dt in data)
            {
                var name = Regex.Replace(dt.Name, "<([a-zA-Z0-9_]+)>.*", "$1");
                var att = typeof(T)
                    .GetMember(name)
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DisplayAttribute>(false);
                var sn = att?.ShortName ?? null;
                if (sn == null) continue;
                var read = i ? name : sn;
                var write = i ? sn : name;
                result.Add($@"""{read}"" = ""{write}""");
                if (!i) continue;
                var order = (int)att?.Order;
                sort[order] = name;
            }
            var res = string.Join("\",\"", sort.OrderBy(d => d.Key).Select(d => d.Value));
            var pre = i ?
                $@"{conn}
{var} <- rename(subset(data.table(sqlQuery(conn, str_c(""select * from {typeof(T).Name} where Entry3Id = {id} ""))), select = -c(Id,Entry3Id))[,c(""{res}"")],c(" :
                $"{var} <- rename({var},c(";
            var post = i ? $@"))
odbcClose(conn)" : "))";
            return pre + string.Join(",", result) + post;
        }

        public static string Sort<T>(string val)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var data = typeof(T).GetFields(bindingFlags).Concat(typeof(T).BaseType.GetFields(bindingFlags)).ToArray();

            var result = new Dictionary<int, string>();

            foreach (var dt in data)
            {
                var name = Regex.Replace(dt.Name, "<([a-zA-Z0-9_]+)>.*", "$1");
                var att = typeof(T)
                    .GetMember(name)
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DisplayAttribute>(false);
                var sn = att?.ShortName ?? null;
                if (sn == null) continue;
                var order = (int)att?.Order;
                result[order] = sn;
            }
            var res = string.Join("\",\"", result.OrderBy(d => d.Key).Select(d => d.Value));
            return $"{val} <- {val}[,c(\"{res}\")]";
        }

        public static string ShortName<T>(string val)
        {
            return typeof(T)
                       .GetMember(val)
                       .FirstOrDefault()
                       ?.GetCustomAttribute<DisplayAttribute>(false)
                       ?.ShortName ?? val;
        }

        public static string MakeScript(IHostingEnvironment _hostingEnvironment, Entry3 entry, 
            string baseUrl)
        {
            var platform = GetPlatform();

            var conn = $@"conn <- odbcDriverConnect(""{GetRformattedConnectionString(_hostingEnvironment, platform)}"")";

            var tmp = platform == "Unix" ? ", tmpdir = '/tmp'" : string.Empty;

            var prodsList = new List<string> { };

            foreach (var e in Enum.GetValues(typeof(LogType)))
            {
                prodsList.Add($@"{nameof(Product3)}$log_type[{nameof(Product3)}$log_type =={(int)e}] = ""{e.ToString()}""");
            }

            var prods = string.Join(Environment.NewLine, prodsList);

            var sim = "sims";
            var simulation = "simulation";

            return $@"
#install and load required packages
write(""upgrade"", stdout())
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
write(""upgrade"", stdout())
new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
write(""upgrade"", stdout())
r<-lapply(list.of.packages, library, character.only = TRUE)
write(""upgrade"", stdout())
#open database and read entry
{ReName<Input3>(nameof(Input3), conn, entry.Id)}
write(""upgrade"", stdout())
{nameof(Input3)}[{nameof(Input3)}=='']<-NA
write(""upgrade"", stdout())
{nameof(Input3)} <- {nameof(Input3)}[with({nameof(Input3)},order(id)),]
write(""upgrade"", stdout())
{nameof(Input3)}${ShortName<Input3>("ThinTypes")} <- as.character({nameof(Input3)}${ShortName<Input3>("ThinTypes")})
write(""upgrade"", stdout())
write(""upgrade"", stdout())
{ReName<Product3>(nameof(Product3), conn, entry.Id)}
write(""upgrade"", stdout())
{prods}
write(""upgrade"", stdout())
{nameof(Product3)}${ShortName<Product3>("X_1")} <- as.character({nameof(Product3)}${ShortName<Product3>("X_1")})
write(""upgrade"", stdout())
{nameof(Product3)} <- {nameof(Product3)}[order(-rank({ShortName<Product3>("Diameter")}),-rank({ShortName<Product3>("Value")}))]
write(""upgrade"", stdout())
{ReName<Parameter3>(nameof(Parameter3), conn, entry.Id)}
write(""upgrade"", stdout())
{SetStage<Entry3>(conn, entry.Id, (int)Stage.Read)}
write(""upgrade"", stdout())
#run simulation
{sim} <- FullSimulation(in_data = list(df = {nameof(Input3)}, params = {nameof(Parameter3)}, prods = {nameof(Product3)}),
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
write(""upgrade"", stdout())
{SetStage<Entry3>(conn, entry.Id, (int)Stage.Simulated)}
write(""upgrade"", stdout())
{ReName<Simulation3>($"{sim}${simulation}", conn, null)}
write(""upgrade"", stdout())
{sim}${simulation}$Entry3Id <- rep({entry.Id},nrow({sim}${simulation}))
write(""upgrade"", stdout())
{SetId<Simulation3>(conn, $"{sim}${simulation}")}
write(""upgrade"", stdout())
{BulkInsert<Simulation3>(conn, tmp, $"{sim}${simulation}")}
write(""upgrade"", stdout())
{SetStage<Entry3>(conn, entry.Id, (int)Stage.SimUploaded)}

#parse and save tapers
write(""upgrade"", stdout())
list.of.packages <- c(""dplyr"")
write(""upgrade"", stdout())
new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
write(""upgrade"", stdout())
library(""dplyr"")
write(""upgrade"", stdout())
vp_harvest <- sims$taper$stand_level$harvest %>% dplyr:: select(grep(""{nameof(VP)}"", names(sims$taper$stand_level$harvest)))
write(""upgrade"", stdout())
vp_thinning <-sims$taper$stand_level$thinning %>% dplyr:: select(grep(""{nameof(VP)}"", names(sims$taper$stand_level$thinning)))
write(""upgrade"", stdout())
taper_stand_harvest <- sims$taper$stand_level$harvest %>% dplyr:: select(setdiff(seq(1,length(sims$taper$stand_level$harvest)),grep(""{nameof(VP)}"", names(sims$taper$stand_level$harvest))))
write(""upgrade"", stdout())
taper_stand_thinning <- sims$taper$stand_level$thinning %>% dplyr:: select(setdiff(seq(1,length(sims$taper$stand_level$thinning)),grep(""{nameof(VP)}"", names(sims$taper$stand_level$thinning))))
write(""upgrade"", stdout())
detach(""package:dplyr"", unload=TRUE)
write(""upgrade"", stdout())
taper_log_thinning <- sims$taper$log_level$thinning
write(""upgrade"", stdout())
taper_log_harvest <- sims$taper$log_level$harvest
write(""upgrade"", stdout())
taper_log_harvest$Type <- rep({(int)TypeLog.Harvest},nrow(taper_log_harvest))
write(""upgrade"", stdout())
taper_log_thinning$Type <- rep({(int)TypeLog.Thinning},nrow(taper_log_thinning))
write(""upgrade"", stdout())
taper_log <- data.frame(rbind(as.matrix(taper_log_harvest),as.matrix(taper_log_thinning)))
write(""upgrade"", stdout())
{ReName<TaperLog>("taper_log", conn, null)}
write(""upgrade"", stdout())
taper_log$Entry3Id <- rep({entry.Id},nrow(taper_log))
write(""upgrade"", stdout())
{SetId<TaperLog>(conn, "taper_log")}
write(""upgrade"", stdout())
{BulkInsert<TaperLog>(conn, tmp, "taper_log")}
write(""upgrade"", stdout())
{ReName<TaperStandHarvest>("taper_stand_harvest", conn, null)}
write(""upgrade"", stdout())
taper_stand_harvest$Entry3Id <- rep({entry.Id},nrow(taper_stand_harvest))
write(""upgrade"", stdout())
if(!""NA"" %in% colnames(taper_stand_harvest))
{{
    taper_stand_harvest[,""NA""] <- NA
}}
write(""upgrade"", stdout())
{SetId<TaperStandHarvest>(conn, "taper_stand_harvest")}
write(""upgrade"", stdout())
{BulkInsert<TaperStandHarvest>(conn, tmp, "taper_stand_harvest")}
write(""upgrade"", stdout())
{ReName<TaperStandThinning>("taper_stand_thinning", conn, null)}
write(""upgrade"", stdout())
taper_stand_thinning$Entry3Id <- rep({entry.Id},nrow(taper_stand_thinning))
write(""upgrade"", stdout())
if(!""NA"" %in% colnames(taper_stand_thinning))
{{
    taper_stand_thinning[,""NA""] <- NA
}}
write(""upgrade"", stdout())
{SetId<TaperStandThinning>(conn, "taper_stand_thinning")}
write(""upgrade"", stdout())
{BulkInsert<TaperStandThinning>(conn, tmp, "taper_stand_thinning")}
write(""upgrade"", stdout())
{SetStage<Entry3>(conn, entry.Id, (int)Stage.TaperUploaded)}
write(""upgrade"", stdout())
#parse and save diameters
{SetId<VP>(conn, "taper_stand_thinning")}
write(""upgrade"", stdout())
{Start<VP>(conn)}
write(""upgrade"", stdout())
vp_harvest$Idg <- taper_stand_harvest$Idg
write(""upgrade"", stdout())
vp_thinning$Idg <- taper_stand_thinning$Idg
write(""upgrade"", stdout())
vp <- data.frame(rbind(as.matrix(vp_harvest),as.matrix(vp_thinning)))
write(""upgrade"", stdout())
tmp <- as.data.frame(list(start[[1]]:(start+nrow(vp)-1)[[1]],
                        vp[,c(1:length(vp) == 1)],
                        rep(names(vp)[[1]],nrow(vp)),
                        c(rep(0,nrow(vp_harvest)),rep(1,nrow(vp_thinning))),
                        vp$Idg))
write(""upgrade"", stdout())
rownames(tmp) <- NULL
write(""upgrade"", stdout())
names(tmp) <- c(""Id"",""Value"",""Class"",""Type"",""Idg"")
write(""upgrade"", stdout())
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
write(""upgrade"", stdout())
tmp$Entry3Id <- rep({entry.Id},nrow(tmp))
write(""upgrade"", stdout())
tmp$Class <- gsub(""{nameof(VP)}"","""",tmp$Class)
write(""upgrade"", stdout())
{BulkInsert<VP>(conn, tmp, "tmp")}
write(""upgrade"", stdout())
{SetStage<Entry3>(conn, entry.Id, (int)Stage.DiamUploaded)}
write(""upgrade"", stdout())
#save time and close
{conn}
write(""upgrade"", stdout())
t<-sqlQuery(conn, paste(""update {nameof(Entry3)} set ProcessTime="", proc.time()[[3]], "" where Id={entry.Id}"", sep=""""))
odbcClose(conn)
write(""upgrade"", stdout())
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
write(""upgrade"", stdout())
key1 = ""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""
write(""upgrade"", stdout())
msg = sprintf('{{\""personalizations\"":
        [{{\""to\"": [{{\""email\"": \""%s\""}}]}}],
          \""from\"": {{\""email\"": \""%s\""}},
          \""subject\"": \""%s"",
          \""content\"": [{{\""type\"": \""text/html\"",
          \""value\"": \""%s\""}}]}}', to, from, subject, body)
write(""upgrade"", stdout())
pp = POST(""https://api.sendgrid.com/v3/mail/send"",
                body = msg,
        config = add_headers(""Authorization"" = sprintf(""Bearer %s"", key1),
                        ""Content-Type"" = ""application/json""),
        verbose())
write(""upgrade"", stdout())
{SetStage<Entry3>(conn, entry.Id, (int)Stage.EmailSent)}
write(""upgrade"", stdout())
";
        }

        public static bool IsRscriptRunning()
        {
            var platform = GetPlatform();
            var result = new List<string>();
            bool empty = false;

            if (platform == "Win32NT")
            {
                var info = new ProcessStartInfo()
                {
                    FileName = "QPROCESS",
                    Arguments = "\"Rscript.exe\"",
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    while (proc.StandardOutput.Peek() > -1)
                    {
                        result.Add(proc.StandardOutput.ReadLine());
                    }
                    while (proc.StandardError.Peek() > -1)
                    {
                        result.Add(proc.StandardError.ReadLine());
                    }
                    proc.WaitForExit();
                    empty = (result.Count() == 1);
                }
            }
            else if (platform == "Unix")
            {
                var cmd = "ps aux | grep -v grep | grep disown";

                var info = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    Arguments = cmd,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    while (proc.StandardOutput.Peek() > -1)
                    {
                        result.Add(proc.StandardOutput.ReadLine());
                    }
                    while (proc.StandardError.Peek() > -1)
                    {
                        result.Add(proc.StandardError.ReadLine());
                    }
                    proc.WaitForExit();
                    empty = (result.Count() == 0);
                }
            }
            return empty;
        }

//        public static void GenerateFiles(
//            string connStr, 
//            string wwwroot, 
//            DownloadViewModel Dl,
//            Entry entry,
//            ApplicationUser user)
//        {
//            var date = DateTime.Now.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "_");

//            var batch =
//$@"
//#save output to variable
//sink(tt <- textConnection(""results"",""w""),split=TRUE)

//list.of.packages <- c(""xlsx"",
//                    ""matrixStats"",
//                    ""plyr"",
//                    ""RODBC"",
//                    ""stringr"",
//                    ""data.table"",
//                    ""rlist"",
//                    ""jsonlite"",
//                    ""sendmailR"",
//                    ""httr"",
//                    ""rJava"")

//new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
//if (length(new.packages)) install.packages(new.packages,repos=""https://cran.us.r-project.org"")
//lapply(list.of.packages, require, character.only = TRUE)

//#open database and read entry
//conn <- odbcDriverConnect(""{connStr}"")

//#get simulations
//s <- rename(subset(data.table(sqlQuery(conn, str_c(""SELECT * FROM Simulations WHERE EntryId={Dl.Id}""))), select = -c(Id,EntryId)),
//    c(""Id_""=""Id""))

//#open database and read entry
//conn <- odbcDriverConnect(""{connStr}"")

//#get Tapers
//t <- rename(subset(data.table(sqlQuery(conn, str_c(""SELECT * FROM Tapers WHERE EntryId={Dl.Id}""))), select = -c(Id,EntryId)),
//    c(""Id_""=""Id""))

//#open database and read entry
//conn <- odbcDriverConnect(""{connStr}"")

//#get Diams
//d <- subset(data.table(sqlQuery(conn, str_c(""SELECT * FROM Diams WHERE EntryId={Dl.Id}""))), select = -c(Id,EntryId))

//if({Dl.Csv.ToString().ToUpper()}){{
//    fwrite(s, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}-Simulations.csv"")
//    fwrite(t, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}-Tapers.csv"")
//    fwrite(d, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}-Diams.csv"")
//}}

//#limit <- 1048575
//limit <- 65530

//if({Dl.Xlsx.ToString().ToUpper()}) {{
//    for(i in 1:ceiling(nrow(s) / limit)){{
//        first <- (i-1)*limit+1
//        last <- i*limit
//        if(last > nrow(s)){{
//            last <- nrow(s)
//        }}
//        write.xlsx(s[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xlsx"", sheetName = paste(""Simulations"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
//    }}
//    for(i in 1:ceiling(nrow(t) / limit)){{
//        first <- (i-1)*limit+1
//        last <- i*limit
//        if(last > nrow(t)){{
//            last <- nrow(t)
//        }}
//        write.xlsx(t[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xlsx"", sheetName = paste(""Tapers"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
//    }}
//    for(i in 1:ceiling(nrow(d) / limit)){{
//        first <- (i-1)*limit+1
//        last <- i*limit
//        if(last > nrow(d)){{
//            last <- nrow(d)
//        }}
//        write.xlsx(d[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xlsx"", sheetName = paste(""Diameters"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
//    }}
//}}

//if({Dl.Xml.ToString().ToUpper()}) {{
//    write.xml(s, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xml"", sheetName = ""Simulations"", col.names = TRUE, row.names = TRUE, append = TRUE)
//}}
//setwd(""{wwwroot}"")
//shell(""7z a {wwwroot}/Data/{date}-Entry_{Dl.Id}.zip {wwwroot}/tmp/{date}-Entry_{Dl.Id}* -p {Dl.Password}""))
//shell(""rm {wwwroot}/tmp/{date}-Entry_{Dl.Id}*"")

//#sink()
//conn <- odbcDriverConnect(""{connStr}"")
//sqlQuery(conn, paste(""update Entries set Stage=6, Output='"", paste(warnings(), collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))

//#save time and close
//conn <- odbcDriverConnect(""{connStr}"")
//sqlQuery(conn, paste(""update Entries set ProcessTime="", proc.time()[[3]], "" where Id={entry.Id}"", sep=""""))
//close(conn)

//#send email
//from <- ""<no-reply@fpfi.cl>""
//to <- ""<{entry.ApplicationUser.Email}>""
//subject <- ""FPFI Entry Results are Ready""
//body <- paste(""
//<h3>Dear {entry.ApplicationUser.Name},</h3>
//<br/>
//The file(s) requested for your entry submition number <strong>{entry.Id}</strong> will be available for 24 hrs at the following link:
//<br/>
//<a href='{HtmlEncoder.Default.Encode("https://localhost:44305/Data/{date}-Entry_{id}.zip")}'>
//    {HtmlEncoder.Default.Encode("https://localhost:44305//Data/{date}-Entry_{id}.zip")}
//</a>
//<br/>
//<strong>Zip file is password protected using your FPFI account password</strong>
//<br/>
//Kind Regards,
//<br/>
//<h4>FPFI Team</h4>
//<br/>
//<p style='color:#f9f9f9'>
//"", Sys.time(), ""</p>"")
//body <- gsub(""\n"","""",body)

//key1 = ""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""

//msg = sprintf('{{\""personalizations\"":
//        [{{\""to\"": [{{\""email\"": \""%s\""}}]}}],
//          \""from\"": {{\""email\"": \""%s\""}},
//          \""subject\"": \""%s"",
//          \""content\"": [{{\""type\"": \""text/html\"",
//          \""value\"": \""%s\""}}]}}', to, from, subject, body)

//pp = POST(""https://api.sendgrid.com/v3/mail/send"",
//                body = msg,
//        config = add_headers(""Authorization"" = sprintf(""Bearer %s"", key1),
//                        ""Content-Type"" = ""application/json""),
//        verbose())

//sink()
//conn <- odbcDriverConnect(""{connStr}"")
//sqlQuery(conn, paste(""update Entries set Stage=7, Output='"", paste(results, collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))
//close(tt)
//";
//            REngineRunner.RunFromCmd(batch, GetPlatform(), "");
//        }

        public static string GetPlatform()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        public static string GetRformattedConnectionString(IHostingEnvironment _hostingEnvironment, string platform)
        {
            var connStr = Exports.GetConnectionString(_hostingEnvironment, platform);
            var conn = new Dictionary<string, string>();

            var server = Regex.Match(connStr,"Server=[^;]+").ToString().Split("=");

            conn.Add(server[0], server[1]);

            var database = Regex.Match(connStr, "Database=[^;]+").ToString().Split("=");

            conn.Add(database[0], database[1]);

            if (platform == "Win32NT")
            {
                conn["Server"] = Regex.Replace(conn["Server"],@"\\",@"\\");
                conn["Driver"] = "SQL Server Native Client 11.0";
                //conn["tmp"] = string.Empty;
            }
            else if (platform == "Unix")
            {
                //conn["tmp"] = ", tmpdir = '/tmp'";
                //conn["Server"] = "localhost";
                conn["Driver"] = "ODBC Driver 17 for SQL Server";
                conn["Port"] = "1433";
                conn["Protocol"] = "TCPIP";
                conn["UID"] = "SA";
                conn["PWD"] = "89ioIOkl";
            }

            var connList = new List<string> { };
            foreach (KeyValuePair<string, string> item in conn)
            {
                //if (item.Key == "Platform" || item.Key == "tmp") continue;
                connList.Add($"{item.Key}={item.Value}");
            }

            return String.Join(";", connList);
        }
    }
}
