// Copyright 2010-2016 by PeopleWare n.v..
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
using System.ComponentModel;
using System.Diagnostics.Contracts;

using PPWCode.Vernacular.Exceptions.I;

namespace PPWCode.Vernacular.Persistence.I
{
    [ContractClass(typeof(IAuditableObjectContract))]
    public interface IAuditableObject :
        IPersistentObject
    {
        DateTime? CreatedAt { get; set; }

        string CreatedBy { get; set; }

        DateTime? LastModifiedAt { get; set; }

        string LastModifiedBy { get; set; }
    }

    /// <exclude />
    /// <summary>This is the contract class for <see cref="IAuditableObject" />.</summary>
    [ContractClassFor(typeof(IAuditableObject))]
    public abstract class IAuditableObjectContract :
        IAuditableObject
    {
        public DateTime? CreatedAt
        {
            get { return default(DateTime?); }
            set { Contract.Ensures(CreatedAt == value); }
        }

        public string CreatedBy
        {
            get { return default(string); }
            set { Contract.Ensures(CreatedBy == value); }
        }

        public DateTime? LastModifiedAt
        {
            get { return default(DateTime?); }
            set { Contract.Ensures(LastModifiedAt == value); }
        }

        public string LastModifiedBy
        {
            get { return default(string); }
            set { Contract.Ensures(LastModifiedBy == value); }
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public abstract long? PersistenceId { get; set; }

        public abstract bool IsTransient { get; }

        public abstract bool HasSamePersistenceId(IPersistentObject other);

        public abstract bool IsSerialized { get; }

#pragma warning disable

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore

        public abstract bool IsCivilized();

        public abstract CompoundSemanticException WildExceptions();

        public abstract void ThrowIfNotCivilized();
    }
}
