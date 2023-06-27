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

Support the Armed Forces of Ukraine and People Affected by Russia’s Aggression on UNITED24, the official fundraising platform of Ukraine: https://u24.gov.ua.

**Слава Україні!**

![Ukrainian Flag](assets/ukraine_flag_bar.png)

<br />

Lenovo Legion Toolkit (LLT) is a utility created for Lenovo Legion series laptops, that allows changing a couple of features that are only available in Lenovo Vantage or Legion Zone.

**If your laptop is not part of Legion series, this software is not for you. Please do NOT open compatibility requests for other devices. Issues will be closed and not looked at!**

It runs no background services, uses less memory, uses virtually no CPU, and contains no telemetry. Just like Lenovo Vantage, this application is Windows only.

&nbsp;

_Join the Legion Series Discord: https://discord.com/invite/legionseries!_

_If you are looking for a Vantage alternative that was made for Linux, check [LenovoLegionLinux](https://github.com/johnfanv2/LenovoLegionLinux) project._

&nbsp;

<img src="assets/screenshot_main.png" width="700" />

<details>
<summary><b><i>Click here for more screenshots...</i></b></summary>

| **Keyboard**                            | **Battery**                                 |
| --------------------------------------- | ------------------------------------------- |
| <img src="assets/screenshot_kb.png" />  | <img src="assets/screenshot_bat.png" />     |

| **Actions**                                | **Downloads**                           |
| ------------------------------------------ | --------------------------------------- |
| <img src="assets/screenshot_actions.png"/> | <img src="assets/screenshot_pkg.png" /> |

| **Custom Mode**                         | **Custom Mode**                         |
| --------------------------------------- | ----------------------------------------|
| <img src="assets/screenshot_cm1.png" /> | <img src="assets/screenshot_cm2.png" /> |

</details>

# Table of Contents
  - [Disclaimer](#disclaimer)
  - [Download](#download)
  - [Compatibility](#compatibility)
  - [Features](#features)
  - [Donate](#donate)
  - [Credits](#credits)
  - [FAQ](#faq)
  - [How to collect logs?](#how-to-collect-logs)
  - [Contribution](#contribution)

## Disclaimer

**The tool comes with no warranty. Use at your own risk.**

Please be patient and read through this readme carefully - it contains important information.

## Download

You can download the installer from the [Releases page](https://github.com/BartoszCichecki/LenovoLegionToolkit/releases/latest) or install using [winget](https://github.com/microsoft/winget-cli):

`winget install BartoszCichecki.LenovoLegionToolkit`

#### Next steps

LLT works best when it's running in the background, so go to Settings and enable _Autorun_ and _Minimize on close_. Next thing is to either disable Vantage and Hotkeys or just uninstall them. After that LLT will always run on startup and will take over all functions that were handled by Vantage and Hotkeys.

If you close LLT completely some functions will not work, like synchronizing power plans with current Power Mode (Fn+Q) or Actions.

Also, check out the [Compatibility](#compatibility) section below!

#### Problems with .NET?

If for whatever reason LLT installer did not setup .NET properly:
1. Go to https://dotnet.microsoft.com/en-us/download/dotnet/6.0
2. Find section ".NET Desktop Runtime"
3. Download x64 Windows installer
4. Run the installer

After following these steps, you can open Terminal and type: `dotnet --info`. In the output look for section `.NET runtimes installed`, in this section you should see something like:

`Microsoft.NETCore.App 6.0.15 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]`

and

`Microsoft.WindowsDesktop.App 6.0.15 [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]`

The exact version number can be different, but as long as it is `6.x.x` it should be fine. If after these steps LLT still shows an error on startup that .NET couldn't be found or similar, the problem is on your machine and not with LLT.

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
- Access to Custom Mode, including Fan Control on 2022 and newer models.
- Spectrum RGB, 4-zone RGB and White backlight keyboards support.
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
* M3CN32WW and higher
* M0CN27WW and higher

Not all features of Custom Mode are supported by all devices.

##### Deactivate discrete nVidia GPU

Sometimes discrete GPU stays active even when it should not. This can happen for example, if you work with an external screen and you disconnect it - some processes will keep running on discrete GPU keeping it alive and shortening battery life.

There are two ways to help the GPU deactivate:

1. killing all processes running on dGPU (this one seems to work better),
2. disabling dGPU for a short amount of time, which will force all processes to move to the integrated GPU.

Deactivate button will be enabled when dGPU is active, you have Hybrid mode enabled and there are no screens connected to dGPU. If you hover over the button, you will see the current P state of dGPU and the list of processes running on it.

Keep in mind that some apps may not like this feature and crash when you deactivate dGPU.

#### Overclock discrete nVidia GPUs

The overclock option is intended for simple overclocking, similar to the one available in Vantage. It is not intended to replace tools like Afterburner. Here are some points to keep in mind:
* Make sure GPU overclocking is enabled in BIOS, if your laptop has such option.
* Overclocking does not work with Vantage or LegionZone running in the background.
* It is not recommended to use the option while using other tools like Afterburner.
* If you edited your Dashboard, you might need to add the control manually.

##### Windows Power Plans

Lenovo Legion Toolkit will automatically switch Windows power plans when Power Mode changes *and* when Lenovo Vantage is disabled.

On some laptops though, Lenovo Vantage never switched power plans. If you have one of the laptops where Lenovo Vantage does not change Windows power plans automatically you can override this behavior in Settings. This will allow Toolkit to always change Windows power plans, even if Lenovo Vantage is running in the background.

Laptops that have S0 Low Power mode enabled, also known as Modern Standby, do not work well with mutliple power plans, especially with performance power plans.

## Donate

If you enjoy using the Lenovo Legion Toolkit, consider donating.

[Donate with PayPal](https://www.paypal.com/donate/?hosted_button_id=22AZE2NBP3HTL)

<img src="LenovoLegionToolkit.WPF/Assets/Donate/paypal_qr.png" width="200" alt="PayPal QR code" />


#### Donate from China

If you live in China, you can also donate in Chinese Yuan using Stripe (supports UnionPay and AliPay):

[Donate with Stripe](https://donate.stripe.com/14k8yM94I980f3q7ss)

<img src="LenovoLegionToolkit.WPF/Assets/Donate/stripe_cny_qr.png" width="200" alt="Stripe (CNY) QR code" />

## Credits

Special thanks to:

* [ViRb3](https://github.com/ViRb3), for creating [Lenovo Controller](https://github.com/ViRb3/LenovoController), which was used as a base for this tool
* [falahati](https://github.com/falahati), for creating [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) and [WindowsDisplayAPI](https://github.com/falahati/WindowsDisplayAPI)
* [SmokelessCPU](https://github.com/SmokelessCPU), for help with 4-zone RGB and Sprectrum keyboard support
* [Mario Bălănică](https://github.com/mariobalanica), for all contributions

Translations provided by:
* Bulgarian - [Ekscentricitet](https://github.com/Ekscentricitet)
* Chinese (Simplified) - [凌卡Karl](https://github.com/KarlLee830)
* Chinese (Traditional) - [flandretw](https://github.com/flandretw)
* Czech - J0sef
* Dutch - Melm, [JarneStaalPXL](https://github.com/JarneStaalPXL)
* French - EliotAku, [Georges de Massol](https://github.com/jojo2massol), Rigbone, ZeroDegree
* German - Sko-Inductor, Running_Dead89
* Greek - GreatApo
* Italian - [Lampadina17](https://github.com/Lampadina17)
* Karakalpak - KarLin, Gulnaz, Niyazbek Tolibaev, Shingis Joldasbaev
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

Sometimes new motherboard does not contain correct model numbers and serial numbers. You should try [this tutorial](https://laptopwiki.eu/laptopwiki/guides/lenovo/legion_bios_lvarrecovery) to try and recover them. If that method does not succeed, you can workaround by going to `%LOCALAPPDATA%\LenovoLegionToolkit` and creating an `args.txt` file. Inside that file paste `--skip-compat-check`. This will disable all compatibility checks in LLT. Use this workaround only if correct model number, serial number etc. can't be restored.

#### Why I can't switch to Performance or Custom Power Mode on battery?

Starting with version 2.11.0, LLT's behavior was aligned with Vantage and Legion Zone and it does not allow using them without an appropriate power source.

If for whatever reason you want to use these modes on battery anyway, you can use `--allow-all-power-modes-on-battery` argument. Note that Power Limits and other settings are not applied correctly on most devices when laptop is not connected to full power AC adapter and unpredictable and weird behavior is expected.

You have been warned, but here are the steps to allow all power modes without AC adapter:
1. Go to `%LOCALAPPDATA%\LenovoLegionToolkit`
2. Create `args.txt` file in there (if you already have it edit)
3. Paste `--allow-all-power-modes-on-battery`
4. Start LLT

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

#### Where can I find CPU Boost Mode settings?

Shortly speaking, in Windows Control Panel. This option was removed from LLT since it was quite annoying to maintain and it's available inside Windows Control Panel. In LLT's Settings you can find a link directly to the power plan setttings page in Control Panel where you can easily edit CPU Boost Mode settings as well as other settings of Windows power plans.

By default this setting is hidden, but can be revealed by running `powercfg.exe -attributes sub_processor perfboostmode -attrib_hide` in Terminal.

I also recommend other apps that make it very easy to manage settings across multiple power plans: [PowerSettingsExplorer](https://forums.guru3d.com/threads/windows-power-plan-settings-explorer-utility.416058/) and [QuickCPU](https://coderbag.com/product/quickcpu).

#### Can I customize hotkeys?

You can customize Fn+F9 hotkey in LLT settings. Other hotkeys can't be customized.

#### What if I overclocked my GPU too much?

If you end up in a situation where your GPU is not stable and you can't boot into Windows, there are two things you can do:

1. Go into BIOS and try to find and option similar to "Enabled GPU Overclocking" and disable it, start Windows, and toggle the BIOS option again to Enabled.
2. Start Windows in Safe Mode, and delete `gpu_oc.json` file under LLT settings, which are located in `"%LOCALAPPDATA%\LenovoLegionToolkit`.

#### Why do I get a message that Vantage is still running, even though I uninstalled it?

Starting from version 2.14.0, LLT is much more strict about detecting leftover processes related to Vantage. Vantage installs 3 components:

1. Lenovo Vantage app
2. Lenovo Vantage Service
3. System Interface Foundation V2 Device

The easiest solution is to go into LLT settings and selection options to disable Lenovo Vantage, LegionZone and Hotkeys (only still installed ones are shown).

If you want to remove them instead, make sure that you uninstall all 3, otherwise some options in LLT will not be available. You can check Task Manager for any processes containing "Vantage" or "ImController". You can also check this guide for more info: [Uninstalling System Interface Foundation V2 Device](https://support.lenovo.com/us/en/solutions/HT506070), if you have troubles getting rid of "ImController" processes.

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
