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

using System.Text.Json.Serialization;

namespace PPWCode.Vernacular.Persistence.V;

/// <inheritdoc/>
public class PagedList<T> : IPagedList<T>
{
    public PagedList(IEnumerable<T> source, int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalCount / pageSize;
        if (totalCount % pageSize > 0)
        {
            TotalPages++;
        }

        Items = source.ToList();
    }

    /// <inheritdoc/>
    [JsonInclude]
    [JsonPropertyOrder(1)]
    public int Page { get; }

    /// <inheritdoc/>
    [JsonInclude]
    [JsonPropertyOrder(2)]
    public int PageSize { get; }

    /// <inheritdoc/>
    [JsonInclude]
    [JsonPropertyOrder(3)]
    public int TotalCount { get; }

    /// <inheritdoc/>
    [JsonInclude]
    [JsonPropertyOrder(4)]
    public int TotalPages { get; }

    /// <inheritdoc/>
    [JsonInclude]
    [JsonPropertyOrder(5)]
    public bool HasPreviousPage
        => Page > 1;

    /// <inheritdoc/>
    [JsonInclude]
    [JsonPropertyOrder(6)]
    public bool HasNextPage
        => Page < TotalPages;

    /// <inheritdoc/>
    [JsonInclude]
    [JsonPropertyOrder(7)]
    public IList<T> Items { get; }
}
