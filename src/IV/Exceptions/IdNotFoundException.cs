// Copyright 2026 by PeopleWare n.v..
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
using System.Collections.Generic;

using PPWCode.Vernacular.Exceptions.IV;
#if NETSTANDARD2_0 || NET462_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace PPWCode.Vernacular.Persistence.IV
{
#if NETSTANDARD2_0 || NET462_OR_GREATER
    [Serializable]
#endif
    public class IdNotFoundException<T, TId> : NotFoundException
        where T : class, IIdentity<TId>
        where TId : IEquatable<TId>
    {
        public IdNotFoundException(TId id)
            : this(id, null)
        {
        }

        public IdNotFoundException(TId id, Exception innerException)
            : base(null, innerException)
        {
            PersistentObjectType = typeof(T);
            Id = id;
        }

#if NETSTANDARD2_0 || NET462_OR_GREATER
        protected IdNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        public Type PersistentObjectType
        {
            get => (Type)Data["PersistentObjectType"];
            private set => Data["PersistentObjectType"] = value;
        }

        public TId Id
        {
            get => (TId)Data["PersistenceId"];
            private set => Data["PersistenceId"] = value;
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            return result
                   && other is IdNotFoundException<T, TId> otherIdNotFoundException
                   && (PersistentObjectType == otherIdNotFoundException.PersistentObjectType)
                   && EqualityComparer<TId>.Default.Equals(Id, otherIdNotFoundException.Id);
        }

        public override string ToString()
            => $"Type: {GetType().Name}; PersistentObjectType={PersistentObjectType}; Id={Id}";
    }
}
