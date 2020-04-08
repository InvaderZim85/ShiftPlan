C# REST Service
===============

Hey ho, before you can start, you have to rebuild (F6) the project to load the missing NuGet packages. 
Alternative you can restore them via right click on the solution > Restore Nuget Packages.

The following packages are included in this project:
- Service framework
    - Topshelf (Version: 4.2.1)
    - Topshelf.NLog (Version: 4.2.1)
- Logging
    - NLog (Version: 4.6.8)
    - NLog.Config (Version: 4.6.8)
    - NLog.Schema (Version: 4.6.8)
- Helper
    - Newtonsoft.Json (Version: 12.0.3)
    - ZimLabs.Utility (Version: 0.1.3)


Notes
-----
Program.cs
    The Program.cs contains the Topshelf HostFactory. You can pass the command line argument "debug" to start the ServiceManager directly.

ServiceManager.cs
    This is your main entry point. Add your code to the Start method

So have fun!