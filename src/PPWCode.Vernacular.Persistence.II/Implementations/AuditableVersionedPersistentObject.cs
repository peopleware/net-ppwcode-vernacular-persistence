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
        [DataMember]
        private DateTime? m_LastModifiedAt;

        [DataMember]
        private string m_LastModifiedBy;

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

        [AuditLogPropertyIgnore]
        public virtual DateTime? LastModifiedAt
        {
            get { return m_LastModifiedAt; }
            set { m_LastModifiedAt = value; }
        }

        [AuditLogPropertyIgnore]
        public virtual string LastModifiedBy
        {
            get { return m_LastModifiedBy; }
            set { m_LastModifiedBy = value; }
        }
    }
}