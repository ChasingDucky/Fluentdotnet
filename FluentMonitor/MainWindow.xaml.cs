using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FluentMonitor.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Wpf.Ui.Appearance;

namespace FluentMonitor
{
    public partial class MainWindow
    {
        private readonly PerformanceMonitor _performanceMonitor;
        private readonly ProcessMonitor _processMonitor;
        private readonly DiskMonitor _diskMonitor;
        private readonly TrayIconService _trayIconService;
        private readonly DispatcherTimer _performanceTimer;
        private readonly DispatcherTimer _processTimer;
        private readonly DispatcherTimer _diskTimer;

        private readonly ObservableCollection<double> _cpuValues = new();
        private readonly ObservableCollection<double> _memoryValues = new();
        private readonly ObservableCollection<double> _diskReadValues = new();
        private readonly ObservableCollection<double> _diskWriteValues = new();
        private readonly ObservableCollection<double> _networkSentValues = new();
        private readonly ObservableCollection<double> _networkReceivedValues = new();

        private DesktopWidget? _desktopWidget;
        private const int MaxDataPoints = 60; // 60 seconds of history
        private SystemInfo? _systemInfo;
        private bool _isClosing = false;

        // Latest performance data for widget and tray
        private float _latestCpuUsage = 0;
        private float _latestMemoryUsage = 0;
        private float _latestMemoryUsedGB = 0;
        private float _latestMemoryTotalGB = 0;
        private float _latestDiskSpeed = 0;
        private float _latestNetworkSpeed = 0;

        public MainWindow()
        {
            InitializeComponent();

            _performanceMonitor = new PerformanceMonitor();
            _processMonitor = new ProcessMonitor();
            _diskMonitor = new DiskMonitor();
            _trayIconService = new TrayIconService();

            InitializeCharts();
            InitializeTimers();
            LoadSystemInfo();
            LoadDiskInfo();
            InitializeTheme();
        }

        private void InitializeTheme()
        {
            // Set default theme to Dark
            ThemeComboBox.SelectedIndex = 0;
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
                _latestMemoryTotalGB = _systemInfo.TotalMemory / 1024f / 1024f / 1024f;

                // Update CPU card details
                CpuSpeedText.Text = $"速度: {_systemInfo.CpuBaseSpeed:F2} GHz";
                CpuCoresText.Text = $"核心: {_systemInfo.CpuCores} / 逻辑: {_systemInfo.CpuLogicalProcessors}";
            }
        }

        private void LoadDiskInfo()
        {
            var diskInfo = _diskMonitor.GetDiskInformation();
            DiskItemsControl.ItemsSource = diskInfo;
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

            // Disk info updates every 5 seconds
            _diskTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _diskTimer.Tick += (s, e) => LoadDiskInfo();
            _diskTimer.Start();
        }

        private void UpdatePerformanceData(object? sender, EventArgs e)
        {
            var data = _performanceMonitor.GetCurrentPerformance();

            // Update system info
            SystemProcessInfo.Text = $"{data.ProcessCount} / {data.ThreadCount} / {data.HandleCount}";

            // Update CPU
            _latestCpuUsage = data.CpuUsage;
            CpuUsageText.Text = $"{data.CpuUsage:F1}%";
            AddDataPoint(_cpuValues, data.CpuUsage);

            // Update Memory
            _latestMemoryUsage = data.MemoryUsage;
            var usedMemoryGB = data.MemoryUsed / 1024.0 / 1024.0 / 1024.0;
            var totalMemoryGB = (data.MemoryUsed + data.MemoryAvailable) / 1024.0 / 1024.0 / 1024.0;
            var committedGB = data.MemoryCommitted / 1024.0 / 1024.0 / 1024.0;
            var cachedGB = data.MemoryCached / 1024.0 / 1024.0 / 1024.0;

            _latestMemoryUsedGB = (float)usedMemoryGB;
            _latestMemoryTotalGB = (float)totalMemoryGB;

            MemoryUsageText.Text = $"{data.MemoryUsage:F1}%";
            MemoryDetailsText.Text = $"已用: {usedMemoryGB:F1} GB / {totalMemoryGB:F1} GB";
            MemoryCommittedText.Text = $"已提交: {committedGB:F1} GB";
            MemoryCachedText.Text = $"已缓存: {cachedGB:F1} GB";
            AddDataPoint(_memoryValues, data.MemoryUsage);

            // Update Disk
            _latestDiskSpeed = data.DiskReadSpeed + data.DiskWriteSpeed;
            DiskActiveTimeText.Text = $"{data.DiskActiveTime:F0}%";
            DiskReadText.Text = $"读取: {data.DiskReadSpeed:F2} MB/s";
            DiskWriteText.Text = $"写入: {data.DiskWriteSpeed:F2} MB/s";
            AddDataPoint(_diskReadValues, data.DiskReadSpeed);
            AddDataPoint(_diskWriteValues, data.DiskWriteSpeed);

            // Update Network
            var totalNetwork = data.NetworkSentSpeed + data.NetworkReceivedSpeed;
            _latestNetworkSpeed = totalNetwork;
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

            // Update widget if visible
            if (_desktopWidget != null && _desktopWidget.IsVisible)
            {
                _desktopWidget.UpdateData(_latestCpuUsage, _latestMemoryUsage,
                    _latestMemoryUsedGB, _latestMemoryTotalGB,
                    _latestDiskSpeed, _latestNetworkSpeed);
            }

            // Update smart tray icon
            if (SmartTrayIconToggle.IsChecked == true)
            {
                try
                {
                    TrayIcon.IconSource = _trayIconService.GenerateCpuIcon(_latestCpuUsage);
                    TrayIcon.ToolTipText = _trayIconService.GenerateTooltipText(
                        _latestCpuUsage, _latestMemoryUsage, _latestDiskSpeed, _latestNetworkSpeed);
                }
                catch
                {
                    // Ignore tray icon update errors
                }
            }
            else
            {
                TrayIcon.ToolTipText = $"Fluent Monitor\n双击打开主窗口";
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

        // Desktop Widget Events
        private void ShowWidgetToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_desktopWidget == null)
            {
                _desktopWidget = new DesktopWidget();
            }

            _desktopWidget.Show();
        }

        private void ShowWidgetToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _desktopWidget?.Hide();
        }

        // System Tray Events
        private void Window_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && MinimizeToTrayToggle.IsChecked == true)
            {
                Hide();
                TrayIcon.Visibility = Visibility.Visible;
            }
        }

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _isClosing = true;
            Application.Current.Shutdown();
        }

        // Process Management
        private void EndProcess_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessDataGrid.SelectedItem is ProcessInfo processInfo)
            {
                var result = MessageBox.Show(
                    $"确定要结束进程 \"{processInfo.ProcessName}\" (PID: {processInfo.ProcessId}) 吗?\n\n警告：结束系统进程可能导致系统不稳定。",
                    "确认结束进程",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var process = Process.GetProcessById(processInfo.ProcessId);
                        process.Kill();
                        MessageBox.Show($"进程 \"{processInfo.ProcessName}\" 已成功结束。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法结束进程: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Theme Management
        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem item)
            {
                var theme = item.Tag?.ToString();
                if (theme == "Dark")
                {
                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                }
                else if (theme == "Light")
                {
                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_isClosing && MinimizeToTrayToggle.IsChecked == true)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
            }
            else
            {
                TrayIcon.Dispose();
                _desktopWidget?.Close();
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _performanceTimer.Stop();
            _processTimer.Stop();
            _diskTimer.Stop();
            _performanceMonitor.Dispose();
            base.OnClosed(e);
        }
    }
}
