using System;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II.Exceptions
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