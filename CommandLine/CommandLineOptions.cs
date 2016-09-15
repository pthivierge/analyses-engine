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
using CommandLine;
using CommandLine.Text;

namespace AnalysesEngine.CommandLine
{
    /// <summary>
    ///     Class that contains command line options, and associated variables.
    ///     see http://commandline.codeplex.com/
    ///     see https://github.com/gsscoder/commandline/wiki
    /// </summary>
    internal class CommandLineOptions
    {

        [Option('s', "server", HelpText = "AF Server to connect to", Required = true)]
        public string AFServerName { get; set; }

        [Option('d', "database", HelpText = "AF Database to use", Required = true)]
        public string AFDatabaseName { get; set; }

        [Option('e', "elements", HelpText = "full path of the file that contains the paths of the elements to calculate. The elements are separated by CRLF.  ", Required = true)]
        public string ElementsFile { get; set; }

        [Option("st", HelpText = "Calculation StartTime. i.e 2015-01-01", Required = true)]
        public string StartTime { get; set; }

        [Option("et", HelpText = "Calculation EndTime", Required = true)]
        public string EndTime { get; set; }

        [Option("calcInterval", HelpText = "Calculation interval - defines the time step at which calculations will be carried out. The value is in seconds, default is one hour (3600s).",DefaultValue = 3600, Required = false)]
        public int Interval { get; set; }

        [Option('t', "threadsCount", HelpText = "Threads Used for elements collections",DefaultValue = 1,Required = false)]
        public int AFThreadCount { get; set; }
        
        [Option('a', "AnalysesThreadsCount", HelpText = "Threads Used to calculate Analyses for each element", DefaultValue = 2, Required = false)]
        public int AnalysesthreadCount { get; set; }

        [Option('w', "DataWriterDelay", HelpText = "Sets the writing interval for the DataWriter thread", DefaultValue = 5, Required = false)]
        public int DataWriterDelay { get; set; }

        [Option('f', "outputFile", HelpText = "Name of the file to output data into.  If specified, data is not written into PI Data Archive. i.e c:\\temp\\datafile  or datafile, .csv extension will be added by the program", DefaultValue = null, Required = false)]
        public string OutputFile { get; set; }

        [Option("EnableWrite", HelpText = "Enables writing calculation results into PI Data Archive.  if -f option is specified, writes into text files.  ex: --EnableWrite", DefaultValue = false, Required = false)]
        public bool EnableWrite { get; set; }

        [Option("EnableAnalysesErrorOutput", HelpText = "Shows Analysis Error when evaluated", DefaultValue = false, Required = false)]
        public bool EnableAnalysesErrorOutput { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
