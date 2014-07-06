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

using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class VersionedPersistentObject<T, TVersion>
        : PersistentObject<T>,
          IVersionedPersistentObject<T, TVersion>
        where T : IEquatable<T>
    {
        [DataMember]
        private TVersion m_PersistenceVersion;

        protected VersionedPersistentObject(T id, TVersion persistenceVersion)
            : base(id)
        {
            PersistenceVersion = persistenceVersion;
        }

        protected VersionedPersistentObject(T id)
            : base(id)
        {
        }

        protected VersionedPersistentObject()
        {
        }

        [AuditLogPropertyIgnore]
        public virtual TVersion PersistenceVersion
        {
            get { return m_PersistenceVersion; }
            private set { m_PersistenceVersion = value; }
        }
    }
}