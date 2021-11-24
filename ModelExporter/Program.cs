using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModelExporter
{
    class Program
    {
        
        static List<string> nsbmd = new List<string>(); //models
        static List<string> nsbtx = new List<string>(); //textures
        static List<string> nsbca = new List<string>(); //animations
        static List<string> nsbtp = new List<string>(); //patterns
        static List<string> nsbta = new List<string>(); //materials
        
        static void Main(string[] args)
        {
            Setup(out var inputFile, out var outputPath, out var forced);
            var extractingTask = ExtractFromNds(inputFile, outputPath);
            while (!extractingTask.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine("\nDone Extracting File.");
            CheckAllNdsFiles(outputPath);
            Console.WriteLine($"\nConverting {nsbmd.Count} Files...");

            int loop = 0;
            
            var convertTask = Task.Run(() =>
            {
                foreach (var model in nsbmd)
                {
                    var modelFile = model;
                    var folder = model.Replace(outputPath + "\\output_nds", "");

                    var proc2 = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "apicula.exe",
                            Arguments =
                                $"convert -f=glb {model}.nsbmd {outputPath + "\\output_nds"}\\*.nsbtx {outputPath + "\\output_nds"}\\*.nsbtp --output {outputPath}\\output_assets{folder}\\",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    proc2.Start();
                    proc2.WaitForExit();
                    loop++;
                }
            });

            while (!convertTask.IsCompleted)
            {
                var percentComplete = (int)Math.Round((double)(100 * loop) / nsbmd.Count);
                Console.Write("\r{0}                                               ", $"[{loop}/{nsbmd.Count}] {percentComplete}%");
            }
            Console.Write("\r{0}                                               ", $"[{nsbmd.Count}/{nsbmd.Count}] 100%");
            Console.WriteLine($"\nDone!");


                /*

            foreach (var texture in nsbtx)
            {
                string name = "";
                string foundin = "";
                
                var info = new Process 
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "apicula.exe",
                        Arguments = $"info {texture}.nsbtx",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                info.Start();
                while (!info.StandardOutput.EndOfStream)
                {
                    string line = info.StandardOutput.ReadLine();
                    line = line.Replace(" ", "");
                    if (line.StartsWith("Name:"))
                    {
                        name = line.Remove(0, 5);
                    }
                    if (line.StartsWith("FoundIn:"))
                    {
                        foundin = line.Remove(0, 8);
                    }
                }
                Console.WriteLine(name + " >> " + foundin);
            }

            foreach (var model in nsbmd)
            {
                continue;
                var modelFile = model;
                var folder = model.Replace(dir, "");
                
                var proc2 = new Process 
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "apicula.exe",
                        Arguments = $"convert -f=glb {model}.nsbmd {dir}\\*.nsbtx {dir}\\*.nsbtp --output {outputPath}\\output_assets{folder}\\",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
            
                proc2.Start();
                while (!proc2.StandardOutput.EndOfStream)
                {
                    string line = proc2.StandardOutput.ReadLine();
                    line = line.Replace(" ", "");
                    if (line.Contains("Objects"))
                    {
                        Console.WriteLine(line);
                    }
                }
                continue;
                if (forced)
                {
                    proc = System.Diagnostics.Process.Start("apicula",
                        $"convert -f=glb {model}.nsbmd {dir}\\*.nsbtx {dir}\\*.nsbtp {dir}\\*.nsbca --output {outputPath}\\output_assets{folder}\\");
                }
                else
                {

                    if (nsbtx.Contains(model))
                    {
                        if (nsbca.Contains(model))
                        {
                            //Console.WriteLine($"convert -f=glb {model}.nsbmd {model}.nsbtx --output {dir}\\ModelExporter{folder}\\");
                            proc = System.Diagnostics.Process.Start("apicula",
                                $"convert -f=glb {model}.nsbmd {model}.nsbtx {model}.nsbca --output {outputPath}\\output_assets{folder}\\");
                        }
                        else
                        {
                            //Console.WriteLine($"convert -f=glb {model}.nsbmd {model}.nsbtx --output {dir}\\ModelExporter{folder}\\");
                            proc = System.Diagnostics.Process.Start("apicula",
                                $"convert -f=glb {model}.nsbmd {model}.nsbtx --output {outputPath}\\output_assets{folder}\\");
                        }
                    }
                    else
                    {
                        if (nsbca.Contains(model))
                        {
                            //Console.WriteLine($"convert -f=glb {model}.nsbmd {model}.nsbtx --output {dir}\\ModelExporter{folder}\\");
                            proc = System.Diagnostics.Process.Start("apicula",
                                $"convert -f=glb {model}.nsbmd {dir}\\*.nsbtx {model}.nsbca --output {outputPath}\\output_assets{folder}\\");
                        }
                        else
                        {
                            proc = System.Diagnostics.Process.Start("apicula",
                                $"convert -f=glb {model}.nsbmd {dir}\\*.nsbtx {dir}\\*.nsbtp --output {outputPath}\\output_assets{folder}\\");
                        }
                    }
                }

                proc.WaitForExit();
            }
            
            //var proc = System.Diagnostics.Process.Start("apicula", "");
*/
        }

        static void CheckAllNdsFiles(string inputPath)
        {
            var fileEntries = Directory.GetFiles(inputPath + "\\output_nds");
            var count = fileEntries.Length;
            var loop = 0;
            
            Console.WriteLine($"Checking {count} Files Now...");
            
            var task = Task.Run(() =>
            {
                foreach (var fileName in fileEntries)
                {
                    var extension = Path.GetExtension(fileName);
                    var fileNameWoe = fileName.Replace(extension, "");

                    if (extension == ".nsbmd")
                    {
                        if (!nsbmd.Contains(fileNameWoe))
                        {
                            nsbmd.Add(fileNameWoe);
                        }
                    }
                    else if (extension == ".nsbtx")
                    {
                        if (!nsbtx.Contains(fileNameWoe))
                        {
                            nsbtx.Add(fileNameWoe);
                        }
                    }
                    else if (extension == ".nsbca")
                    {
                        if (!nsbca.Contains(fileNameWoe))
                        {
                            nsbca.Add(fileNameWoe);
                        }
                    }
                    else if (extension == ".nsbtp")
                    {
                        if (!nsbtp.Contains(fileNameWoe))
                        {
                            nsbtp.Add(fileNameWoe);
                        }
                    }
                    else if (extension == ".nsbta")
                    {
                        if (!nsbta.Contains(fileNameWoe))
                        {
                            nsbta.Add(fileNameWoe);
                        }
                    }

                    loop++;
                }
            });
            
            while (!task.IsCompleted)
            {
                var percentComplete = (int)Math.Round((double)(100 * loop) / count);
                Console.Write("\r{0}                                               ", $"[{loop}/{count}] {percentComplete}%");
            }
            Console.Write("\r{0}                                               ", $"[{count}/{count}] 100%");
            Console.WriteLine($"\nDone!");
        }
        
        static async Task ExtractFromNds(string inputPath, string outputPath)
        {
            var procExtract = new Process 
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "apicula.exe",
                    Arguments = $"extract {inputPath} -o {outputPath}\\output_nds",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            Console.Write("Extracting Files...");
            procExtract.Start();
            await procExtract.WaitForExitAsync();
        }

        static void Setup(out string inputPath, out string outputPath, out bool forced)
        {
            Console.ForegroundColor = ConsoleColor.White;
            inputPath = AskForFilePath("Enter Path Of .NDS File", "nds");
            outputPath = AskForDirectoryPath("Enter Path Where The Output Folder Should Be Created");
            SetupOutputFolders(outputPath);
            //AskForYesNoOption("Do You Want To Use The Forced Animation Mode");
            forced = AskForYesNoOption("Do You Want To Use The Forced Mode (Used Anyway)");
        }

        static string AskForFilePath(string message, string extension, string errorNotFound = "File Not Found, Try Again.", string errorExtension = "File Has The Wrong Extension, Try Another File.")
        {
            Console.Write(message + ": ");
            var filePath = Console.ReadLine();

            var accept = false;

            while (!accept)
            {
                if (!File.Exists(filePath))
                {
                    WriteErrorLine(errorNotFound);
                    filePath = Console.ReadLine();
                }
                else
                {
                    if (Path.HasExtension(filePath))
                    {
                        if (Path.GetExtension(filePath) != "." + extension)
                        {
                            WriteErrorLine(errorExtension);
                            filePath = Console.ReadLine();
                        }
                        else
                        {
                            accept = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine(errorNotFound);
                        filePath = Console.ReadLine();
                    }
                }
            }
            return filePath;
        }
        static string AskForDirectoryPath(string message, string error = "Path Not Found, Try Again.")
        {
            Console.Write(message + ": ");
            var directoryPath = Console.ReadLine();

            while (!Directory.Exists(directoryPath))
            {
                WriteErrorLine(error);
                directoryPath = Console.ReadLine();
            }

            return directoryPath;
        }
        static bool AskForYesNoOption(string message)
        {
            ConsoleKey  response;
            
            do 
            {
                Console.Write(message + " [Y/N]: ");
                response = Console.ReadKey(false).Key;
            } while (response != ConsoleKey.Y && response != ConsoleKey.N);
            Console.WriteLine();
            return response == ConsoleKey.Y;
        }
        
        static void SetupOutputFolders(string outputPath)
        {
            SetupOutputAssetFolder(outputPath);
            SetupOutputNdsFolder(outputPath);
        }
        
        static void SetupOutputAssetFolder(string outputPath)
        {
            var directory = outputPath + "\\output_assets";
            Console.WriteLine($"Checking If {directory} Exists...");
            if (Directory.Exists(directory))
            {
                Console.WriteLine($"{directory} Exists!");
                Console.Write($"Deleting {directory} ...");
                var deleteTask = Task.Run(() =>
                {
                    Directory.Delete(directory, true);
                });
                while (!deleteTask.IsCompleted)
                {
                    Console.Write(".");
                    Thread.Sleep(100);
                }
                
                Console.WriteLine($"\nDeleted {directory}");
            }
            Console.WriteLine($"Creating {directory} ...");
            Directory.CreateDirectory(directory);
            Console.WriteLine($"Created {directory}");
        }
        
        static void SetupOutputNdsFolder(string outputPath)
        {
            var directory = outputPath + "\\output_nds";
            Console.WriteLine($"Checking If {directory} Exists...");
            if (!Directory.Exists(directory)) return;
            Console.WriteLine($"{directory} Exists!");
            Console.Write($"Deleting {directory} ...");
            var deleteTask = Task.Run(() =>
            {
                Directory.Delete(directory, true);
            });
            while (!deleteTask.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.WriteLine($"\nDeleted {directory}");
        }

        static void WriteErrorLine(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
    }
}