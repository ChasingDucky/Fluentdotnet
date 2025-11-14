using System;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Linq;

namespace FluentMonitor.Services
{
    public class PerformanceMonitor
    {
        private PerformanceCounter? cpuCounter;
        private PerformanceCounter? ramCounter;
        private PerformanceCounter? diskReadCounter;
        private PerformanceCounter? diskWriteCounter;
        private PerformanceCounter? networkSentCounter;
        private PerformanceCounter? networkReceivedCounter;

        public PerformanceMonitor()
        {
            InitializeCounters();
        }

        private void InitializeCounters()
        {
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

                // Disk counters
                diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

                // Network counters - try to get first network interface
                var networkCategory = new PerformanceCounterCategory("Network Interface");
                var instanceNames = networkCategory.GetInstanceNames();

                if (instanceNames.Length > 0)
                {
                    var instanceName = instanceNames[0];
                    networkSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instanceName);
                    networkReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instanceName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing performance counters: {ex.Message}");
            }
        }

        public float GetCpuUsage()
        {
            try
            {
                return cpuCounter?.NextValue() ?? 0f;
            }
            catch
            {
                return 0f;
            }
        }

        public float GetMemoryUsage()
        {
            try
            {
                return ramCounter?.NextValue() ?? 0f;
            }
            catch
            {
                return 0f;
            }
        }

        public (float readSpeed, float writeSpeed) GetDiskActivity()
        {
            try
            {
                var read = diskReadCounter?.NextValue() ?? 0f;
                var write = diskWriteCounter?.NextValue() ?? 0f;
                return (read / 1024f / 1024f, write / 1024f / 1024f); // Convert to MB/s
            }
            catch
            {
                return (0f, 0f);
            }
        }

        public (float sent, float received) GetNetworkActivity()
        {
            try
            {
                var sent = networkSentCounter?.NextValue() ?? 0f;
                var received = networkReceivedCounter?.NextValue() ?? 0f;
                return (sent / 1024f, received / 1024f); // Convert to KB/s
            }
            catch
            {
                return (0f, 0f);
            }
        }

        public async Task<string> GetCpuName()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher("select Name from Win32_Processor");
                    foreach (var obj in searcher.Get())
                    {
                        return obj["Name"]?.ToString() ?? "Unknown CPU";
                    }
                }
                catch
                {
                    return "Unknown CPU";
                }
                return "Unknown CPU";
            });
        }

        public async Task<(long total, long available)> GetMemoryInfo()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher("select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OperatingSystem");
                    foreach (var obj in searcher.Get())
                    {
                        var total = Convert.ToInt64(obj["TotalVisibleMemorySize"]) * 1024;
                        var free = Convert.ToInt64(obj["FreePhysicalMemory"]) * 1024;
                        return (total, free);
                    }
                }
                catch
                {
                    return (0L, 0L);
                }
                return (0L, 0L);
            });
        }

        public void Dispose()
        {
            cpuCounter?.Dispose();
            ramCounter?.Dispose();
            diskReadCounter?.Dispose();
            diskWriteCounter?.Dispose();
            networkSentCounter?.Dispose();
            networkReceivedCounter?.Dispose();
        }
    }
}
