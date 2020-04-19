using FPFI.Data;
using FPFI.Hubs;
using FPFI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FPFI.Services
{
    public class REngineRunner
    {
        public static void RunFromCmd(string batch, string rootPath, int id,
            int ver, ApplicationDbContext context, IHubContext<EntryHub> hubContext)
        {
            // Not required. But our R scripts use allmost all CPU resources if run multiple instances
            //lock (typeof(REngineRunner))
            //{
            var platform = RTools.GetPlatform();
            // Save R code to temp file
            //var args = $"> {rootPath}/Logs/_{ver}{id}.cshtml 2>&1";

            var scriptPath = Path.GetTempFileName();
            var scriptFile = File.Create(scriptPath);
            var scriptWriter = new StreamWriter(scriptFile);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { batch = batch.Replace("\r\n", "\n"); }
            scriptWriter.WriteLine(batch);
            scriptWriter.Dispose();

            var result = new List<string>();

            try
            {
                //using (var streamWriter = new StreamWriter(new FileStream(file, FileMode.Open, FileAccess.Write)))
                //{
                //    streamWriter.NewLine = Environment.NewLine;
                //    streamWriter.Write(batch);
                //}

                //var log = Path.Combine(rootPath, "Views","Logs", $"_{ver}{id}.cshtml");
                //using (var streamWriter = new StreamWriter(log))
                //{
                //    streamWriter.WriteLine($"Entry: {id} FPFI version {ver}");
                //    streamWriter.Close();
                //}

                var pgr = 30;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Get path to R
                    var rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core") ??
                                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\R-core");
                    var is64Bit = Environment.Is64BitProcess;
                    if (rCore != null)
                    {
                        var rd = rCore.OpenSubKey(is64Bit ? "R64" : "R");
                        var installPath = (string)rd.GetValue("InstallPath");
                        var RPath = Path.Combine(installPath, "bin");
                        RPath = Path.Combine(RPath, is64Bit ? "x64" : "i386");
                        RPath = Path.Combine(RPath, "Rscript.exe");
                        string strCmdLine = $@"/c ""{RPath}"" {scriptPath}";

                        var info = new ProcessStartInfo("cmd", strCmdLine)
                        {
                            FileName = "cmd",
                            Arguments = strCmdLine,
                            RedirectStandardInput = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using (var proc = new Process())
                        {
                            //using (StreamWriter output = new StreamWriter(log))
                            //{
                            //    output.WriteLine($"Entry: {id} FPFI version {ver}");
                                proc.StartInfo = info;

                            proc.Start();

                            proc.OutputDataReceived += (sender, e) =>
                                {
                                    if (e.Data != null)
                                    {
                                        if (e.Data == "upgrade")
                                        {
                                            pgr++;
                                            hubContext
                                                .Clients.All
                                                .SendAsync("Update", "progress", pgr);
                                        }
                                        else if (e.Data.Contains("stage"))
                                        {
                                            var stg = e.Data.Split(" ");
                                            var html = $@"Stage <span class=""stageNumber"">{stg[1]}</span> of 7:<br/>{stg[2]}";
                                            hubContext
                                                .Clients.All
                                                .SendAsync("Update", "stage", html);
                                        }
                                        else
                                        {
                                            hubContext
                                                .Clients.All
                                                .SendAsync("Update", "log", e.Data);
                                            result.Add(e.Data);
                                        }
                                    }
                                };

                            proc.BeginOutputReadLine();

                            proc.ErrorDataReceived += (sender, e) =>
                                {
                                    if (e.Data != null)
                                    {
                                        result.Add(e.Data);
                                    }
                                };

                            proc.BeginErrorReadLine();

                            hubContext
                                .Clients.All
                                .SendAsync("Update", "progress", pgr);

                            //proc.BeginOutputReadLine();
                            //proc.BeginErrorReadLine();
                            proc.WaitForExit();
                            //}
                            proc.Close();

                        }
                    }
                    else
                    {
                        result.Add("R-Core not found in registry");
                    }
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string Rscript = string.Empty;
                    var pathInfo = new ProcessStartInfo()
                    {
                        FileName = "/bin/bash",
                        Arguments = "which Rscript",
                        RedirectStandardInput = false,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (var getPath = new Process())
                    {
                        getPath.StartInfo = pathInfo;
                        getPath.Start();
                        Rscript = getPath.StandardOutput.ReadToEnd().TrimEnd( Environment.NewLine.ToCharArray() );
                    }

                    var info = new ProcessStartInfo()
                    {
                        FileName = Rscript,
                        Arguments = $"{scriptPath}",
                        RedirectStandardInput = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (var proc = new Process())
                    {
                        //using (StreamWriter output = new StreamWriter(log))
                        //{
                            //output.WriteLine($"Entry: {id} FPFI version {ver}");
                            proc.StartInfo = info;

                        proc.Start();

                        proc.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data != null)
                            {
                                if (e.Data == "upgrade")
                                {
                                    pgr++;
                                    hubContext
                                        .Clients.All
                                        .SendAsync("Update", "progress", pgr);
                                }
                                else if (e.Data.Contains("stage"))
                                {
                                    var stg = e.Data.Split(" ");
                                    var html = $@"Stage <span class=""stageNumber"">{stg[1]}</span> of 7:<br/>{stg[2]}";
                                    hubContext
                                        .Clients.All
                                        .SendAsync("Update", "stage", html);
                                }
                                else
                                {
                                    hubContext
                                        .Clients.All
                                        .SendAsync("Update", "log", e.Data);
                                    result.Add(e.Data);
                                }
                            }
                        };

                        proc.BeginOutputReadLine();

                                proc.ErrorDataReceived += (sender, e) =>
                                {
                                    result.Add(e.Data);
                                };

                        proc.BeginErrorReadLine();

                        hubContext
                            .Clients.All
                            .SendAsync("Update", "progress", pgr);

                        proc.WaitForExit();

                        proc.Close();
                        //}
                    }
                }

                var entry = context.Find<Entry3>(id);

                entry.Output = string.Join(Environment.NewLine, result);

                context.Entry3.Update(entry);

                context.SaveChanges();

                hubContext
                    .Clients.All
                    .SendAsync("Update", "complete", true);
            }
            catch (Exception ex)
            {
                throw new Exception("R failed to compute. Output: " + result, ex);
            }
        }
    }

    public class TempFileHelper
    {
        public static string CreateTmpFile(bool throwException = true)
        {
            string fileName = string.Empty;

            try
            {
                // Get the full name of the newly created Temporary file. 
                // Note that the GetTempFileName() method actually creates
                // a 0-byte file and returns the name of the created file.
                fileName = Path.GetTempFileName();

                // Craete a FileInfo object to set the file's attributes
                var fileInfo = new FileInfo(fileName)
                {
                    // Set the Attribute property of this file to Temporary. 
                    // Although this is not completely necessary, the .NET Framework is able 
                    // to optimize the use of Temporary files by keeping them cached in memory.
                    Attributes = FileAttributes.Temporary
                };
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new Exception("Unable to create TEMP file or set its attributes: " + ex.Message, ex);
                }
            }

            return fileName;
        }

        public static void DeleteTmpFile(string tmpFile, bool throwException = true)
        {
            try
            {
                // Delete the temp file (if it exists)
                if (File.Exists(tmpFile))
                {
                    File.Delete(tmpFile);
                }
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new Exception("Error deleteing TEMP file: " + ex.Message, ex);
                }
            }
        }
    }
}
