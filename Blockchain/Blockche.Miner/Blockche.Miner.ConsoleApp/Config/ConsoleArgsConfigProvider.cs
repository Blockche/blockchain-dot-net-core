using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blockche.Miner.ConsoleApp.Config
{
    public class ConsoleArgsConfigProvider : IConfigProvider
    {
        public ConsoleArgsConfigProvider()
        {
            Console.WriteLine("Enter args:");

            var cmd = Console.ReadLine();

            var tokens = cmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "--threads" && i < tokens.Length - 1)
                {
                    ThreadsCount = int.Parse(tokens[i + 1]);
                }
                else if (tokens[i] == "--url" && i < tokens.Length - 1)
                {
                    JobProducerUrls = tokens[i + 1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.TrimEnd('/'));
                }
                else if (tokens[i] == "--address" && i < tokens.Length - 1)
                {
                    Address = tokens[i + 1];
                }
                else if (tokens[i] == "--pool")
                {
                    UsePool = true;
                }
                else if (tokens[i] == "--test")
                {
                    IsTest = true;
                }
            }

            Console.WriteLine($"ThreadsCount: {this.ThreadsCount}");
            Console.WriteLine($"JobProducerUrls: {string.Join(", ", this.JobProducerUrls)}");
            Console.WriteLine($"Address: {this.Address}");
            Console.WriteLine($"UsePool: {this.UsePool}");
            Console.WriteLine($"IsTest: {this.IsTest}");
        }

        public int ThreadsCount { get; } = 1;

        public IEnumerable<string> JobProducerUrls { get; } = new List<string>() { "http://localhost:59415" };

        public string Address { get; set; } = "0x0000000000000000000000000000000000000000";

        public bool UsePool { get; } = false;

        public bool IsTest { get; } = false;
    }
}
