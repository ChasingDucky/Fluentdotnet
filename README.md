# Fluent Monitor - Windows 系统性能监测器

<div align="center">

🖥️ 基于 Microsoft Fluent Design 设计的现代化 Windows 系统性能监测工具

</div>

## ✨ 特性

- 🎨 **现代化界面** - 采用 Microsoft Fluent Design System 设计语言
- 📊 **实时监测** - 持续实时监控 CPU、内存、磁盘和网络性能（1秒刷新）
- 📈 **动态图表** - 使用 LiveCharts 展示 60 秒历史数据趋势
- 🌓 **主题切换** - 支持深色/浅色主题，Windows 11 风格的 Mica 材质和圆角窗口
- ⚡ **高性能** - 轻量级设计，低资源占用
- 🎯 **精准数据** - 使用 Windows Performance Counters 获取准确的系统信息
- 🔍 **进程管理** - 实时显示资源占用最高的进程，支持右键结束进程
- 💾 **磁盘监控** - 显示所有磁盘分区的详细使用情况
- 🔔 **系统托盘** - 支持最小化到系统托盘，后台持续监控
- 📋 **详细指标** - 对标 Windows 任务管理器的专业级监测面板
- 🖥️ **多核监测** - 支持多核 CPU 的独立监测
- 📊 **系统信息** - 显示 CPU 型号、频率、核心数、内存、运行时间等
- 🪟 **桌面悬浮小部件** - 透明悬浮窗口实时显示关键性能指标，可拖动定位
- 🎯 **智能托盘图标** - 托盘图标动态显示 CPU 使用率，支持颜色预警（蓝色/橙色/红色）
- 🔧 **专业硬件分析** - 详细展示 CPU、内存、主板、BIOS、GPU、存储、网络等完整硬件规格，Geek 专属
- 🏆 **性能跑分测试** - 综合测试 CPU、内存、磁盘性能，获取专业评分，对标 Geekbench/Cinebench
- 💡 **智能升级建议** - 自动评估硬件年代，提供设备估值、回收价格、维修/换新建议，帮助用户做出明智的升级决策

## 📸 功能概览

应用程序提供以下监测功能:

### 🎯 性能概览页面

#### 系统信息栏
- **处理器信息** - CPU 型号、核心数、逻辑处理器数、运行频率
- **系统内存** - 总内存容量
- **系统运行时间** - 自上次启动以来的运行时间
- **进程/线程/句柄** - 系统当前的进程、线程和句柄总数

#### 实时性能卡片
- **CPU 使用率** - 显示总体 CPU 使用率、运行速度、核心/逻辑处理器数
- **内存使用率** - 显示内存使用百分比、已用/总容量、已提交内存、缓存内存
- **磁盘活动** - 显示磁盘活动时间百分比、读取速度、写入速度
- **网络活动** - 显示总网络速度、上传速度、下载速度

#### 历史数据图表（60秒滚动）
- **CPU 使用率历史** - 蓝色曲线展示 CPU 使用率变化
- **内存使用率历史** - 绿色曲线展示内存使用率变化
- **磁盘读写活动** - 橙色（读取）和紫色（写入）双曲线
- **网络上传/下载** - 红色（发送）和绿色（接收）双曲线

### 🔧 硬件信息页面
- **智能设备评估** - 自动检测硬件年代（全新/较新/中等/较旧/很旧/过时），针对老旧硬件显示升级建议面板
- **设备估值与回收** - 根据硬件型号和年代提供市场估值和回收价格参考
- **维修与换新对比** - 显示官方售后和第三方维修的成本对比（维修费用 vs 换新费用）
- **升级推荐方案** - 智能推荐适合的升级型号、预估升级成本、性能提升幅度
- **处理器详细信息** - CPU 型号、制造商、架构、核心/线程数、频率、L2/L3 缓存、处理器 ID、插槽
- **内存详细信息** - 每条内存的制造商、型号、容量、频率、类型（DIMM/SODIMM）、插槽位置
- **主板信息** - 主板制造商、型号、版本、序列号
- **BIOS 信息** - BIOS 制造商、版本、发布日期
- **显卡详细信息** - GPU 型号、视频处理器、显存容量、驱动版本、分辨率、刷新率
- **存储设备信息** - 所有硬盘/SSD 的型号、接口类型、介质类型、容量、固件版本、序列号、分区数
- **网络适配器信息** - 网卡型号、制造商、速度、MAC 地址、适配器类型
- **操作系统信息** - Windows 版本、内部版本号、架构、安装日期、系统目录

