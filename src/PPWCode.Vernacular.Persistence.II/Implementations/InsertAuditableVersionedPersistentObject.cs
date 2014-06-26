using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class InsertAuditableVersionedPersistentObject<T, TVersion>
        : VersionedPersistentObject<T, TVersion>,
          IInsertAuditable
        where T : IEquatable<T>
    {
        [DataMember]
        private DateTime? m_CreatedAt;

        [DataMember]
        private string m_CreatedBy;

        protected InsertAuditableVersionedPersistentObject(T id, TVersion persistenceVersion)
            : base(id, persistenceVersion)
        {
        }

        protected InsertAuditableVersionedPersistentObject(T id)
            : base(id)
        {
        }

        protected InsertAuditableVersionedPersistentObject()
        {
        }

        [AuditLogPropertyIgnore]
        public virtual DateTime? CreatedAt
        {
            get { return m_CreatedAt; }
            set { m_CreatedAt = value; }
        }

        [AuditLogPropertyIgnore]
        public virtual string CreatedBy
        {
            get { return m_CreatedBy; }
            set { m_CreatedBy = value; }
        }
    }
}