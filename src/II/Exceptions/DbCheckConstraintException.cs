using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable]
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

        [ExcludeFromCodeCoverage]
        protected DbCheckConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}