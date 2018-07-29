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
    public abstract class InsertAuditablePersistentObject<T>
        : PersistentObject<T>,
          IInsertAuditable
        where T : IEquatable<T>
    {
        [DataMember(Name = "CreatedAt")]
        private DateTime? _createdAt;

        [DataMember(Name = "CreatedBy")]
        private string _createdBy;

        protected InsertAuditablePersistentObject(T id)
            : base(id)
        {
        }

        protected InsertAuditablePersistentObject()
        {
        }

        [AuditLogPropertyIgnore]
        public virtual DateTime? CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
        }

        [AuditLogPropertyIgnore]
        public virtual string CreatedBy
        {
            get => _createdBy;
            set => _createdBy = value;
        }
    }
}
