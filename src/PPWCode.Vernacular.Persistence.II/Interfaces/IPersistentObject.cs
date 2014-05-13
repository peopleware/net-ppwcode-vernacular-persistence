using System;

namespace PPWCode.Vernacular.Persistence.II
{
    public interface IPersistentObject<T>
        : IIdentity<T>
        where T : IEquatable<T>
    {
    }
}