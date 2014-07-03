using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class InsertAuditablePersistentObject<T>
        : PersistentObject<T>,
          IInsertAuditable
        where T : IEquatable<T>
    {
        [DataMember]
        private DateTime? m_CreatedAt;

        [DataMember]
        private string m_CreatedBy;

        protected InsertAuditablePersistentObject(T id)
            : base(id)
        {
        }

        protected InsertAuditablePersistentObject()
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