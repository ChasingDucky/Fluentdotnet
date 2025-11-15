using System;
using System.Windows;
using System.Windows.Input;

namespace FluentMonitor
{
    public partial class DesktopWidget : Window
    {
        private Point _dragStartPoint;
        private bool _isDragging = false;

        public DesktopWidget()
        {
            InitializeComponent();

            // Position at bottom right of screen
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Bottom - Height - 20;
        }

        public void UpdateData(float cpuUsage, float memoryUsage, float memoryUsedGB, float memoryTotalGB,
                              float diskSpeed, float networkSpeed)
        {
            Dispatcher.Invoke(() =>
            {
                WidgetCpuText.Text = $"{cpuUsage:F1}%";
                WidgetMemoryText.Text = $"{memoryUsage:F1}%";
                WidgetMemoryDetailText.Text = $"{memoryUsedGB:F1}/{memoryTotalGB:F1}GB";

                if (diskSpeed < 1)
                {
                    WidgetDiskText.Text = $"{diskSpeed * 1024:F0} KB/s";
                }
                else
                {
                    WidgetDiskText.Text = $"{diskSpeed:F1} MB/s";
                }

                if (networkSpeed < 1024)
                {
                    WidgetNetworkText.Text = $"{networkSpeed:F0} KB/s";
                }
                else
                {
                    WidgetNetworkText.Text = $"{networkSpeed / 1024:F1} MB/s";
                }

                UpdateTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
            });
        }

        private void Widget_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Double click to open main window
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.Activate();
                }
            }
            else
            {
                // Single click to drag
                _isDragging = true;
                _dragStartPoint = e.GetPosition(this);
                CaptureMouse();
            }
        }

        private void Widget_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Right click shows context menu
            var contextMenu = new System.Windows.Controls.ContextMenu();

            var openItem = new System.Windows.Controls.MenuItem { Header = "打开主窗口" };
            openItem.Click += (s, args) =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.Activate();
                }
            };

            var hideItem = new System.Windows.Controls.MenuItem { Header = "隐藏小部件" };
            hideItem.Click += (s, args) => Hide();

            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(new System.Windows.Controls.Separator());
            contextMenu.Items.Add(hideItem);

            contextMenu.IsOpen = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(this);
                var offset = currentPosition - _dragStartPoint;

                Left += offset.X;
                Top += offset.Y;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_isDragging)
            {
                _isDragging = false;
                ReleaseMouseCapture();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Don't actually close, just hide
            e.Cancel = true;
            Hide();
        }
    }
}
