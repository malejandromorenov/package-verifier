﻿// <copyright company="RealDimensions Software, LLC" file="TransactionLockObject.cs">
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

namespace chocolatey.package.verifier.Infrastructure.Synchronization
{
    /// <summary>
    ///   The internal locking object for transaction locks
    /// </summary>
    internal sealed class TransactionLockObject
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="TransactionLockObject" /> class.
        /// </summary>
        /// <param name="name">The name of the lock.</param>
        public TransactionLockObject(string name)
        {
            this.Name = name;
        }

        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
    }
}