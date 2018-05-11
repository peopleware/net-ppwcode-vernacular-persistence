using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable]
    public class DbNotNullConstraintException : DbConstraintException
    {
        public DbNotNullConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo)
            : this(message, entityId, entityName, sql, constraintName, extraInfo, null)
        {
        }

        public DbNotNullConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo, Exception innerException)
            : base(message, entityId, entityName, sql, DbConstraintTypeEnum.NOT_NULL, constraintName, extraInfo, innerException)
        {
        }

        [ExcludeFromCodeCoverage]
        protected DbNotNullConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}