### 🔍 进程监控页面
- **实时进程列表** - 显示资源占用最高的前 15 个进程
- **进程详细信息** - 进程 ID、进程名称、CPU 使用率、内存占用、线程数
- **自动排序** - 按 CPU 使用率和内存占用自动排序
- **进程管理** - 右键菜单结束进程（附带安全确认）
- **2秒刷新** - 每 2 秒更新一次进程列表

### 💾 磁盘监控页面
- **分区列表** - 显示所有固定磁盘分区
- **使用率可视化** - 进度条直观展示磁盘使用情况
- **详细信息** - 卷标、驱动器号、文件系统、总容量、已用空间、可用空间、使用率百分比
- **5秒刷新** - 每 5 秒更新一次磁盘信息

### 🏆 性能跑分测试页面
- **CPU 单核测试** - 素数计算测试单线程性能，评估单核心计算能力
- **CPU 多核测试** - 并行素数计算测试所有核心协同工作能力
- **内存带宽测试** - 大数组拷贝测试内存读写速度和带宽性能
- **磁盘读写测试** - 100MB 顺序读写测试系统盘性能
- **综合评分系统** - 加权计算总分（CPU单核 15% + CPU多核 25% + 内存 30% + 磁盘读 15% + 磁盘写 15%）
- **性能等级评定** - 自动评定性能等级（卓越/优秀/良好/中等/一般/较低）
- **实时进度显示** - 显示当前测试项目、进度条、状态信息
- **详细结果展示** - 分项得分、耗时统计、颜色编码
- **可中断测试** - 支持随时停止测试

### ⚙️ 设置页面
- **主题切换** - 在深色和浅色主题之间切换
- **桌面悬浮小部件** - 开关桌面性能监控悬浮窗
- **智能托盘图标** - 启用/禁用托盘图标动态 CPU 显示
- **系统托盘选项** - 配置最小化到系统托盘行为
- **应用信息** - 显示版本号和许可证信息

### 🪟 桌面悬浮小部件
- **透明悬浮窗** - 280x320 半透明深色卡片，始终置顶
- **实时性能显示** - CPU、内存、磁盘、网络 4 项关键指标
- **可拖动定位** - 鼠标左键拖动调整位置，默认定位在屏幕右下角
- **快捷操作** - 双击打开主窗口，右键显示菜单，点击 × 关闭
- **详细信息** - 内存显示已用/总量 GB，磁盘和网络自动单位换算
- **颜色编码** - CPU 蓝色、内存绿色、磁盘橙色、网络红色

