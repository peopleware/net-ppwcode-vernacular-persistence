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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Principal;

using Iesi.Collections.Generic;

using PPWCode.Util.OddsAndEnds.I.Extensions;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf;

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    public abstract class ClientCrudDao :
        ClientDao
    {
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(CrudDao != null);
        }

        protected ClientCrudDao(IWcfCrudDao crudDao)
            : base(crudDao)
        {
            Contract.Requires(crudDao != null);
            Contract.Ensures(crudDao == CrudDao);
            Contract.Ensures(WindowsIdentity == null);
        }

        protected ClientCrudDao(IWcfCrudDao crudDao, WindowsIdentity windowsIdentity)
            : base(crudDao, windowsIdentity)
        {
            Contract.Requires(crudDao != null);
            Contract.Ensures(crudDao == CrudDao);
            Contract.Ensures(WindowsIdentity == windowsIdentity);
        }

        [Pure]
        public IWcfCrudDao CrudDao
        {
            get
            {
                CheckObjectAlreadyDisposed();
                return (IWcfCrudDao)Obj;
            }
        }

        public T Retrieve<T>(long? id)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            return (T)Retrieve(typeof(T).GetQualifiedName(), id);
        }

        public IPersistentObject Retrieve(string persistenceObjectType, long? id)
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return CrudDao.Retrieve(persistenceObjectType, id);
                }
            }

            return CrudDao.Retrieve(persistenceObjectType, id);
        }

        public ICollection<T> RetrieveAll<T>()
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            ICollection<IPersistentObject> items;
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    items = CrudDao.RetrieveAll(typeof(T).GetQualifiedName());
                }
            }
            else
            {
                items = CrudDao.RetrieveAll(typeof(T).GetQualifiedName());
            }

            ICollection<T> result = new HashedSet<T>();
            foreach (IPersistentObject item in items)
            {
                result.Add((T)item);
            }

            return result;
        }

        public T Create<T>(T po)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return (T)CrudDao.Create(po);
                }
            }

            return (T)CrudDao.Create(po);
        }

        public T Update<T>(T po)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return (T)CrudDao.Update(po);
                }
            }

            return (T)CrudDao.Update(po);
        }

        public ICollection<T> UpdateAll<T>(IEnumerable<T> col)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return CrudDao.UpdateAll(col.OfType<IPersistentObject>().ToList()).OfType<T>().ToList();
                }
            }

            return CrudDao.UpdateAll(col.OfType<IPersistentObject>().ToList()).OfType<T>().ToList();
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
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return (T)CrudDao.Delete(po);
                }
            }

            return (T)CrudDao.Delete(po);
        }

        public T Delete<T>(long? id)
            where T : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return (T)CrudDao.DeleteById(typeof(T).GetQualifiedName(), id);
                }
            }

            return (T)CrudDao.DeleteById(typeof(T).GetQualifiedName(), id);
        }

        public T GetPropertyValue<T>(IPersistentObject po, string propertyName)
            where T : class
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return (T)CrudDao.GetPropertyValue(po, propertyName);
                }
            }

            return (T)CrudDao.GetPropertyValue(po, propertyName);
        }

        public ICollection<T> GetChildren<T>(IPersistentObject po, string propertyName)
            where T : class
        {
            CheckObjectAlreadyDisposed();
            ICollection<IPersistentObject> children;
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    children = CrudDao.GetChildren(po, propertyName);
                }
            }
            else
            {
                children = CrudDao.GetChildren(po, propertyName);
            }

            ICollection<T> result = new HashedSet<T>();
            foreach (IPersistentObject child in children)
            {
                result.Add((T)child);
            }

            return result;
        }

        protected IList<T> GetChildrenList<T>(IPersistentObject po, string propertyName)
            where T : class
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    return CrudDao
                        .GetChildren(po, propertyName)
                        .Cast<T>()
                        .ToList();
                }
            }

            return CrudDao
                .GetChildren(po, propertyName)
                .Cast<T>()
                .ToList();
        }

        public void FlushAllCaches()
        {
            CheckObjectAlreadyDisposed();
            if (WindowsIdentity != null)
            {
                using (WindowsIdentity.Impersonate())
                {
                    CrudDao.FlushAllCaches();
                    return;
                }
            }

            CrudDao.FlushAllCaches();
        }
    }
}
