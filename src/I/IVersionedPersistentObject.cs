// Copyright 2010-2016 by PeopleWare n.v..
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

using System.ComponentModel;
using System.Diagnostics.Contracts;

using PPWCode.Vernacular.Exceptions.I;

namespace PPWCode.Vernacular.Persistence.I
{
    /// <summary>
    ///     <p>
    ///         Persistency should always be implemented with versioning (optimistic locking). For that, the property
    ///         <see cref="PersistenceVersion" /> is added to this interface. There are several different possible
    ///         types of versioning, using an integer, date, or even a GUID. For that reason, the type of the property
    ///         is generic. We advise however to use Integer as version type. The property is kept generic to allow
    ///         for legacy systems.
    ///     </p>
    /// </summary>
    [ContractClass(typeof(IVersionedPersistentObjectContract))]
    public interface IVersionedPersistentObject :
        IPersistentObject
    {
        int? PersistenceVersion { get; }

        [Pure]
        bool HasSamePersistenceVersion(IVersionedPersistentObject persistentObject);
    }

    /// <exclude />
    /// <summary>This is the contract class for <see cref="IVersionedPersistentObject" />.</summary>
    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IVersionedPersistentObject))]
    public abstract class IVersionedPersistentObjectContract :
        IVersionedPersistentObject
    {
        /// <summary>
        ///     This method should not appear in this interface. Once a version is set,
        ///     it should always remain the same (final, immutable property).
        ///     Persistence engines need a way to set the property, but that is it.
        ///     The question is whether it is possible to do testing than?
        ///     This was introduced here for the first time for conversion
        ///     to a VersionedPersistentBean from a JSON object: we need property setters
        ///     then.
        /// </summary>
        public int? PersistenceVersion
        {
            get { return default(int?); }
            // ReSharper disable UnusedMember.Local
            private set { Contract.Ensures(PersistenceVersion == value); }
            // ReSharper restore UnusedMember.Local
        }

        public bool HasSamePersistenceVersion(IVersionedPersistentObject other)
        {
            Contract.Ensures(Contract.Result<bool>() ==
                             (other == null
                                  ? false
                                  : !other.PersistenceVersion.HasValue || !PersistenceVersion.HasValue
                                      ? false
                                      : PersistenceVersion.Value == other.PersistenceVersion.Value));

            return default(bool);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public abstract long? PersistenceId { get; set; }

        public abstract bool IsTransient { get; }

        public abstract bool HasSamePersistenceId(IPersistentObject other);

        public abstract bool IsSerialized { get; }

#pragma warning disable

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore

        public abstract bool IsCivilized();

        public abstract CompoundSemanticException WildExceptions();

        public abstract void ThrowIfNotCivilized();
    }
}
