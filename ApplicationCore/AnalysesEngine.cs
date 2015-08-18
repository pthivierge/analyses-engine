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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AnalysesEngine.Core.AnalysesLogic;
using AnalysesEngine.Core.Helpers;
using log4net;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;

namespace AnalysesEngine.Core
{
    public class AnalysesEngine
    {

        private static readonly ILog _logger = LogManager.GetLogger(typeof(AnalysesEngine));
       
        
        public void RunCalculations(IEnumerable<string> elementNames, DateTime startTime, DateTime endTime,
            TimeStampsGenerator.TimeStampsInterval interval, string afServerName, string afDatabaseName
            , int afThreadCount, int analysesThreadCount, int dataWriterDelay, string outputFile, bool enableWrite)
        {

            var statistics = new Statistics();
            var dateWriter = new DataWriter();
            var times = TimeStampsGenerator.Get(interval, startTime, endTime);
            var aftimes = times.Select(t => new AFTime(t)).ToList();
            ConcurrentQueue<List<AFValue>> dataWritingQueue = null;

            #region AFConnection

            // connects to the the PI system
            _logger.InfoFormat("Connecting to AF server:{0} and database: {1}...", afServerName, afDatabaseName);
            var afServers = new PISystems();
            var afServer = afServers[afServerName];
            AFDatabase database;
            if (!afServer.ConnectionInfo.IsConnected)
            {
                afServer.Connect();
                _logger.Info("AF server connected");
            }

            database = afServer.Databases[afDatabaseName];

            #endregion AFConnection

            statistics.Run();

            if (enableWrite)
            {
                dateWriter.Run(dataWriterDelay, outputFile);
            }
            
            var elements = new List<AFElement>();
            _logger.Info("Loading elements");
            foreach (var elementName in elementNames)
            {
                var element = database.Elements[elementName];

                if (element != null)
                    elements.Add(element);
                else
                    _logger.WarnFormat("Passed element name \"{0}\" did not return an Element object. AF Database {1}", elementName, database.Name);
            }

            AFElement.LoadElements(elements);

            _logger.InfoFormat("{0} Elements Loaded...", elements.Count);
            _logger.InfoFormat("Starting calculation from {0} to {1}. Means {2} evaluations per analysis to execute.", startTime, endTime, aftimes.Count);
            _logger.InfoFormat("Configuration:{0} threads for elements and {1} threads for Analyses", afThreadCount, analysesThreadCount);

            // for each element we carry out the calculations
            if (enableWrite) { dataWritingQueue = DataWriter.DataQueue;}
            Parallel.ForEach(elements, new ParallelOptions() { MaxDegreeOfParallelism = afThreadCount }, (element) => RunAnalysesParallel(element, aftimes, analysesThreadCount, dataWritingQueue));

        }


        public static void RunAnalysesParallel(AFElement element, List<AFTime> aftimes, int analysesThreadCount, ConcurrentQueue<List<AFValue>> dataQueue)
        {
            // for each analyse configured for our element
            Parallel.ForEach(element.Analyses, new ParallelOptions() { MaxDegreeOfParallelism = analysesThreadCount }, (afAnalysis) =>
            {
                TimeSpan evaluationTime;
                int evaluationErrorsCount;
                var analysisRunner = new AnalysisRunner();

                var results = analysisRunner.Run(afAnalysis, aftimes, out evaluationTime, out evaluationErrorsCount);

                var stats = new StatisticsInfo()
                {
                    AnalyseName = afAnalysis.Name,
                    Duration = evaluationTime,
                    ElementName = string.Format("{0}\\{1}", element.Parent.Name, element.Name),
                    EvaluationsCount = aftimes.Count,
                    EvaluationsErrorCount = evaluationErrorsCount

                };

                // we add statistics to the queue
                Statistics.StatisticsQueue.Add(stats);
                
                // send data to queue
                if (dataQueue != null)
                {
                    dataQueue.Enqueue(results);
                }
                    
                

            });
        }


    }
}
