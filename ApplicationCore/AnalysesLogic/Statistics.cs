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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace AnalysesEngine.Core
{

    /// <summary>
    /// This class exposes a BlockingCollection (based on the ConcurrentQueue) to make sure information can be gathered smotthly form other threads.
    /// This class is dedicated to print out statistical information.
    /// </summary>
    public class Statistics
    {


        private readonly ILog _logger = LogManager.GetLogger(typeof(Statistics));
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private static readonly BlockingCollection<StatisticsInfo> _statisticsQueue = new BlockingCollection<StatisticsInfo>();
        private Int64 _totalEvaluationsCount;
        private Int64 _totalEvaluationsErrorsCount;
        List<Task> _tasks = new List<Task>();
        CancellationTokenSource cancelToken = new CancellationTokenSource();

        public static BlockingCollection<StatisticsInfo> StatisticsQueue
        {
            get { return _statisticsQueue; }
        }

        public void Run()
        {
            _logger.Info("Starting Statistics...");
            _stopwatch.Start();
            _tasks.Add(Task.Run(() => PrintEvaluationsStatistics(cancelToken.Token)));

        }

        public void Stop()
        {
            // operation is done,this will make the blocking collection terminate
            _logger.Info("Stopping Statistics...");
            _statisticsQueue.CompleteAdding();

        }


        /// <summary>
        /// prints statistics on the console
        /// </summary>
        private void PrintEvaluationsStatistics(CancellationToken cancellationToken)
        {

            foreach (var statInfo in _statisticsQueue.GetConsumingEnumerable(cancellationToken))
            {
                    _totalEvaluationsCount += statInfo.EvaluationsCount;
                    _totalEvaluationsErrorsCount += statInfo.EvaluationsErrorCount;

                    _logger.InfoFormat(
                        "global-> {0:#00.00} eval/sec {5:#000} eval {1:#000} err | {3:#00.00}s - {2:#00.00} evals <- {4} ",
                        Math.Round(_totalEvaluationsCount/_stopwatch.Elapsed.TotalSeconds, 2),
                        _totalEvaluationsErrorsCount,
                        Math.Round(statInfo.EvaluationsCount / statInfo.Duration.TotalSeconds, 2),
                        Math.Round(statInfo.Duration.TotalSeconds, 2)
                        , statInfo.ElementName + ":" + statInfo.AnalyseName
                        , _totalEvaluationsCount
                        );
            }

        }

        /// <summary>
        /// This function is not used, but could be useful to diagnose code performances further
        /// </summary>
        /// <param name="cancelToken"></param>
        public void PrintSystemStatistics(CancellationToken cancelToken)
        {
            Process curProcess = Process.GetCurrentProcess();
            TimeSpan totcpu, usercpu;
            int convfactor = 1000000;
            double elapseCPUinMillisec;

            while (true)
            {

                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                curProcess.Refresh();

                totcpu = curProcess.TotalProcessorTime;
                elapseCPUinMillisec = totcpu.TotalMilliseconds;
                usercpu = curProcess.UserProcessorTime;

                _logger.InfoFormat("totcpu {0:F0} usrcpu {1:F0} Privbyte {2} virtbyte {3} CLR {4} GC0 {5},GC1 {6},GC2 {7}",
                     elapseCPUinMillisec, usercpu.TotalMilliseconds,
                    (curProcess.PrivateMemorySize64 / convfactor), (curProcess.VirtualMemorySize64 / convfactor),
                    (GC.GetTotalMemory(false) / convfactor), GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));

                Thread.Sleep(500);
            }




        }

    }
}
