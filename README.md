# Fluent Monitor - Windows 系统性能监测器

<div align="center">

🖥️ 基于 Microsoft Fluent Design 设计的现代化 Windows 系统性能监测工具

</div>

## ✨ 特性

- 🎨 **现代化界面** - 采用 Microsoft Fluent Design System 设计语言
- 📊 **实时监测** - 实时监控 CPU、内存、磁盘和网络性能
- 📈 **动态图表** - 使用 LiveCharts 展示历史数据趋势
- 🌓 **深色模式** - 支持 Windows 11 风格的 Mica 材质和圆角窗口
- ⚡ **高性能** - 轻量级设计,低资源占用
- 🎯 **精准数据** - 使用 Windows Performance Counters 获取准确的系统信息

## 📸 截图

应用程序提供以下监测功能:

### 📊 实时性能卡片
- **CPU 使用率** - 显示处理器使用率和 CPU 型号
- **内存使用率** - 显示内存使用百分比和已用/总容量
- **磁盘活动** - 显示读写速度
- **网络活动** - 显示上传/下载速度

### 📈 历史数据图表
- CPU 使用率历史曲线
- 内存使用率历史曲线
- 磁盘读写活动趋势
- 网络上传/下载趋势

## 🚀 快速开始

### 系统要求

- Windows 10/11
- .NET 8.0 Runtime 或更高版本

### 编译项目

1. 克隆仓库:
```bash
git clone https://github.com/yourusername/Fluentdotnet.git
cd Fluentdotnet
```

2. 恢复 NuGet 包:
```bash
dotnet restore
```

3. 编译项目:
```bash
dotnet build
```

4. 运行应用:
```bash
dotnet run --project FluentMonitor
```

### 发布可执行文件

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 🏗️ 技术栈

- **框架**: .NET 8.0 WPF
- **UI 库**: [WPF-UI](https://github.com/lepoco/wpfui) - Fluent Design 组件库
- **图表库**: [LiveCharts2](https://livecharts.dev/) - 现代化图表库
- **性能监测**: System.Management、PerformanceCounters
- **硬件信息**: Hardware.Info

## 📦 主要依赖

```xml
<PackageReference Include="WPF-UI" Version="3.0.5" />
<PackageReference Include="LiveChartsCore.SkiaSharpView.WPF" Version="2.0.0-rc2" />
<PackageReference Include="System.Management" Version="8.0.0" />
<PackageReference Include="Hardware.Info" Version="100.1.0.1" />
```

## 🎯 功能模块

### PerformanceMonitor 服务
位于 `Services/PerformanceMonitor.cs`,负责:
- 初始化 Windows Performance Counters
- 获取 CPU 使用率
- 获取内存使用率和详细信息
- 监测磁盘读写速度
- 监测网络上传/下载速度
- 获取硬件信息(CPU 型号等)

### MainWindow
位于 `MainWindow.xaml` 和 `MainWindow.xaml.cs`,负责:
- 展示 Fluent Design 用户界面
- 实时更新性能数据
- 管理图表数据集合
- 处理数据点添加和限制

## 🎨 界面设计

应用使用了以下 Fluent Design 元素:

- **Mica 材质** - Windows 11 风格的半透明背景
- **圆角窗口** - 现代化的窗口边框
- **Fluent 图标** - 使用 Segoe Fluent Icons
- **卡片布局** - 信息卡片组织方式
- **动画效果** - 流畅的过渡动画
- **深色主题** - 默认深色配色方案

## 🔧 配置

### 修改刷新间隔
在 `MainWindow.xaml.cs` 中修改:
```csharp
_updateTimer = new DispatcherTimer
{
    Interval = TimeSpan.FromSeconds(1) // 修改此值
};
```

### 修改图表数据点数量
在 `MainWindow.xaml.cs` 中修改:
```csharp
private const int MaxDataPoints = 60; // 修改此值
```

## 📝 待办事项

- [ ] 添加 GPU 监测功能
- [ ] 添加温度监测
- [ ] 添加进程管理器
- [ ] 添加自启动功能
- [ ] 添加系统托盘支持
- [ ] 添加数据导出功能
- [ ] 添加自定义主题
- [ ] 添加多语言支持

## 🤝 贡献

欢迎提交 Issue 和 Pull Request!

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 🙏 鸣谢

- [WPF-UI](https://github.com/lepoco/wpfui) - 优秀的 Fluent Design WPF 组件库
- [LiveCharts2](https://livecharts.dev/) - 强大的图表库
- Microsoft Fluent Design System

---

<div align="center">

Made with ❤️ using Microsoft Fluent Design

</div>
