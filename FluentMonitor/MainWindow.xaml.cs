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
        private readonly DispatcherTimer _updateTimer;

        private readonly ObservableCollection<double> _cpuValues = new();
        private readonly ObservableCollection<double> _memoryValues = new();
        private readonly ObservableCollection<double> _diskReadValues = new();
        private readonly ObservableCollection<double> _diskWriteValues = new();
        private readonly ObservableCollection<double> _networkSentValues = new();
        private readonly ObservableCollection<double> _networkReceivedValues = new();

        private const int MaxDataPoints = 60;
        private long _totalMemory;
        private long _availableMemory;

        public MainWindow()
        {
            InitializeComponent();
            _performanceMonitor = new PerformanceMonitor();

            InitializeCharts();
            InitializeTimer();
            LoadSystemInfo();
        }

        private async void LoadSystemInfo()
        {
            var cpuName = await _performanceMonitor.GetCpuName();
            CpuNameText.Text = cpuName;

            var memInfo = await _performanceMonitor.GetMemoryInfo();
            _totalMemory = memInfo.total;
        }

        private void InitializeCharts()
        {
            // CPU Chart
            CpuChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _cpuValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 3 }
                }
            };

            CpuChart.YAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 100 } };

            // Memory Chart
            MemoryChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _memoryValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    Stroke = new SolidColorPaint(SKColors.MediumSeaGreen) { StrokeThickness = 3 }
                }
            };

            MemoryChart.YAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 100 } };

            // Disk Chart
            DiskChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name = "读取",
                    Values = _diskReadValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 }
                },
                new LineSeries<double>
                {
                    Name = "写入",
                    Values = _diskWriteValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    Stroke = new SolidColorPaint(SKColors.Purple) { StrokeThickness = 2 }
                }
            };

            // Network Chart
            NetworkChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name = "上传",
                    Values = _networkSentValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 }
                },
                new LineSeries<double>
                {
                    Name = "下载",
                    Values = _networkReceivedValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    Stroke = new SolidColorPaint(SKColors.LimeGreen) { StrokeThickness = 2 }
                }
            };
        }

        private void InitializeTimer()
        {
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _updateTimer.Tick += UpdatePerformanceData;
            _updateTimer.Start();
        }

        private async void UpdatePerformanceData(object? sender, EventArgs e)
        {
            // CPU
            var cpuUsage = _performanceMonitor.GetCpuUsage();
            CpuUsageText.Text = $"{cpuUsage:F1}%";
            AddDataPoint(_cpuValues, cpuUsage);

            // Memory
            var memoryUsage = _performanceMonitor.GetMemoryUsage();
            MemoryUsageText.Text = $"{memoryUsage:F1}%";

            var memInfo = await _performanceMonitor.GetMemoryInfo();
            var usedMemory = (memInfo.total - memInfo.available) / 1024.0 / 1024.0 / 1024.0;
            var totalMemory = memInfo.total / 1024.0 / 1024.0 / 1024.0;
            MemoryDetailsText.Text = $"{usedMemory:F1} GB / {totalMemory:F1} GB";
            AddDataPoint(_memoryValues, memoryUsage);

            // Disk
            var diskActivity = _performanceMonitor.GetDiskActivity();
            var totalDiskActivity = diskActivity.readSpeed + diskActivity.writeSpeed;
            DiskUsageText.Text = $"{totalDiskActivity:F1} MB/s";
            DiskDetailsText.Text = $"读: {diskActivity.readSpeed:F1} MB/s  写: {diskActivity.writeSpeed:F1} MB/s";
            AddDataPoint(_diskReadValues, diskActivity.readSpeed);
            AddDataPoint(_diskWriteValues, diskActivity.writeSpeed);

            // Network
            var networkActivity = _performanceMonitor.GetNetworkActivity();
            var totalNetworkActivity = networkActivity.sent + networkActivity.received;
            NetworkSpeedText.Text = $"{totalNetworkActivity:F0} KB/s";
            NetworkDetailsText.Text = $"↓ {networkActivity.received:F0} KB/s  ↑ {networkActivity.sent:F0} KB/s";
            AddDataPoint(_networkSentValues, networkActivity.sent);
            AddDataPoint(_networkReceivedValues, networkActivity.received);
        }

        private void AddDataPoint(ObservableCollection<double> collection, double value)
        {
            collection.Add(value);
            if (collection.Count > MaxDataPoints)
            {
                collection.RemoveAt(0);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _updateTimer.Stop();
            _performanceMonitor.Dispose();
            base.OnClosed(e);
        }
    }
}
