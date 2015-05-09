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
    public abstract class AuditablePersistentObject<T>
        : InsertAuditablePersistentObject<T>,
          IAuditable
        where T : IEquatable<T>
    {
        [DataMember(Name = "LastModifiedAt")]
        private DateTime? m_LastModifiedAt;

        [DataMember(Name = "LastModifiedBy")]
        private string m_LastModifiedBy;

        protected AuditablePersistentObject(T id)
            : base(id)
        {
        }

        protected AuditablePersistentObject()
        {
        }

        [AuditLogPropertyIgnore]
        public virtual DateTime? LastModifiedAt
        {
            get { return m_LastModifiedAt; }
            set { m_LastModifiedAt = value; }
        }

        [AuditLogPropertyIgnore]
        public virtual string LastModifiedBy
        {
            get { return m_LastModifiedBy; }
            set { m_LastModifiedBy = value; }
        }
    }
}