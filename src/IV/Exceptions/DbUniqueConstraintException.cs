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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.IV
{
    [Serializable]
    public class DbUniqueConstraintException : DbConstraintException
    {
        public DbUniqueConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo)
            : this(message, entityId, entityName, sql, constraintName, extraInfo, null)
        {
        }

        public DbUniqueConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo, Exception innerException)
            : base(message, entityId, entityName, sql, DbConstraintTypeEnum.UNIQUE, constraintName, extraInfo, innerException)
        {
        }

        [ExcludeFromCodeCoverage]
        protected DbUniqueConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
