using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentMonitor.Services
{
    // Benchmark Result Models
    public class BenchmarkResult
    {
        public DateTime TestDate { get; set; }
        public int CpuSingleCoreScore { get; set; }
        public int CpuMultiCoreScore { get; set; }
        public int MemoryScore { get; set; }
        public int DiskReadScore { get; set; }
        public int DiskWriteScore { get; set; }
        public int OverallScore { get; set; }
        public TimeSpan TotalDuration { get; set; }

        // Individual test durations
        public TimeSpan CpuSingleCoreDuration { get; set; }
        public TimeSpan CpuMultiCoreDuration { get; set; }
        public TimeSpan MemoryDuration { get; set; }
        public TimeSpan DiskDuration { get; set; }
    }

    public class BenchmarkProgress
    {
        public string CurrentTest { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class BenchmarkService
    {
        private const int CPU_ITERATIONS = 10000000;
        private const int MEMORY_SIZE_MB = 512;
        private const int DISK_TEST_SIZE_MB = 100;

        public event EventHandler<BenchmarkProgress>? ProgressChanged;

        private void ReportProgress(string test, int progress, string status)
        {
            ProgressChanged?.Invoke(this, new BenchmarkProgress
            {
                CurrentTest = test,
                Progress = progress,
                Status = status
            });
        }

        public async Task<BenchmarkResult> RunFullBenchmarkAsync(CancellationToken cancellationToken = default)
        {
            var result = new BenchmarkResult
            {
                TestDate = DateTime.Now
            };

            var totalStopwatch = Stopwatch.StartNew();

            try
            {
                // CPU Single Core Test
                ReportProgress("CPU 单核", 0, "正在测试 CPU 单核性能...");
                var cpuSingleStart = Stopwatch.StartNew();
                result.CpuSingleCoreScore = await Task.Run(() => RunCpuSingleCoreTest(cancellationToken), cancellationToken);
                cpuSingleStart.Stop();
                result.CpuSingleCoreDuration = cpuSingleStart.Elapsed;
                ReportProgress("CPU 单核", 25, $"单核得分: {result.CpuSingleCoreScore}");

                if (cancellationToken.IsCancellationRequested) return result;

                // CPU Multi Core Test
                ReportProgress("CPU 多核", 25, "正在测试 CPU 多核性能...");
                var cpuMultiStart = Stopwatch.StartNew();
                result.CpuMultiCoreScore = await Task.Run(() => RunCpuMultiCoreTest(cancellationToken), cancellationToken);
                cpuMultiStart.Stop();
                result.CpuMultiCoreDuration = cpuMultiStart.Elapsed;
                ReportProgress("CPU 多核", 50, $"多核得分: {result.CpuMultiCoreScore}");

                if (cancellationToken.IsCancellationRequested) return result;

                // Memory Test
                ReportProgress("内存", 50, "正在测试内存带宽...");
                var memoryStart = Stopwatch.StartNew();
                result.MemoryScore = await Task.Run(() => RunMemoryTest(cancellationToken), cancellationToken);
                memoryStart.Stop();
                result.MemoryDuration = memoryStart.Elapsed;
                ReportProgress("内存", 75, $"内存得分: {result.MemoryScore}");

                if (cancellationToken.IsCancellationRequested) return result;

                // Disk Test
                ReportProgress("磁盘", 75, "正在测试磁盘读写速度...");
                var diskStart = Stopwatch.StartNew();
                var diskScores = await Task.Run(() => RunDiskTest(cancellationToken), cancellationToken);
                result.DiskReadScore = diskScores.Item1;
                result.DiskWriteScore = diskScores.Item2;
                diskStart.Stop();
                result.DiskDuration = diskStart.Elapsed;
                ReportProgress("磁盘", 95, $"磁盘读取: {result.DiskReadScore}, 写入: {result.DiskWriteScore}");

                // Calculate Overall Score
                result.OverallScore = CalculateOverallScore(result);

                totalStopwatch.Stop();
                result.TotalDuration = totalStopwatch.Elapsed;

                ReportProgress("完成", 100, $"总分: {result.OverallScore}");
            }
            catch (OperationCanceledException)
            {
                ReportProgress("已取消", 0, "测试已被用户取消");
            }
            catch (Exception ex)
            {
                ReportProgress("错误", 0, $"测试出错: {ex.Message}");
            }

            return result;
        }

        private int RunCpuSingleCoreTest(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            // Prime number calculation (CPU intensive, single threaded)
            int primeCount = 0;
            for (int num = 2; num < CPU_ITERATIONS / 100; num++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                bool isPrime = true;
                int sqrt = (int)Math.Sqrt(num);
                for (int i = 2; i <= sqrt; i++)
                {
                    if (num % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime) primeCount++;
            }

            stopwatch.Stop();

            // Score based on operations per second (higher is better)
            // Base score around 1000, normalized
            double opsPerSecond = (CPU_ITERATIONS / 100.0) / stopwatch.Elapsed.TotalSeconds;
            int score = (int)(opsPerSecond / 10);

            return Math.Max(100, Math.Min(10000, score));
        }

        private int RunCpuMultiCoreTest(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            int coreCount = Environment.ProcessorCount;

            // Parallel prime calculation
            int totalPrimes = 0;
            var lockObj = new object();

            Parallel.For(0, coreCount, new ParallelOptions { CancellationToken = cancellationToken }, core =>
            {
                int localPrimes = 0;
                int start = core * (CPU_ITERATIONS / coreCount / 100);
                int end = (core + 1) * (CPU_ITERATIONS / coreCount / 100);

                for (int num = start; num < end; num++)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    bool isPrime = true;
                    int sqrt = (int)Math.Sqrt(num);
                    for (int i = 2; i <= sqrt; i++)
                    {
                        if (num % i == 0)
                        {
                            isPrime = false;
                            break;
                        }
                    }
                    if (isPrime) localPrimes++;
                }

                lock (lockObj)
                {
                    totalPrimes += localPrimes;
                }
            });

            stopwatch.Stop();

            // Multi-core score (should scale with cores)
            double opsPerSecond = (CPU_ITERATIONS / 100.0) / stopwatch.Elapsed.TotalSeconds;
            int score = (int)(opsPerSecond / 10);

            return Math.Max(100, Math.Min(50000, score));
        }

        private int RunMemoryTest(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            // Memory bandwidth test
            int arraySize = MEMORY_SIZE_MB * 1024 * 1024 / 8; // 8 bytes per long
            long[] array1 = new long[arraySize];
            long[] array2 = new long[arraySize];

            // Fill arrays
            for (int i = 0; i < arraySize; i++)
            {
                if (cancellationToken.IsCancellationRequested) break;
                array1[i] = i;
            }

            // Copy test (memory bandwidth)
            for (int iteration = 0; iteration < 5; iteration++)
            {
                if (cancellationToken.IsCancellationRequested) break;
                Array.Copy(array1, array2, arraySize);
            }

            stopwatch.Stop();

            // Score based on MB/s (higher is better)
            double totalMB = MEMORY_SIZE_MB * 5.0;
            double mbPerSecond = totalMB / stopwatch.Elapsed.TotalSeconds;
            int score = (int)(mbPerSecond * 10);

            return Math.Max(100, Math.Min(50000, score));
        }

        private Tuple<int, int> RunDiskTest(CancellationToken cancellationToken)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"benchmark_{Guid.NewGuid()}.tmp");
            int readScore = 0;
            int writeScore = 0;

            try
            {
                // Write Test
                var writeStopwatch = Stopwatch.StartNew();
                byte[] buffer = new byte[1024 * 1024]; // 1MB buffer
                new Random().NextBytes(buffer);

                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
                {
                    for (int i = 0; i < DISK_TEST_SIZE_MB; i++)
                    {
                        if (cancellationToken.IsCancellationRequested) break;
                        fs.Write(buffer, 0, buffer.Length);
                    }
                    fs.Flush();
                }
                writeStopwatch.Stop();

                double writeMBps = DISK_TEST_SIZE_MB / writeStopwatch.Elapsed.TotalSeconds;
                writeScore = (int)(writeMBps * 10);

                // Read Test
                var readStopwatch = Stopwatch.StartNew();
                using (var fs = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan))
                {
                    byte[] readBuffer = new byte[1024 * 1024];
                    while (fs.Read(readBuffer, 0, readBuffer.Length) > 0)
                    {
                        if (cancellationToken.IsCancellationRequested) break;
                    }
                }
                readStopwatch.Stop();

                double readMBps = DISK_TEST_SIZE_MB / readStopwatch.Elapsed.TotalSeconds;
                readScore = (int)(readMBps * 10);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Disk test error: {ex.Message}");
                readScore = 0;
                writeScore = 0;
            }
            finally
            {
                // Cleanup
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
                catch { }
            }

            return Tuple.Create(
                Math.Max(100, Math.Min(10000, readScore)),
                Math.Max(100, Math.Min(10000, writeScore))
            );
        }

        private int CalculateOverallScore(BenchmarkResult result)
        {
            // Weighted average
            // CPU: 40% (Single: 15%, Multi: 25%)
            // Memory: 30%
            // Disk: 30% (Read: 15%, Write: 15%)

            double score =
                result.CpuSingleCoreScore * 0.15 +
                result.CpuMultiCoreScore * 0.25 +
                result.MemoryScore * 0.30 +
                result.DiskReadScore * 0.15 +
                result.DiskWriteScore * 0.15;

            return (int)score;
        }

        public string GetPerformanceRating(int score)
        {
            if (score >= 8000) return "卓越";
            if (score >= 6000) return "优秀";
            if (score >= 4000) return "良好";
            if (score >= 2000) return "中等";
            if (score >= 1000) return "一般";
            return "较低";
        }

        public string GetScoreColor(int score)
        {
            if (score >= 8000) return "#10893E"; // Green
            if (score >= 6000) return "#0078D4"; // Blue
            if (score >= 4000) return "#00CC66"; // Light Green
            if (score >= 2000) return "#FF8C00"; // Orange
            if (score >= 1000) return "#E81123"; // Red
            return "#808080"; // Gray
        }
    }
}
