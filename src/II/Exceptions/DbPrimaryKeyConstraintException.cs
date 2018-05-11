using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable]
    public class DbPrimaryKeyConstraintException : DbConstraintException
    {
        public DbPrimaryKeyConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo)
            : this(message, entityId, entityName, sql, constraintName, extraInfo, null)
        {
        }

        public DbPrimaryKeyConstraintException(string message, object entityId, string entityName, string sql, string constraintName, string extraInfo, Exception innerException)
            : base(message, entityId, entityName, sql, DbConstraintTypeEnum.PRIMARY_KEY, constraintName, extraInfo, innerException)
        {
        }

        [ExcludeFromCodeCoverage]
        protected DbPrimaryKeyConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}