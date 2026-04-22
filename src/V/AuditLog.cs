// Copyright 2026 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ComponentModel.DataAnnotations;

namespace PPWCode.Vernacular.Persistence.V
{
    public abstract class AuditLog<TId, TTimestamp>
        : PersistentObject<TId>
        where TId : IEquatable<TId>
        where TTimestamp : struct, IComparable<TTimestamp>, IEquatable<TTimestamp>
    {
        [Required]
        public virtual string? EntryType { get; set; }

        [Required]
        public virtual string? EntityName { get; set; }

        [Required]
        public virtual string? EntityId { get; set; }

        [Required]
        public virtual TTimestamp? CreatedAt { get; set; }

        [Required]
        public virtual string? CreatedBy { get; set; }

        public virtual string? PropertyName { get; set; }
        public virtual string? OldValue { get; set; }
        public virtual string? NewValue { get; set; }
    }
}
