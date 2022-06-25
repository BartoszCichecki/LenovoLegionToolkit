# Lenovo Legion Toolkit

[![Build](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml) <img src="https://img.shields.io/discord/761178912230473768?label=Discord" alt="Join Discord" />

This is a utility created for Lenovo Legion 5, 5 Pro, 7 etc. laptops, that allows to change a couple of features that are only available in Lenovo Vantage.

It runs no background services, uses less memory, virtually no CPU and contains no telemetry.

Join the Legion Series Discord: https://discord.com/invite/legionseries

![screenshot](assets/screenshot.png)

## Disclaimer

**The tool comes with no warranty. Use at you own risk.**

This is a hobby project. I want to make it available on more devices, but it will take some time, so please be patient and read through this readme carefully.

## Donate

If you enjoy using the Lenovo Legion Toolkit, consider donating.

<a href="https://www.paypal.com/donate/?hosted_button_id=22AZE2NBP3HTL"><img src="LenovoLegionToolkit.WPF/Assets/paypal_button.png" width="200" alt="PayPal Donate" /></a>

<img src="LenovoLegionToolkit.WPF/Assets/paypal_qr.png" width="200" alt="PayPal QR code" />

## Download

You can download the installer from Releases page here: [Latest release](https://github.com/BartoszCichecki/LenovoLegionToolkit/releases/latest).

## Compatibility

Toolkit is compatible with a lot of Lenovo Legion laptops from 2020 and 2021 running Windows 10 and 11. All testing done by me is done always on latest Windows 11 update and Legion 5 Pro 16ACH6H.

If you are getting an incompatible message on startup, you can check *Contribution* section down at the bottom, to see how can you help. Keep in mind, that not always I am able to make all options compatible with all hardware, since I do not have access to it.

**Note:** Y-models (Y540, Y740 etc) have limited compatibility only, meaning not all options work.

List of supported models can be found here: [Compatibility.cs](https://github.com/BartoszCichecki/LenovoLegionToolkit/blob/master/LenovoLegionToolkit.Lib/Utils/Compatibility.cs).

## Features

The app allows to:

* Change settings like power mode, battery charging mode etc. That are available only through Vantage.
* Change display refresh rate (built-in display only).
* Deactivate discrete GPU (nVidia only).
* View battery statistics.
* Define Actions that will run when laptop is i.e. connected to AC power.
* Disable/enable Lenovo Vantage and Fn Keys service without uninstalling it.

##### Disable/enable Lenovo Vantage

You can disable Lenovo Vantage without uninstalling it. It it especially useful, you want to keep Vantage around i.e. for checking updates.

You can do it from the Settings page. If you disable Vantage, it will stop and disable it's services, and disable all funky Lenovo Scheduled tasks. Once Vantage is disabled, it will no longer start any background processes on startup.

If you want to open Lenovo Vantage, you need to re-enable it from the tool, otherwise it will prompt to reinstall itself.

##### Disable/enable Fn Keys service

This app allows you to disable Lenovo Vantage without uninstalling it. It it especially useful, you want to keep Vantage around i.e. for checking updates.

You can do it from the Tools menu on top. If you disable Vantage, it will stop and disable it's services, and disable all funky Lenovo Scheduled tasks. Once Vantage is disabled, it will no longer start any background processes on startup. If you want to open Lenovo Vantage, you need to re-enable it from the tool, otherwise it will prompt to reinstall itself.

##### Deactivate discrete GPU

Sometimes discrete GPU stays active even when it should not. This can happen for example, if you work with external screen and you disconnect it - some processes will keep running on discrete GPU keeping it alive and shortening battery life.

There are two ways to help the GPU deactivate:

1. killing all processes running on dGPU,
2. disabling dGPU for a short amount of time, which will force all processes to move to the integrated GPU.

Deactivate button will be enabled when dGPU is active, you have Hybrid mode enabled and there are no screens connected to dGPU. If you hover over the button, you will see the current P state of dGPU and list of processes running on it.

Keep in mind that some apps may not like this feature and crash, when you deactivate dGPU.

##### Windows Power Plans

Toolkit will automatically switch Windows power plans when Power Mode changes _and_ when Lenovo Vantage is disabled.

On some laptops though, Lenovo Vantage never switched power plans. If you have one of the laptops where Lenovo Vantage does not change Windows power plans automatically you can override this behavior in Settings. This will allow Toolkit to always change Windows power plans, even if Lenovo Vantage is running in the background.

##### CPU Boost Modes

This allows to modify hidden setting of Windows Power Plans called *Processor performance boost mode*. It is a little bit cryptic what these options do, but the best explanation is provided here:

[Power and performance tuning @microsoft.com](https://docs.microsoft.com/en-us/windows-server/administration/performance-tuning/hardware/power/power-performance-tuning#processor-performance-boost-mode)

[ProcessorPerformanceBoostMode @microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/microsoft.windows.eventtracing.power.processorperformanceboostmode?view=trace-processor-dotnet-1.0)

## Credits

Credits go to [ViRb3](https://github.com/ViRb3), for creating [Lenovo Controller](https://github.com/ViRb3/LenovoController), which was used as a base for this tool. 

Special thanks to [falahati](https://github.com/falahati), for creating [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) and [WindowsDisplayAPI](https://github.com/falahati/WindowsDisplayAPI).

## Contribution

I appreciate any feedback that you have, so please do not hesitate to report issues. PRs are also welcome!

#### Bugs

If you find any bugs in the app, please report them. It will be very helpful if you start the app with `--trace` parameter and reproduce the issue. You can find logs in `%LOCALAPPDATA%\LenovoLegionToolkit\log`. Of course attach the latest log to the issue here on GitHub.

Just, don't run with this parameter all the time, it creates a ***really large*** amount of logs.

#### Compatibility

It would be great to expand the list of compatible devices, but to do it your help is needed!

If you are willing to check if this app works correctly on your device that is currently unsupported, you can do it by starting the app with ``--skip-compat-check`` argument. Remember that some functions may not function properly, so keep this in mind.

If you do it, I would really appreciate if you create an issue here on GitHub with the results of your testing. Make sure to include following information in your issue:

1. Full model name (i.e. Legion 5 Pro 16ACH6H)
2. List of features that are working as expected.
3. List of features that seem to not work.
4. List of features that crash the app.

The more info you add, the better the app will get over time.

Thanks in advance!
