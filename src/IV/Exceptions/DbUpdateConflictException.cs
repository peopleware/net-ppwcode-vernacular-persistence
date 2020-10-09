// Copyright 2020 by PeopleWare n.v..
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
using System.Text;

using PPWCode.Vernacular.Exceptions.IV;

namespace PPWCode.Vernacular.Persistence.IV
{
    [Serializable]
    public class DbUpdateConflictException : RepositorySqlException
    {
        private const string EntityIdKey = "DbUpdateConflictException.EntityId";
        private const string EntityNameKey = "DbUpdateConflictException.EntityName";

        public DbUpdateConflictException(
            string message,
            object entityId,
            string entityName,
            string sql,
            Exception innerException)
            : base(message, sql, innerException)
        {
            EntityId = entityId;
            EntityName = entityName;
        }

        [ExcludeFromCodeCoverage]
        protected DbUpdateConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public object EntityId
        {
            get => Data[EntityIdKey];
            private set => Data[EntityIdKey] = value;
        }

        public string EntityName
        {
            get => (string)Data[EntityNameKey];
            private set => Data[EntityNameKey] = value;
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            return result
                   && other is DbUpdateConflictException e
                   && (EntityId == e.EntityId)
                   && string.Equals(EntityName, e.EntityName, StringComparison.InvariantCulture);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("EntityId: {0}", EntityId);
            sb.AppendLine();
            sb.AppendFormat("EntityName: {0}", EntityName);
            sb.AppendLine();
            sb.AppendFormat("Sql: {0}", Sql);
            sb.AppendLine();
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
   }
}
