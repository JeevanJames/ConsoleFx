using System.Resources;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("ConsoleFx")]
[assembly: AssemblyDescription(@"ConsoleFx is a framework to build command-line applications, with support for command-line argument parsing and handling, including built-in error handling and validation support.
This is the binary package for the ConsoleFx core, which provides a fluent API for creating command-line applications. Use this package if you want add the ConsoleFx core assembly references to your application instead of having a dependency on the ConsoleFx assemblies.
If you wish to embed the ConsoleFx code in your application, then use the ConsoleFx.Code package instead.
Visit http://consolefx.codeplex.com/wikipage?title=Adv_Deployment for details on ConsoleFx deployment scenarios.
Visit the project URL for documentation or download the full package to obtain the sample applications.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Jeevan James")]
[assembly: AssemblyProduct("ConsoleFx")]
[assembly: AssemblyCopyright("Copyright © Jeevan James 2007-2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyInformationalVersion("2.0.0-rc03")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("ConsoleFx.CmdLineParser.UnixStyle")]
[assembly: InternalsVisibleTo("ConsoleFx.CmdLineParser.WindowsStyle")]
