// Copyright 2018 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.III
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class AuditLog<T>
        : PersistentObject<T>
        where T : IEquatable<T>
    {
        [DataMember(Name = "CreatedAt")]
        private DateTime _createdAt;

        [DataMember(Name = "CreatedBy")]
        private string _createdBy;

        [DataMember(Name = "EntityId")]
        private string _entityId;

        [DataMember(Name = "EntityName")]
        private string _entityName;

        [DataMember(Name = "EntryType")]
        private string _entryType;

        [DataMember(Name = "NewValue")]
        private string _newValue;

        [DataMember(Name = "OldValue")]
        private string _oldValue;

        [DataMember(Name = "PropertyName")]
        private string _propertyName;

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
            get => _entryType;
            set => _entryType = value;
        }

        public virtual string EntityName
        {
            get => _entityName;
            set => _entityName = value;
        }

        public virtual string EntityId
        {
            get => _entityId;
            set => _entityId = value;
        }

        public virtual string PropertyName
        {
            get => _propertyName;
            set => _propertyName = value;
        }

        public virtual string OldValue
        {
            get => _oldValue;
            set => _oldValue = value;
        }

        public virtual string NewValue
        {
            get => _newValue;
            set => _newValue = value;
        }

        public virtual DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
        }

        public virtual string CreatedBy
        {
            get => _createdBy;
            set => _createdBy = value;
        }
    }
}
