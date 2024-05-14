using System;
using System.Diagnostics;
using System.Threading;

namespace ProcessMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: ProcessMonitor.exe [process name] [max lifetime in minutes] [monitor frequency in minutes]");
                return;
            }

            string processName = args[0];
            if (!int.TryParse(args[1], out int maxLifetime) || !int.TryParse(args[2], out int monitorFrequency))
            {
                Console.WriteLine("Invalid arguments. Please ensure that max lifetime and monitor frequency are integers.");
                return;
            }

            Console.WriteLine($"Monitoring {processName} every {monitorFrequency} minute(s). Will kill if running longer than {maxLifetime} minute(s).");
            Console.WriteLine("Press 'q' to quit.");

            Timer timer = new Timer((e) =>
            {
                CheckAndKillProcess(processName, maxLifetime);
            }, null, 0, monitorFrequency * 60000);

            while (Console.Read() != 'q')
            {
                Thread.Sleep(100);
            }

            timer.Dispose();
        }

        static void CheckAndKillProcess(string processName, int maxLifetime)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                TimeSpan runtime = DateTime.Now - process.StartTime;
                if (runtime.TotalMinutes > maxLifetime)
                {
                    Console.WriteLine($"Process {process.ProcessName} (ID: {process.Id}) has been running for {runtime.TotalMinutes} minutes, killing now.");
                    process.Kill();
                }
            }
        }
    }
}
