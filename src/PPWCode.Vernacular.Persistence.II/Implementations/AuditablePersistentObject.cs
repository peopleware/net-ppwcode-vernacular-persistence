using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class AuditablePersistentObject<T>
        : InsertAuditablePersistentObject<T>,
          IAuditable
        where T : IEquatable<T>
    {
        protected AuditablePersistentObject(T id)
            : base(id)
        {
        }

        protected AuditablePersistentObject()
        {
        }

        [DataMember, AuditLogPropertyIgnore]
        public virtual DateTime? LastModifiedAt { get; set; }

        [DataMember, AuditLogPropertyIgnore]
        public virtual string LastModifiedBy { get; set; }
    }
}