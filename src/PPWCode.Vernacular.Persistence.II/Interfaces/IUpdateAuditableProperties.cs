// Copyright 2014 by PeopleWare n.v..
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

using System.Diagnostics.Contracts;

namespace PPWCode.Vernacular.Persistence.II
{
    [ContractClass(typeof(IUpdateAuditablePropertiesContract))]
    public interface IUpdateAuditableProperties
    {
        string LastModifiedAtPropertyName { get; }

        string LastModifiedByPropertyName { get; }
    }

    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof(IUpdateAuditableProperties))]
    public abstract class IUpdateAuditablePropertiesContract : IUpdateAuditableProperties
    {
        public string LastModifiedAtPropertyName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return default(string);
            }
        }

        public string LastModifiedByPropertyName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return default(string);
            }
        }
    }
}