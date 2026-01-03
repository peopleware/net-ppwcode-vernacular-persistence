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

using PPWCode.Vernacular.Exceptions.IV;
#if NETSTANDARD2_0 || NET462_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace PPWCode.Vernacular.Persistence.IV
{
#if NETSTANDARD2_0 || NET462_OR_GREATER
    [Serializable]
#endif
    public class RepositorySqlException : SemanticException
    {
        private const string SqlKey = "RepositorySqlException.Sql";

        public RepositorySqlException(string message, string sql)
            : this(message, sql, null)
        {
        }

        public RepositorySqlException(string message, string sql, Exception innerException)
            : base(message, innerException)
        {
            Sql = sql;
        }

#if NETSTANDARD2_0 || NET462_OR_GREATER
        protected RepositorySqlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        public string Sql
        {
            get => (string)Data[SqlKey];
            private set => Data[SqlKey] = value;
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            RepositorySqlException otherRepositorySqlException = other as RepositorySqlException;
            return result
                   && (otherRepositorySqlException != null)
                   && string.Equals(Sql, otherRepositorySqlException.Sql, StringComparison.InvariantCulture);
        }

        public override string ToString()
            => string.Format(@"Type: {0}; SqlString={1}", GetType().Name, Sql);
    }
}
