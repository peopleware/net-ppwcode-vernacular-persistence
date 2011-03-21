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
using PPWCode.Vernacular.Semantics.I;

#endregion

namespace PPWCode.Vernacular.Persistence.I
{
    /// <summary>
    /// <p> Persistent classes need a primary key. Persistent objects always represent real-world objects,
    ///     and therefore should be implemented as {@link RousseauBean}s. This interface enforces the correct
    ///     behavior. Supporting code is offered by {@link AbstractPersistentBean}.</p>
    /// <p> Users should be aware that this means that there can be more than 1 Java object that represents
    ///     the same instance in the persistent storage. To check whether 2 persistent objects represent the
    ///     same persistent instance, use {@link #hasSamePersistenceId(PersistentBean)}.</p>
    /// <p> Persistent beans are not {@link Cloneable} however. Implementing clone for a semantic inheritance
    ///     tree is a large investment, and should not be enforced. Furthermore, it still is a bad idea to make
    ///     any semantic object {@link Cloneable}. From experience we know that it is very difficult to decide
    ///     in general how deep a clone should go. Persistent beans are {@link Serializable} though, because
    ///     they are often used also as Data Transfer Objects in multi-tier applications.</p>
    /// <p> {@code _Id_} must be {@link Serializable}, because PersistentBeans are {@link Serializable}
    ///     and the {@link #getPersistenceId()} is not {@code transient}. (And BTW, id's must be
    ///     {@link Serializable} for Hibernate too ... :-) ).</p>
    /// <p> Initial is persistenceID equals to null</p>
    /// </summary>
    [ContractClass(typeof(IPersistentObjectContract))]
    public interface IPersistentObject :
        IRousseauObject
    {
        /// <summary>
        /// Basic
        /// </summary>
        long? PersistenceId { get; set; }

        /// <summary>
        /// Is object nog niet gepersisteerd?
        /// </summary>
        bool IsTransient { get; }

        /// <summary>
        /// In SQL databases null doesn't equals to null, but here it will be.
        /// </summary>
        /// <param name="persistentObject"></param>
        /// <returns></returns>
        [Pure]
        bool HasSamePersistenceId(IPersistentObject persistentObject);
    }

    /// <exclude />
    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IPersistentObject))]
    public abstract class IPersistentObjectContract :
        IPersistentObject
    {
        #region IPersistentObject Members

        public long? PersistenceId
        {
            get { return default(long?); }
            set { Contract.Ensures(PersistenceId == value); }
        }

        public bool IsTransient
        {
            get
            {
                Contract.Ensures(Contract.Result<bool>() == (!PersistenceId.HasValue));

                return default(bool);
            }
        }

        public bool HasSamePersistenceId(IPersistentObject other)
        {
            Contract.Ensures(Contract.Result<bool>() ==
                             (other == null
                                  ? false
                                  : !other.PersistenceId.HasValue || !PersistenceId.HasValue
                                        ? false
                                        : PersistenceId.Value == other.PersistenceId.Value));

            return default(bool);
        }

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
