using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class UpdateAuditablePersistentObject<T> :
        PersistentObject<T>,
        IUpdateAuditable
        where T : IEquatable<T>
    {
        protected UpdateAuditablePersistentObject(T id)
            : base(id)
        {
        }

        protected UpdateAuditablePersistentObject()
        {
        }

        [DataMember, AuditLogPropertyIgnore]
        public virtual DateTime? LastModifiedAt { get; set; }

        [DataMember, AuditLogPropertyIgnore]
        public virtual string LastModifiedBy { get; set; }
    }
}