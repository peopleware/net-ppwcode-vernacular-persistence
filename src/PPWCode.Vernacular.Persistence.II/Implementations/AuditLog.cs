using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class AuditLog<T>
        : PersistentObject<T>
        where T : IEquatable<T>
    {
        protected AuditLog(T id)
            : base(id)
        {
        }

        protected AuditLog()
            : this(default(T))
        {
        }

        [DataMember]
        public virtual string EntryType { get; set; }

        [DataMember]
        public virtual string EntityName { get; set; }

        [DataMember]
        public virtual string EntityId { get; set; }

        [DataMember]
        public virtual string PropertyName { get; set; }

        [DataMember]
        public virtual string OldValue { get; set; }

        [DataMember]
        public virtual string NewValue { get; set; }

        [DataMember]
        public virtual DateTime CreatedAt { get; set; }

        [DataMember]
        public virtual string CreatedBy { get; set; }
    }
}