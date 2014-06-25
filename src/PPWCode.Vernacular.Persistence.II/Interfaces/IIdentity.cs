using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace PPWCode.Vernacular.Persistence.II
{
    [ContractClass(typeof(IIdentityContract<>))]
    public interface IIdentity<T>
        where T : IEquatable<T>
    {
        T Id { get; }

        [Pure]
        bool IsTransient { get; }

        [Pure]
        bool IsSame(IIdentity<T> other);
    }

    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof(IIdentity<>))]
    public abstract class IIdentityContract<T> : IIdentity<T>
        where T : IEquatable<T>
    {
        public abstract T Id { get; }

        [Pure]
        public bool IsTransient
        {
            get
            {
                Contract.Ensures(Contract.Result<bool>() == EqualityComparer<T>.Default.Equals(Id, default(T)));
                return default(bool);
            }
        }

        public abstract bool IsSame(IIdentity<T> other);
    }
}