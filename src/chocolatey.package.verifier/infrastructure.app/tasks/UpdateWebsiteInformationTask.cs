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

namespace chocolatey.package.verifier.infrastructure.app.tasks
{
    using System;
    using System.Net;
    using System.Text;
    using configuration;
    using infrastructure.messaging;
    using infrastructure.tasks;
    using messaging;
    using NuGet;
    using services;
    using HttpUtility = System.Web.HttpUtility;

    public class UpdateWebsiteInformationTask : ITask
    {
        private readonly IConfigurationSettings _configurationSettings;
        private readonly INuGetService _nugetService;
        private IDisposable _subscription;

        public UpdateWebsiteInformationTask(IConfigurationSettings configurationSettings, INuGetService nugetService)
        {
            _configurationSettings = configurationSettings;
            _nugetService = nugetService;
        }

        public void initialize()
        {
            _subscription = EventManager.subscribe<FinalPackageTestResultMessage>(update_website, null, null);
            this.Log().Info(() => "{0} is now ready and waiting for FinalPackageTestResultMessage".format_with(GetType().Name));
        }

        public void shutdown()
        {
            if (_subscription != null) _subscription.Dispose();
        }

        public event EventHandler<WebRequestEventArgs> SendingRequest = delegate { };

        private const string SERVICE_ENDPOINT = "/api/v2/test";

        private void update_website(FinalPackageTestResultMessage message)
        {
            this.Log().Info(() => "Updating website with success '{0}' and results url: '{1}'".format_with(message.Success, message.ResultDetailsUrl));

            var url = string.Join("/", SERVICE_ENDPOINT, message.PackageId, message.PackageVersion);
            HttpClient client = _nugetService.get_client(_configurationSettings.PackagesUrl, url, "POST", "application/x-www-form-urlencoded");

            StringBuilder postData = new StringBuilder();
            postData.Append("apikey=" + HttpUtility.UrlEncode(_configurationSettings.PackagesApiKey));
            postData.Append("&success=" + HttpUtility.UrlEncode(message.Success.to_string().to_lower()));
            postData.Append("&resultDetailsUrl=" + HttpUtility.UrlEncode(message.ResultDetailsUrl));
            var form = postData.ToString();
            var data = Encoding.ASCII.GetBytes(form);

            client.SendingRequest += (sender, e) =>
            {
                SendingRequest(this, e);
                var request = (HttpWebRequest)e.Request;
                request.Timeout = 30000;
                request.Headers.Add(_nugetService.ApiKeyHeader, _configurationSettings.PackagesApiKey);

                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            };

            _nugetService.ensure_successful_response(client);
        }

        // EventManager.publish(new WebsiteUpdateMessage());
    }
}
