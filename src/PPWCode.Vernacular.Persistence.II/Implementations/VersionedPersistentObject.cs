using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class VersionedPersistentObject<T, TVersion>
        : PersistentObject<T>,
          IVersionedPersistentObject<T, TVersion>
        where T : IEquatable<T>
    {
        [DataMember]
        private TVersion m_PersistenceVersion;

        protected VersionedPersistentObject(T id, TVersion persistenceVersion)
            : base(id)
        {
            PersistenceVersion = persistenceVersion;
        }

        protected VersionedPersistentObject(T id)
            : base(id)
        {
        }

        protected VersionedPersistentObject()
        {
        }

        [AuditLogPropertyIgnore]
        public virtual TVersion PersistenceVersion
        {
            get { return m_PersistenceVersion; }
            private set { m_PersistenceVersion = value; }
        }
    }
}