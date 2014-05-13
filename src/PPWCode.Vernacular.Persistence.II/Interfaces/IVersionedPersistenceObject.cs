using System;

namespace PPWCode.Vernacular.Persistence.II
{
    public interface IVersionedPersistentObject<TId, TVersion> : IPersistentObject<TId>
        where TId : IEquatable<TId>
    {
        TVersion PersistenceVersion { get; }
    }
}