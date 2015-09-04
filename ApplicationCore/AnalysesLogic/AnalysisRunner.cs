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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;

namespace AnalysesEngine.Core
{
    /// <summary>
    /// Class that manage Analysis calculation, from a list of timestamps
    /// </summary>
    public class AnalysisRunner
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AnalysisRunner));
        private bool _debug;


        public AnalysisRunner(bool debug = false)
        {
            _debug = debug;
        }

        /// <summary>
        /// Run calculations for specific time stamps.
        /// </summary>
        /// inspired from:
        /// <see cref="https://pisquare.osisoft.com/message/28537#28537"/>
        /// <param name="analysis">Analysis that needs to be evaluated</param>
        /// <param name="times">List of times</param>
        /// <param name="evaluationTime">output, contains the time taken to execute all the calculations</param>
        /// <param name="evaluationsErrorsCount">output, contains the number of evaluations in error.</param>
        /// <returns></returns>
        public List<AFValue> Run(AFAnalysis analysis, IEnumerable<AFTime> times, out TimeSpan evaluationTime, out int evaluationsErrorsCount)
        {
            evaluationsErrorsCount = 0;
            var results = new List<AFValue>();
            var stopwatch = Stopwatch.StartNew();


            var analysisConfiguration = analysis.AnalysisRule.GetConfiguration();
            var state = new AFAnalysisRuleState(analysisConfiguration);

            foreach (var time in times)
            {
               //  Console.WriteLine("Evaluating for {0}", time);
                state.Reset();
                state.SetExecutionTimeAndPopulateInputs(time);
                analysis.AnalysisRule.Run(state);

                if (state.EvaluationError != null)
                {
                    if (_debug)
                        _logger.ErrorFormat("Analyse in error at time: {3:s}  - {0} - at {1}, Error: {2}",analysis.Name, analysis.GetPath(), state.EvaluationError,time);

                    evaluationsErrorsCount += 1;
                }
                

                // this merges the state (results) with the configuration so its easier to loop with both...
                var resultSet = analysisConfiguration.ResolvedOutputs.Zip(state.Outputs, Tuple.Create);

                foreach (var result in resultSet)
                {
                    // for more clarty, we take out our data into clearer variables
                    AFAnalysisRuleResolvedOutput analysisRow = result.Item1;
                    var calcRes = (AFValue)result.Item2;

                    // we filter to get only the results that have an output attribute ( an AFValue )
                    if (analysisRow.Attribute != null)
                    {
                        // add new AF Value into the results table
                        results.Add(new AFValue((AFAttribute)analysisRow.Attribute, calcRes.Value,
                            calcRes.Timestamp));
                    }
                }
            }
            
            stopwatch.Stop();
            evaluationTime = stopwatch.Elapsed;
            return results;
        }


        /* Not used, for information

        /// <summary>
        /// Checks the configuration for errors
        /// https://pisquare.osisoft.com/message/28537
        /// </summary>
        /// <param name="configuration"></param>
        private void CheckForErrors(AFAnalysisRuleConfiguration configuration)
        {
            if (configuration.HasExceptions)
            {
                var exceptionGroups = configuration.ConfigurationExceptions.ToLookup(ex => ex.Severity);
                Console.WriteLine("Configuration warnings: {0}", exceptionGroups[AFAnalysisErrorSeverity.Warning].Count());
                foreach (var warning in exceptionGroups[AFAnalysisErrorSeverity.Warning])
                {
                    Console.WriteLine("\t{0}", warning.Message);
                }

                // warnings mean ok to run, but something may not work quite as expected.  

                int errorCount = exceptionGroups[AFAnalysisErrorSeverity.Error].Count();
                Console.WriteLine("Configuration errors: {0}", errorCount);
                foreach (var error in exceptionGroups[AFAnalysisErrorSeverity.Error])
                {
                    Console.WriteLine("\t{0}", error.Message);
                }

                // can't run if there are errors.  
                if (errorCount > 0)
                {
                    Environment.Exit(0);
                }
            }
        }
     
     */

    }
}