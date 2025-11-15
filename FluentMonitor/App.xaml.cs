using System.Windows;
using System;
using System.Windows.Threading;
using System.IO;

namespace FluentMonitor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 捕获未处理的异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
            MessageBox.Show($"应用程序发生错误：\n\n{e.Exception.Message}\n\n详细信息已记录到 error.log",
                "Fluent Monitor - 错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogException(exception);
                MessageBox.Show($"应用程序发生严重错误：\n\n{exception.Message}\n\n详细信息已记录到 error.log",
                    "Fluent Monitor - 严重错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogException(Exception ex)
        {
            try
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                var message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{ex}\n\n";
                File.AppendAllText(logPath, message);
            }
            catch
            {
                // 忽略日志写入错误
            }
        }
    }
}
