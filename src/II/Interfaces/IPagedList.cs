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

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace PPWCode.Vernacular.Persistence.II
{
    [ContractClass(typeof(IPagedListContract<>))]
    public interface IPagedList<T>
    {
        [Pure]
        int PageIndex { get; }

        [Pure]
        int PageSize { get; }

        [Pure]
        int TotalCount { get; }

        [Pure]
        int TotalPages { get; }

        [Pure]
        bool HasPreviousPage { get; }

        [Pure]
        bool HasNextPage { get; }

        IList<T> Items { get; }
    }

    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof(IPagedList<>))]
    public abstract class IPagedListContract<T> : IPagedList<T>
    {
        [Pure]
        public int PageIndex
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);

                return default(int);
            }
        }

        [Pure]
        public int PageSize
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);

                return default(int);
            }
        }

        [Pure]
        public int TotalCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return default(int);
            }
        }

        [Pure]
        public int TotalPages
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == ((TotalCount / PageSize) + (TotalCount % PageSize > 0 ? 1 : 0)));

                return default(int);
            }
        }

        [Pure]
        public bool HasPreviousPage
        {
            get
            {
                Contract.Ensures(Contract.Result<bool>() == (PageIndex > 1));

                return default(bool);
            }
        }

        [Pure]
        public bool HasNextPage
        {
            get
            {
                Contract.Ensures(Contract.Result<bool>() == (PageIndex < TotalPages));

                return default(bool);
            }
        }

        public IList<T> Items
        {
            get
            {
                Contract.Ensures(Contract.Result<IList<T>>() != null);

                return default(IList<T>);
            }
        }
    }
}