### 🎯 智能托盘图标
- **动态 CPU 显示** - 16x16 托盘图标实时显示 CPU 使用率百分比
- **颜色预警** - 蓝色 (<50%)、橙色 (50-80%)、红色 (>80%)
- **详细提示信息** - 鼠标悬停显示 CPU、内存、磁盘、网络完整数据
- **自动更新** - 每秒刷新托盘图标和提示文本
- **可配置** - 在设置页面开关智能托盘图标功能

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
- **UI 库**: [WPF-UI](https://github.com/lepoco/wpfui) 3.0.5 - Fluent Design 组件库
- **图表库**: [LiveCharts2](https://livecharts.dev/) - 现代化图表库（SkiaSharp 渲染）
- **性能监测**: System.Management 8.0.0、PerformanceCounters
- **硬件信息**: Hardware.Info 100.1.0.1
- **系统托盘**: Hardcodet.NotifyIcon.Wpf 1.1.0
- **图像处理**: System.Drawing.Common 8.0.0

## 📦 主要依赖

```xml
<PackageReference Include="WPF-UI" Version="3.0.5" />
<PackageReference Include="LiveChartsCore.SkiaSharpView.WPF" Version="2.0.0-rc2" />
<PackageReference Include="System.Management" Version="8.0.0" />
<PackageReference Include="Hardware.Info" Version="100.1.0.1" />
<PackageReference Include="Hardcodet.NotifyIcon.Wpf" Version="1.1.0" />
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
```

## 🎯 功能模块

### PerformanceMonitor 服务
位于 `FluentMonitor/Services/PerformanceMonitor.cs`,负责:
- 初始化 Windows Performance Counters（CPU、内存、磁盘、网络）
- 获取总体 CPU 使用率和多核 CPU 独立使用率
- 获取详细内存信息（使用率、已用、可用、已提交、缓存）
- 监测磁盘读写速度和活动时间百分比
- 监测网络上传/下载速度
- 获取系统信息（CPU 型号、频率、核心数、内存总量、运行时间）
- 获取系统进程、线程、句柄总数

### ProcessMonitor 服务
位于 `FluentMonitor/Services/ProcessMonitor.cs`,负责:
- 获取系统中所有运行的进程
- 计算每个进程的 CPU 使用率
- 获取进程内存占用、线程数等信息
- 按 CPU 和内存使用率排序
- 返回资源占用最高的前 N 个进程

### DiskMonitor 服务
位于 `FluentMonitor/Services/DiskMonitor.cs`,负责:
- 获取系统中所有固定磁盘分区
- 读取分区的详细信息（卷标、文件系统、容量等）
- 计算磁盘使用率
- 格式化显示磁盘容量单位

### HardwareInfoService 服务
位于 `FluentMonitor/Services/HardwareInfoService.cs`,负责:
- 获取详细的 CPU 硬件信息（型号、架构、频率、缓存、ID 等）
- 获取内存条详细规格（制造商、型号、容量、频率、类型）
- 获取主板和 BIOS 信息
- 获取 GPU 详细信息（型号、显存、驱动版本、分辨率）
- 获取所有存储设备信息（型号、接口、容量、固件版本）
- 获取网络适配器信息（速度、MAC 地址、类型）
- 获取操作系统详细信息
- 使用 Hardware.Info 库深度探测硬件规格

### TrayIconService 服务
位于 `FluentMonitor/Services/TrayIconService.cs`,负责:
- 动态生成 16x16 托盘图标显示 CPU 使用率
- 根据 CPU 使用率设置颜色预警（蓝色/橙色/红色）
- 生成详细的工具提示文本（CPU、内存、磁盘、网络）
- Bitmap 到 BitmapImage 格式转换
- 格式化速度和容量单位显示

### BenchmarkService 服务
位于 `FluentMonitor/Services/BenchmarkService.cs`,负责:
- CPU 单核性能测试（素数计算算法）
- CPU 多核性能测试（并行计算）
- 内存带宽测试（大数组拷贝）
- 磁盘读写速度测试（顺序 I/O）
- 综合评分计算（加权算法）
- 性能等级评定（卓越/优秀/良好/中等/一般/较低）
- 实时进度报告（进度百分比、当前测试、状态信息）
- 支持取消操作（CancellationToken）

### DesktopWidget 窗口
位于 `FluentMonitor/DesktopWidget.xaml` 和 `DesktopWidget.xaml.cs`,负责:
- 展示透明悬浮性能监控小部件（280x320）
- 实时更新 CPU、内存、磁盘、网络数据
- 支持鼠标拖动调整位置
- 双击打开主窗口，右键显示上下文菜单
- 自动定位到屏幕右下角
- 格式化显示各项性能指标

### MainWindow
位于 `MainWindow.xaml` 和 `MainWindow.xaml.cs`,负责:
- 展示 Fluent Design 用户界面（6个选项卡式布局）
- 实时更新性能数据（1秒刷新）
- 加载并显示详细硬件信息
- 管理性能跑分测试流程
- 实时更新进程列表（2秒刷新）
- 实时更新磁盘信息（5秒刷新）
- 管理图表数据集合（60秒滚动窗口）
- 系统托盘集成和窗口状态管理
- 桌面悬浮小部件管理和数据同步
- 智能托盘图标动态更新
- 主题切换功能
- 进程结束功能（带确认对话框）
- 格式化显示各种数据单位

## 🎨 界面设计

应用使用了以下 Fluent Design 元素:

- **Mica 材质** - Windows 11 风格的半透明背景
- **圆角窗口** - 现代化的窗口边框
- **Fluent 图标** - 使用 Segoe Fluent Icons
- **卡片布局** - 信息卡片组织方式
- **动画效果** - 流畅的过渡动画
- **主题系统** - 支持深色/浅色主题切换
- **选项卡导航** - 6个功能页面：性能概览、硬件信息、进程、磁盘、性能测试、设置
- **系统托盘** - 最小化到托盘，双击托盘图标恢复窗口

## 🔧 配置

### 修改性能数据刷新间隔
在 `MainWindow.xaml.cs:160-165` 中修改:
```csharp
_performanceTimer = new DispatcherTimer
{
    Interval = TimeSpan.FromSeconds(1) // 修改此值（秒）
};
```

### 修改进程列表刷新间隔
在 `MainWindow.xaml.cs:168-173` 中修改:
```csharp
_processTimer = new DispatcherTimer
{
    Interval = TimeSpan.FromSeconds(2) // 修改此值（秒）
};
```

### 修改磁盘信息刷新间隔
在 `MainWindow.xaml.cs:176-181` 中修改:
```csharp
_diskTimer = new DispatcherTimer
{
    Interval = TimeSpan.FromSeconds(5) // 修改此值（秒）
};
```

### 修改图表历史数据长度
在 `MainWindow.xaml.cs:32` 中修改:
```csharp
private const int MaxDataPoints = 60; // 修改此值（数据点数量）
```

### 修改进程列表显示数量
在 `MainWindow.xaml.cs:231` 中修改:
```csharp
var processes = _processMonitor.GetTopProcesses(15); // 修改此值
```

## 📝 待办事项

- [x] 添加进程监控功能
- [x] 添加多核 CPU 监测
- [x] 添加详细的内存信息
- [x] 提高刷新频率到 1 秒
- [x] 添加进程结束功能
- [x] 添加磁盘详细信息（各分区使用情况）
- [x] 添加系统托盘支持
- [x] 添加深色/浅色主题切换
- [x] 添加桌面悬浮小部件（Widget）
- [x] 添加智能托盘图标（动态 CPU 显示）
- [x] 添加专业硬件信息页面（CPU、内存、主板、BIOS、GPU、存储、网络、操作系统）
- [x] 添加性能跑分测试（CPU 单核/多核、内存带宽、磁盘读写、综合评分）
- [ ] 添加 GPU 性能实时监测功能
- [ ] 添加温度监测
- [ ] 添加启动项管理
- [ ] 添加开机自启动功能
- [ ] 添加数据导出功能（CSV/JSON）
- [ ] 添加性能警报功能
- [ ] 添加多语言支持（英文/中文）
- [ ] 添加性能历史记录
- [ ] 添加 CPU 核心单独监控页面

## 🤝 贡献

欢迎提交 Issue 和 Pull Request!

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 🙏 鸣谢

- [WPF-UI](https://github.com/lepoco/wpfui) - 优秀的 Fluent Design WPF 组件库
- [LiveCharts2](https://livecharts.dev/) - 强大的图表库
- [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon) - 系统托盘支持
- Microsoft Fluent Design System

---

<div align="center">

Made with ❤️ using Microsoft Fluent Design

</div>
