using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Hardware.Info;

namespace FluentMonitor.Services
{
    // CPU Information
    public class CpuHardwareInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public uint NumberOfCores { get; set; }
        public uint NumberOfLogicalProcessors { get; set; }
        public uint MaxClockSpeed { get; set; } // MHz
        public uint CurrentClockSpeed { get; set; } // MHz
        public string Architecture { get; set; } = string.Empty;
        public uint L2CacheSize { get; set; } // KB
        public uint L3CacheSize { get; set; } // KB
        public string ProcessorId { get; set; } = string.Empty;
        public string SocketDesignation { get; set; } = string.Empty;
        public uint AddressWidth { get; set; }
    }

    // Memory Information
    public class MemoryHardwareInfo
    {
        public string Manufacturer { get; set; } = string.Empty;
        public ulong Capacity { get; set; } // Bytes
        public string CapacityFormatted { get; set; } = string.Empty;
        public uint Speed { get; set; } // MHz
        public string MemoryType { get; set; } = string.Empty;
        public string FormFactor { get; set; } = string.Empty;
        public string DeviceLocator { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
    }

    // Motherboard Information
    public class MotherboardInfo
    {
        public string Manufacturer { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
    }

    // BIOS Information
    public class BiosInfo
    {
        public string Manufacturer { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
    }

    // GPU Information
    public class GpuInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DriverVersion { get; set; } = string.Empty;
        public uint AdapterRAM { get; set; } // Bytes
        public string AdapterRAMFormatted { get; set; } = string.Empty;
        public string VideoProcessor { get; set; } = string.Empty;
        public uint CurrentRefreshRate { get; set; }
        public uint CurrentHorizontalResolution { get; set; }
        public uint CurrentVerticalResolution { get; set; }
    }

    // Storage Device Information
    public class StorageDeviceInfo
    {
        public string Model { get; set; } = string.Empty;
        public string InterfaceType { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty; // HDD or SSD
        public ulong Size { get; set; } // Bytes
        public string SizeFormatted { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string FirmwareRevision { get; set; } = string.Empty;
        public uint Partitions { get; set; }
    }

    // Network Adapter Information
    public class NetworkAdapterInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string MACAddress { get; set; } = string.Empty;
        public ulong Speed { get; set; } // bps
        public string SpeedFormatted { get; set; } = string.Empty;
        public string AdapterType { get; set; } = string.Empty;
        public bool NetEnabled { get; set; }
    }

    // Operating System Information
    public class OsInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string BuildNumber { get; set; } = string.Empty;
        public string Architecture { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string InstallDate { get; set; } = string.Empty;
        public string SystemDirectory { get; set; } = string.Empty;
    }

    public class HardwareInfoService : IDisposable
    {
        private readonly IHardwareInfo _hardwareInfo;
        private bool _isInitialized = false;

        public HardwareInfoService()
        {
            _hardwareInfo = new HardwareInfo();
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                try
                {
                    _hardwareInfo.RefreshAll();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hardware refresh error: {ex.Message}");
                }
            }
        }

        public List<CpuHardwareInfo> GetCpuInfo()
        {
            EnsureInitialized();
            var cpuList = new List<CpuHardwareInfo>();

            try
            {
                foreach (var cpu in _hardwareInfo.CpuList)
                {
                    cpuList.Add(new CpuHardwareInfo
                    {
                        Name = cpu.Name ?? "Unknown",
                        Manufacturer = cpu.Manufacturer ?? "Unknown",
                        NumberOfCores = cpu.NumberOfCores,
                        NumberOfLogicalProcessors = cpu.NumberOfLogicalProcessors,
                        MaxClockSpeed = cpu.MaxClockSpeed,
                        CurrentClockSpeed = cpu.CurrentClockSpeed,
                        Architecture = "x64", // Simplified for compatibility
                        L2CacheSize = cpu.L2CacheSize,
                        L3CacheSize = cpu.L3CacheSize,
                        ProcessorId = cpu.ProcessorId ?? "N/A",
                        SocketDesignation = cpu.SocketDesignation ?? "N/A",
                        AddressWidth = 64 // Default to 64-bit
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CPU info error: {ex.Message}");
            }

            return cpuList;
        }

        public List<MemoryHardwareInfo> GetMemoryInfo()
        {
            EnsureInitialized();
            var memoryList = new List<MemoryHardwareInfo>();

            try
            {
                foreach (var memory in _hardwareInfo.MemoryList)
                {
                    if (memory.Capacity > 0) // Only show populated slots
                    {
                        memoryList.Add(new MemoryHardwareInfo
                        {
                            Manufacturer = memory.Manufacturer ?? "Unknown",
                            Capacity = memory.Capacity,
                            CapacityFormatted = FormatBytes(memory.Capacity),
                            Speed = memory.Speed,
                            MemoryType = memory.FormFactor.ToString(),
                            FormFactor = memory.FormFactor.ToString(),
                            DeviceLocator = memory.BankLabel ?? "N/A",
                            PartNumber = memory.PartNumber ?? "N/A"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Memory info error: {ex.Message}");
            }

            return memoryList;
        }

        public MotherboardInfo? GetMotherboardInfo()
        {
            EnsureInitialized();

            try
            {
                var motherboard = _hardwareInfo.MotherboardList.FirstOrDefault();
                if (motherboard != null)
                {
                    return new MotherboardInfo
                    {
                        Manufacturer = motherboard.Manufacturer ?? "Unknown",
                        Product = motherboard.Product ?? "Unknown",
                        Version = "N/A", // Version property not available in current API
                        SerialNumber = motherboard.SerialNumber ?? "N/A"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Motherboard info error: {ex.Message}");
            }

            return null;
        }

        public BiosInfo? GetBiosInfo()
        {
            EnsureInitialized();

            try
            {
                var bios = _hardwareInfo.BiosList.FirstOrDefault();
                if (bios != null)
                {
                    return new BiosInfo
                    {
                        Manufacturer = bios.Manufacturer ?? "Unknown",
                        Name = bios.Name ?? "Unknown",
                        Version = bios.Version ?? "N/A",
                        ReleaseDate = bios.ReleaseDate ?? "N/A"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BIOS info error: {ex.Message}");
            }

            return null;
        }

        public List<GpuInfo> GetGpuInfo()
        {
            EnsureInitialized();
            var gpuList = new List<GpuInfo>();

            try
            {
                foreach (var gpu in _hardwareInfo.VideoControllerList)
                {
                    gpuList.Add(new GpuInfo
                    {
                        Name = gpu.Name ?? "Unknown",
                        DriverVersion = gpu.DriverVersion ?? "N/A",
                        AdapterRAM = (uint)Math.Min(gpu.AdapterRAM, uint.MaxValue),
                        AdapterRAMFormatted = FormatBytes(gpu.AdapterRAM),
                        VideoProcessor = gpu.VideoProcessor ?? "N/A",
                        CurrentRefreshRate = gpu.CurrentRefreshRate,
                        CurrentHorizontalResolution = gpu.CurrentHorizontalResolution,
                        CurrentVerticalResolution = gpu.CurrentVerticalResolution
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GPU info error: {ex.Message}");
            }

            return gpuList;
        }

        public List<StorageDeviceInfo> GetStorageDevices()
        {
            EnsureInitialized();
            var storageList = new List<StorageDeviceInfo>();

            try
            {
                foreach (var drive in _hardwareInfo.DriveList)
                {
                    storageList.Add(new StorageDeviceInfo
                    {
                        Model = drive.Model ?? "Unknown",
                        InterfaceType = "Unknown", // InterfaceType property not available in current API
                        MediaType = "Unknown", // MediaType property not available in current API
                        Size = drive.Size,
                        SizeFormatted = FormatBytes(drive.Size),
                        SerialNumber = drive.SerialNumber ?? "N/A",
                        FirmwareRevision = drive.FirmwareRevision ?? "N/A",
                        Partitions = drive.Partitions
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Storage info error: {ex.Message}");
            }

            return storageList;
        }

        public List<NetworkAdapterInfo> GetNetworkAdapters()
        {
            EnsureInitialized();
            var networkList = new List<NetworkAdapterInfo>();

            try
            {
                foreach (var adapter in _hardwareInfo.NetworkAdapterList)
                {
                    // Skip adapters without MAC address
                    if (!string.IsNullOrEmpty(adapter.MACAddress))
                    {
                        networkList.Add(new NetworkAdapterInfo
                        {
                            Name = adapter.Name ?? "Unknown",
                            Manufacturer = adapter.Manufacturer ?? "Unknown",
                            MACAddress = adapter.MACAddress,
                            Speed = adapter.Speed,
                            SpeedFormatted = FormatNetworkSpeed(adapter.Speed),
                            AdapterType = adapter.AdapterType ?? "Unknown",
                            NetEnabled = true // NetEnabled property not available in current API
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network info error: {ex.Message}");
            }

            return networkList;
        }

        public OsInfo? GetOperatingSystemInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                var os = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

                if (os != null)
                {
                    var installDate = os["InstallDate"]?.ToString();
                    DateTime? parsedDate = null;

                    if (!string.IsNullOrEmpty(installDate))
                    {
                        try
                        {
                            parsedDate = ManagementDateTimeConverter.ToDateTime(installDate);
                        }
                        catch { }
                    }

                    return new OsInfo
                    {
                        Name = os["Caption"]?.ToString() ?? "Unknown",
                        Version = os["Version"]?.ToString() ?? "N/A",
                        BuildNumber = os["BuildNumber"]?.ToString() ?? "N/A",
                        Architecture = os["OSArchitecture"]?.ToString() ?? "N/A",
                        Manufacturer = os["Manufacturer"]?.ToString() ?? "Unknown",
                        InstallDate = parsedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A",
                        SystemDirectory = os["SystemDirectory"]?.ToString() ?? "N/A"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OS info error: {ex.Message}");
            }

            return null;
        }

        private string GetArchitectureName(ushort architecture)
        {
            return architecture switch
            {
                0 => "x86",
                1 => "MIPS",
                2 => "Alpha",
                3 => "PowerPC",
                5 => "ARM",
                6 => "ia64",
                9 => "x64",
                12 => "ARM64",
                _ => "Unknown"
            };
        }

        private string GetMemoryTypeName(ushort formFactor)
        {
            return formFactor switch
            {
                8 => "DIMM",
                12 => "SODIMM",
                13 => "RIMM",
                _ => "Unknown"
            };
        }

        private string FormatBytes(ulong bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:F2} {sizes[order]}";
        }

        private string FormatNetworkSpeed(ulong bps)
        {
            if (bps == 0) return "Unknown";

            double mbps = bps / 1000000.0;
            double gbps = bps / 1000000000.0;

            if (gbps >= 1)
                return $"{gbps:F1} Gbps";
            else
                return $"{mbps:F0} Mbps";
        }

        public void Dispose()
        {
            // Hardware.Info doesn't require explicit disposal
        }
    }
}
