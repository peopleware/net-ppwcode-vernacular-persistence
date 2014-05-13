using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class AuditableVersionedPersistentObject<T, TVersion>
        : InsertAuditableVersionedPersistentObject<T, TVersion>,
          IAuditable
        where T : IEquatable<T>
    {
        protected AuditableVersionedPersistentObject(T id, TVersion persistenceVersion)
            : base(id, persistenceVersion)
        {
        }

        protected AuditableVersionedPersistentObject(T id)
            : base(id)
        {
        }

        protected AuditableVersionedPersistentObject()
        {
        }

        [DataMember, AuditLogPropertyIgnore]
        public virtual DateTime? LastModifiedAt { get; set; }

        [DataMember, AuditLogPropertyIgnore]
        public virtual string LastModifiedBy { get; set; }
    }
}