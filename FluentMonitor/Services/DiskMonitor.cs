using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FluentMonitor.Services
{
    public class DiskInfo
    {
        public string DriveName { get; set; } = string.Empty;
        public string VolumeLabel { get; set; } = string.Empty;
        public string DriveFormat { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public long UsedSpace { get; set; }
        public long FreeSpace { get; set; }
        public double UsagePercentage { get; set; }
        public string TotalSizeFormatted { get; set; } = string.Empty;
        public string UsedSpaceFormatted { get; set; } = string.Empty;
        public string FreeSpaceFormatted { get; set; } = string.Empty;
    }

    public class DiskMonitor
    {
        public List<DiskInfo> GetDiskInformation()
        {
            var diskInfoList = new List<DiskInfo>();

            try
            {
                var drives = DriveInfo.GetDrives();

                foreach (var drive in drives)
                {
                    try
                    {
                        // Only process ready drives (exclude CD-ROM, network drives that aren't ready, etc.)
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            var totalSize = drive.TotalSize;
                            var freeSpace = drive.AvailableFreeSpace;
                            var usedSpace = totalSize - freeSpace;
                            var usagePercentage = totalSize > 0 ? (double)usedSpace / totalSize * 100.0 : 0;

                            var diskInfo = new DiskInfo
                            {
                                DriveName = drive.Name,
                                VolumeLabel = string.IsNullOrEmpty(drive.VolumeLabel) ? "本地磁盘" : drive.VolumeLabel,
                                DriveFormat = drive.DriveFormat,
                                TotalSize = totalSize,
                                UsedSpace = usedSpace,
                                FreeSpace = freeSpace,
                                UsagePercentage = usagePercentage,
                                TotalSizeFormatted = FormatBytes(totalSize),
                                UsedSpaceFormatted = FormatBytes(usedSpace),
                                FreeSpaceFormatted = FormatBytes(freeSpace)
                            };

                            diskInfoList.Add(diskInfo);
                        }
                    }
                    catch
                    {
                        // Skip drives that can't be accessed
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting disk information: {ex.Message}");
            }

            return diskInfoList.OrderBy(d => d.DriveName).ToList();
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
