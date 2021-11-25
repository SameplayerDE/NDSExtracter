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
        private static Semaphore _pool;
        
        static List<string> nsbmd = new List<string>(); //models
        static List<string> nsbtx = new List<string>(); //textures
        static List<string> nsbca = new List<string>(); //animations
        static List<string> nsbtp = new List<string>(); //patterns
        static List<string> nsbta = new List<string>(); //materials
        
        private static Stopwatch _stopWatch = new Stopwatch();
        
        static void Main(string[] args)
        {
            Setup(out var inputFile, out var outputPath, out var forcedTexture, out var forcedAnimations, out var findAnimations, out var mode);
 
            _pool = new Semaphore(mode, mode);
            _stopWatch.Start();
            var extractingTask = ExtractFromNds(inputFile, outputPath);
            while (!extractingTask.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
            _stopWatch.Stop();
            Console.WriteLine($"\nDone Extracting File. [{_stopWatch.ElapsedMilliseconds}ms]");
            
            _stopWatch.Restart();
            CheckAllNdsFiles(outputPath);
            _stopWatch.Stop();
            Console.WriteLine($"\nDone Checking Files. [{_stopWatch.ElapsedMilliseconds}ms]");
            
            Console.WriteLine($"\nConverting {nsbmd.Count} Files...");
            var convertAllFilesTask = Task.Run(()=>ConvertAllModels(outputPath, 0, findAnimations, forcedTexture, forcedAnimations));
            Task.WaitAll(convertAllFilesTask);
            
            Console.WriteLine("\nPress Any Key To Close The Console.");
            Console.ReadKey();
            Process.Start("explorer.exe",outputPath);
            /*var convertAllFiles = Tasks.StartAndWaitAllThrottledAsync(listOfTasks, 3);
            while (!convertAllFiles.IsCompleted)
            {
                Console.WriteLine($"\nWaiting...");
                int count = 0;
                foreach (var task in listOfTasks)
                {
                    if (task.IsCompleted)
                    {
                        count++;
                    }
                }
                var percentComplete = (int)Math.Round((double)(100 * count) / nsbmd.Count);
                Console.Write("\r{0}                                               ", $"[{count}/{nsbmd.Count}] {percentComplete}%");
            }
            Console.WriteLine($"\nDone!");
            /*
            var convertTask = Task.Run(() =>
            {
                Task task1 = Task.CompletedTask;
                Task task2 = Task.CompletedTask;
                int tasksRunning = 0;

                
                    
                    
                    if (task1.IsCompleted)
                    {
                        proc2.Start();
                        task1 = proc2.WaitForExitAsync();
                    }
                    else
                    {
                        if (task2.IsCompleted)
                        {
                            proc2.Start();
                            task2 = proc2.WaitForExitAsync();
                        }
                        else
                        {
                            Task.WaitAny(task1, task2);
                        }
                    }
                    
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

        static void ConvertAllModels(string outputPath, int mode, bool findAnimations, bool forcedTextures, bool forcedAnimations)
        {
            _stopWatch.Restart();
            if (mode == 0) // sem
            {
                var listOfTasks = new List<Task>();
                foreach (var model in nsbmd)
                {
                    listOfTasks.Add(Task.Run(() => ConvertModelSem(model, outputPath, findAnimations, forcedTextures, forcedAnimations)));
                }

                var convertAllFiles = Task.WhenAll(listOfTasks);

                while (!convertAllFiles.IsCompleted)
                {
                    //Console.WriteLine($"\nWaiting...");
                    int count = 0;
                    foreach (var task in listOfTasks)
                    {
                        if (task.IsCompleted)
                        {
                            count++;
                        }
                    }

                    var percentComplete = (int)Math.Round((double)(100 * count) / listOfTasks.Count);
                    Console.Write("\r{0}                                               ",
                        $"[{count}/{listOfTasks.Count}] {percentComplete}%");
                }
                Console.Write("\r{0}                                               ",
                    $"[{listOfTasks.Count}/{listOfTasks.Count}] 100%");
            }
            else if (mode == 1) // async
            {
                int count = 0;
                foreach (var model in nsbmd)
                {
                    var task = ConvertModelAsync(model, outputPath);
                    Task.WaitAll(task);
                    count++;
                    var percentComplete = (int)Math.Round((double)(100 * count) / nsbmd.Count);
                    Console.Write("\r{0}                                               ",
                        $"[{count}/{nsbmd.Count}] {percentComplete}%");
                }
            }
            else if (mode == 2) // single
            {
                int count = 0;
                foreach (var model in nsbmd)
                {
                    ConvertModel(model, outputPath);
                    count++;
                }
            }
            _stopWatch.Stop();
            Console.WriteLine($"\nDone Converting Files. [{_stopWatch.ElapsedMilliseconds}ms]");
        }
        
        static void ConvertModel(string inputPath, string outputPath)
        {
            var fileName = Path.GetFileName(inputPath);
            var procConvert = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "apicula.exe",
                    Arguments =
                        $"convert -f=glb {inputPath}.nsbmd {outputPath}\\output_nds\\*.nsbtx --output {outputPath}\\output_assets\\{fileName}\\", //{outputPath + "\\output_nds"}\\*.nsbtp
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            procConvert.Start();
            procConvert.WaitForExit();
        }
        
        static async Task ConvertModelAsync(string inputPath, string outputPath)
        {
            var fileName = Path.GetFileName(inputPath);

            var procConvert = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "apicula.exe",
                    Arguments =
                        $"convert -f=glb {inputPath}.nsbmd {outputPath}\\output_nds\\*.nsbtx --output {outputPath}\\output_assets\\{fileName}\\", //{outputPath + "\\output_nds"}\\*.nsbtp
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            procConvert.Start();
            await procConvert.WaitForExitAsync();
        }
        
        static void ConvertModelSem(string inputPath, string outputPath, bool findAnimations, bool forcedTextures, bool forcedAnimations)
        {
            _pool.WaitOne();
            var fileName = Path.GetFileName(inputPath);

            var outputString =  $" --output {outputPath}\\output_assets\\{fileName}\\";
            var argument = $"convert -f=glb {inputPath}.nsbmd"; //{outputPath + "\\output_nds"}\\*.nsbtp

            if (forcedTextures)
            {
                argument += $" {outputPath}\\output_nds\\*.nsbtx";
            }
            
            if (findAnimations)
            {
                if (nsbca.Contains(inputPath))
                {
                    argument += $" {inputPath}.nsbca";
                }
            }
            else if (forcedAnimations)
            {
                argument += $" {outputPath}\\output_nds\\*.nsbca";
            }
            
            var procConvert = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "apicula.exe",
                    Arguments = argument + outputString,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            procConvert.Start();
            procConvert.WaitForExit();
            _pool.Release();
        }
        
        static void CheckAllNdsFiles(string inputPath)
        {
            var fileEntries = Directory.GetFiles(inputPath + "\\output_nds");
            var count = fileEntries.Length;
            var loop = 0;
            
            Console.WriteLine($"\nChecking {count} Files Now...");
            
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
            Console.Write("\nExtracting Files...");
            procExtract.Start();
            await procExtract.WaitForExitAsync();
        }

        static void Setup(out string inputPath, out string outputPath, out bool forcedTexture, out bool forcedAnimations, out bool findAnimations, out int mode)
        {
            Console.ForegroundColor = ConsoleColor.White;
            inputPath = AskForFilePath("Enter Path Of .NDS File", "nds");
            outputPath = AskForDirectoryPath("Enter Path Where The Output Folder Should Be Created");
            //AskForYesNoOption("Do You Want To Use The Forced Animation Mode");
            findAnimations = AskForYesNoOption("Do You Want To Use The Safe Animation Mode (Will Look For Animations That Have The Same Name As The Model, Recommended)");
            forcedAnimations = !findAnimations && AskForYesNoOption("Do You Want To Use The Forced Animation Mode (Takes Much Longer And Will Break Some Models, Not Recommended)");
            forcedTexture = AskForYesNoOption("Do You Want To Use The Forced Texture Mode (Takes Much Longer, Recommended)");
            mode = AskForIntOption("How Many Files Should Be Converted At Once (High Number Can Slow Down Your System)", 1, 10);
            
            _stopWatch.Restart();
            SetupOutputFolders(outputPath);
            _stopWatch.Stop();
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
        static int AskForIntOption(string message, int min, int max)
        {
            int number;
            var accept = false;
            do 
            {
                Console.Write($"{message} [{min}-{max}]: ");
                var input = Console.ReadLine();
                accept = int.TryParse(input, out number);
                if (!accept) continue;
                if (!(number > min && number < max))
                {
                    //TODO print error
                }
            } while (!accept || (number < min || number > max));
            Console.WriteLine();
            return number;
        }
        
        static void SetupOutputFolders(string outputPath)
        {
            Console.Write("\nSetting Up Directories...");
            SetupOutputAssetFolder(outputPath);
            SetupOutputNdsFolder(outputPath);
            Console.WriteLine($"\nDone Setting Up Directories. [{_stopWatch.ElapsedMilliseconds}ms]");
        }
        
        static void SetupOutputAssetFolder(string outputPath)
        {
            var directory = outputPath + "\\output_assets";
            //Console.WriteLine($"Checking If {directory} Exists...");
            if (Directory.Exists(directory))
            {
                //Console.WriteLine($"{directory} Exists!");
                //Console.Write($"Deleting {directory} ...");
                var deleteTask = Task.Run(() =>
                {
                    Directory.Delete(directory, true);
                });
                //Task.WaitAll(deleteTask);
                while (!deleteTask.IsCompleted)
                {
                    Console.Write(".");
                    Thread.Sleep(100);
                }
                
                //Console.WriteLine($"\nDeleted {directory}");
            }
            //Console.WriteLine($"Creating {directory} ...");
            Directory.CreateDirectory(directory);
            //Console.WriteLine($"Created {directory}");
        }
        
        static void SetupOutputNdsFolder(string outputPath)
        {
            var directory = outputPath + "\\output_nds";
            //Console.WriteLine($"Checking If {directory} Exists...");
            if (!Directory.Exists(directory)) return;
            //Console.WriteLine($"{directory} Exists!");
            //Console.Write($"Deleting {directory} ...");
            var deleteTask = Task.Run(() =>
            {
                Directory.Delete(directory, true);
            });
            //Task.WaitAll(deleteTask);
            while (!deleteTask.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            //Console.WriteLine($"\nDeleted {directory}");
        }

        static void WriteErrorLine(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
    }
}