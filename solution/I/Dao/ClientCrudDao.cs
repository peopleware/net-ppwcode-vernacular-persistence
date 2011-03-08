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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using Iesi.Collections.Generic;

using PPWCode.Util.OddsAndEnds.I.Extensions;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    public abstract class ClientCrudDao :
        ClientDao
    {
        #region Invariant

        [ContractInvariantMethod]
        // ReSharper disable UnusedMember.Local
        private void ObjectInvariant()
        {
            Contract.Invariant(CrudDao != null);
        }

        // ReSharper restore UnusedMember.Local

        #endregion

        #region Constructors

        // ReSharper disable SuspiciousTypeConversion.Global
        protected ClientCrudDao(IWcfCrudDao crudDao)
            : base(crudDao)
        {
            Contract.Requires(crudDao != null);
            Contract.Ensures(crudDao == CrudDao);

            m_CrudDao = crudDao;
        }

        // ReSharper restore SuspiciousTypeConversion.Global

        #endregion

        #region Properties

        private IWcfCrudDao m_CrudDao;

        [Pure]
        // ReSharper disable MemberCanBePrivate.Global
            public IWcfCrudDao CrudDao
        {
            get
            {
                CheckObjectAlreadyDisposed();
                return m_CrudDao;
            }
        }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Methods

        public T Retrieve<T>(long? id)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return (T)m_CrudDao.Retrieve(typeof(T).GetQualifiedName(), id);
        }

        public IPersistentObject Retrieve(string persistenceObjectType, long? id)
        {
            CheckObjectAlreadyDisposed();
            return m_CrudDao.Retrieve(persistenceObjectType, id);
        }

        public ICollection<T> RetrieveAll<T>()
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            ICollection<T> result = new HashedSet<T>();
            ICollection<IPersistentObject> tmp = m_CrudDao.RetrieveAll(typeof(T).GetQualifiedName());
            foreach (IPersistentObject obj in tmp)
            {
                result.Add((T)obj);
            }
            return result;
        }

        public T Create<T>(T po)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return (T)m_CrudDao.Create(po);
        }

        public T Update<T>(T po)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return (T)m_CrudDao.Update(po);
        }

        public ICollection<T> UpdateAll<T>(IEnumerable<T> col)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return m_CrudDao.UpdateAll(col.OfType<IPersistentObject>().ToList()).OfType<T>().ToList();
        }

        public T Save<T>(T po)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return po.PersistenceId.HasValue ? Update(po) : Create(po);
        }

        public T Delete<T>(T po)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return (T)m_CrudDao.Delete(po);
        }

        public T Delete<T>(long? id)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return (T)m_CrudDao.DeleteById(typeof(T).GetQualifiedName(), id);
        }

        public T GetPropertyValue<T>(IPersistentObject po, string propertyName)
            where T : class
        {
            CheckObjectAlreadyDisposed();
            return (T)m_CrudDao.GetPropertyValue(po, propertyName);
        }

        public ICollection<T> GetChildren<T>(IPersistentObject po, string propertyName)
            where T : class
        {
            CheckObjectAlreadyDisposed();
            ICollection<T> result = new HashedSet<T>();
            ICollection<IPersistentObject> tmp = m_CrudDao.GetChildren(po, propertyName);
            foreach (IPersistentObject obj in tmp)
            {
                result.Add((T)obj);
            }
            return result;
        }

        protected IList<T> GetChildrenList<T>(IPersistentObject po, string propertyName)
            where T : class
        {
            CheckObjectAlreadyDisposed();
            return m_CrudDao
                .GetChildren(po, propertyName)
                .Cast<T>()
                .ToList();
        }

        public void FlushAllCaches()
        {
            CheckObjectAlreadyDisposed();
            m_CrudDao.FlushAllCaches();
        }

        #endregion

        #region Overrides of ClientDao

        protected override void Cleanup()
        {
            base.Cleanup();
            m_CrudDao = null;
        }

        #endregion
    }
}
