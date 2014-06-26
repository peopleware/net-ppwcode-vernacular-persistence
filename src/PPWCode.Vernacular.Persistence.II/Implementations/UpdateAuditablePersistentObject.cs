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
        [DataMember]
        private DateTime? m_LastModifiedAt;

        [DataMember]
        private string m_LastModifiedBy;

        protected UpdateAuditablePersistentObject(T id)
            : base(id)
        {
        }

        protected UpdateAuditablePersistentObject()
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