﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
/**********************************************************
* USING NAMESPACES
**********************************************************/
using System.IO;
using QuantConnect.Configuration;
using QuantConnect.Interfaces;
using QuantConnect.Packets;

namespace QuantConnect.Tasks
{
    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Implementation of local/desktop job request:
    /// </summary>
    public class Tasks : ITaskHandler
    {
        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Configurations settings, lean runmode.
        /// </summary>
        private bool BacktestingMode
        {
            get
            {
                return Config.GetBool("backtesting-mode");
            }
        }

        /// <summary>
        /// Physical location of Algorithm DLL.
        /// </summary>
        private string AlgorithmLocation
        {
            get
            {
                // we expect this dll to be copied into the output directory
                return "QuantConnect.Algorithm.dll";
            }
        }

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Desktop/Local initialization of task/queue provider. 
        /// </summary>
        public void Initialize(bool liveMode)
        {
            //Nothing to do.
        }

        /// <summary>
        /// Desktop/Local Get Next Task - Get task from the Algorithm folder of VS Solution.
        /// </summary>
        /// <returns></returns>
        public AlgorithmNodePacket NextTask(out string location)
        {
            location = AlgorithmLocation;

            //If this isn't a backtesting mode/request, attempt a live job.
            if (!BacktestingMode)
            {
                var liveJob = new LiveNodePacket
                {
                    ResultEndpoint = ResultHandlerEndpoint.Console,
                    SetupEndpoint = SetupHandlerEndpoint.Tradier,
                    DataEndpoint = DataFeedEndpoint.Tradier,
                    TransactionEndpoint = TransactionHandlerEndpoint.Tradier,
                    RealTimeEndpoint = RealTimeEndpoint.LiveTrading,
                    Type = PacketType.LiveNode,
                    Algorithm = File.ReadAllBytes(AlgorithmLocation)
                };
                return liveJob;
            }

            //Default run a backtesting job.
            var backtestJob = new BacktestNodePacket(0, 0, "", new byte[] {}, 10000, "local")
            {
                ResultEndpoint = ResultHandlerEndpoint.Console,
                SetupEndpoint = SetupHandlerEndpoint.Console,
                DataEndpoint = DataFeedEndpoint.FileSystem,
                TransactionEndpoint = TransactionHandlerEndpoint.Backtesting,
                RealTimeEndpoint = RealTimeEndpoint.Backtesting,
                Type = PacketType.BacktestNode,
                Algorithm = File.ReadAllBytes(AlgorithmLocation)
            };
            return backtestJob;
        }

        /// <summary>
        /// Desktop/Local acknowledge the task processed. Nothing to do.
        /// </summary>
        /// <param name="job"></param>
        public void AcknowledgeTask(AlgorithmNodePacket job)
        {
            //
        }
    }

}
