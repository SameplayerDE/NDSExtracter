using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ModelExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Process proc;
            Console.Write("Enter Path Of .NDS File: ");
            string ndsPath = Console.ReadLine();

            if (!File.Exists(ndsPath))
            {
                Console.Write("File Does Not Exist!");
                Console.Read();
                return;
            }
            
            Console.Write("Enter Path Where The Output Folder Should Be Created: ");
            string outputPath = Console.ReadLine();

            if (!Directory.Exists(outputPath))
            {
                Console.Write("Path Does Not Exist!");
                Console.Read();
                return;
            }
            
            if (Directory.Exists(outputPath + "\\output_assets"))
            {
                Directory.Delete(outputPath + "\\output_assets", true);
            }
            Directory.CreateDirectory(outputPath + "\\output_assets");
            string dir = outputPath + "\\output_nds";

            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            
            

            List<string> nsbmd = new List<string>(); //models
            List<string> nsbtx = new List<string>(); //textures
            List<string> nsbca = new List<string>(); //animations
            
            proc = System.Diagnostics.Process.Start("apicula",
                $"extract {ndsPath} -o {dir}");

            proc.WaitForExit(1000 * 60 * 5);
            
            string [] fileEntries = Directory.GetFiles(dir);
            foreach (string fileName in fileEntries)
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
                if (extension == ".nsbtx")
                {
                    if (!nsbtx.Contains(fileNameWoe))
                    {
                        //nsbtx.Add(fileNameWoe);
                    }
                }
                if (extension == ".nsbca")
                {
                    if (!nsbca.Contains(fileNameWoe))
                    {
                        nsbca.Add(fileNameWoe);
                    }
                }
            }

            foreach (var model in nsbmd)
            {
                var modelFile = model;
                var folder = model.Replace(dir, "");
                
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
                            $"convert -f=glb {model}.nsbmd {dir}\\*.nsbtx --output {outputPath}\\output_assets{folder}\\");
                    }
                }

                proc.WaitForExit();
            }
            
            //var proc = System.Diagnostics.Process.Start("apicula", "");

        }
    }
}