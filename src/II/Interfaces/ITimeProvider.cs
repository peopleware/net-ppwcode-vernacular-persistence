// Copyright 2017 by PeopleWare n.v..
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PPWCode.Vernacular.Persistence.II
{
    [ContractClass(typeof(ITimeProviderContract))]
    public interface ITimeProvider
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed")]
    [ContractClassFor(typeof(ITimeProvider))]
    public abstract class ITimeProviderContract : ITimeProvider
    {
        public DateTime UtcNow
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc);
                return default(DateTime);
            }
        }

        public DateTime Now
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Local);
                return default(DateTime);
            }
        }
    }
}
