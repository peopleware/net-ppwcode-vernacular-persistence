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
#if NETSTANDARD2_0 || NET462_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
#endif

namespace PPWCode.Vernacular.Persistence.IV
{
#if NETSTANDARD2_0 || NET462_OR_GREATER
    [Serializable]
#endif
    public class DbCheckConstraintException : DbConstraintException
    {
        public DbCheckConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo)
            : this(message, entityId, entityName, sql, constraintName, extraInfo, null)
        {
        }

        public DbCheckConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo, Exception innerException)
            : base(message, entityId, entityName, sql, DbConstraintTypeEnum.CHECK, constraintName, extraInfo, innerException)
        {
        }

#if NETSTANDARD2_0 || NET462_OR_GREATER
        [ExcludeFromCodeCoverage]
        protected DbCheckConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
