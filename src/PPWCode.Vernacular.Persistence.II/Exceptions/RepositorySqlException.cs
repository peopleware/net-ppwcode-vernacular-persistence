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
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable]
    public class RepositorySqlException : SemanticException
    {
        public RepositorySqlException()
        {
        }

        public RepositorySqlException(string message)
            : base(message)
        {
        }

        public RepositorySqlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RepositorySqlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string SqlString
        {
            get { return (string)Data["SqlString"]; }
            set { Data["SqlString"] = value; }
        }

        public string Constraint
        {
            get { return (string)Data["Constraint"]; }
            set { Data["Constraint"] = value; }
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            RepositorySqlException otherRepositorySqlException = other as RepositorySqlException;
            return result
                   && otherRepositorySqlException != null
                   && string.Equals(SqlString, otherRepositorySqlException.SqlString, StringComparison.Ordinal)
                   && string.Equals(Constraint, otherRepositorySqlException.Constraint, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return string.Format(@"Type: {0}; SqlString={1}; Constraint={2}", GetType().Name, SqlString, Constraint);
        }
    }
}