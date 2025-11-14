using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using FluentMonitor.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace FluentMonitor
{
    public partial class MainWindow
    {
        private readonly PerformanceMonitor _performanceMonitor;
        private readonly ProcessMonitor _processMonitor;
        private readonly DispatcherTimer _performanceTimer;
        private readonly DispatcherTimer _processTimer;

        private readonly ObservableCollection<double> _cpuValues = new();
        private readonly ObservableCollection<double> _memoryValues = new();
        private readonly ObservableCollection<double> _diskReadValues = new();
        private readonly ObservableCollection<double> _diskWriteValues = new();
        private readonly ObservableCollection<double> _networkSentValues = new();
        private readonly ObservableCollection<double> _networkReceivedValues = new();

        private const int MaxDataPoints = 60; // 60 seconds of history
        private SystemInfo? _systemInfo;

        public MainWindow()
        {
            InitializeComponent();

            _performanceMonitor = new PerformanceMonitor();
            _processMonitor = new ProcessMonitor();

            InitializeCharts();
            InitializeTimers();
            LoadSystemInfo();
        }

        private async void LoadSystemInfo()
        {
            _systemInfo = await _performanceMonitor.GetSystemInfoAsync();

            if (_systemInfo != null)
            {
                // Update system info header
                SystemCpuName.Text = _systemInfo.CpuName;
                SystemTotalMemory.Text = FormatBytes(_systemInfo.TotalMemory);
                SystemUptime.Text = _systemInfo.SystemUptime;

                // Update CPU card details
                CpuSpeedText.Text = $"速度: {_systemInfo.CpuBaseSpeed:F2} GHz";
                CpuCoresText.Text = $"核心: {_systemInfo.CpuCores} / 逻辑: {_systemInfo.CpuLogicalProcessors}";
            }
        }

        private void InitializeCharts()
        {
            // CPU Chart - Blue gradient
            CpuChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _cpuValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3,
                    Stroke = new SolidColorPaint(new SKColor(0, 120, 215)) { StrokeThickness = 2 }
                }
            };
            CpuChart.YAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 100 } };

            // Memory Chart - Green gradient
            MemoryChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _memoryValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3,
                    Stroke = new SolidColorPaint(new SKColor(16, 137, 62)) { StrokeThickness = 2 }
                }
            };
            MemoryChart.YAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 100 } };

            // Disk Chart - Read (Orange) and Write (Purple)
            DiskChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name = "读取",
                    Values = _diskReadValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3,
                    Stroke = new SolidColorPaint(new SKColor(255, 140, 0)) { StrokeThickness = 2 }
                },
                new LineSeries<double>
                {
                    Name = "写入",
                    Values = _diskWriteValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3,
                    Stroke = new SolidColorPaint(new SKColor(138, 43, 226)) { StrokeThickness = 2 }
                }
            };

            // Network Chart - Sent (Red) and Received (Green)
            NetworkChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name = "发送",
                    Values = _networkSentValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3,
                    Stroke = new SolidColorPaint(new SKColor(232, 17, 35)) { StrokeThickness = 2 }
                },
                new LineSeries<double>
                {
                    Name = "接收",
                    Values = _networkReceivedValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3,
                    Stroke = new SolidColorPaint(new SKColor(0, 204, 106)) { StrokeThickness = 2 }
                }
            };
        }

        private void InitializeTimers()
        {
            // Performance data updates every 1 second
            _performanceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _performanceTimer.Tick += UpdatePerformanceData;
            _performanceTimer.Start();

            // Process list updates every 2 seconds (less frequent to avoid overhead)
            _processTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _processTimer.Tick += UpdateProcessList;
            _processTimer.Start();
        }

        private void UpdatePerformanceData(object? sender, EventArgs e)
        {
            var data = _performanceMonitor.GetCurrentPerformance();

            // Update system info
            SystemProcessInfo.Text = $"{data.ProcessCount} / {data.ThreadCount} / {data.HandleCount}";

            // Update CPU
            CpuUsageText.Text = $"{data.CpuUsage:F1}%";
            AddDataPoint(_cpuValues, data.CpuUsage);

            // Update Memory
            MemoryUsageText.Text = $"{data.MemoryUsage:F1}%";
            var usedMemoryGB = data.MemoryUsed / 1024.0 / 1024.0 / 1024.0;
            var totalMemoryGB = (data.MemoryUsed + data.MemoryAvailable) / 1024.0 / 1024.0 / 1024.0;
            var committedGB = data.MemoryCommitted / 1024.0 / 1024.0 / 1024.0;
            var cachedGB = data.MemoryCached / 1024.0 / 1024.0 / 1024.0;

            MemoryDetailsText.Text = $"已用: {usedMemoryGB:F1} GB / {totalMemoryGB:F1} GB";
            MemoryCommittedText.Text = $"已提交: {committedGB:F1} GB";
            MemoryCachedText.Text = $"已缓存: {cachedGB:F1} GB";
            AddDataPoint(_memoryValues, data.MemoryUsage);

            // Update Disk
            DiskActiveTimeText.Text = $"{data.DiskActiveTime:F0}%";
            DiskReadText.Text = $"读取: {data.DiskReadSpeed:F2} MB/s";
            DiskWriteText.Text = $"写入: {data.DiskWriteSpeed:F2} MB/s";
            AddDataPoint(_diskReadValues, data.DiskReadSpeed);
            AddDataPoint(_diskWriteValues, data.DiskWriteSpeed);

            // Update Network
            var totalNetwork = data.NetworkSentSpeed + data.NetworkReceivedSpeed;
            NetworkTotalText.Text = FormatSpeed(totalNetwork);
            NetworkSentText.Text = $"↑ 发送: {data.NetworkSentSpeed:F1} KB/s";
            NetworkReceivedText.Text = $"↓ 接收: {data.NetworkReceivedSpeed:F1} KB/s";
            AddDataPoint(_networkSentValues, data.NetworkSentSpeed);
            AddDataPoint(_networkReceivedValues, data.NetworkReceivedSpeed);

            // Update system uptime
            if (_systemInfo != null)
            {
                SystemUptime.Text = _systemInfo.SystemUptime;
            }
        }

        private void UpdateProcessList(object? sender, EventArgs e)
        {
            var processes = _processMonitor.GetTopProcesses(15);
            ProcessDataGrid.ItemsSource = processes;
        }

        private void AddDataPoint(ObservableCollection<double> collection, double value)
        {
            collection.Add(value);
            if (collection.Count > MaxDataPoints)
            {
                collection.RemoveAt(0);
            }
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

            return $"{len:F2} {sizes[order]}";
        }

        private string FormatSpeed(float kbps)
        {
            if (kbps < 1024)
            {
                return $"{kbps:F1} KB/s";
            }
            else if (kbps < 1024 * 1024)
            {
                return $"{kbps / 1024:F2} MB/s";
            }
            else
            {
                return $"{kbps / 1024 / 1024:F2} GB/s";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _performanceTimer.Stop();
            _processTimer.Stop();
            _performanceMonitor.Dispose();
            base.OnClosed(e);
        }
    }
}
