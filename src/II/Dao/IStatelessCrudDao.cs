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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

#endregion

namespace PPWCode.Vernacular.Persistence.II.Dao
{
    [ContractClass(typeof(IStatelessCrudDaoContract))]
    public interface IStatelessCrudDao :
        IDao
    {
        /// <summary>
        /// Return a persistent object instance that represents the data of the record with key id of type
        /// PersistentObjectType in the persistent storage.
        /// Of particular note is the fact that returned objects need not necessarily need to be civilized
        /// This is strange, and probably a bad practice, but we have encountered situations where our code
        /// needs to be more stringent (in creates and updates) than legacy data existing already in the database.
        /// If the aID is not found in the store a valid IdNotFoundException is thrown
        /// </summary>
        PersistentObjectType Retrieve<PersistentObjectType>(Type poType, long? aID)
            where PersistentObjectType : class, IPersistentObject;

        /// <summary>
        /// Returns a complete collection of entities
        /// </summary>
        ICollection<PersistentObjectType> RetrieveAll<PersistentObjectType>(Type poType)
            where PersistentObjectType : class, IPersistentObject;

        /// <summary>
        ///  Create the object {@code pb} in persistent storage. Return that object with filled-out {@link PersistentObject.PersistenceId()}.
        ///  Before commit, the {@link Rousseauobject#civilized() civility} is verified on {@code pb} and all of its upstream objects
        ///  (to-one relationships), in their state such as they exist in the database. All upstream objects should exist in the database, and
        ///  be unchanged. Otherwise, an {@link AlreadyChangedException} is thrown. No validation is done on downstream objects: there should
        ///  be no downstream objects in {@code pb}. It is a programming error to submit a object with downstream associated objects.
        /// </summary>
        PersistentObjectType Create<PersistentObjectType>(PersistentObjectType aObject)
            where PersistentObjectType : class, IPersistentObject;

        /// <summary>
        /// Update the object {@code pb} in persistent storage. Return that object. Before commit, the
        /// {@link Rousseauobject#civilized() civility} is verified on {@code pb} and all of its upstream objects
        /// (to-one relationships), in their state such as they exist in the database. All upstream objects
        /// should exist in the database, and be unchanged. Otherwise, an {@link AlreadyChangedException}
        /// is thrown. No validation is done on downstream objects: there should be no downstream objects in
        /// {@code pb}.
        /// </summary>
        PersistentObjectType Update<PersistentObjectType>(PersistentObjectType aObject)
            where PersistentObjectType : class, IPersistentObject;

        /// <summary>
        /// Delete the object {@code pb}, and associated objects, depending on cascade DELETE settings, from persistent storage.
        /// The entire object is returned, for reasons of consistency with the other methods.
        /// </summary>
        PersistentObjectType Delete<PersistentObjectType>(PersistentObjectType aObject)
            where PersistentObjectType : class, IPersistentObject;

        /// <summary>
        /// Retrieve property of a persistent object.
        /// </summary>
        PropertyType GetPropertyValue<PersistentObjectType, PropertyType>(PersistentObjectType aObject, string propertyName)
            where PersistentObjectType : class, IPersistentObject
            where PropertyType : class;

        /// <summary>
        /// Retrieve children of a persistent object.
        /// </summary>
        ICollection<PropertyType> GetChildren<PersistentObjectType, PropertyType>(PersistentObjectType aObject, string propertyName)
            where PersistentObjectType : class, IPersistentObject
            where PropertyType : class, IPersistentObject;

        /// <summary>
        /// Indicates if the system must be flushed after use.
        /// </summary>
        /// <returns></returns>
        bool IsFlushable();

        /// <summary>
        /// Flush remaining operations
        /// </summary>
        void DoFlush();

        /// <summary>
        /// Inspecteer en indien nodig vorm de bestaande exceptie om.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        void TriageException(Exception exception, string message);
    }

