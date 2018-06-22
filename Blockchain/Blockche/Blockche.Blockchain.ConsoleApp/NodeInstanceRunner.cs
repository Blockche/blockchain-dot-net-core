using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

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

                Console.WriteLine("Node running at:" + url);

                //while (!proc.StandardOutput.EndOfStream)
                //{
                //    string line = proc.StandardOutput.ReadLine();
                //    // do something with line
                //}
            }



        }


        public static void ConnectBetweenInstances(string[] urls,string explorerUrl)
        {
            var doesExplorerResponds = WebRequester.Ping(explorerUrl);
            if (doesExplorerResponds)
            {
                ConnectPeers(explorerUrl, urls[0]);
            }

            //it's intentionally up to urls.Length-1
            for (int i = 0; i < urls.Length-1; i++) 
            {
                ConnectPeers(urls[i], urls[i + 1]);
            }
        }

        private static void ConnectPeers( string urlFrom, string urlTo)
        {
            try
            {
                Console.WriteLine("Connecting {0} to {1} and vise versa", urlFrom, urlTo);
                //prevent recursive callings
               WebRequester.Post(urlFrom + "/api/Node/peers/connect", new Peer() { NodeUrl = urlTo, IsRecursive = false });
                //WebRequester.Post(urlFrom + "/api/Node/peers/connect", new Peer() { NodeUrl = urlTo ,IsRecursive=true});
                //Thread.Sleep(2000);
                //WebRequester.Post(urlTo + "/api/Node/peers/connect", new Peer() { NodeUrl = urlFrom, IsRecursive = true });
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}
