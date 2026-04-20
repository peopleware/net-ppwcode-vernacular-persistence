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

namespace PPWCode.Vernacular.Persistence.V
{
    public abstract class VersionedPersistentObject<T, TVersion>
        : PersistentObject<T>,
          IVersionedPersistentObject<T, TVersion>
        where T : IEquatable<T>
        where TVersion : IEquatable<TVersion>
    {
        [AuditLogPropertyIgnore]
        public virtual TVersion? PersistenceVersion { get; init; }

        public override bool IsSame(IIdentity<T>? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!(other is IVersionedPersistentObject<T, TVersion> versionedOther))
            {
                return base.IsSame(other);
            }

            EqualityComparer<TVersion> equalityComparer = EqualityComparer<TVersion>.Default;

            return base.IsSame(other)
                   && !equalityComparer.Equals(PersistenceVersion, default(TVersion))
                   && !equalityComparer.Equals(versionedOther.PersistenceVersion, default(TVersion))
                   && equalityComparer.Equals(PersistenceVersion, versionedOther.PersistenceVersion);
        }
    }
}
