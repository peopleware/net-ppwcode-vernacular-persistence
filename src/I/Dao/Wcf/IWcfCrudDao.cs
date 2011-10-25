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
using System.ServiceModel;
using System.Transactions;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf
{
    /// <summary>
    /// <p>
    /// This interface should extend IStatelessCrudDao. However, currently this is not feasible,
    /// because IStatelessCrudDao is generic (which is very good), but generics are not supported over
    /// WCF services. This interface provides the same functionality, but only with polymorphism,
    /// without generics.
    /// </p>
    /// <p>
    /// In support of remote use of the ppwcode architecture, this interface adds a method to retrieve
    /// lazy loaded (and thus not-serialized) to-many associations.
    /// </p>
    /// </summary>
    [ContractClass(typeof(WcfCrudDaoContract))]
    [ServiceContract(Namespace = "http://peopleware.be/WcfCrudDao")]
    public interface IWcfCrudDao : IDao
    {
        /// <summary>
        /// Return a persistent object instance that represents the data of the record with key id of type
        /// PersistentObjectType in the persistent storage.
        /// Of particular note is the fact that returned objects need not necessarily need to be civilized
        /// This is strange, and probably a bad practice, but we have encountered situations where our code
        /// needs to be more stringent (in creates and updates) than legacy data existing already in the database.
        /// If the aID is not found in the store a valid IdNotFoundException is thrown
        /// </summary>
        /// <param name="persistentObjectType">The name of the type of the object we are looking for. This must
        /// be a subtype of IPersistenObject.</param>
        /// <param name="aID">The primary key of the object we are looking for.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IPersistentObject Retrieve(string persistentObjectType, long? id);

        /// <summary>
        /// Returns a complete collection of entities
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        ICollection<IPersistentObject> RetrieveAll(string persistentObjectType);

        /// <summary>
        ///  Create the object {@code pb} in persistent storage. Return that object with filled-out {@link PersistentObject.PersistenceId()}.
        ///  Before commit, the {@link Rousseauobject#civilized() civility} is verified on {@code pb} and all of its upstream objects
        ///  (to-one relationships), in their state such as they exist in the database. All upstream objects should exist in the database, and
        ///  be unchanged. Otherwise, an {@link AlreadyChangedException} is thrown. No validation is done on downstream objects: there should
        ///  be no downstream objects in {@code pb}. It is a programming error to submit a object with downstream associated objects.
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IPersistentObject Create(IPersistentObject po);

        /// <summary>
        /// Update the object {@code pb} in persistent storage. Return that object. Before commit, the
        /// {@link Rousseauobject#civilized() civility} is verified on {@code pb} and all of its upstream objects
        /// (to-one relationships), in their state such as they exist in the database. All upstream objects
        /// should exist in the database, and be unchanged. Otherwise, an {@link AlreadyChangedException}
        /// is thrown. No validation is done on downstream objects: there should be no downstream objects in
        /// {@code pb}.
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IPersistentObject Update(IPersistentObject po);

        /// <summary>
        /// Update all the objects {@code pb} in persistent storage. Return the objects. Before commit, the
        /// {@link Rousseauobject#civilized() civility} is verified on {@code pb} and all of its upstream objects
        /// (to-one relationships), in their state such as they exist in the database. All upstream objects
        /// should exist in the database, and be unchanged. Otherwise, an {@link AlreadyChangedException}
        /// is thrown. No validation is done on downstream objects: there should be no downstream objects in
        /// {@code pb}.
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        ICollection<IPersistentObject> UpdateAll(ICollection<IPersistentObject> col);

        /// <summary>
        /// Delete the object {@code pb}, and associated objects, depending on cascade DELETE settings, from persistent storage.
        /// The entire object is returned, for reasons of consistency with the other methods.
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IPersistentObject Delete(IPersistentObject po);

        /// <summary>
        /// Delete the object {@code pb} identified by <paramref name="id"/>, and associated objects, depending on cascade DELETE settings, from persistent storage.
        /// The entire object is returned, for reasons of consistency with the other methods.
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IPersistentObject DeleteById(string persistentObjectType, long? id);

        /// <summary>
        /// Retrieve the value of the property. This is usefull for retrieving lazily loaded properties.
        /// </summary>
        /// <param name="po"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        object GetPropertyValue(IPersistentObject po, string propertyName);

        /// <summary>
        /// Retrieve the value of the property. This is usefull for retrieving lazily loaded properties.
        /// </summary>
        /// <param name="po"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        ICollection<IPersistentObject> GetChildren(IPersistentObject po, string propertyName);

        /// <summary>
        /// If a services implements caches internal, then this method should flush alle these caches.
        /// A good example for using this method is when you are writing integration tests.
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void FlushAllCaches();
    }

    /// <exclude />
    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IWcfCrudDao))]
    public abstract class WcfCrudDaoContract :
        IWcfCrudDao
    {
        public IPersistentObject Retrieve(string PersistentObjectType, long? Id)
        {
            Contract.Requires(Transaction.Current != null);
            Contract.Requires(!string.IsNullOrEmpty(PersistentObjectType));
            Contract.Requires(typeof(IPersistentObject).IsAssignableFrom(Type.GetType(PersistentObjectType)));
            Contract.Requires(Id.HasValue);
            //Contract.Requires(IsOperational);

            Contract.Ensures(Type.GetType(PersistentObjectType).IsInstanceOfType(Contract.Result<IPersistentObject>()));
            Contract.Ensures(Contract.Result<IPersistentObject>() != null);
            Contract.Ensures(Contract.Result<IPersistentObject>().PersistenceId == Id);
            Contract.EnsuresOnThrow<IdNotFoundException>(true /* The ID cannot be found inside the store*/);

            return default(IPersistentObject);
        }

        public ICollection<IPersistentObject> RetrieveAll(string PersistentObjectType)
        {
            Contract.Requires(Transaction.Current != null);
            Contract.Requires(!string.IsNullOrEmpty(PersistentObjectType));
            Contract.Requires(typeof(IPersistentObject).IsAssignableFrom(Type.GetType(PersistentObjectType)));
            //Contract.Requires(IsOperational);

            Contract.Ensures(Contract.Result<ICollection<IPersistentObject>>() != null);
            Contract.Ensures(Contract.ForAll(
                Contract.Result<ICollection<IPersistentObject>>(),
                a => Type.GetType(PersistentObjectType).IsInstanceOfType(a)));
            //Contract.Ensures(Contract.ForAll<IPersistentObject>(
            //    Contract.Result<ICollection<IPersistentObject>>(),
            //    a => Contract.Result<ICollection<IPersistentObject>>()
            //            .Where(b => Type.ReferenceEquals(a, b))
            //            .Count() == 1));
            //Contract.Ensures(Contract.ForAll<IPersistentObject>(
            //    Contract.Result<ICollection<IPersistentObject>>(),
            //    a => Contract.Result<ICollection<IPersistentObject>>()
            //            .Where(b => b.HasSamePersistenceId(a))
            //            .Count() == 1));

            return default(ICollection<IPersistentObject>);
        }

        public IPersistentObject Create(IPersistentObject po)
        {
            Contract.Requires(Transaction.Current != null);
            //Contract.Requires(IsOperational);
            Contract.Requires(po != null);
            Contract.Requires(!po.PersistenceId.HasValue);

            Contract.Ensures(Contract.Result<IPersistentObject>() != null);
            Contract.Ensures(Contract.Result<IPersistentObject>().GetType() == po.GetType());
            Contract.Ensures(!Contract.Result<IPersistentObject>().IsTransient);

            return default(IPersistentObject);
        }

        public IPersistentObject Update(IPersistentObject po)
        {
            Contract.Requires(Transaction.Current != null);
            //Contract.Requires(IsOperational);
            Contract.Requires(po != null);
            Contract.Requires(po.PersistenceId.HasValue);

            Contract.Ensures(Contract.Result<IPersistentObject>() != null);
            Contract.Ensures(Contract.Result<IPersistentObject>().GetType() == po.GetType());
            Contract.Ensures(Contract.Result<IPersistentObject>().HasSamePersistenceId(po));

            return default(IPersistentObject);
        }

        public ICollection<IPersistentObject> UpdateAll(ICollection<IPersistentObject> col)
        {
            Contract.Requires(Transaction.Current != null);
            Contract.Ensures(Contract.Result<ICollection<IPersistentObject>>().Count == col.Count);

            return default(ICollection<IPersistentObject>);
        }

        public IPersistentObject Delete(IPersistentObject po)
        {
            Contract.Requires(Transaction.Current != null);
            //Contract.Requires(IsOperational);
            Contract.Requires(po != null);
            Contract.Requires(po.PersistenceId.HasValue);

            Contract.Ensures(Contract.Result<IPersistentObject>() != null);
            Contract.Ensures(Contract.Result<IPersistentObject>().GetType() == po.GetType());
            Contract.Ensures(!Contract.Result<IPersistentObject>().PersistenceId.HasValue);

            return default(IPersistentObject);
        }

        public IPersistentObject DeleteById(string PersistentObjectType, long? Id)
        {
            Contract.Requires(Transaction.Current != null);
            Contract.Requires(!string.IsNullOrEmpty(PersistentObjectType));
            Contract.Requires(typeof(IPersistentObject).IsAssignableFrom(Type.GetType(PersistentObjectType)));
            Contract.Requires(Id.HasValue);
            //Contract.Requires(IsOperational);

            Contract.Ensures(Type.GetType(PersistentObjectType).IsInstanceOfType(Contract.Result<IPersistentObject>()));
            Contract.Ensures(Contract.Result<IPersistentObject>() != null);
            Contract.Ensures(!Contract.Result<IPersistentObject>().PersistenceId.HasValue);
            Contract.EnsuresOnThrow<IdNotFoundException>(true /* The ID cannot be found inside the store*/);

            return default(IPersistentObject);
        }

        public abstract void FlushAllCaches();

        public object GetPropertyValue(IPersistentObject po, string PropertyName)
        {
            Contract.Requires(Transaction.Current != null);
            Contract.Requires(!string.IsNullOrEmpty(PropertyName));
            //Contract.Requires(IsOperational);

            //Contract.Ensures(Contract.Result<ICollection<IPersistentObject>>() != null);
            //// TODO: Tests if ICollection<IPersistenObject> contains elements of type described by the PropertyName
            ////       (This PropertyName should point in the first version to an ICollection).
            //Contract.Ensures(Contract.ForAll<IPersistentObject>(
            //    Contract.Result<ICollection<IPersistentObject>>(),
            //    a => Contract.Result<ICollection<IPersistentObject>>()
            //            .Where(b => Type.ReferenceEquals(a, b))
            //            .Count() == 1));
            //Contract.Ensures(Contract.ForAll<IPersistentObject>(
            //    Contract.Result<ICollection<IPersistentObject>>(),
            //    a => Contract.Result<ICollection<IPersistentObject>>()
            //            .Where(b => b.HasSamePersistenceId(a))
            //            .Count() == 1));

            return default(object);
        }

        public ICollection<IPersistentObject> GetChildren(IPersistentObject po, string PropertyName)
        {
            Contract.Requires(Transaction.Current != null);
            Contract.Requires(!string.IsNullOrEmpty(PropertyName));
            //Contract.Requires(IsOperational);

            return default(ICollection<IPersistentObject>);
        }

        #region IDao Members

        public abstract bool IsOperational { get; }

        #endregion

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
