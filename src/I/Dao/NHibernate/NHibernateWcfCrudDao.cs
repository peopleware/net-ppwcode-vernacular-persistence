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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.ServiceModel;

using NHibernate;

using PPWCode.Util.OddsAndEnds.I.Extensions;
using PPWCode.Vernacular.Exceptions.I;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    [NHibernateSerializationServiceBehavior]
    [ErrorLogBehavior]
    public abstract class NHibernateWcfCrudDao :
        WcfCrudDao
    {
        private readonly ISessionFactory m_SessionFactory;

        protected NHibernateWcfCrudDao(IStatelessCrudDao statelessCrudDao, ISessionFactory sessionFactory)
            : base(statelessCrudDao)
        {
            Contract.Requires(statelessCrudDao != null);
            Contract.Requires(sessionFactory != null);
            Contract.Ensures(StatelessCrudDao == statelessCrudDao);
            Contract.Ensures(SessionFactory == sessionFactory);

            m_SessionFactory = sessionFactory;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(StatelessCrudDao != null);
            Contract.Invariant(SessionFactory != null);
        }

        public ISessionFactory SessionFactory
        {
            get { return m_SessionFactory; }
        }

        /// <summary>
        ///     This method checks if the current thread has enough permission (role-based)
        ///     to execute the requested action <paramref name="securityActionFlag" /> on
        ///     type <paramref name="type" />.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <param name="securityActionFlag">The intended action.</param>
        /// <returns>A <see cref="bool" /> indicating whether the action is allowed.</returns>
        public bool HasSufficientSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            IPPWSecurity sec = StatelessCrudDao as IPPWSecurity;
            return sec != null ? sec.HasSufficientSecurity(type, securityActionFlag) : true;
        }

        /// <summary>
        ///     This method checks if the current thread has enough permission (role-based)
        ///     to execute the requested action A on type T.  The method throws an exception
        ///     of type <see cref="DaoSecurityException" /> if the current user does not
        ///     have enough rights for the action.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <param name="securityActionFlag">The intended action.</param>
        protected void CheckSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            IPPWSecurity sec = StatelessCrudDao as IPPWSecurity;
            if (sec != null)
            {
                sec.CheckSecurity(type, securityActionFlag);
            }
        }

        protected T Retrieve<T>(T po)
            where T : IPersistentObject
        {
            T dbPo = (T)Retrieve(po.GetType().GetQualifiedName(), po.PersistenceId);
            if (po is IVersionedPersistentObject)
            {
                if (!((IVersionedPersistentObject)dbPo).HasSamePersistenceVersion((IVersionedPersistentObject)po))
                {
                    throw new ObjectAlreadyChangedException(dbPo);
                }
            }

            return dbPo;
        }

        protected T Retrieve<T>(T po, SecurityActionFlag saf)
            where T : IPersistentObject
        {
            T result = Retrieve(po);
            CheckSecurity(result.GetType(), saf);
            return result;
        }

        protected enum Operation
        {
            /// <summary>
            ///     The create operation.
            /// </summary>
            CREATE,

            /// <summary>
            ///     The update operation.
            /// </summary>
            UPDATE,

            /// <summary>
            ///     The delete operation.
            /// </summary>
            DELETE
        }

        protected class FactoryKey
        {
            private Type DomainType { get; set; }

            private Operation DomainOperation { get; set; }

            public FactoryKey(Type domainType, Operation domainOperaion)
            {
                DomainType = domainType;
                DomainOperation = domainOperaion;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (obj == this)
                {
                    return true;
                }

                FactoryKey fk = obj as FactoryKey;
                if (fk == null)
                {
                    return false;
                }

                return DomainType == fk.DomainType && DomainOperation == fk.DomainOperation;
            }

            public override int GetHashCode()
            {
                int result = 14;
                result = (29 * result) + DomainType.GetHashCode();
                result = (29 * result) + DomainOperation.GetHashCode();
                return result;
            }
        }

        protected abstract Dictionary<FactoryKey, Func<IPersistentObject, IPersistentObject>> GetFactory();

        private IPersistentObject CUDGateway(IPersistentObject po, Operation operation, Dictionary<FactoryKey, Func<IPersistentObject, IPersistentObject>> factory)
        {
            Func<IPersistentObject, IPersistentObject> f;
            if (!factory.TryGetValue(new FactoryKey(po.GetType(), operation), out f))
            {
                throw new ProgrammingError(string.Format("{0} on object {1} not allowed / registered.", operation, po));
            }

            IPersistentObject result = f.Invoke(po);

            // safety check, call civilized
            if (operation == Operation.CREATE || operation == Operation.UPDATE)
            {
                result.ThrowIfNotCivilized();
            }

            DoFlush();
            return result;
        }

        [OperationBehavior(
             TransactionScopeRequired = true,
             TransactionAutoComplete = true)]
        public override IPersistentObject Create(IPersistentObject po)
        {
            return CUDGateway(po, Operation.CREATE, GetFactory());
        }

        [OperationBehavior(
             TransactionScopeRequired = true,
             TransactionAutoComplete = true)]
        public override IPersistentObject Update(IPersistentObject po)
        {
            return CUDGateway(po, Operation.UPDATE, GetFactory());
        }

        [OperationBehavior(
             TransactionScopeRequired = true,
             TransactionAutoComplete = true)]
        public override IPersistentObject Delete(IPersistentObject po)
        {
            return CUDGateway(po, Operation.DELETE, GetFactory());
        }

        public T BaseCreate<T>(T po)
            where T : IPersistentObject
        {
            return (T)base.Create(po);
        }

        public IPersistentObject BaseCreate(IPersistentObject po)
        {
            return base.Create(po);
        }

        public T BaseUpdate<T>(T po)
            where T : IPersistentObject
        {
            return (T)base.Update(po);
        }

        public IPersistentObject BaseUpdate(IPersistentObject po)
        {
            return base.Update(po);
        }

        public T BaseDelete<T>(T po)
            where T : IPersistentObject
        {
            return (T)base.Delete(po);
        }

        public IPersistentObject BaseDelete(IPersistentObject po)
        {
            return base.Delete(po);
        }

        public override void FlushAllCaches()
        {
            SessionFactory.EvictQueries();
            foreach (var collectionMetadata in SessionFactory.GetAllCollectionMetadata())
            {
                SessionFactory.EvictCollection(collectionMetadata.Key);
            }

            foreach (var classMetadata in SessionFactory.GetAllClassMetadata())
            {
                SessionFactory.EvictEntity(classMetadata.Key);
            }
        }
    }
}
