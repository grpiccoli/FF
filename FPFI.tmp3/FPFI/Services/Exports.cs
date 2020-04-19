using FPFI.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FPFI.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FPFI.Services
{
    public class Exports
    {
        public IConfiguration Configuration { get; }

        public static string GetConnectionString(IHostingEnvironment _hostingEnvironment, string platform)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(_hostingEnvironment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configurationRoot = configurationBuilder.Build();

            var connStr = configurationRoot.GetConnectionString(platform + "Connection");

            return connStr;
        }

        public static DataTable GetDataTable(int Id, string col, SqlConnection connection)
        {
            string queryString = $"SELECT * FROM {col} WHERE EntryId={Id}";
            SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dt.Columns.Remove("Id");
            dt.Columns.Remove("EntryId");

            return dt;
        }

        public static Dictionary<string, DataTable> GetData(int Id, SqlConnection connection)
        {
            connection.Open();

            SqlCommand simCommand = new SqlCommand(
                $"SELECT * FROM Simulations WHERE EntryId={Id}",
                connection);

            SqlDataReader sdr = simCommand.ExecuteReader();

            DataTable sdt = new DataTable();

            sdt.Columns.Add("Id");
            sdt.Columns.Add("Macrostand");
            sdt.Columns.Add("Hd");
            sdt.Columns.Add("Age");
            sdt.Columns.Add("Dg");
            sdt.Columns.Add("BA");
            sdt.Columns.Add("N");
            sdt.Columns.Add("Vt");
            sdt.Columns.Add("Sd");
            sdt.Columns.Add("Thin_trees");
            sdt.Columns.Add("ThinAction");
            sdt.Columns.Add("Thin_types");
            sdt.Columns.Add("Thin_coef");
            sdt.Columns.Add("Distr");
            sdt.Columns.Add("Idg");
            sdt.Columns.Add("CAI_Dg");
            sdt.Columns.Add("CAI_Vt");
            sdt.Columns.Add("MAI_Dg");
            sdt.Columns.Add("MAI_Vt");

            while (sdr.Read())
            {
                sdt.Rows.Add(
                    sdr[9],
                    sdr[13],
                    sdr[8],
                    sdr[1],
                    sdr[5],
                    sdr[2],
                    sdr[14],
                    sdr[20],
                    sdr[15],
                    sdr[18],
                    sdr[19],
                    sdr[17],
                    sdr[16],
                    sdr[6],
                    sdr[10],
                    sdr[3],
                    sdr[4],
                    sdr[11],
                    sdr[12]);
            }

            SqlCommand tapCommand = new SqlCommand(
                $"SELECT * FROM Tapers WHERE EntryId={Id}",
                connection);

            SqlDataReader tdr = tapCommand.ExecuteReader();

            DataTable tdt = new DataTable();

            tdt.Columns.Add("Id");
            tdt.Columns.Add("Idg");
            tdt.Columns.Add("Dbh");
            tdt.Columns.Add("Ht");
            tdt.Columns.Add("Freq");
            tdt.Columns.Add("Idgu");
            tdt.Columns.Add("Hp");
            tdt.Columns.Add("Hm");
            tdt.Columns.Add("MerchVol");

            while (tdr.Read())
            {
                tdt.Rows.Add(
                    tdr[7],
                    tdr[8],
                    tdr[1],
                    tdr[6],
                    tdr[3],
                    tdr[9],
                    tdr[5],
                    tdr[4],
                    tdr[10]
                    );
            }

            SqlCommand diamCommand = new SqlCommand(
                $"SELECT * FROM Diams WHERE EntryId={Id}",
                connection);

            SqlDataReader ddr = diamCommand.ExecuteReader();

            DataTable ddt = new DataTable();

            ddt.Columns.Add("TaperId");
            ddt.Columns.Add("Name");
            ddt.Columns.Add("Value");

            while (ddr.Read())
            {
                ddt.Rows.Add(
                    ddr[3],
                    ddr[2],
                    ddr[4]
                    );
            }
            return (new Dictionary<string, DataTable>
                {
                    { "Simulations", sdt },
                    { "Tapers", tdt },
                    { "Diameters", ddt }
                });
        }

        public static void Csv(Dictionary<string, DataTable> datas, string file)
        {
            foreach (var data in datas)
            {
                using (var writer = new StreamWriter($@"{file}-{data.Key}.csv"))
                {
                    writer.WriteLine(string.Join(",", data.Value.Columns.Cast<DataColumn>().Select(d => d.ColumnName)));
                    foreach (DataRow row in data.Value.Rows)
                    {
                        writer.WriteLine(string.Join(",", row.ItemArray));
                    }
                    writer.Close();
                }
            }
        }

        public static void Xml(List<DataTable> datas, string file)
        {
            string xmlFile = $@"{file}.xml";

            var memory = new MemoryStream();
            using (var fs = new FileStream(xmlFile, FileMode.Create, FileAccess.Write))
            {
                DataSet ds = new DataSet();
                foreach(var table in datas)
                {
                    ds.Tables.Add(table);
                }

                ds.WriteXml(fs);
            }
        }

        public static void Export(DownloadViewModel download, 
            string wwwroot, 
            SqlConnection connection, 
            string email, IEmailSender emailSender,
            string name, string baseUrl,
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            //var dataframes = GetData(download.Id, connection);
            var tables = new Dictionary<string, string[]>
            {
                { "Simulations", new string[]{ "Id_", "Macrostand","Hd","Age","Dg","BA","N","Vt","Sd","Thin_trees","Thinaction","ThinTypes","ThinCoefs","Distr","Idg","CAI_Dg","CAI_Vt","MAI_Dg","MAI_Vt" } },
                { "Tapers", new string[]{ "Id_","Idg","Dbh","Ht","Freq","Idgu","Hp","Hm","MerchVol" } },
                { "Diams", new string[]{ "TaperId","Name","Value" } },
            };
            var dataframes = new Dictionary<string, DataTable>();
            connection.Open();
            foreach (var table in tables)
            {
                dataframes.Add(table.Key, GetDataTable(download.Id, table.Key, connection).SetColumnsOrder(table.Value));
            }
            connection.Close();
            var date = DateTime.Now.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "_");
            var file = $@"{wwwroot}/tmp/{date}-Entry_{download.Id}";
            List<string> files = new List<string>();
            if (download.Csv)
            {
                Csv(dataframes, file);
                foreach (var title in dataframes.Keys)
                {
                    files.Add($@"{file}-{title}.csv");
                }
            }
            if (download.Xml)
            {
                Xml(dataframes.Values.ToList(), file);
                files.Add($@"{file}.xml");
            }
            if (download.Xls || download.Xlsx)
            {
                Excel(dataframes, file, download.Xls, download.Xlsx);
                files.Add($@"{file}.xls");
            }
            CreateZipFile($@"{wwwroot}/data/{date}-Entry_{download.Id}", $@"{file}*", download.Password);

            var dlEmail = new DownloadEmail
            {
                Name = name,
                Id_ = download.Id,
                Date = date,
                BaseUrl = baseUrl,
                Password = download.Password
            };

            var renderer = new RazorViewToStringRenderer(viewEngine, tempDataProvider,serviceProvider);

            var result = renderer.RenderViewToString("DownloadEmail", dlEmail).GetAwaiter().GetResult();

            emailSender.SendEmailAsync(email, $@"FPFI Entry {download.Id} requested files", result);
        }

        public static string CreateZipFile(string fileName, string folder, string password)
        {
            var result = string.Empty;
            var platform = RTools.GetPlatform();
            if (platform == "Win32NT")
            {
                string strCmdLine = $@"/c ""C:\Program Files\7-Zip\7z.exe"" -p{password} a -tzip {fileName}.zip {folder} ";

                var info = new ProcessStartInfo("cmd", strCmdLine)
                {
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    //result = proc.StandardOutput.ReadToEnd();
                }
            }
            else if (platform == "Unix")
            {
                var info = new ProcessStartInfo()
                {
                    FileName = "zip",
                    Arguments = $@"--password {password} {fileName}.zip {folder} & disown",
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    //result = proc.StandardOutput.ReadToEnd();
                }
            }
            return result;
        }

        public static void Excel(Dictionary<string, DataTable> datas, string file, bool xls, bool xlsx)
        {
            //string xlsFile = $@"{file}.xlsx";
            //string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            //FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            //var fs = new FileStream(xlsFile, FileMode.Create, FileAccess.Write))

            var memory = new MemoryStream();

            using (ExcelPackage pck = new ExcelPackage())
            {
                var limit = 1048575;

                foreach (var data in datas)
                {
                    var Count = data.Value.Rows.Count;
                    for (var i = 1; i <= Math.Ceiling((double)Count / limit); i++)
                    {
                        var first = (i - 1) * limit;
                        var last = (i * limit > Count) ? Count : i * limit;
                        var tmpRows = data.Value.Rows.Cast<DataRow>()
                            .Skip(first)
                            .Take(last);
                        DataTable tmpDataTable = data.Value.Clone();
                        foreach (var row in tmpRows)
                        {
                            tmpDataTable.Rows.Add(row.ItemArray);
                        }
                        ExcelWorksheet Sheet = pck.Workbook.Worksheets.Add($"{data.Key}{i}");
                        Sheet.Cells["A1"].LoadFromDataTable(tmpDataTable, true);
                    }
                }
                //var simCount = datas["Simulations"].Rows.Count;
                //for (var i = 1; i <= Math.Ceiling((double)simCount / limit); i++)
                //{
                //    var first = (i - 1) * limit;
                //    var last = (i * limit > simCount) ? simCount : i * limit;
                //    var tmpRows = datas["Simulations"].Rows.Cast<DataRow>()
                //        .Skip(first)
                //        .Take(last);
                //    DataTable tmpDataTable = datas["Simulations"].Clone();
                //    foreach (var row in tmpRows)
                //    {
                //        tmpDataTable.Rows.Add(row.ItemArray);
                //    }
                //    ExcelWorksheet simsSheet = pck.Workbook.Worksheets.Add($"Simulations{i}");
                //    simsSheet.Cells["A1"].LoadFromDataTable(tmpDataTable, true);
                //}

                //var tapperCount = datas["Tapers"].Rows.Count;
                //for (var i = 1; i <= Math.Ceiling((double)tapperCount / limit); i++)
                //{
                //    var first = (i - 1) * limit;
                //    var last = (i * limit > tapperCount) ? tapperCount : i * limit;
                //    var tmpRows = datas["Tapers"].Rows.Cast<DataRow>()
                //        .Skip(first)
                //        .Take(last);
                //    DataTable tmpDataTable = datas["Tapers"].Clone();
                //    foreach (var row in tmpRows)
                //    {
                //        tmpDataTable.Rows.Add(row.ItemArray);
                //    }
                //    ExcelWorksheet tapersSheet = pck.Workbook.Worksheets.Add($"Tapers{i}");
                //    tapersSheet.Cells["A1"].LoadFromDataTable(tmpDataTable, true);
                //}

                //var diamCount = datas["Diameters"].Rows.Count;
                //for (var i = 1; i <= Math.Ceiling((double)diamCount / limit); i++)
                //{
                //    var first = (i - 1) * limit;
                //    var last = (i * limit > diamCount) ? diamCount : i * limit;
                //    var tmpRows = datas["Diameters"].Rows.Cast<DataRow>()
                //        .Skip(first)
                //        .Take(last);
                //    DataTable tmpDataTable = datas["Diameters"].Clone();
                //    foreach (var row in tmpRows)
                //    {
                //        tmpDataTable.Rows.Add(row.ItemArray);
                //    }
                //    ExcelWorksheet diamsSheet = pck.Workbook.Worksheets.Add($"Diams{i}");
                //    diamsSheet.Cells["A1"].LoadFromDataTable(tmpDataTable, true);
                //}
                if (xlsx)
                {
                    pck.SaveAs(new FileInfo($@"{file}.xlsx"));
                }
                if (xls)
                {
                    pck.SaveAs(new FileInfo($@"{file}.xls"));
                }
            }
            //using (var stream = new FileStream(xlsFile, FileMode.Open))
            //{
            //    stream.CopyTo(memory);
            //}
            //memory.Position = 0;
            //return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", xlsFile);
        }
    }
}
