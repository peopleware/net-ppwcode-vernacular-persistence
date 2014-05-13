using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class UpdateAuditableVersionedPersistentObject<T, TVersion>
        : VersionedPersistentObject<T, TVersion>,
          IUpdateAuditable
        where T : IEquatable<T>
    {
        protected UpdateAuditableVersionedPersistentObject(T id, TVersion persistenceVersion)
            : base(id, persistenceVersion)
        {
        }

        protected UpdateAuditableVersionedPersistentObject(T id)
            : base(id)
        {
        }

        protected UpdateAuditableVersionedPersistentObject()
        {
        }

        [DataMember, AuditLogPropertyIgnore]
        public virtual DateTime? LastModifiedAt { get; set; }

        [DataMember, AuditLogPropertyIgnore]
        public virtual string LastModifiedBy { get; set; }
    }
}