    /// <exclude />
    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IStatelessCrudDao))]
    public abstract class IStatelessCrudDaoContract :
        IStatelessCrudDao
    {
        public PersistentObjectType Retrieve<PersistentObjectType>(Type poType, long? aID)
            where PersistentObjectType : class, IPersistentObject
        {
            Contract.Requires(poType != null);
            Contract.Requires(typeof(PersistentObjectType).IsAssignableFrom(poType));
            Contract.Requires(aID.HasValue);
            //Contract.Requires(IsOperational);
            Contract.Ensures(Contract.Result<PersistentObjectType>() != null);
            Contract.Ensures(Contract.Result<PersistentObjectType>().PersistenceId == aID);
            Contract.EnsuresOnThrow<IdNotFoundException>(true /* The ID cannot be found inside the store*/);

            return default(PersistentObjectType);
        }

        public ICollection<PersistentObjectType> RetrieveAll<PersistentObjectType>(Type poType)
            where PersistentObjectType : class, IPersistentObject
        {
            Contract.Requires(poType != null);
            Contract.Requires(typeof(PersistentObjectType).IsAssignableFrom(poType));
            //Contract.Requires(IsOperational);
            Contract.Ensures(Contract.Result<ICollection<PersistentObjectType>>() != null);
            // TODO: ccrewrite werkt blijkbaar niet met deze constructie.
            //Contract.Ensures(Contract.ForAll<PersistentObjectType>(
            //    Contract.Result<ICollection<PersistentObjectType>>(),
            //    a => Contract.Result<ICollection<PersistentObjectType>>().Where(b => Type.ReferenceEquals(a, b)).Count() == 1));
            //Contract.Ensures(Contract.ForAll<PersistentObjectType>(
            //    Contract.Result<ICollection<PersistentObjectType>>(),
            //    a => Contract.Result<ICollection<PersistentObjectType>>().Where(b => b.HasSamePersistenceId(a)).Count() == 1));

            return default(ICollection<PersistentObjectType>);
        }

        public PersistentObjectType Create<PersistentObjectType>(PersistentObjectType aObject)
            where PersistentObjectType : class, IPersistentObject
        {
            //Contract.Requires(IsOperational);
            Contract.Requires(aObject != null);
            Contract.Requires(!aObject.PersistenceId.HasValue);

            Contract.Ensures(Contract.Result<PersistentObjectType>() != null);
            Contract.Ensures(Contract.Result<PersistentObjectType>().HasSamePersistenceId(aObject));

            return default(PersistentObjectType);
        }

        public PersistentObjectType Update<PersistentObjectType>(PersistentObjectType aObject)
            where PersistentObjectType : class, IPersistentObject
        {
            //Contract.Requires(IsOperational);
            Contract.Requires(aObject != null);
            Contract.Requires(aObject.PersistenceId.HasValue);

            Contract.Ensures(Contract.Result<PersistentObjectType>() != null);
            Contract.Ensures(Contract.Result<PersistentObjectType>().HasSamePersistenceId(aObject));

            return default(PersistentObjectType);
        }

        public PersistentObjectType Delete<PersistentObjectType>(PersistentObjectType aObject)
            where PersistentObjectType : class, IPersistentObject
        {
            //Contract.Requires(IsOperational);
            Contract.Requires(aObject != null);
            Contract.Requires(aObject.PersistenceId.HasValue);

            Contract.Ensures(Contract.Result<PersistentObjectType>() != null);
            Contract.Ensures(!aObject.PersistenceId.HasValue);
            Contract.Ensures(!Contract.Result<PersistentObjectType>().PersistenceId.HasValue);

            return default(PersistentObjectType);
        }

        public PropertyType GetPropertyValue<PersistentObjectType, PropertyType>(PersistentObjectType aObject, string propertyName)
            where PersistentObjectType : class, IPersistentObject
            where PropertyType : class
        {
            //Contract.Requires(IsOperational);
            Contract.Requires(aObject != null);
            Contract.Requires(aObject.PersistenceId.HasValue);
            Contract.Requires(!string.IsNullOrEmpty(propertyName));
            //Contract.Requires(IsOperational);

            return default(PropertyType);
        }

        public ICollection<PropertyType> GetChildren<PersistentObjectType, PropertyType>(PersistentObjectType aObject, string propertyName)
            where PersistentObjectType : class, IPersistentObject
            where PropertyType : class, IPersistentObject
        {
            //Contract.Requires(IsOperational);
            Contract.Requires(aObject != null);
            Contract.Requires(aObject.PersistenceId.HasValue);
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            return default(ICollection<PropertyType>);
        }

        public bool IsFlushable()
        {
            return default(bool);
        }

        public void DoFlush()
        {
            //NOP
        }

        public void TriageException(Exception e, string message)
        {
            //NOP
        }

        #region IDao Members

        public abstract bool IsOperational { get; }

        #endregion

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
