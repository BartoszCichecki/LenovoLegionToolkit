<img height="128" align="left" src="assets/logo.png" alt="Logo">

# Lenovo Legion Toolkit

[![Build](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml)
[![Crowdin](https://badges.crowdin.net/llt/localized.svg)](https://crowdin.com/project/llt)
[![Join Discord](https://img.shields.io/discord/761178912230473768?label=Legion%20Series%20Discord)](https://discord.com/invite/legionseries)

---

#### Other language versions of this README file:
* [简体中文版简介](README_zh-hans.md)

---

![Ukrainian Flag](assets/ukraine_flag_bar.png)

Support the Armed Forces of Ukraine and People Affected by Russia’s Aggression:

* Humanitarian Aid: https://bank.gov.ua/en/about/humanitarian-aid-to-ukraine
* Support Armed Forces of Ukraine: https://bank.gov.ua/en/about/support-the-armed-forces

**Слава Україні!**

![Ukrainian Flag](assets/ukraine_flag_bar.png)

<br />

Lenovo Legion Toolkit (LLT) is a utility created for Lenovo Legion series laptops, that allows changing a couple of features that are only available in Lenovo Vantage or Legion Zone.

**If your laptop is not part of Legion series, this software is not for you. Please do NOT open compatibility requests for other devices. Issues will be closed and not looked at!**

It runs no background services, uses less memory, uses virtually no CPU, and contains no telemetry. Just like Lenovo Vantage, this application is Windows only.

Join the Legion Series Discord: https://discord.com/invite/legionseries!

<img src="assets/screenshot.png" width="700" alt="PayPal QR code" />


# Table of Contents
  - [Disclaimer](#disclaimer)
  - [Donate](#donate)
  - [Download](#download)
  - [Compatibility](#compatibility)
  - [Features](#features)
  - [Credits](#credits)
  - [FAQ](#faq)
  - [How to collect logs?](#how-to-collect-logs)
  - [Contribution](#contribution)

## Disclaimer

**The tool comes with no warranty. Use at your own risk.**

This is a hobby project. I want to make it available on more devices, but it will take some time, so please be patient and read through this readme carefully.

## Donate

If you enjoy using the Lenovo Legion Toolkit, consider donating.

[Donate with PayPal](https://www.paypal.com/donate/?hosted_button_id=22AZE2NBP3HTL)

<img src="LenovoLegionToolkit.WPF/Assets/Donate/paypal_qr.png" width="200" alt="PayPal QR code" />


#### Donate from China

If you live in China, you can also donate in Chinese Yuan using Stripe (supports UnionPay and AliPay):

[Donate with Stripe](https://donate.stripe.com/14k8yM94I980f3q7ss)

<img src="LenovoLegionToolkit.WPF/Assets/Donate/stripe_cny_qr.png" width="200" alt="Stripe (CNY) QR code" />

## Download

You can download the installer from the [Releases page](https://github.com/BartoszCichecki/LenovoLegionToolkit/releases/latest).

Or install using [winget](https://github.com/microsoft/winget-cli):

`winget install BartoszCichecki.LenovoLegionToolkit`

#### Want to help with testing?

Join the [Legion Series Discord](https://discord.com/invite/legionseries) and head to `#legion-toolkit` channel. Beta versions of future releases are posted there frequently!

## Compatibility

Lenovo Legion Toolkit is made for Lenovo Legion laptops released in 2020 or later.

Some features work (or mostly work) on  models released before 2020 and on Ideapad Gaming 3 laptops, but you may experience some smaller issues. The list of models on which LLT was tested and is working can be found here: [Compatibility.cs](https://github.com/BartoszCichecki/LenovoLegionToolkit/blob/master/LenovoLegionToolkit.Lib/Utils/Compatibility.cs).

If you are getting an incompatible message on startup, you can check the *Contribution* section down at the bottom, to see how can you help. Keep in mind, that not always I can make all options compatible with all hardware since I do not have access to it.

**Support for other laptop that are not part of Legion series is not planned.**

### Lenovo's software

Overall the recommendation is to disable or uninstall Vantage, Hotkeys and Legion Zone while using LLT. There are some functions that cause conflicts or may not work properly when LLT is working along side other Lenovo apps.

### Other remarks

LLT currently does not support installation for multiple users, so if you need to have multiple users on you laptop you might encounter issues. Same goes for accounts without Administrator rights - LLT needs an account with Administrator rights. If you install LLT on an account without such rights, LLT will not work properly.

## Features

The app allows to:

- Change settings like power mode, battery charging mode, etc. that are available only through Vantage.
- Access to Custom Mode available only in Legion Zone, including Fan Control on 2022 and newer models.
- Spectrum RGB and White backlight keyboards support.
- Change display refresh rate (built-in display only).
- Deactivate discrete GPU (nVidia only).
- View battery statistics.
- Download software updates.
- Define Actions that will run when the laptop is i.e. connected to AC power.
- Disable/enable Lenovo Vantage, Legion Zone and Lenovo Hotkeys service without uninstalling it.

##### Custom Mode

Custom Mode is supported on following BIOS versions:
* GKCN49WW and higher
* H1CN49WW and higher
* HACN31WW and higher
* HHCN23WW and higher
* K1CN31WW and higher
* K9CN34WW and higher
* KFCN32WW and higher
* KWCN28WW and higher
* J2CN40WW and higher
* JUCN51WW and higher
* JYCN39WW and higher

Not all features of Custom Mode are supported by all devices.

##### Deactivate discrete GPU

Sometimes discrete GPU stays active even when it should not. This can happen for example, if you work with an external screen and you disconnect it - some processes will keep running on discrete GPU keeping it alive and shortening battery life.

There are two ways to help the GPU deactivate:

1. killing all processes running on dGPU (this one seems to work better),
2. disabling dGPU for a short amount of time, which will force all processes to move to the integrated GPU.

Deactivate button will be enabled when dGPU is active, you have Hybrid mode enabled and there are no screens connected to dGPU. If you hover over the button, you will see the current P state of dGPU and the list of processes running on it.

Keep in mind that some apps may not like this feature and crash when you deactivate dGPU.

##### Windows Power Plans

Lenovo Legion Toolkit will automatically switch Windows power plans when Power Mode changes *and* when Lenovo Vantage is disabled.

On some laptops though, Lenovo Vantage never switched power plans. If you have one of the laptops where Lenovo Vantage does not change Windows power plans automatically you can override this behavior in Settings. This will allow Toolkit to always change Windows power plans, even if Lenovo Vantage is running in the background.

##### CPU Boost Modes

This allows modifying the hidden setting of Windows Power Plans called *Processor performance boost mode*. It is a little bit cryptic what these options do, but the best explanation is provided here:

[Power and performance tuning @microsoft.com](https://docs.microsoft.com/en-us/windows-server/administration/performance-tuning/hardware/power/power-performance-tuning#processor-performance-boost-mode)

[ProcessorPerformanceBoostMode @microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/microsoft.windows.eventtracing.power.processorperformanceboostmode?view=trace-processor-dotnet-1.0)

## Credits

Special thanks to:

* [ViRb3](https://github.com/ViRb3), for creating [Lenovo Controller](https://github.com/ViRb3/LenovoController), which was used as a base for this tool
* [falahati](https://github.com/falahati), for creating [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) and [WindowsDisplayAPI](https://github.com/falahati/WindowsDisplayAPI)
* [SmokelessCPU](https://github.com/SmokelessCPU), for help with 4-zone RGB and Sprectrum keyboard support
* [Mario Bălănică](https://github.com/mariobalanica), for all contributions

Translations provided by:
* Chinese - [凌卡Karl](https://github.com/KarlLee830)
* Czech - J0sef
* Dutch - Melm, [JarneStaalPXL](https://github.com/JarneStaalPXL)
* French - EliotAku, [Georges de Massol](https://github.com/jojo2massol), Rigbone, ZeroDegree
* German - Sko-Inductor, Running_Dead89
* Greek - GreatApo
* Italian - [Lampadina17](https://github.com/Lampadina17)
* Romanian - [Mario Bălănică](https://github.com/mariobalanica)
* Slovak - Mitschud, Newbie414
* Spanish - M.A.G.
* Portugese - dvsilva
* Portuguese (Brasil) - Vernon
* Russian - [Edward Johan](https://github.com/younyokel)
* Turkish - Undervolt
* Ukrainian -  [Vladyslav Prydatko](https://github.com/va1dee)
* Vietnamese - Not_Nhan, Kuri

Many thanks to everyone else, who monitors and corrects translations!

## FAQ

#### Why is my antivirus reporting that the installer contains a virus/trojan/malware?

LLT makes us of many low-level Windows APIs that can be falsely flagged by antiviruses as suspicious, resulting in a false-positive. LLT is open source and can easily be audited by anyone who has any doubts as to what this software does. All installers are built directly on GitHub with GitHub Actions, so that there is no doubt what they contain. This problem could be solved by signing all code, but I can't afford spending hundreds of dollars per year for an Extended Validation certificate.

If you downloaded the installer from this projects website, you shouldn't worry - the warning is a false-positive. That said, if you can help with resolving this issue, let's get in touch.


#### My motherboard was replaced and now LLT gives me incompatible message. What do I do?

Sometimes new motherboard does not contain correct model numbers and serial numbers. You should try [this tutorial](https://laptopwiki.eu/index.php/guides-and-tutorials/important-bios-related-guides/recover-original-model-sku-values/) to try and recover them. If that method does not succeed, you can workaround by going to `%LOCALAPPDATA%\LenovoLegionToolkit` and creating an `args.txt` file. Inside that file paste `--skip-compat-check`. This will disable all compatibility checks in LLT. Use this workaround only if correct model number, serial number etc. can't be restored.

#### Will iCue RGB keyboards be supported?

No. Check out [OpenRGB](https://openrgb.org/) project.

#### Can I have more RGB effects?

Only options natively supported by hardware are available; adding support for custom effects is not planned. If you would like more customization check out [L5P-Keyboard-RGB](https://github.com/4JX/L5P-Keyboard-RGB) or [OpenRGB](https://openrgb.org/).

#### Can I use other RGB software while using LLT?

In general yes. LLT will disable RGB controls when Vantage is running to avoid conflicts. If you use other RGB software like [L5P-Keyboard-RGB](https://github.com/4JX/L5P-Keyboard-RGB) or [OpenRGB](https://openrgb.org/), you can disable RGB in LLT to avoid conflicts.

To disable RGB:
1. Go to `%LOCALAPPDATA%\LenovoLegionToolkit`
2. Create `args.txt` file in there (if you already have it edit)
3. Paste `--force-disable-rgbkb` or `--force-disable-spectrumkb` depending on which keyboard you have (if you have other parameters, there should be 1 per line)
4. Start LLT

#### Can you add fan control to other models?

If you have a 2022 or newer Legion that does not support it make a ticket and we'll try to add suport for it. Older models will not be supported due to technical limitations.

#### Why is my Fn lock is flipped?

Some devices experience this issue and frankly, I have no idea why. It is known issue, but if you know how to solve it, check out the Contribution section.

#### Why is switching to Performance mode seems buggy, when AI Engine is enabled?

It seems that some BIOS versions indeed have a weird issue when using Fn+Q. Only hope is to wait for Lenovo to fix it.

#### Why isn't a game detected, even though Actions are configured properly?

Game detection feature is built on top of Windows' game detection, meaning LLT will react to EXE files that Windows considers "a game". That also means that if you nuked Xbox Game Bar from your installation, there is 99.9% chance this feature will not work.

Windows probably doesn't recognize all games properly, but you can mark any program as game in Xbox Game Bar settings (Win+G). You can find list of recognized games in registry: `HKEY_CURRENT_USER\System\GameConfigStore\Children`.

#### Why don't I see the custom tooltip when I hover LLT icon in tray?

In Windows 10 and 11, Microsoft did plenty of changes to the tray, breaking a lot of things on the way. As a results custom tooltips not always work properly. Solution? Update your Windows and keep fingers crossed.

## How to collect logs?

In some cases it will be super useful if you can provide logs that this app can create. This helps with debugging and other issue solving.


To collect logs:

1. Make sure that Lenovo Legion Toolkit is not running (also gone from tray area).
2. Open `Run` (Win+R) and type there: `"%LOCALAPPDATA%\Programs\LenovoLegionToolkit\Lenovo Legion Toolkit.exe" --trace` and hit OK
3. LLT will start and in the title bar you should see: `[LOGGING ENABLED]`
4. Reproduce the issue you have (i.e. try to use the option that causes issues)
5. Close LLT (also make sure it's gone from tray area)
6. Again, in `Run` (Win+R) type `"%LOCALAPPDATA%\LenovoLegionToolkit\log"`
7. You should see at least one file. Theses are the logs you should attach to the issue.

## Contribution

I appreciate any feedback that you have, so please do not hesitate to report issues.
Pull Requests are also welcome, but make sure to check out [CONTRIBUTING.md](CONTRIBUTING.md) first!

#### Translation

Crowdin has been selected as the tool for handling translations. If you want to contribute, go to https://crowdin.com/project/llt and request access.

#### Bugs

If you find any bugs in the app, please report them. It will be very helpful if you attach logs that will help to trace the root cause of the issue. You can find logs in `%LOCALAPPDATA%\LenovoLegionToolkit\log`. Of course attach the latest log to the issue here on GitHub.

#### Compatibility

It would be great to expand the list of compatible devices, but to do it your help is needed!

If you are willing to check if this app works correctly on your device that is currently unsupported, click _Continue_ on the popup you saw on startup. Lenovo Legion Toolkit will start logging automatically so you can submit them if anything goes wrong.

*Remember that some functions may not function properly, so keep this in mind.*

I would appreciate it, if you create an issue here on GitHub with the results of your testing.

Make sure to include the following information in your issue:

1. Full model name (i.e. Legion 5 Pro 16ACH6H)
2. List of features that are working as expected.
3. List of features that seem to not work.
4. List of features that crash the app.

The more info you add, the better the app will get over time. If anything seems off, write down precisely what was wrong and attach logs (`%LOCALAPPDATA%\LenovoLegionToolkit\log`). 



Thanks in advance!
