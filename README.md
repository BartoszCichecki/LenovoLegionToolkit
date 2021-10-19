# Lenovo Legion Toolkit

This is a small utility (less than 10MB) created for Lenovo Legion laptops, that allows to change a couple of features that are usually part of Lenovo Vantage.

It is a simple executable that needs no installation and runs no background services. It uses less memory and contains no telemetry.

![screenshot](.\assets\screenshot.png)



## Compatibility

* Lenovo Legion 5 Pro 16ACH6H


The application should work on most new Legion 2021 laptops, but since I have no way of testing if all features work correctly, it will not run on incompatible machines.



## Features

The app allows to:

* Switching between Quiet, Balance and Performance modes, including changing Windows power plans,

* Enabling and disabling Hybrid Mode,

* Changing Battery charging options: conservation, rapid charging and normal charging,

* Always on USB charge settings: on, off and on when sleeping,

* Flip to boot, Over Drive, Fn and Touchpad locks.

  

This app also allows you to disable Lenovo Vantage without uninstalling it. You can do it from the File menu on top. If you disable Vantage, it will stop and disable it's services, and disable all funky Lenovo Scheduled tasks.

Once Vantage is disabled, it will no longer start any background processes on startup. If you want to open Lenovo Vantage, you need to re-enable it from the tool, otherwise it will prompt to reinstall itself.



## Requirements

This tool is written with .NET 5 which means you need to have **.NET Desktop Runtime 5**. You can get it from here:

https://dotnet.microsoft.com/download/dotnet/5.0



## Credits

Credits go to ViRb3, for creating Lenovo Controller, which was used as a base for this tool.

Check out his repo: https://github.com/ViRb3/LenovoController
