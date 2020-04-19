using FPFI.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Services
{
    public class REngineRunner
    {
        public static void RunFromCmd(string batch, string platform, params string[] args)
        {
            // Not required. But our R scripts use allmost all CPU resources if run multiple instances
            //lock (typeof(REngineRunner))
            //{
            string file = string.Empty;
            string result = string.Empty;
            try
            {
                // Save R code to temp file
                file = TempFileHelper.CreateTmpFile();
                using (var streamWriter = new StreamWriter(new FileStream(file, FileMode.Open, FileAccess.Write)))
                {
                    streamWriter.Write(batch);
                }

                if (platform == "Win32NT")
                {
                    // Get path to R
                    var rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core") ??
                                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\R-core");
                    var is64Bit = Environment.Is64BitProcess;
                    if (rCore != null)
                    {
                        var rd = rCore.OpenSubKey(is64Bit ? "R64" : "R");
                        var r = rd.OpenSubKey("3.4.3");
                        var installPath = (string)r.GetValue("InstallPath");
                        var binPath = Path.Combine(installPath, "bin");
                        binPath = Path.Combine(binPath, is64Bit ? "x64" : "i386");
                        binPath = Path.Combine(binPath, "Rscript.exe");
                        string strCmdLine = @"/c """ + binPath + @""" " + file;
                        if (args.Any())
                        {
                            strCmdLine += " " + string.Join(" ", args);
                        }
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
                            System.Threading.Thread.Sleep(1000);
                            //result = proc.StandardOutput.ReadToEnd();
                        }
                    }
                    else
                    {
                        result += "R-Core not found in registry";
                    }
                    Console.WriteLine(result);
                }
                else if (platform == "Unix")
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
                        Rscript = getPath.StandardOutput.ReadToEnd();
                    }
                    var strCmdLine = Rscript+" "+file;
                    if (args.Any())
                    {
                        strCmdLine += " " + string.Join(" ", args);
                    }
                    var info = new ProcessStartInfo()
                    {
                        FileName = "Rscript",
                        Arguments = file + " & disown",
                        RedirectStandardInput = false,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (var proc = new Process())
                    {
                        proc.StartInfo = info;
                        proc.Start();
                        System.Threading.Thread.Sleep(1000);
                        //result = proc.StandardOutput.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("R failed to compute. Output: " + result, ex);
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    TempFileHelper.DeleteTmpFile(file, false);
                }
            }
            //}
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
                var fileInfo = new FileInfo(fileName);

                // Set the Attribute property of this file to Temporary. 
                // Although this is not completely necessary, the .NET Framework is able 
                // to optimize the use of Temporary files by keeping them cached in memory.
                fileInfo.Attributes = FileAttributes.Temporary;
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
