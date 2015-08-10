﻿// Copyright © 2015 - Present RealDimensions Software, LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// 
// You may obtain a copy of the License at
// 
// 	http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.package.verifier.Host
{
    using System;
    using System.ServiceProcess;
    using SimpleInjector;
    using host.infrastructure.registration;
    using infrastructure.app;
    using infrastructure.app.registration;
    using infrastructure.tasks;
    using log4net;

    /// <summary>
    ///   The service that registers tasks and schedules to run
    /// </summary>
    public partial class Service : ServiceBase
    {
        private readonly ILog _logger;
        private Container _container;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Service" /> class.
        /// </summary>
        public Service()
        {
            InitializeComponent();
            Bootstrap.initialize();
            _logger = LogManager.GetLogger(typeof(Service));
        }

        /// <summary>
        ///   Runs as console.
        /// </summary>
        /// <param name="args">The args.</param>
        public void run_as_console(string[] args)
        {
            OnStart(args);
            OnStop();
        }

        /// <summary>
        ///   When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the Start command.</param>
        protected override void OnStart(string[] args)
        {
            _logger.InfoFormat("Starting {0} service.", ApplicationParameters.Name);

            try
            {
                Bootstrap.startup();
                ////AutoMapperInitializer.Initialize();
                SimpleInjectorContainer.start();
                _container = SimpleInjectorContainer.Container;

                var tasks = _container.GetAllInstances<ITask>();
                foreach (var task in tasks)
                {
                    task.initialize();
                }

                _logger.InfoFormat("{0} service is now operational.", ApplicationParameters.Name);

                if ((args.Length > 0) && (Array.IndexOf(args, "/console") != -1))
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            } catch (Exception ex)
            {
                _logger.ErrorFormat(
                    "{0} service had an error on {1} (with user {2}):{3}{4}",
                    ApplicationParameters.Name,
                    Environment.MachineName,
                    Environment.UserName,
                    Environment.NewLine,
                    ex);
            }
        }

        /// <summary>
        ///   When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                _logger.InfoFormat("Stopping {0} service.", ApplicationParameters.Name);

                if (_container != null)
                {
                    var tasks = _container.GetAllInstances<ITask>();
                    foreach (var task in tasks.or_empty_list_if_null())
                    {
                        task.shutdown();
                    }
                }

                Bootstrap.shutdown();
                SimpleInjectorContainer.stop();

                _logger.InfoFormat("{0} service has shut down.", ApplicationParameters.Name);
            } catch (Exception ex)
            {
                _logger.ErrorFormat(
                    "{0} service had an error on {1} (with user {2}):{3}{4}",
                    ApplicationParameters.Name,
                    Environment.MachineName,
                    Environment.UserName,
                    Environment.NewLine,
                    ex);
            }
        }
    }
}