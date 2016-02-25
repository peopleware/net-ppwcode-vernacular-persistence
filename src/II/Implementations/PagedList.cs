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
using System.Linq;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public class PagedList<T> : IPagedList<T>
    {
        private readonly IList<T> m_Items;
        private readonly int m_PageIndex;
        private readonly int m_PageSize;
        private readonly int m_TotalCount;
        private readonly int m_TotalPages;

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            Contract.Requires(source != null);
            Contract.Requires(pageIndex > 0);
            Contract.Requires(pageSize > 0);
            Contract.Requires(totalCount >= 0);

            m_PageIndex = pageIndex;
            m_PageSize = pageSize;
            m_TotalCount = totalCount;
            m_TotalPages = totalCount / pageSize;
            if (totalCount % pageSize > 0)
            {
                m_TotalPages++;
            }

            m_Items = source.ToList();
        }

        [DataMember(Order = 1)]
        public int PageIndex
        {
            get { return m_PageIndex; }
        }

        [DataMember(Order = 2)]
        public int PageSize
        {
            get { return m_PageSize; }
        }

        [DataMember(Order = 3)]
        public int TotalCount
        {
            get { return m_TotalCount; }
        }

        [DataMember(Order = 4)]
        public int TotalPages
        {
            get { return m_TotalPages; }
        }

        [DataMember(Order = 5)]
        public bool HasPreviousPage
        {
            get { return PageIndex > 1; }
        }

        [DataMember(Order = 6)]
        public bool HasNextPage
        {
            get { return PageIndex < TotalPages; }
        }

        [DataMember(Order = 7)]
        public IList<T> Items
        {
            get { return m_Items; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(TotalPages <= TotalCount);
        }
    }
}