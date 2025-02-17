<img height="128" align="left" src="assets/logo.png" alt="Logo">

# 拯救者工具箱 Lenovo Legion Toolkit

[![Build](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml)
[![Crowdin](https://badges.crowdin.net/llt/localized.svg)](https://crowdin.com/project/llt)
[![Join Discord](https://img.shields.io/discord/761178912230473768?label=Legion%20Series%20Discord)](https://discord.com/invite/legionseries)
<a href="https://hellogithub.com/repository/dd55be3ac0c146208259f17b29d2162f" target="_blank"><img src="https://abroad.hellogithub.com/v1/widgets/recommend.svg?rid=dd55be3ac0c146208259f17b29d2162f&claim_uid=LBbuUlZqTIm1JAP&theme=small" alt="Featured｜HelloGitHub" /></a>

---

联想拯救者工具箱 Lenovo Legion Toolkit (LLT) 是为联想拯救者系列笔记本打造的轻量化工具箱。可实现原来联想软件如 Lenovo Vantage、Legion Zone、联想电脑管家才可实现的功能。

**本软件仅适配拯救者系列，如果你不是拯救者系列笔记本则这个软件不适合你，请不要提出兼容 Issue ，如果提出将被直接关闭不作受理。**

本软件不运行后台服务，使用较少的内存，几乎不使用 CPU，并且不收集用户信息。本程序仅适用于 Windows。

加入 Legion Series Discord 频道: https://discord.com/invite/legionseries!

**中文用户可加入[拯救者工具箱 QQ 频道](https://pd.qq.com/s/jj0737)，频道内推送正式版/测试版更新并附带国内镜像加速下载链接。**

<img src="assets/screenshot_zh_hans.png" width="700" />

&nbsp;

# 目录
  - [免责声明](#免责声明)
  - [下载](#下载)
  - [兼容性](#兼容性)
  - [功能介绍](#功能介绍)
  - [赞助](#赞助)
  - [贡献者](#贡献者)
  - [FAQ](#faq)
  - [如何开启记录Log](#如何开启记录Log)
  - [贡献此项目](#贡献此项目)

## 免责声明

**本软件非联想官方出品软件，使用需要自担风险。**

请仔细耐心地阅读本文档，以便了解关于本软件的一些重要信息。

## 下载

- 你可以在这里下载最新版本：[发行版页面 Releases page](https://github.com/BartoszCichecki/LenovoLegionToolkit/releases/latest)。
- 使用 [winget](https://github.com/microsoft/winget-cli)：

  ```sh
  winget install BartoszCichecki.LenovoLegionToolkit
  ```

- 使用 [Scoop](https://scoop.sh)：

  ```sh
  scoop bucket add versions
  ```

  ```sh
  scoop bucket add extras
  ```

  ```sh
  scoop install extras/lenovolegiontoolkit
  ```

> [!TIP]
> 如果你正在寻找一个 Lenovo Vantage 在 Linux 系统下的替代品，请查看 [LenovoLegionLinux](https://github.com/johnfanv2/LenovoLegionLinux) 项目。

#### 接下来的步骤

拯救者工具箱在后台运行时效果最好，所以去设置中启用_开机启动_和_关闭时最小化_。接下载就是在设置中禁用 Lenovo Vantage, Legion Zone 与 Lenovo Hotkeys，或者你也可以直接卸载他们。之后，拯救者工具箱将会在开机后自启并在后台一直保持运行，并接管 Lenovo Vantage, Legion Zone 与 Lenovo Hotkeys 的功能。

> [!WARNING]
> 如果你完全关闭拯救者工具箱，一些功能例如同步不同的性能模式（Fn + Q）与电源计划，键盘宏以及自动化功能将无法正常工作。这是因为 LLT **不运行任何后台服务**，也就意味着其无法在被完全关闭时响应指令或执行设置同步。

另外，请查看下方的 [兼容性](#兼容性) 部分。

#### 驱动依赖

如果你是在一个纯净的 Windows 系统上安装 LLT，请确保你已经安装了必要的驱动，否则一些选项将不可用。请特别注意以下两个驱动已经被安装在你的系统上了：

1. Lenovo Energy Management
2. Lenovo Vantage Gaming Feature Driver

#### 在安装 .NET 依赖时出现问题？

如果拯救者工具箱安装程序没有正确安装 .NET 依赖，则请按照以下步骤手动安装：

1. 打开 https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0
2. 找到 “.NET 桌面运行时” 一栏；
3. 点击安装程序下的“X64”一栏下载安装程序；
4. 运行安装程序并按照指引进行安装。

> [!NOTE]
> 如果你是使用 Scoop 安装了 LLT，.NET 8 依赖应该已经被自动安装。如果它没有被安装或 LLT 无法正常启动，可以使用 `scoop update` 以更新所有软件包并加上 `--force` 参数以强制重新安装 LLT。


在完成这些步骤后，你可以打开终端并输入： `dotnet --info`。在输出中寻找 "已安装的 .NET 运行时 "部分，你应该能看到类似的内容：

`Microsoft.NETCore.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]`

和

`Microsoft.WindowsDesktop.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]`

确切的版本号可能不同，但只要是`8.x.x`就应该没问题。如果经过上述步骤确认后，拯救者工具箱在启动时仍然报错提示找不到 .NET 之类的信息，那么就是你的机器或系统的问题，而不是拯救者工具箱的问题。

#### 想要帮助我们测试？

加入 [Legion Series Discord](https://discord.com/invite/legionseries) 并前往 `#legion-toolkit` 子频道，Beta 测试版与未来更新将会在那里频繁更新。

中文用户可加入 [拯救者工具箱 QQ 频道](https://pd.qq.com/s/jj0737) 并前往 `#测试版更新💻` 子频道，Beta 测试版将会经常在那里同步发布。

## 兼容性

拯救者工具箱适配 2020 款及其之后的拯救者机型。

一些功能在 2020 款之前发布的机型和 Ideapad Gaming 3 笔记本可以正常工作（或者说大部分工作），但可能会遇到一些小问题。这里是测试过支持的型号列表 [Compatibility.cs](https://github.com/BartoszCichecki/LenovoLegionToolkit/blob/master/LenovoLegionToolkit.Lib/Utils/Compatibility.cs)。

如果你在启动时看到不兼容弹窗，你可以查看底部的*贡献*部分，查看你能否帮助我适配你的机型。谨记我没有那么多的笔记本型号，所以一些功能我无法适配。

**拯救者系列与 Ideapad Gaming 系列之外的笔记本暂不考虑适配**

### 联想软件兼容

总的来说，建议在使用拯救者工具箱时禁用或卸载 Lenovo Vantage、Hotkeys 和 Legion Zone。当拯救者工具箱与其他联想应用程序一起工作时，可能会导致部分功能冲突或可能无法正常工作。

> [!TIP]
> 一般来说最简单的解决方法就是使用拯救者工具箱内的禁用选项。

### 备注

拯救者工具箱目前不支持多用户安装，所以如果你的笔记本电脑上有多个用户，你可能会遇到兼容性问题，即使是没有管理员权限的账户。拯救者工具箱需要一个有管理员权限的账户，拯救者工具箱无法在普通账户上正常运行。

## 功能介绍

拯救者工具箱可以做到：

- 改变诸如性能模式、充电模式等只有通过 Lenovo Vantage、联想电脑管家才能更改的设置。
- 使用并修改自定义模式，包括 2022 款及更新的机型的调节风扇曲线功能。
- 支持调节 Spectrum RGB 键盘、4 分区 RGB 键盘和白色背光的键盘。
- 监控独立显卡活动（仅限英伟达显卡）。
- 设置当电脑接通电源时自动运行的操作。
- 查看电池统计数据。
- 从命令行控制设备。
- 下载、更新驱动。
- 无需卸载即可禁用 Lenovo Vantage、Legion Zone 和 Lenovo Hotkeys 服务。

### 自定义模式

以下版本的 BIOS 支持自定义模式：
* GKCN（24及以上）
* GKCN（46及以上）
* H1CN（39及以上）
* HACN（31及以上）
* HHCN（20及以上）

请确保你的 BIOS 版本达到以上最低版本限制。如果你仍然在使用更老版本的 BIOS，你需要先更新才能使用自定义模式。

### RGB 和灯光控制

LLT 支持 Spectrum 单键 RGB 和四分区 RGB 背光。请在使用这些功能前确保你已经禁用了 Lenovo Vantage，以避免双方同时发送控制信息造成冲突。如果你在使用别的可能和 LLT 冲突的 RGB 控制软件，请参考 [FAQ](#faq) 里的解决方案。

LLT 也支持其他像一级或三级白色键盘背光，Legion Logo 背光和后部接口背光等灯光控制功能，不过有以下限制：

* GKCN54WW 及以下版本的 BIOS 由于其中的 bug 会造成系统蓝屏，因此部分灯光控制功能被禁用。
* 部分 Legion 笔记本型号（尤其是 2021 年的型号）可能不显示所有控制选项或显示部分不存在的选项。这主要是由于 BIOS 中对于功能可用性的配置错误。

需要 Corsair iCue 的灯光控制不会被 LLT 支持。

> [!IMPORTANT]
> 目前已知 Riot Vanguard DRM（使用它的游戏有例如：瓦罗兰特） 会造成 RGB 和灯光控制功能出现问题。如果你在安装了它之后发现了 LLT 内 RGB 设置消失的错误，请卸载它或确保它并非开机自启项。

### 混合模式和显卡工作模式

> [!NOTE]
> 请注意这些功能和英伟达 Advanced Optimus 动态显示切换不同，也不与其一同工作。

你有两种主要工作模式来控制你的独立显卡工作方式：

1. 启用混合模式 - 内置显示器会被连接到集成显卡，独立显卡只会在需要时工作以延长续航。
2. 禁用混合模式（独显直连）- 内置显示器被直接连接到独立显卡以提高性能，但这也意味着电池续航会缩短。

切换这两种工作模式需要重启电脑。

在 2022 及 2023 年的型号上混合模式还有两个附加选项：

1. 禁用独立显卡 - 这会断开独立显卡连接，以最大限度延长电池续航。
2. 自动混合模式 - 这会在笔记本使用电池供电时尝试断开独立显卡连接，并在重新连接电源时重新启用独立显卡。

当独立显卡在被使用时（包括有软件或程序调用了独立显卡，连接了外部显示器或其他联想没有说明的情况）独立显卡连接大概率无法被断开。请在使用 LLT 的强制休眠显卡功能之前确保 LLT 已经监测到了独立显卡关闭并没有连接外部显示器，否则可能会造成错误。

所有以上提到的功能均是通过调用 EC（嵌入式控制器）的功能实现的，因此这些功能工作与否取决于联想固件的适配性，而非 LLT。据我的观察和测试，这些功能在大多数时候都是可靠的，除非你频繁切换显卡工作模式。电脑可能需要一定的时间才能完成切换，因此请在每次切换后稍作等待。LLT 也尝试通过阻止频繁切换显卡工作模式和在 EC 未能正常唤醒独立显卡时重试唤醒来缓解这个问题。因此在切换到混合模式时，当 EC 未能唤醒独立显卡时，独立显卡可能需要十秒左右才能重新出现。

如果你还遇到问题，请尝试[命令行参数](#命令行参数)内的其他实验性显卡工作模式。

> [!WARNING]
> 通过设备管理器禁用独立显卡并不会断开独立显卡连接，并同时会导致高耗电量！

### 强制休眠英伟达显卡

有时独立显卡会一直保持活动状态。例如在你插上外接显示器并断开后，一些进程会继续使用独显上运行，导致续航骤减。

在拯救者工具箱中有两种办法强制休眠显卡。

1. 强制关闭所有在独显上运行的进程。（这种方式貌似更有效）,
2. 短暂强制禁用独立显卡，使在独显上运行的进程全部切换到核显。

当独显处于活动状态，并使用混合模式且没有外接显示器到独显上时，强制休眠显卡才会亮起。如果你将鼠标悬停在右侧的问号标志上，将会看到独显的状态与正在使用独显的进程。

> [!NOTE]
> 强制休眠显卡可能会导致一些应用崩溃。

### 超频英伟达独立显卡

此超频选项用于简单的超频，类似于 Legion Zone 与 Vantage 中的超频。它并不打算取代微星小飞机（Afterburner）等工具。同时以下有几点你需要注意：
* 确保在 BIOS 中开启了 GPU 超频选项（如果你的电脑有的话）。
* 当 Vantage 或 Legion Zone 运行时，超频无法生效。
* 不建议在使用微星小飞机（Afterburner）等超频工具时使用此选项。
* 如果你之前修改过控制台，那么你需要点击“自定义”按钮并添加此选项才能看到此选项。

### Windows 电源计划和 Windows 性能模式

首先，你在 LLT 中（或使用 Fn+Q）切换的性能模式和 Windows 电源计划或 Windows 性能模式**是不同的**。

现代的（也是更推荐的）管理 Windows 电源计划及性能模式的方法是只使用一个默认的“平衡（推荐）”电源计划，同时在 Windows 设置内选择以下三种 Windows 性能模式其中之一：

* 推荐的项目
* 更好的性能
* 最佳性能

（译者注：原文此处的三种性能模式直译为：最佳效能，平衡，最佳性能，但在译者的设备上三种性能模式则是上文所写的三种。我无法确定这是系统翻译的不一致性，还是不同代设备之间的差异性造成的，故在使用译者设备上系统内的名称的同时在此注解。）

你可以在 LLT 的设置中为 LLT 性能模式（安静、均衡、野兽和自定义模式）分配对应的 Windows 性能模式。在这之后，当你切换 LLT 性能模式时，LLT 会自动切换 Windows 性能模式。

传统的方法是使用多个“电源计划”，有些设备出厂时已经安装了这些计划。如果你决定使用这种传统的方法，请将 Windows 设置中的设置保持为默认或推荐设置，并将 LLT 设置为在切换性能模式时自动切换电源计划。

如果你在同步电源模式/计划，尤其是在以上提到的两种方法之间切换时遇到问题，你可以使用 `powercfg -restoredefaultschemes; shutdown /r /t ` 命令重置 Windows 电源设置。此命令会重置**所有** Windows 电源计划回默认并重启电脑。这意味着，所有除“平衡（推荐）”以外的电源计划均将会被删除，因此如果你在这之后还希望使用这些电源计划，请务必在此之前手动备份。

### 开机画面

在 2021 及 2022 年的拯救者笔记本上可以使用 LLT 更改开机画面（默认为拯救者的 Logo 图像）。

开机画面**并不被储存在 UEFI 内**，而是在启动盘的 UEFI 分区内。在设置开机画面时，LLT 会做一些基本的图像检查，比如分辨率、图像格式检查，并计算校验和以确保兼容性。不过，**LLT 无法保证通过检查的图像一定会正确的被 UEFI 读取**。在更改开机画面后的下一次启动时，UEFI 会尝试从 UEFI 分区中加载图像并在开机时显示出来，但若加载失败，则会沿用默认图像。具体的标准除了分辨率和格式外尚不清楚。

若你设置的开机画面无法被正确显示，请尝试别的图片。

### 在自动化中运行程序或脚本

你可以在自动化中使用“运行”步骤执行任何程序或脚本。在配置时你需要提供程序（`.exe`）或脚本（`.bat`）的路径。你也可以提供程序或脚本的参数，就像在命令行下运行它们一样。

<details>
<summary>实例</summary>

_关闭电脑_
 - 执行路径：`shutdown`
 - 参数：`/s /t 0`

_重启电脑_
 - 执行路径：`shutdown`
 - 参数：`/r`

_运行程序_
 - 执行路径：`C:\path\to\the\program.exe`（如果该程序所在文件夹已经被加入了 PATH 环境变量，你也可以直接输入程序名）
 - 参数：` `（可选，请查阅你使用的程序的文档或网站以获取可用的参数列表）

_运行脚本_
 - 执行路径：`C:\path\to\the\script.bat`（如果该脚本所在文件夹已经被加入了 PATH 环境变量，你也可以直接输入程序名）
 - 参数：` `（可选，请查阅你使用的程序的文档或网站以获取可用的参数列表）

_运行 Python 脚本_
 - 执行路径：`C:\path\to\python.exe`（若你已经将 Python 的安装路径加入了 PATH 环境变量，你也可以直接使用 `python`）
 - 参数：`C:\path\to\script.py`

 </details>

#### 环境变量

LLT 会自动在进程运行环境内添加一些可被访问的环境变量。这些环境变量对于需要执行上下文的高级脚本会十分实用。根据触发器的不同 LLT 会添加不同的环境变量。

<details>
<summary>环境变量</summary>

- 当电源适配器插入时
	- `LLT_IS_AC_ADAPTER_CONNECTED=TRUE`
- 当较低功率电源适配器插入时
	- `LLT_IS_AC_ADAPTER_CONNECTED=TRUE`
	- `LLT_IS_AC_ADAPTER_LOW_POWER=TRUE`
- 当电源适配器断开时
	- `LLT_IS_AC_ADAPTER_CONNECTED=FALSE`
- 当性能模式改变时
	- `LLT_POWER_MODE=<value>`，`value` 的值由当前性能模式决定：`1` 为安静模式，`2` 为均衡模式，`3` 为野兽模式，`255` 为自定义模式
	- `LLT_POWER_MODE_NAME=<value>`，`value` 的值为当前性能模式对应的英语大写名称：`QUIET`, `BALANCE`, `PERFORMANCE`, `CUSTOM`
- 当打开游戏时
	- `LLT_IS_GAME_RUNNING=TRUE`
- 当关闭游戏时
	- `LLT_IS_GAME_RUNNING=FALSE`
- 当应用程序启动时
	- `LLT_PROCESSES_STARTED=TRUE`
	- `LLT_PROCESSES=<value>`，`value` 的值为以逗号分隔的进程名
- 当指定的应用关闭时
	- `LLT_PROCESSES_STARTED=FALSE`
	- `LLT_PROCESSES=<value>`，`value` 的值为以逗号分隔的进程名	
- 打开盖子时
	- `LLT_IS_LID_OPEN=TRUE`
- 合上盖子时
	- `LLT_IS_LID_OPEN=FALSE`
- 当显示器打开时
	- `LLT_IS_DISPLAY_ON=TRUE`
- 当显示器关闭时
	- `LLT_IS_DISPLAY_ON=FALSE`
- 当连接了外置屏幕后
	- `LLT_IS_EXTERNAL_DISPLAY_CONNECTED=TRUE`
- 当断开了外置屏幕后
	- `LLT_IS_EXTERNAL_DISPLAY_CONNECTED=FALSE`
- 当启用 HDR 时
	- `LLT_IS_HDR_ON=TRUE`
- 当关闭 HDR 时
	- `LLT_IS_HDR_ON=FALSE`
- 当与 Wi-Fi 连接时
	- `LLT_WIFI_CONNECTED=TRUE`
	- `LLT_WIFI_SSID=<value>`，`value` 的值为网络的 SSID
- 当与 Wi-Fi 断开连接时
	- `LLT_WIFI_CONNECTED=FALSE`	
- 在特定的时间
	- `LLT_IS_SUNSET=<value>`，`value` 的值为 `TRUE` 或 `FALSE`，取决于触发器的设置
	- `LLT_IS_SUNRISE=<value>`，`value` 的值为 `TRUE` 或 `FALSE`，取决于触发器的设置
	- `LLT_TIME"`，`value` 的值为 `HH:mm`，取决于触发器的设置
	- `LLT_DAYS"`, `value` 的值为以逗号分隔的包含以下内容的列表：`MONDAY`、`TUESDAY`、`WEDNESDAY`、`THURSDAY`、`FRIDAY`、`SATURDAY`、`SUNDAY`，取决于触发器的设置
- 循环自动化
	- `LLT_PERIOD=<value>`，`value` 的值为间隔的秒数
- 当开机时
	- `LLT_STARTUP=TRUE`
- 当唤醒时
	- `LLT_RESUME=TRUE`

</details>

#### 程序输出

当 `等待运行结束` 被启用时，LLT 会抓取被启动的进程的标准输出流内的内容。这些程序输出会被存入 `$RUN_OUTPUT$` 变量，并可在“显示提示弹窗”步骤中使用。

### 命令行界面

你可以在命令行内直接控制 LLT 的部分功能。LLT 命令行界面的可执行文件位于安装文件夹下，名为 `llt.exe`。

命令行界面需要 LLT 在后台运行并且在设置内启用命令行界面，否则其无法正常工作。你也可以选择将命令行界面添加至你的用户 `PATH` 环境变量。

使用命令行界面无需管理员权限。

<details>
<summary>功能</summary>

* `llt quickAction --list` - 列出所有快捷操作
* `llt quickAction <name>` - 执行快捷操作 `<name>`
* `llt feature --list` - 列出所有可用功能
* `llt feature get <name>` - 打印功能 `<name>` 当前的值
* `llt feature set <name> --list` - 列出功能 `<name>` 所有可设定的值
* `llt feature set <name> <value>` - 将功能 `<name>` 的值设定为 `<value>`
* `llt spectrum profile get` - 打印当前 Spectrum RGB 预设
* `llt spectrum profile set <profile>` - 将 Spectrum RGB 预设设定为 `<profile>`
* `llt spectrum brightness get` - 打印当前 Spectrum RGB 的亮度
* `llt spectrum brightness set <brightness>` - 将 Spectrum RGB 的亮度设定为 `<brightness>`
* `llt rgb get` - 打印当前四分区 RGB 预设
* `llt rgb set <profile>` - 将四分区 RGB 预设设定为 `<preset>`

</details>

## 赞助

开发不易，如果你觉得拯救者工具箱不错的话，可以考虑赞助以支持开发。

[使用PayPal赞助](https://www.paypal.com/donate/?hosted_button_id=22AZE2NBP3HTL)

<img src="LenovoLegionToolkit.WPF/Assets/Donate/paypal_qr.png" width="200" alt="PayPal QR code" />

## 贡献者

特别感谢：

* [ViRb3](https://github.com/ViRb3) 创建了 [Lenovo Controller](https://github.com/ViRb3/LenovoController)，这是拯救者工具箱的基础。
* [falahati](https://github.com/falahati) 创建了 [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) 和 [WindowsDisplayAPI](https://github.com/falahati/WindowsDisplayAPI)
* [SmokelessCPU](https://github.com/SmokelessCPU) 帮助我适配了四分区 RGB 键盘背光。
* [Mario Bălănică](https://github.com/mariobalanica) 的所有贡献。
* [Ace-Radom](https://github.com/Ace-Radom) 的所有贡献。

翻译贡献者：
* 保加利亚语 - [Ekscentricitet](https://github.com/Ekscentricitet)
* 简体中文 - [凌卡Karl](https://github.com/KarlLee830), [Ace-Radom](https://github.com/Ace-Radom)
* 繁体中文 - [flandretw](https://github.com/flandretw)
* 捷克语 - J0sef
* 荷兰语 - Melm, [JarneStaalPXL](https://github.com/JarneStaalPXL)
* 法语 - EliotAku, [Georges de Massol](https://github.com/jojo2massol), Rigbone, ZeroDegree
* 德语 - Sko-Inductor, Running_Dead89
* 希腊语 - GreatApo
* 意大利语 - [Lampadina17](https://github.com/Lampadina17)
* 卡拉卡尔帕克语 - KarLin, Gulnaz, Niyazbek Tolibaev, Shingis Joldasbaev
* 拉脱维亚语 - RJSkudra
* 罗马尼亚语 - [Mario Bălănică](https://github.com/mariobalanica)
* 斯洛伐克语 - Mitschud, Newbie414
* 西班牙语 - M.A.G.
* 葡萄牙语 - dvsilva
* 葡萄牙语（巴西） - Vernon
* 俄语 - [Edward Johan](https://github.com/younyokel)
* 土耳其语 - Undervolt
* 乌克兰语 - [Vladyslav Prydatko](https://github.com/va1dee), [Dmytro Zozulia](https://github.com/Nollasko)
* 越南语 - Not_Nhan, Kuri, Nagidrop

## FAQ

* [为什么即使我已经卸载了 Vantage，我依然可以看到它正在运行？](#为什么即使我已经卸载了-Vantage，我依然可以看到它正在运行？)
* [为什么我的杀毒软件报告安装程序含有病毒/木马/恶意软件？](#为什么我的杀毒软件报告安装程序含有病毒/木马/恶意软件？)
* [我能自定义热键吗？](#我能自定义热键吗？)
* [我可以自定义节能模式充电阈值吗？](#我可以自定义节能模式充电阈值吗？)
* [我可以更改安静，均衡及野兽模式的风扇策略吗？](#我可以更改安静，均衡及野兽模式的风扇策略吗？)
* [更换主板后提示不兼容？](#更换主板后提示不兼容？)
* [为什么我无法在使用电池供电时切换到野兽与自定义模式？](#为什么我无法在使用电池供电时切换到野兽与自定义模式？)
* [我可以在使用拯救者工具箱时使用其他RGB软件吗？](#我可以在使用拯救者工具箱时使用其他RGB软件吗？)
* [支持 iCue RGB 键盘吗？](#支持-iCue-RGB-键盘吗？)
* [能不能多增加一些 RGB 效果？](#能不能多增加一些-RGB-效果？)
* [2022 款之前的机器能否自定义调节风扇曲线？](#2022-款之前的机器能否自定义调节风扇曲线？)
* [为啥在启用 Legion AI 引擎后切换性能模式看起来似乎有些问题？](#为啥在启用-Legion-AI-引擎后切换性能模式看起来似乎有些问题？)
* [为何即使自动化配置正确，游戏检测仍无法正常触发？](#为何即使自动化配置正确，游戏检测仍无法正常触发？)
* [为什么将鼠标悬停在拯救者工具箱托盘图标上却看不到自定义工具提示？](#为什么将鼠标悬停在拯救者工具箱托盘图标上却看不到自定义工具提示？)
* [我在哪里可以找到 CPU 睿频模式的设置？](#我在哪里可以找到-CPU-睿频模式的设置？)
* [如果我在对 GPU 进行超频时超过了阈值，该怎么办？](#如果我在对-GPU-进行超频时超过了阈值，该怎么办？)
* [我的开机画面为什么没有生效？](#我的开机画面为什么没有生效？)
* [为什么使用智能 Fn 锁时会出现卡顿？](#为什么使用智能-Fn-锁时会出现卡顿？)

#### 为什么即使我已经卸载了 Vantage，我依然可以看到它正在运行？

自 2.14.0 版本开始，LLT 对于 Vantage 残留进程检测更加严格。总的来说，Vantage 安装了三个组件：

1. Lenovo Vantage App
2. Lenovo Vantage Service
3. System Interface Foundation V2 Device

最简单的解决方法是进入 LLT 的设置界面并禁用 Lenovo Vantage，LegionZone 和 Hotkeys（只有没有被卸载的软件会被显示在设置界面）。

如果你还是想卸载它们，请确保卸载全部三个组件，否则 LLT 的部分功能可能无法正常运行。你可以检查任务管理器中是否有名称中包含 “Vantage” 或是 “ImController” 字样的进程。如果你需要别的帮助或是在卸载 `ImController` 过程中遇到问题，可以参考：[Uninstalling System Interface Foundation V2 Device](https://support.lenovo.com/us/en/solutions/HT506070)。

#### 为什么我的杀毒软件报告安装程序含有病毒/木马/恶意软件？

拯救者工具箱使用了许多底层的 Windows API，杀毒软件可能会识别这些 API 的调用为可疑的，从而造成误报。拯救者工具箱本身是开源的，任何感觉此软件有问题的人可以很简单的审查此软件源代码。同时所有安装包都是直接在 Github 上使用 Github Actions 构建的，所以你也不需要担心安装包含有恶意内容。此问题可以通过对程序签名来解决，但此项目只是我业余时间来制作的开源项目，我无法负担每年花几百美元买一个证书。

简而言之，如果你从这个项目的网站上下载了安装程序，不需要担心杀毒软件的报毒，这只是一个误报。同时如果你有能力帮助解决杀毒软件的误报问题，欢迎联系我。

#### 我能自定义热键吗？

你可以在 LLT 的设置界面内自定义 Fn+F9 的热键功能。其余的热键是无法被自定义的。

#### 我可以自定义节能模式充电阈值吗？

不能，该阈值是在固件中被锁死的，无法被更改。对于 2021 年及以前的型号该阈值为 60%，对于之后的型号则为 80%。

#### 我可以更改安静，均衡及野兽模式的风扇策略吗？

不能。除自定义模式以外，风扇策略都是不能更改的。

#### 更换主板后提示不兼容？

有时新主板内的机型和序列号信息出错，你可以尝试 [这篇教程](https://laptopwiki.eu/laptopwiki/guides/lenovo/legion_bios_lvarrecovery) 来恢复。如果这不起作用你可以尝试打开 `%LOCALAPPDATA%\LenovoLegionToolkit` 并创建一个名为 `args.txt` 的文件并打开文件后呼入 `--skip-compat-check` ，这会禁用拯救者工具箱的兼容性检查。我们建议你在无法正确恢复型号、序列号等信息的情况下再使用这个办法。

#### 为什么我无法在使用电池供电时切换到野兽与自定义模式？

在 2.11.0 版本后，拯救者工具箱与 Lenovo Vantage 与 Legion Zone 的行为保持一致，将不再允许在没有插入适当电源适配器的情况下启用野兽与自定义模式。

如果出于某种原因，你想在电池供电的情况下使用这些模式，你可以使用`--allow-all-power-modes-on-battery`参数来启用（参见[命令行参数](#命令行参数)）。

> [!WARNING]
> 当笔记本电脑没有连接到全功率的电源适配器时，功耗限制与其他设置在大多数设备上无法正常应用，同时可能会出现一些未知的问题。

#### 我可以在使用拯救者工具箱时使用其他RGB软件吗？

总的来说，可以。 当 Lenovo Vantage 运行时，拯救者工具箱将禁用 RGB 控制，以避免冲突。如果你想使用其他 RGB 软件，如 [L5P-Keyboard-RGB](https://github.com/4JX/L5P-Keyboard-RGB) 或 [OpenRGB](https://openrgb.org/)，你可以在拯救者工具箱中使用 `--force-disable-rgbkb` 或 `--force-disable-spectrumkb` 参数禁用 RGB 以避免冲突（参考[命令行参数](#命令行参数)）。

#### 支持 iCue RGB 键盘吗？

不支持，我推荐你看看 [OpenRGB](https://openrgb.org/) 这个项目。

#### 能不能多增加一些 RGB 效果？

只有硬件支持的选项可用，不计划支持自定义效果。如果你想要自定义效果可以看看 [L5P-Keyboard-RGB](https://github.com/4JX/L5P-Keyboard-RGB) 或 [OpenRGB](https://openrgb.org/)。

#### 2022 款之前的机器能否自定义调节风扇曲线？

如果你是 2022 款与更新的机型的拯救者，但不支持自定义风扇曲线的话，请提交 Issue，我们会尝试适配。2022 款之前的机型由于技术原因无法支持。

#### 为啥在启用 Legion AI 引擎后切换性能模式看起来似乎有些问题？

貌似有些 BIOS 版本在使用 Fn+Q 快捷键时有一个奇怪的问题，你可以试试更新 BIOS，除此之外只能等联想修复它了。

#### 为何即使自动化配置正确，游戏检测仍无法正常触发？

拯救者工具箱的游戏检测功能是基于 Windows 游戏检测的。这意味着游戏检测功能仅能被 Windows 认为是游戏的 EXE 文件触发。同时如果你删除了 Xbox Game Bar，此功能大概率将无法正常工作。

Windows 可能无法正确识别所有的游戏，但你可以在 Xbox Game Bar (Win + G) 设置中将任何程序标记为游戏。你可以在注册表中找到可识别的游戏列表：`HKEY_CURRENT_USER\System\GameConfigStore\Children`。

#### 为什么将鼠标悬停在拯救者工具箱托盘图标上却看不到自定义工具提示？

在 Windows 10 和 11 中，微软对托盘做了大量的修改，修改导致了很多东西产生了变化，从而导致许多东西无法正常工作。因此，自定义工具提示有时会无法正常工作。你可以试试更新你的 Windows，除此之外没什么好办法了。

#### 我在哪里可以找到 CPU 睿频模式的设置？

简而言之，在 Windows 控制面板中。此选项因很难维护已经从拯救者工具箱中删除了。在拯救者工具箱的设置中，你可以找到一个直接跳转到控制面板中电源计划设置页面的按钮，在那里你可以轻松地编辑 CPU 睿频模式设置以及 Windows 电源计划的其他设置。

默认情况下，这个设置是隐藏的，但你也可以通过在终端运行`powercfg.exe -attributes sub_processor perfboostmode -attrib_hide`来重新打开这个选项。
另外我还推荐其他一些应用程序可以轻松地管理多个电源计划设置：[PowerSettingsExplorer](https://forums.guru3d.com/threads/windows-power-plan-settings-explorer-utility.416058/) 与 [QuickCPU](https://coderbag.com/product/quickcpu)。

#### 如果我在对 GPU 进行超频时超过了阈值，该怎么办？

如果你超频到 GPU 无法稳定运行，甚至无法启动 Windows 的情况，你可以通过以下两种方法尝试解决：

1. 进入 BIOS，尝试找到类似于 “Enabled GPU Overclocking” 与“显卡超频”的选项并将其禁用，启动 Windows，修改拯救者工具箱中的超频参数，并将此选项再次启用。
2. 在安全模式下启动 Windows，删除拯救者工具箱设置下的 `gpu_oc.json` 文件，该文件位于 `"%LOCALAPPDATA%\LenovoLegionToolkit`。

#### 我的开机画面为什么没有生效？

当你设置开机画面时，LLT会做一些基本的检查以确保图像的分辨率和格式符合标准。如果 LLT 显示开机画面已被设置，意味着该图像已经被写入了启动盘的 UEFI 分区。如果你在启动时没有看到开机画面，这意味着你的开机画面无法被正确显示，即使相关选项已经在 UEFI 中被成功配置。在这种情况下，你也许可以尝试使用另外的一张图片，更改图片的格式，或使用别的图像编辑软件，等等。如果你尝试了所有可能的解决方案但你的开机画面还是无法被正常显示，那这也许就是由 BIOS 造成的问题了，你可以尝试更新 BIOS 版本再重试。

#### 为什么使用智能 Fn 锁时会出现卡顿？

在一些版本的 BIOS 上切换 Fn 锁是会造成一定的卡顿。由于智能 Fn 锁本质上是自动的 Fn 锁切换，因此也会受到这个问题的影响。

如果你遇到了这个问题，你可以尝试关闭 BIOS 内的 `Fn键动态替换`（英语版 BIOS 则为 `Fool Proof Fn Ctrl`）功能。这也许可以解决 Fn 锁切换的卡顿。

#### 为什么我无法在设备信息中找到保修信息？

由于联想不断更新国行保修服务的 API 造成此功能越来越难以维护，在最新版本中 LLT 移除了该功能对国内拯救者全部型号的支持。如果你曾经通过 LLT 获取过保修信息那它将保持正常显示，但一旦你手动刷新保修信息或删除了储存的数据这些信息就会消失。这一改动仅会影响使用国行拯救者型号的用户。

## 命令行参数

一些并不常用的功能在 GUI 中没有对应的启动开关。这些功能需要通过在启动 LLT 时添加命令行参数，或将参数添加到 `args.txt` 中的方式启用。

* `--trace` - 启用日志记录并将日志保存到 `%LOCALAPPDATA%\LenovoLegionToolkit\log`
* `--minimized` - 以最小化到托盘的方式启动 LLT
* `--skip-compat-check` - 在启动 LLT 时不检查设备兼容性 _（使用该参数时 LLT 不保证能够正常运行，也不会为此参数造成的问题提供技术支持）_
* `--disable-tray-tooltip` - 当鼠标悬停在托盘图标上方时不显示 LLT 托盘自定义工具提示
* `--allow-all-power-modes-on-battery` - 允许在未接通外部电源的情况下启用所有性能模式 _（使用该参数时 LLT 不保证能够正常运行，也不会为此参数造成的问题提供技术支持）_
* `--enable-hybrid-mode-automation` - 允许使用 LLT 自动化操作切换混合模式或其他显卡工作模式 _（使用该参数时 LLT 不保证能够正常运行，也不会为此参数造成的问题提供技术支持）_
* `--force-disable-rgbkb` - 禁用四分区 RGB 键盘的所有光效控制功能
* `--force-disable-spectrumkb` - 禁用 Spectrum 单键 RGB 的所有光效控制功能
* `--force-disable-lenovolighting` - 禁用拯救者 Logo，白色键盘背光，和其他如端口背光的光效控制功能
* `--experimental-gpu-working-mode` - 将显卡工作模式切换至和 LegionZone 相同的实验性模式 _（使用该参数时 LLT 不保证能够正常运行，也不会为此参数造成的问题提供技术支持）_
* `--proxy-url=example.com` - 指定 LLT 应该使用的代理服务器地址
* `--proxy-username=some_username` - 如果需要，指定 LLT 使用的代理服务器的用户名
* `--proxy-password=some_password` - 如果需要，指定 LLT 使用的代理服务器的密码
* `--proxy-allow-all-certs` - 如果需要，放宽通过代理服务器建立 HTTPS/SSL 连接的所需标准
* `--disable-update-checker` - 禁用 LLT 自动新版本检测 _（若你希望依赖于 winget，scoop 等等软件更新 LLT，你可以启用此选项）_

如果你希望将所需参数保存至 `args.txt` 文件内：
1. 进入 `%LOCALAPPDATA%\LenovoLegionToolkit` 文件夹
2. 在那里创建一个名为 `args.txt` 的文本文件
3. 在文件内的每一行添加**一个**参数
4. 启动 LLT

任何没有在上方列出的，曾经可用的命令行参数均已被废弃，也无法再使用。

## 如何开启记录Log

在一些情况下如果你能提交应用记录的日志信息，将会对我调试和解决问题十分十分有用。

记录 Log 日志的步骤：

1. 确保拯救者工具箱已关闭（后台也记得关掉）；
2. 打开 `运行` （使用 Win + R 打开）然后输入 `"%LOCALAPPDATA%\Programs\LenovoLegionToolkit\Lenovo Legion Toolkit.exe" --trace` 然后点击确定；
3. 拯救者工具箱将会启动并且可以在左上角能看到 `[LOGGING ENABLED]` ；
4. 复现你遇到的问题；
5. 关闭拯救者工具箱 （同样记得关掉后台）；
6. 然后打开 `运行` （使用 Win + R 打开）然后输入 `"%LOCALAPPDATA%\LenovoLegionToolkit\log"` ；
7. 这里就是存放日志文件的地方了，请在 Issue 内汇报 Bug 时一并提交。

## 贡献此项目

我感谢你们提交的任何反馈！不要犹豫，直接提交 Issue。我们也欢迎提交 PR，但提交 PR 前务必查看 [CONTRIBUTING.md](CONTRIBUTING.md) 文件！

> [!IMPORTANT]
> 译者提示：由于 LLT 并非由国人发起的项目，主要开发者也大多来自欧美，为了整体交流环境的统一和协调，所有 Issue，PR 和 Commit Message **必须**使用英语书写，否则将被直接关闭并锁定，**没有例外**。此点也已在 [CONTRIBUTING.md](CONTRIBUTING.md) 中说明。若你无法流畅地使用英语表达，你可以在使用中文完成草稿后使用百度翻译或 [DeepL](https://www.deepl.com/zh/translator) 等翻译网站或软件将草稿翻译为英语后提交。

#### 适配

> [!IMPORTANT]
> 拯救者工具箱只目标适配联想拯救者（海内及海外版）、IdeaPad Gaming 和 LOQ 系列。请不要为除上述系列以外的设备提出兼容请求。

如果能适配更多设备就更好了！但要做到这点，我真的很需要你的帮助！

如果你愿意在未适配的机型上试试这个软件，请在启动时点击弹窗的继续按钮，拯救者工具箱会自动打开日志记录，这样你就可以在提交 Issue 时提交了！

*注意一些功能可能无法正常运行*

如果你在 Github 上提交 Issue 并附上你的测试结果和日志我将十分感谢你！

请确保在你提交的 Issue 中包含以下信息：

1. 完整的设备型号 (例如：Legion Y9000X 2022款 IAH7)
2. 正常工作的功能
3. 出错的功能
4. 会导致崩溃闪退的功能

你提交的信息越多，随着时间的推进，拯救者工具箱就会变得越来越好！如果有什么出错的地方请准确写下问题并附上日志。(日志保存地址 `%LOCALAPPDATA%\LenovoLegionToolkit\log`). 

**万分感谢！**
