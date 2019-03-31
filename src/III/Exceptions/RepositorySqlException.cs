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

using PPWCode.Vernacular.Exceptions.IV;

namespace PPWCode.Vernacular.Persistence.III
{
    [Serializable]
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

        protected RepositorySqlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

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
