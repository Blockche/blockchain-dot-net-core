using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Blockche.Blockchain.ConsoleApp
{
    public class NodeInstanceRunner
    {
        //https://stackoverflow.com/questions/15234448/run-shell-commands-using-c-sharp-and-get-the-info-into-string

      

        public static void StartMultipleInstances(string[] urls)
        {

            List<Process> runningProcesses = new List<Process>();
            foreach (var url in urls)
            {
                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"../Blockche.Blockchain.Web/bin/Release/netcoreapp2.1/win10-x64/Blockche.Blockchain.Web.exe",
                        Arguments = "--urls " + url,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                runningProcesses.Add(proc);

                proc.Start();

                Console.WriteLine("Node running at:" +url);

                //while (!proc.StandardOutput.EndOfStream)
                //{
                //    string line = proc.StandardOutput.ReadLine();
                //    // do something with line
                //}
            }

            
           
        }
    }
}
