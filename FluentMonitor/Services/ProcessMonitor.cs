using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FluentMonitor.Services
{
    public class ProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public double CpuUsage { get; set; }
        public long MemoryUsage { get; set; }
        public string MemoryUsageFormatted { get; set; } = string.Empty;
        public int ThreadCount { get; set; }
    }

    public class ProcessMonitor
    {
        private Dictionary<int, (DateTime time, TimeSpan totalProcessorTime)> previousCpuUsage = new();
        private DateTime lastUpdateTime = DateTime.Now;

        public List<ProcessInfo> GetTopProcesses(int count = 10)
        {
            var currentTime = DateTime.Now;
            var processes = new List<ProcessInfo>();

            try
            {
                var allProcesses = Process.GetProcesses();

                foreach (var process in allProcesses)
                {
                    try
                    {
                        var processInfo = new ProcessInfo
                        {
                            ProcessId = process.Id,
                            ProcessName = process.ProcessName,
                            MemoryUsage = process.WorkingSet64,
                            MemoryUsageFormatted = FormatBytes(process.WorkingSet64),
                            ThreadCount = process.Threads.Count
                        };

                        // Calculate CPU usage
                        var currentTotalProcessorTime = process.TotalProcessorTime;

                        if (previousCpuUsage.TryGetValue(process.Id, out var previous))
                        {
                            var timeDiff = (currentTime - previous.time).TotalMilliseconds;
                            var cpuDiff = (currentTotalProcessorTime - previous.totalProcessorTime).TotalMilliseconds;

                            if (timeDiff > 0)
                            {
                                var cpuUsage = (cpuDiff / timeDiff) * 100.0 / Environment.ProcessorCount;
                                processInfo.CpuUsage = Math.Min(100, cpuUsage);
                            }
                        }

                        previousCpuUsage[process.Id] = (currentTime, currentTotalProcessorTime);
                        processes.Add(processInfo);
                    }
                    catch
                    {
                        // Skip processes we can't access
                    }
                }

                // Clean up old entries
                var currentProcessIds = new HashSet<int>(allProcesses.Select(p => p.Id));
                var keysToRemove = previousCpuUsage.Keys.Where(id => !currentProcessIds.Contains(id)).ToList();
                foreach (var key in keysToRemove)
                {
                    previousCpuUsage.Remove(key);
                }

                foreach (var process in allProcesses)
                {
                    process.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting process info: {ex.Message}");
            }

            lastUpdateTime = currentTime;

            // Return top processes by CPU usage
            return processes
                .OrderByDescending(p => p.CpuUsage)
                .ThenByDescending(p => p.MemoryUsage)
                .Take(count)
                .ToList();
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:F1} {sizes[order]}";
        }
    }
}
