using FPFI.Models;
using FPFI.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FPFI.Services
{
    public class RTools
    {
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
                var cmd = $@"ps aux | grep -v grep | grep disown";

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

        public static void GenerateFiles(
            string connStr, 
            string wwwroot, 
            DownloadViewModel Dl,
            Entry entry,
            ApplicationUser user)
        {
            var date = DateTime.Now.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "_");

            var batch =
$@"
#save output to variable
sink(tt <- textConnection(""results"",""w""),split=TRUE)

list.of.packages <- c(""xlsx"",
                    ""matrixStats"",
                    ""plyr"",
                    ""RODBC"",
                    ""stringr"",
                    ""data.table"",
                    ""rlist"",
                    ""jsonlite"",
                    ""sendmailR"",
                    ""httr"",
                    ""rJava"")

new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[, ""Package""])]
if (length(new.packages)) install.packages(new.packages,repos=""https://cran.us.r-project.org"")
lapply(list.of.packages, require, character.only = TRUE)

#open database and read entry
conn <- odbcDriverConnect(""{connStr}"")

#get simulations
s <- rename(subset(data.table(sqlQuery(conn, str_c(""SELECT * FROM Simulations WHERE EntryId={Dl.Id}""))), select = -c(Id,EntryId)),
    c(""Id_""=""Id""))

#open database and read entry
conn <- odbcDriverConnect(""{connStr}"")

#get Tapers
t <- rename(subset(data.table(sqlQuery(conn, str_c(""SELECT * FROM Tapers WHERE EntryId={Dl.Id}""))), select = -c(Id,EntryId)),
    c(""Id_""=""Id""))

#open database and read entry
conn <- odbcDriverConnect(""{connStr}"")

#get Diams
d <- subset(data.table(sqlQuery(conn, str_c(""SELECT * FROM Diams WHERE EntryId={Dl.Id}""))), select = -c(Id,EntryId))

if({Dl.Csv.ToString().ToUpper()}){{
    fwrite(s, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}-Simulations.csv"")
    fwrite(t, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}-Tapers.csv"")
    fwrite(d, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}-Diams.csv"")
}}

#limit <- 1048575
limit <- 65530

if({Dl.Xls.ToString().ToUpper()}) {{
    for(i in 1:ceiling(nrow(s) / limit)){{
        first <- (i-1)*limit+1
        last <- i*limit
        if(last > nrow(s)){{
            last <- nrow(s)
        }}
        write.xlsx(s[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xls"", sheetName = paste(""Simulations"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
    }}
    for(i in 1:ceiling(nrow(t) / limit)){{
        first <- (i-1)*limit+1
        last <- i*limit
        if(last > nrow(t)){{
            last <- nrow(t)
        }}
        write.xlsx(t[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xls"", sheetName = paste(""Tapers"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
    }}
    for(i in 1:ceiling(nrow(d) / limit)){{
        first <- (i-1)*limit+1
        last <- i*limit
        if(last > nrow(d)){{
            last <- nrow(d)
        }}
        write.xlsx(d[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xls"", sheetName = paste(""Diameters"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
    }}
}}
if({Dl.Xlsx.ToString().ToUpper()}) {{
    for(i in 1:ceiling(nrow(s) / limit)){{
        first <- (i-1)*limit+1
        last <- i*limit
        if(last > nrow(s)){{
            last <- nrow(s)
        }}
        write.xlsx(s[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xlsx"", sheetName = paste(""Simulations"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
    }}
    for(i in 1:ceiling(nrow(t) / limit)){{
        first <- (i-1)*limit+1
        last <- i*limit
        if(last > nrow(t)){{
            last <- nrow(t)
        }}
        write.xlsx(t[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xlsx"", sheetName = paste(""Tapers"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
    }}
    for(i in 1:ceiling(nrow(d) / limit)){{
        first <- (i-1)*limit+1
        last <- i*limit
        if(last > nrow(d)){{
            last <- nrow(d)
        }}
        write.xlsx(d[first:last,], ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xlsx"", sheetName = paste(""Diameters"",i), col.names = TRUE, row.names = TRUE, append = TRUE)
    }}
}}

if({Dl.Xml.ToString().ToUpper()}) {{
    write.xml(s, ""{wwwroot}/tmp/{date}-Entry_{Dl.Id}.xml"", sheetName = ""Simulations"", col.names = TRUE, row.names = TRUE, append = TRUE)
}}
setwd(""{wwwroot}"")
shell(""7z a {wwwroot}/Data/{date}-Entry_{Dl.Id}.zip {wwwroot}/tmp/{date}-Entry_{Dl.Id}* -p {Dl.Password}""))
shell(""rm {wwwroot}/tmp/{date}-Entry_{Dl.Id}*"")

#sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set Stage=6, Output='"", paste(warnings(), collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))

#save time and close
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set ProcessTime="", proc.time()[[3]], "" where Id={entry.Id}"", sep=""""))
close(conn)

#send email
from <- ""<no-reply@fpfi.cl>""
to <- ""<{entry.ApplicationUser.Email}>""
subject <- ""FPFI Entry Results are Ready""
body <- paste(""
<h3>Dear {entry.ApplicationUser.Name},</h3>
<br/>
The file(s) requested for your entry submition number <strong>{entry.Id}</strong> will be available for 24 hrs at the following link:
<br/>
<a href='{HtmlEncoder.Default.Encode("https://localhost:44305/Data/{date}-Entry_{id}.zip")}'>
    {HtmlEncoder.Default.Encode("https://localhost:44305//Data/{date}-Entry_{id}.zip")}
</a>
<br/>
<strong>Zip file is password protected using your FPFI account password</strong>
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

sink()
conn <- odbcDriverConnect(""{connStr}"")
sqlQuery(conn, paste(""update Entries set Stage=7, Output='"", paste(results, collapse=""\r\n""), ""' where Id={entry.Id}"", sep=""""))
close(tt)
";
            REngineRunner.RunFromCmd(batch, GetPlatform(), "");
        }

        public static string GetPlatform()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        public static string GetRformattedConnectionString(IHostingEnvironment _hostingEnvironment, string platform)
        {
            var connStr = Exports.GetConnectionString(_hostingEnvironment, platform);
            //IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            //    .SetBasePath(_hostingEnvironment.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //IConfigurationRoot configurationRoot = configurationBuilder.Build();

            ////var platform = GetPlatform();

            //var connStr = configurationRoot.GetConnectionString(platform+"Connection");

            //must have format Server=ipsum;Database=lorum;...
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
                conn["Driver"] = "ODBC Driver 13 for SQL Server";
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
