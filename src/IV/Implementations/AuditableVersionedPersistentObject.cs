﻿// Copyright 2018 by PeopleWare n.v..
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

namespace PPWCode.Vernacular.Persistence.IV
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class AuditableVersionedPersistentObject<T, TVersion>
        : InsertAuditableVersionedPersistentObject<T, TVersion>,
          IAuditable
        where T : IEquatable<T>
        where TVersion : IEquatable<TVersion>
    {
        [DataMember(Name = "LastModifiedAt")]
        private DateTime? _lastModifiedAt;

        [DataMember(Name = "LastModifiedBy")]
        private string _lastModifiedBy;

        protected AuditableVersionedPersistentObject(T id, TVersion persistenceVersion)
            : base(id, persistenceVersion)
        {
        }

        protected AuditableVersionedPersistentObject(T id)
            : base(id)
        {
        }

        protected AuditableVersionedPersistentObject()
        {
        }

        [AuditLogPropertyIgnore]
        public virtual DateTime? LastModifiedAt
        {
            get => _lastModifiedAt;
            set => _lastModifiedAt = value;
        }

        [AuditLogPropertyIgnore]
        public virtual string LastModifiedBy
        {
            get => _lastModifiedBy;
            set => _lastModifiedBy = value;
        }
    }
}
