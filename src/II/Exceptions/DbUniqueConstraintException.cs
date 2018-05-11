using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
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