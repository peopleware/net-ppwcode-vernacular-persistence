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
using System.Configuration;
using System.ServiceModel;

using HibernatingRhinos.Profiler.Appender.NHibernate;

using NHibernate;

using PPWCode.Util.OddsAndEnds.I.Extensions;
using PPWCode.Vernacular.Exceptions.I;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    [NHibernateSerializationBehavior]
    [ErrorLogBehavior]
    public abstract class NHibernateWcfCrudDao :
        WcfCrudDao
    {
        #region Constructors

        static NHibernateWcfCrudDao()
        {
            object valueFromConfig = ConfigurationManager.AppSettings["UseSecurity"];
            UseSecurity = valueFromConfig != null ? Convert.ToBoolean(valueFromConfig) : true;
            valueFromConfig = ConfigurationManager.AppSettings["UseProfiler"];
            UseProfiler = valueFromConfig != null ? Convert.ToBoolean(valueFromConfig) : false;
            if (UseProfiler)
            {
                StartNHibernateProfiler();
                AppDomain.CurrentDomain.ProcessExit += (sender, e) => EndNHibernateProfiler();
            }
        }

        protected NHibernateWcfCrudDao()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            SessionFactory = GetSessionFactory();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            NHibernateStatelessCrudDao nHibernateStatelessCrudDao = UseSecurity ? new NHibernateSecurityStatelessCrudDao() : new NHibernateStatelessCrudDao();
            nHibernateStatelessCrudDao.Session = SessionFactory.OpenSession();
            Session = nHibernateStatelessCrudDao.Session;
            StatelessCrudDao = nHibernateStatelessCrudDao;
        }

        #endregion

        #region Abstract GetSessionFactory Method

        protected abstract ISessionFactory GetSessionFactory();

        #endregion

        #region properties

        public ISession Session { get; set; }
        protected ISessionFactory SessionFactory { get; private set; }
        protected static bool UseSecurity { get; private set; }
        protected static bool UseProfiler { get; private set; }

        #endregion

        #region nHibernate Profiles

        private static void StartNHibernateProfiler()
        {
            NHibernateProfiler.Initialize();
        }

        private static void EndNHibernateProfiler()
        {
            NHibernateProfiler.Stop();
        }

        #endregion

        #region security

        /// <summary>
        /// This method checks if the current thread has enough permission (role-based)
        /// to execute the requested action A on type T.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="securityActionFlag"></param>
        /// <returns></returns>
        public bool HasSufficientSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            IPPWSecurity sec = StatelessCrudDao as IPPWSecurity;
            return sec != null ? sec.HasSufficientSecurity(type, securityActionFlag) : true;
        }

        /// <summary>
        /// This method checks if the current thread has enough permission (role-based)
        /// to execute the requested action A on type T.
        /// </summary>
        /// <param name="securityPermission"></param>
        protected void CheckSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            IPPWSecurity sec = StatelessCrudDao as IPPWSecurity;
            if (sec != null)
            {
                sec.CheckSecurity(type, securityActionFlag);
            }
        }

        #endregion

        #region retrieve with version check

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

        #endregion

        #region gateway

        protected enum Operation
        {
            /// <summary>
            /// Createe
            /// </summary>
            CREATE,
            /// <summary>
            /// Update
            /// </summary>
            UPDATE,
            /// <summary>
            /// Delete
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

        #region Create/Update/Delete gateways

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

        #endregion

        #endregion

        #region BaseOperations

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

        #endregion

        #region Overrides WcfCrudDao

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

        #endregion
    }
}