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
using System.Collections.Generic;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable]
    public class IdNotFoundException<T, TId> : NotFoundException
        where T : class, IIdentity<TId>
        where TId : IEquatable<TId>
    {
        public IdNotFoundException()
        {
        }

        public IdNotFoundException(string message)
            : base(message)
        {
        }

        public IdNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IdNotFoundException(TId id)
            : base(null, null)
        {
            PersistentObjectType = GetType();
            Id = id;
        }

        protected IdNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type PersistentObjectType
        {
            get { return (Type)Data["PersistentObjectType"]; }
            private set { Data["PersistentObjectType"] = value; }
        }

        public TId Id
        {
            get { return (TId)Data["PersistenceId"]; }
            private set { Data["PersistenceId"] = value; }
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            IdNotFoundException<T, TId> otherIdNotFoundException = other as IdNotFoundException<T, TId>;
            return result
                   && otherIdNotFoundException != null
                   && PersistentObjectType == otherIdNotFoundException.PersistentObjectType
                   && EqualityComparer<TId>.Default.Equals(Id, otherIdNotFoundException.Id);
        }

        public override string ToString()
        {
            return string.Format(@"Type: {0}; PersistentObjectType={1}; Id={2}", GetType().Name, PersistentObjectType, Id);
        }
    }
}