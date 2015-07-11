﻿// <copyright company="RealDimensions Software, LLC" file="ApplicationParameters.cs">
//   Copyright 2015 - Present RealDimensions Software, LLC
// </copyright>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// 
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.package.verifier.Infrastructure.App
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Web;

    using chocolatey.package.verifier.Infrastructure.Configuration;

    /// <summary>
    ///   Parameters used application wide are found here.
    /// </summary>
    public static class ApplicationParameters
    {
        /// <summary>
        ///   Gets the name of the application
        /// </summary>
        public const string Name = "chocolatey.package.verifier";

        /// <summary>
        ///   The name of the connection string in the config
        /// </summary>
        public const string ConnectionStringName = "chocolatey.package.verifier";

        /// <summary>
        ///   The email regular expression
        /// </summary>
        public const string EmailRegularExpression = @"[.\S]+\@[.\S]+\.[.\S]+";

        /// <summary>
        ///   The phone number regular expression, just need the string to contain 10 digits
        /// </summary>
        public const string PhoneNumberRegularExpression = @"(\D*\d){10}\D*";

        /// <summary>
        ///   The zip code regular expression
        /// </summary>
        public const string ZipCodeRegularExpression = @"\d{5}";

        public const char WebSeparatorChar = '/';

        public const string DatabaseTransactionLockName = "yo";

        public static int RepositoryCacheIntervalMinutes
        {
            get
            {
                return Config.GetConfigurationSettings().RepositoryCacheIntervalMinutes;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether we are in Debug Mode?
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                return TryGetConfig(() => Config.GetConfigurationSettings().IsDebugMode, false);
            }
        }

        /// <summary>
        ///   Gets a value indicating whether OVLP should insert test data. This should be false unless locally testing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [insert test data]; otherwise, <c>false</c>.
        /// </value>
        public static bool InsertTestData
        {
            get
            {
                return ConfigurationManager.AppSettings["InsertTestData"].Equals(
                    bool.TrueString, 
                    StringComparison.InvariantCultureIgnoreCase);
            }
        }
        
        /// <summary>
        ///   Gets the connection string.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            }
        }

        /// <summary>
        ///   Gets the site URL.
        /// </summary>
        public static string SiteUrl
        {
            get
            {
                return Config.GetConfigurationSettings().SiteUrl;
            }
        }

        /// <summary>
        ///   Gets the file version.
        /// </summary>
        public static string FileVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = versionInfo.FileVersion;

                ////string version = Assembly.GetEntryAssembly().GetName().Version;
                return version;
            }
        }

        /// <summary>
        /// Gets the current user's name
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCurrentUserName()
        {
            string userName = Environment.UserName;

            if (HttpContext.Current != null)
            {
                var httpUser = HttpContext.Current.User;
                if (httpUser != null && httpUser.Identity != null)
                {
                    userName = httpUser.Identity.Name;
                }
            }

            return userName;
        }
        
        /// <summary>
        ///   Gets the system email address.
        /// </summary>
        /// <returns>The email address from system.net/mailsettings/from; otherwise null.</returns>
        public static string GetSystemEmailAddress()
        {
            return Config.GetConfigurationSettings().SystemEmailAddress;
        }

        private static T TryGetConfig<T>(Func<T> func, T defaultValue)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}