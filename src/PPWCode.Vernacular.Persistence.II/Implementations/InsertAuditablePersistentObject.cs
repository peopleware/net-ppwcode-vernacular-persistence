using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class InsertAuditablePersistentObject<T>
        : PersistentObject<T>,
          IInsertAuditable
        where T : IEquatable<T>
    {
        protected InsertAuditablePersistentObject(T id)
            : base(id)
        {
        }

        protected InsertAuditablePersistentObject()
        {
        }

        [DataMember, AuditLogPropertyIgnore]
        public virtual DateTime? CreatedAt { get; set; }

        [DataMember, AuditLogPropertyIgnore]
        public virtual string CreatedBy { get; set; }
    }
}