// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace PPWCode.Vernacular.Persistence.V;

/// <summary>
///     Represents one page of a paged list of items.
/// </summary>
/// <typeparam name="T">the type of the items</typeparam>
public interface IPagedList<T>
{
    /// <summary>
    ///     The number of the page
    /// </summary>
    /// <remarks>
    ///     For the first page this has the value <c>1</c>
    /// </remarks>
    int Page { get; }

    /// <summary>
    ///     The size of the page
    /// </summary>
    int PageSize { get; }

    /// <summary>
    ///     The total number of items in the complete list
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    ///     The total number of pages in the complete list
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    ///     Does this page have a previous page?
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    ///     Does this page have a next page?
    /// </summary>
    bool HasNextPage { get; }

    /// <summary>
    ///     The list of items on this page
    /// </summary>
    IList<T> Items { get; }
}
