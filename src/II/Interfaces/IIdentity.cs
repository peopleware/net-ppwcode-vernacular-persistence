// Copyright 2014 by PeopleWare n.v..
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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