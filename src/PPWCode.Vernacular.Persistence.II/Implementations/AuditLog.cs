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
    public abstract class AuditLog<T>
        : PersistentObject<T>
        where T : IEquatable<T>
    {
        [DataMember(Name = "CreatedAt")]
        private DateTime m_CreatedAt;

        [DataMember(Name = "CreatedBy")]
        private string m_CreatedBy;

        [DataMember(Name = "EntityId")]
        private string m_EntityId;

        [DataMember(Name = "EntityName")]
        private string m_EntityName;

        [DataMember(Name = "EntryType")]
        private string m_EntryType;

        [DataMember(Name = "NewValue")]
        private string m_NewValue;

        [DataMember(Name = "OldValue")]
        private string m_OldValue;

        [DataMember(Name = "PropertyName")]
        private string m_PropertyName;

        protected AuditLog(T id)
            : base(id)
        {
        }

        protected AuditLog()
            : this(default(T))
        {
        }

        public virtual string EntryType
        {
            get { return m_EntryType; }
            set { m_EntryType = value; }
        }

        public virtual string EntityName
        {
            get { return m_EntityName; }
            set { m_EntityName = value; }
        }

        public virtual string EntityId
        {
            get { return m_EntityId; }
            set { m_EntityId = value; }
        }

        public virtual string PropertyName
        {
            get { return m_PropertyName; }
            set { m_PropertyName = value; }
        }

        public virtual string OldValue
        {
            get { return m_OldValue; }
            set { m_OldValue = value; }
        }

        public virtual string NewValue
        {
            get { return m_NewValue; }
            set { m_NewValue = value; }
        }

        public virtual DateTime CreatedAt
        {
            get { return m_CreatedAt; }
            set { m_CreatedAt = value; }
        }

        public virtual string CreatedBy
        {
            get { return m_CreatedBy; }
            set { m_CreatedBy = value; }
        }
    }
}