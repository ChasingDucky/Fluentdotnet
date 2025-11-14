using System;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace FluentMonitor.Services
{
    public class SystemInfo
    {
        public string CpuName { get; set; } = string.Empty;
        public int CpuCores { get; set; }
        public int CpuLogicalProcessors { get; set; }
        public double CpuBaseSpeed { get; set; }
        public double CpuMaxSpeed { get; set; }
        public long TotalMemory { get; set; }
        public string SystemUptime { get; set; } = string.Empty;
    }

    public class PerformanceData
    {
        public float CpuUsage { get; set; }
        public float MemoryUsage { get; set; }
        public long MemoryUsed { get; set; }
        public long MemoryAvailable { get; set; }
        public long MemoryCommitted { get; set; }
        public long MemoryCached { get; set; }
        public float DiskReadSpeed { get; set; }
        public float DiskWriteSpeed { get; set; }
        public float DiskActiveTime { get; set; }
        public float NetworkSentSpeed { get; set; }
        public float NetworkReceivedSpeed { get; set; }
        public int ProcessCount { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public List<float> CpuCoreUsages { get; set; } = new();
    }

    public class PerformanceMonitor : IDisposable
    {
        private PerformanceCounter? cpuCounter;
        private PerformanceCounter? ramCounter;
        private PerformanceCounter? diskReadCounter;
        private PerformanceCounter? diskWriteCounter;
        private PerformanceCounter? diskActiveTimeCounter;
        private PerformanceCounter? networkSentCounter;
        private PerformanceCounter? networkReceivedCounter;
        private PerformanceCounter? committedBytesCounter;
        private PerformanceCounter? cacheCounter;
        private PerformanceCounter? processCountCounter;
        private PerformanceCounter? threadCountCounter;
        private PerformanceCounter? handleCountCounter;

        private List<PerformanceCounter> cpuCoreCounters = new();
        private SystemInfo? systemInfo;
        private DateTime startTime;

        public PerformanceMonitor()
        {
            startTime = DateTime.Now;
            InitializeCounters();
        }

        private void InitializeCounters()
        {
            try
            {
                // CPU counters
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                // Initialize per-core CPU counters
                var processorCategory = new PerformanceCounterCategory("Processor");
                var instanceNames = processorCategory.GetInstanceNames();

                foreach (var instanceName in instanceNames)
                {
                    if (instanceName != "_Total")
                    {
                        try
                        {
                            var coreCounter = new PerformanceCounter("Processor", "% Processor Time", instanceName);
                            cpuCoreCounters.Add(coreCounter);
                        }
                        catch { }
                    }
                }

                // Memory counters
                ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                committedBytesCounter = new PerformanceCounter("Memory", "Committed Bytes");
                cacheCounter = new PerformanceCounter("Memory", "Cache Bytes");

                // Disk counters
                diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
                diskActiveTimeCounter = new PerformanceCounter("PhysicalDisk", "% Idle Time", "_Total");

                // Network counters
                var networkCategory = new PerformanceCounterCategory("Network Interface");
                var networkInstances = networkCategory.GetInstanceNames();

                if (networkInstances.Length > 0)
                {
                    var instanceName = networkInstances[0];
                    networkSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instanceName);
                    networkReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instanceName);
                }

                // System counters
                processCountCounter = new PerformanceCounter("System", "Processes");
                threadCountCounter = new PerformanceCounter("System", "Threads");
                handleCountCounter = new PerformanceCounter("Process", "Handle Count", "_Total");

                // Initialize counters (first call often returns 0)
                _ = cpuCounter?.NextValue();
                foreach (var counter in cpuCoreCounters)
                {
                    _ = counter.NextValue();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing performance counters: {ex.Message}");
            }
        }

        public async Task<SystemInfo> GetSystemInfoAsync()
        {
            if (systemInfo != null)
                return systemInfo;

            return await Task.Run(() =>
            {
                var info = new SystemInfo();

                try
                {
                    // CPU Information
                    using var cpuSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
                    foreach (var obj in cpuSearcher.Get().Cast<ManagementObject>())
                    {
                        info.CpuName = obj["Name"]?.ToString() ?? "Unknown CPU";
                        info.CpuCores = Convert.ToInt32(obj["NumberOfCores"]);
                        info.CpuLogicalProcessors = Convert.ToInt32(obj["NumberOfLogicalProcessors"]);
                        info.CpuMaxSpeed = Convert.ToDouble(obj["MaxClockSpeed"]) / 1000.0; // MHz to GHz

                        var currentSpeed = obj["CurrentClockSpeed"];
                        if (currentSpeed != null)
                        {
                            info.CpuBaseSpeed = Convert.ToDouble(currentSpeed) / 1000.0;
                        }
                        break;
                    }

                    // Memory Information
                    using var memSearcher = new ManagementObjectSearcher("select TotalVisibleMemorySize from Win32_OperatingSystem");
                    foreach (var obj in memSearcher.Get().Cast<ManagementObject>())
                    {
                        info.TotalMemory = Convert.ToInt64(obj["TotalVisibleMemorySize"]) * 1024;
                        break;
                    }

                    // System Uptime
                    using var osSearcher = new ManagementObjectSearcher("select LastBootUpTime from Win32_OperatingSystem");
                    foreach (var obj in osSearcher.Get().Cast<ManagementObject>())
                    {
                        var bootTime = ManagementDateTimeConverter.ToDateTime(obj["LastBootUpTime"].ToString() ?? string.Empty);
                        var uptime = DateTime.Now - bootTime;
                        info.SystemUptime = $"{uptime.Days}天 {uptime.Hours}:{uptime.Minutes:D2}:{uptime.Seconds:D2}";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting system info: {ex.Message}");
                }

                systemInfo = info;
                return info;
            });
        }

        public PerformanceData GetCurrentPerformance()
        {
            var data = new PerformanceData();

            try
            {
                // CPU
                data.CpuUsage = cpuCounter?.NextValue() ?? 0f;

                // CPU cores
                foreach (var coreCounter in cpuCoreCounters)
                {
                    try
                    {
                        data.CpuCoreUsages.Add(coreCounter.NextValue());
                    }
                    catch
                    {
                        data.CpuCoreUsages.Add(0f);
                    }
                }

                // Memory
                data.MemoryUsage = ramCounter?.NextValue() ?? 0f;
                data.MemoryCommitted = (long)(committedBytesCounter?.NextValue() ?? 0);
                data.MemoryCached = (long)(cacheCounter?.NextValue() ?? 0);

                // Get physical memory info
                using var searcher = new ManagementObjectSearcher("select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OperatingSystem");
                foreach (var obj in searcher.Get().Cast<ManagementObject>())
                {
                    var total = Convert.ToInt64(obj["TotalVisibleMemorySize"]) * 1024;
                    var free = Convert.ToInt64(obj["FreePhysicalMemory"]) * 1024;
                    data.MemoryAvailable = free;
                    data.MemoryUsed = total - free;
                    break;
                }

                // Disk
                data.DiskReadSpeed = (diskReadCounter?.NextValue() ?? 0) / 1024f / 1024f; // MB/s
                data.DiskWriteSpeed = (diskWriteCounter?.NextValue() ?? 0) / 1024f / 1024f; // MB/s
                var idleTime = diskActiveTimeCounter?.NextValue() ?? 100f;
                data.DiskActiveTime = 100f - idleTime; // Active time = 100 - idle time

                // Network
                data.NetworkSentSpeed = (networkSentCounter?.NextValue() ?? 0) / 1024f; // KB/s
                data.NetworkReceivedSpeed = (networkReceivedCounter?.NextValue() ?? 0) / 1024f; // KB/s

                // System
                data.ProcessCount = (int)(processCountCounter?.NextValue() ?? 0);
                data.ThreadCount = (int)(threadCountCounter?.NextValue() ?? 0);
                data.HandleCount = (int)(handleCountCounter?.NextValue() ?? 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting performance data: {ex.Message}");
            }

            return data;
        }

        public void Dispose()
        {
            cpuCounter?.Dispose();
            ramCounter?.Dispose();
            diskReadCounter?.Dispose();
            diskWriteCounter?.Dispose();
            diskActiveTimeCounter?.Dispose();
            networkSentCounter?.Dispose();
            networkReceivedCounter?.Dispose();
            committedBytesCounter?.Dispose();
            cacheCounter?.Dispose();
            processCountCounter?.Dispose();
            threadCountCounter?.Dispose();
            handleCountCounter?.Dispose();

            foreach (var counter in cpuCoreCounters)
            {
                counter?.Dispose();
            }
            cpuCoreCounters.Clear();
        }
    }
}
