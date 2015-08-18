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
using System;
using System.IO;
using AnalysesEngine.Core;
using CommandLine;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AnalysesEngine.Core.Helpers;

namespace AnalysesEngine.CommandLine
{
    /// <summary>
    ///     Command line program "Main"
    ///     logs are both sent to the log file and in the console
    ///     This can be configured in CommandLine.Log4Net.cfg
    ///     
    ///     Main program logic is implemented in AnalysesEngine.Core
    ///     This is the entry point for the command line
    /// </summary>
    /// 
    internal class Program
    {
        private static void Main(string[] args)
        {
            ILog _logger = LogManager.GetLogger(typeof (Program));

            IEnumerable<string> elementNames=null;
            

                _logger.Info("Command Line Started"); // you could delete this line ... 

                var options = new CommandLineOptions();


                // Checks for options passed in the command line
                if (Parser.Default.ParseArguments(args, options))
                {
                    
                    if(!File.Exists(options.ElementsFile))
                        throw new FileNotFoundException("Required file with elements names does not exist");

                    
                    // retrieves lines in the passed file, filters out comments (# comment).
                    elementNames = File.ReadLines(options.ElementsFile).ToList().Where(line=>!line.StartsWith("#"));

                    var engine = new Core.AnalysesEngine();
                    engine.RunCalculations(elementNames, options.StartTime.toDate(), options.EndTime.toDate(), options.Interval.ToEnum<TimeStampsGenerator.TimeStampsInterval>(), options.AFServerName, options.AFDatabaseName, options.AFThreadCount, options.AnalysesthreadCount, options.DataWriterDelay, options.OutputFile, options.EnableWrite);


                    // exit ok
                    Environment.Exit(0);
                }
                else
                {
                    // exit with error
                    Environment.Exit(1);
                }

        }
    }
}
