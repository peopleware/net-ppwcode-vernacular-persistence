/*
 * Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Using

using System.ComponentModel;
using System.Diagnostics.Contracts;

using PPWCode.Vernacular.Exceptions.I;

#endregion

namespace PPWCode.Vernacular.Persistence.I
{
    /// <summary>
    /// <p> Persistency should always be implemented with versioning (optimistic locking). For that, the property
    ///     {@link #getPersistenceVersion() version} is added to this interface. There are several different possible
    ///     types of versioning, using an integer, date, or even a GUID. For that reason, the type of the property
    ///     is generic. We advise however to use Integer as version type. The property is kept generic to allow
    ///     for legacy systems.</p>
    /// <p> _Version_ must be {@link Serializable}, because PersistentBeans are {@link Serializable} and the
    ///     {@link #getPersistenceVersion()} is not {@code transient}. Although there is a case to be made
    ///     to enfore that {@code _Version_} should be {@link Comparable}, in essence the ability to check for
    ///     {@link Object#equals(Object) equality} suffices.</p>
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
    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IVersionedPersistentObject))]
    public abstract class IVersionedPersistentObjectContract :
        IVersionedPersistentObject
    {
        #region IVersionedPersistentObject Members

        /// <summary>
        /// This method should not appear in this interface. Once a version is set,
        /// it should always remain the same (final, immutable property).
        /// Persistence engines need a way to set the property, but that is it.
        /// The question is whether it is possible to do testing than?
        /// This was introduced here for the first time for conversion
        /// to a VersionedPersistentBean from a JSON object: we need property setters
        /// then (!?)
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

        #endregion

        #region IPersistentObject Members

        public abstract long? PersistenceId { get; set; }

        public abstract bool IsTransient { get; }

        public abstract bool HasSamePersistenceId(IPersistentObject other);

        #endregion

        #region ISemanticObject Members

        public abstract bool IsSerialized { get; }

        #endregion

        #region INotifyPropertyChanged Members

#pragma warning disable

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore

        #endregion

        #region IRousseauObject Members

        public abstract bool IsCivilized();
        public abstract CompoundSemanticException WildExceptions();
        public abstract void ThrowIfNotCivilized();

        #endregion
    }
}
