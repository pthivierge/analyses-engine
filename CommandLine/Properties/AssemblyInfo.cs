#region Copyright
// /*
// 
//    Copyright 2015 Patrice Thivierge Fortin
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  
//  */
#endregion
using System.Reflection;
using System.Runtime.InteropServices;
using log4net.Config;

[assembly: XmlConfigurator(ConfigFile = "AnalysesEngineCommandLine.log4net.cfg.xml", Watch = true)]

//Company shipping the assembly

[assembly: AssemblyCompany("OSISoft - Development Support")]

//Friendly name for the assembly

[assembly: AssemblyTitle("Analyses Calculation Engine Command Line")]

//Short description of the assembly

[assembly: AssemblyDescription("Analyses Calculation Engine command line version of the application.")]
[assembly: AssemblyConfiguration("")]

//Product Name

[assembly: AssemblyProduct("Analyses Calculation Engine")]

//Copyright information

[assembly: AssemblyCopyright("Copyright OSISoft - Development Support © 2015")]

//Enumeration indicating the target culture for the assembly

[assembly: AssemblyCulture("")]

//

[assembly: ComVisible(false)]

// COM GUID if exposed to com

[assembly: Guid("2D97AA3F-4E29-41CB-858E-DB4A30E54E28")]

//Version number expressed as a string

[assembly: AssemblyVersion("1.0.*")]
//[assembly: AssemblyFileVersion("1.0.0.0")]
