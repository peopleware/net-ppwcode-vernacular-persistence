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
        [DataMember]
        private DateTime? m_LastModifiedAt;

        [DataMember]
        private string m_LastModifiedBy;

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