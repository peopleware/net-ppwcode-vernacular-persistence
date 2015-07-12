// Copyright 2010-2015 by PeopleWare n.v..
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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using Iesi.Collections.Generic;

using log4net;

using NHibernate;
using NHibernate.Exceptions;

using PPWCode.Util.OddsAndEnds.I.Extensions;
using PPWCode.Vernacular.Exceptions.I;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public class NHibernateStatelessCrudDao :
        AbstractNHibernateDao,
        IStatelessCrudDao
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(NHibernateStatelessCrudDao));

        /// <summary>
        ///     This method *has* to convert whatever NHibernate exception to a valid PPWCode exception
        ///     TODO Handle the exceptions
        ///     Some hibernate exceptions might be semantic, some might be errors
        ///     This may depend on the actual product.
        ///     This method translates semantic exceptions in PPWCode.Util.Exception.SemanticException and throws them
        ///     and all other exceptions in PPWCode.Util.Exception.Error and throws them.
        /// </summary>
        /// <param name="exception">The hibernate exception we are triaging.</param>
        /// <param name="message">This message will be used in the logging in the case aException = Error.</param>
        public virtual void TriageException(Exception exception, string message)
        {
            s_Logger.Debug(message, exception);
            GenericADOException genericAdoException = exception as GenericADOException;
            if (genericAdoException != null)
            {
                DaoSqlException daoSqlException = new DaoSqlException(message, genericAdoException.InnerException)
                                                  {
                                                      SqlString = genericAdoException.SqlString,
                                                  };
                SqlException sqlException = genericAdoException.InnerException as SqlException;
                if (sqlException != null)
                {
                    daoSqlException.Constraint = sqlException.GetConstraint();
                }

                throw daoSqlException;
            }

            throw new ExternalError(message, exception);
        }

        /// <summary>
        ///     Check if the object(graph) is civilized.
        /// </summary>
        /// <typeparam name="PersistentObjectType">The type of the persistent object.</typeparam>
        /// <param name="po">The persistent object.</param>
        private static void ValidateObject<PersistentObjectType>(PersistentObjectType po)
            where PersistentObjectType : class, IPersistentObject
        {
            po.ThrowIfNotCivilized();
        }

        /// <summary>
        ///     This method checks if the current thread has enough permission (role-based)
        ///     to execute the requested action A on type T.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <param name="securityActionFlag">The intended action.</param>
        /// <returns>A <see cref="bool"/> indicating true or false.</returns>
        private bool HasSufficientSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            IPPWSecurity sec = this as IPPWSecurity;
            return sec != null ? sec.HasSufficientSecurity(type, securityActionFlag) : true;
        }

        /// <summary>
        ///     This method checks if the current thread has enough permission (role-based)
        ///     to execute the requested action A on type T.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <param name="securityActionFlag">The intended action.</param>
        private void CheckSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            IPPWSecurity sec = this as IPPWSecurity;
            if (sec != null)
            {
                sec.CheckSecurity(type, securityActionFlag);
            }
        }

        /// <summary>
        ///     Check if object is already disposed.
        ///     Method throws ObjectDisposedException if disposed.
        /// </summary>
        private void CheckObjectAlreadyDisposed()
        {
            if (Disposed)
            {
                throw new ObjectAlreadyDisposedError(GetType().FullName);
            }
        }

        /// <summary>
        ///     Return a persistent object instance that represents the data of the record with key id of type
        ///     PersistentObjectType in the persistent storage.
        ///     Of particular note is the fact that returned objects need not necessarily need to be civilized
        ///     This is strange, and probably a bad practice, but we have encountered situations where our code
        ///     needs to be more stringent (in creates and updates) than legacy data existing already in the database.
        /// </summary>
        /// <typeparam name="PersistentObjectType">The type of the persistent object.</typeparam>
        /// <param name="poType">The type of the persistent object, as a type.</param>
        /// <param name="id">The given primary key.</param>
        /// <returns>The persistent object.</returns>
        public PersistentObjectType Retrieve<PersistentObjectType>(Type poType, long? id)
            where PersistentObjectType : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Retrieve called for class {0}, ID {1}", typeof(PersistentObjectType).Name, id ?? -1));
            }

            PersistentObjectType retrievedPo = null;
            try
            {
                retrievedPo = (PersistentObjectType)Session.Get(poType, id);

                if (s_Logger.IsDebugEnabled)
                {
                    s_Logger.Debug(string.Format("Retrieve succeeded for class {0}, ID {1}", typeof(PersistentObjectType).Name, id ?? -1));
                }
            }
            catch (HibernateException he)
            {
                string message = string.Format("Hibernate retrieve failed for class {0}, ID={1}", typeof(PersistentObjectType).Name, id ?? -1);
                TriageException(he, message);
            }

            if (retrievedPo == null)
            {
                throw new IdNotFoundException(typeof(PersistentObjectType), id ?? -1);
            }

            // Check result type instead of requested type, result can be a sub-class of the requested type.
            CheckSecurity(retrievedPo.GetType(), SecurityActionFlag.RETRIEVE);

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Retrieve completed for class {0}, ID {1}, {2}", typeof(PersistentObjectType).Name, id, retrievedPo));
            }

            return retrievedPo;
        }

        /// <summary>
        ///     Returns a complete collection of entities.
        /// </summary>
        /// <typeparam name="PersistentObjectType">The type of the persistent object.</typeparam>
        /// <param name="poType">The type of the persistent object as a type.</param>
        /// <returns>A collection of persistent objects.</returns>
        public ICollection<PersistentObjectType> RetrieveAll<PersistentObjectType>(Type poType)
            where PersistentObjectType : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("RetrieveAll called for class {0}", typeof(PersistentObjectType).Name));
            }

            IList list = null;
            try
            {
                list = Session
                    .CreateCriteria(poType)
                    .List();
            }
            catch (HibernateException he)
            {
                string message = string.Format("RetrieveAll failed for class {0}", typeof(PersistentObjectType).Name);
                TriageException(he, message);
            }

            // Check result type instead of requested type, result can be a sub-class of the requested type.
            // result items without security are being eliminated.
            Contract.Assume(list != null);
            IEnumerable<PersistentObjectType> resultEnumerable = list
                .Cast<PersistentObjectType>()
                .Where(r => HasSufficientSecurity(r.GetType(), SecurityActionFlag.RETRIEVE));

            ICollection<PersistentObjectType> result = new HashedSet<PersistentObjectType>();
            foreach (PersistentObjectType obj in resultEnumerable)
            {
                result.Add(obj);
            }

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Retrieve completed for class {0}, {1}", typeof(PersistentObjectType).Name, result));
            }

            return result;
        }

        /// <summary>
        ///     Create the object in persistent storage. Return that object with the primary key filled in.
        ///     Before commit, the civility is verified and all of its upstream
        ///     objects
        ///     (to-one relationships), in their state such as they exist in the database. All upstream objects should exist in the
        ///     database, and
        ///     be unchanged. Otherwise, an <see cref="ObjectAlreadyChangedException"/> is thrown. No validation is done on downstream objects:
        ///     there should
        ///     be no downstream objects. It is a programming error to submit a object with downstream associated
        ///     objects.
        /// </summary>
        /// <typeparam name="PersistentObjectType">The type of the persistent object.</typeparam>
        /// <param name="po">The persistent object.</param>
        /// <returns>The created persistent object.</returns>
        public PersistentObjectType Create<PersistentObjectType>(PersistentObjectType po)
            where PersistentObjectType : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            CheckSecurity(po.GetType(), SecurityActionFlag.CREATE);

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Create called for class {0}, {1}", typeof(PersistentObjectType).Name, po));
            }

            try
            {
                ValidateObject(po);
                Session.Save(po);
            }
            catch (HibernateException he)
            {
                string message = string.Format("Create failed for class {0}, object {1}", typeof(PersistentObjectType).Name, po);
                TriageException(he, message);
            }

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Create completed for class {0}, {1}", typeof(PersistentObjectType).Name, po));
            }

            return po;
        }

        /// <summary>
        ///     Update the object in persistent storage. Return that object. Before commit, the
        ///     civility is verified and all of its upstream objects
        ///     (to-one relationships), in their state such as they exist in the database. All upstream objects
        ///     should exist in the database, and be unchanged. Otherwise, an <see cref="ObjectAlreadyChangedException"/>
        ///     is thrown. No validation is done on downstream objects: there should be no downstream objects.
        /// </summary>
        /// <typeparam name="PersistentObjectType">The type of the persistent object.</typeparam>
        /// <param name="po">The persistent object.</param>
        /// <returns>The updated persistent object.</returns>
        public PersistentObjectType Update<PersistentObjectType>(PersistentObjectType po)
            where PersistentObjectType : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            CheckSecurity(po.GetType(), SecurityActionFlag.UPDATE);

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Update called for class {0}, {1}", typeof(PersistentObjectType).Name, po));
            }

            PersistentObjectType updatedPo = null;
            try
            {
                ValidateObject(po);
                updatedPo = (PersistentObjectType)Session.Merge(po);
            }
            catch (InvalidCastException ice)
            {
                string errmsg = string.Format("Hibernate threw an InvalidCastException during update on class {0}, {1}", typeof(PersistentObjectType).Name, po);
                s_Logger.Error(errmsg, ice);
                throw new ProgrammingError(errmsg, ice);
            }
            catch (StaleObjectStateException sose)
            {
                string errmsg = string.Format("Object already changed, class {0}, {1}", typeof(PersistentObjectType).Name, po);
                s_Logger.Debug(errmsg, sose);
                throw new ObjectAlreadyChangedException(po);
            }
            catch (HibernateException he)
            {
                string message = string.Format("Hibernate Update failed for class {0}, object {1}", typeof(PersistentObjectType).Name, po);
                TriageException(he, message);
            }

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Create completed for class {0}, {1}", typeof(PersistentObjectType).Name, updatedPo));
            }

            return updatedPo;
        }

        /// <summary>
        ///     Delete the object and associated objects, depending on cascade DELETE settings, from persistent
        ///     storage.
        ///     The entire object is returned, for reasons of consistency with the other methods.
        /// </summary>
        /// <typeparam name="PersistentObjectType">The type of the persistent object.</typeparam>
        /// <param name="po">The persistent object.</param>
        /// <returns>The deleted persistent object.</returns>
        public PersistentObjectType Delete<PersistentObjectType>(PersistentObjectType po)
            where PersistentObjectType : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            CheckSecurity(po.GetType(), SecurityActionFlag.DELETE);

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Delete called for class {0}, {1}", typeof(PersistentObjectType).Name, po));
            }

            try
            {
                Session.Delete(po);
            }
            catch (StaleObjectStateException sose)
            {
                string errmsg = string.Format(@"Object already changed, class {0}, {1}", typeof(PersistentObjectType).Name, po);
                s_Logger.Debug(errmsg, sose);
                throw new ObjectAlreadyChangedException(po);
            }
            catch (HibernateException he)
            {
                string message = string.Format(@"Delete failed for class {0}, object {1}", typeof(PersistentObjectType).Name, po);
                TriageException(he, message);
            }

            po.PersistenceId = null;

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("Delete completed for class {0}, {1}", typeof(PersistentObjectType).Name, po));
            }

            return po;
        }

        public PropertyType GetPropertyValue<PersistentObjectType, PropertyType>(PersistentObjectType po, string property)
            where PersistentObjectType : class, IPersistentObject
            where PropertyType : class
        {
            CheckObjectAlreadyDisposed();
            CheckSecurity(po.GetType(), SecurityActionFlag.RETRIEVE);

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format(
                    "GetPropertyValue called for class {0} ({1}) and property {2}",
                    typeof(PersistentObjectType).Name,
                    po,
                    property));
            }

            PropertyType result = null;
            try
            {
                Type poType = po.GetType();
                IVersionedPersistentObject freshObject = Retrieve<IVersionedPersistentObject>(poType, po.PersistenceId);
                PropertyInfo propertyInfo = poType.GetProperty(property);
                result = (PropertyType)propertyInfo.GetValue(freshObject, null);

                ICollection genericCollection = result as ICollection;
                if (genericCollection != null)
                {
                    throw new ProgrammingError("Use GetChildren to retrieve a property that is a collection.");
                }
            }
            catch (HibernateException he)
            {
                string message = string.Format("GetPropertyValue for property {2} failed for class {0}, object {1}", typeof(PersistentObjectType).Name, po, property);
                TriageException(he, message);
            }

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("GetPropertyValue completed for class {0} ({1}) and property {2} ({3})", typeof(PersistentObjectType).Name, po, property, result));
            }

            return result;
        }

        public ICollection<PropertyType> GetChildren<PersistentObjectType, PropertyType>(PersistentObjectType po, string property)
            where PersistentObjectType : class, IPersistentObject
            where PropertyType : class, IPersistentObject
        {
            CheckObjectAlreadyDisposed();
            CheckSecurity(po.GetType(), SecurityActionFlag.RETRIEVE);

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format(
                    "GetChildren called for class {0} ({1}) and property {2}",
                    typeof(PersistentObjectType).Name,
                    po,
                    property));
            }

            Type poType = po.GetType();
            IPersistentObject freshObject = Retrieve<IPersistentObject>(poType, po.PersistenceId);
            PropertyInfo propertyInfo = poType.GetProperty(property);
            ICollection collection = null;
            try
            {
                collection = propertyInfo.GetValue(freshObject, null) as ICollection;
                if (collection == null)
                {
                    throw new ProgrammingError("Invalid cast during GetChildren");
                }
            }
            catch (InvalidCastException exc)
            {
                throw new ProgrammingError("Invalid cast during GetChildren", exc);
            }
            catch (HibernateException he)
            {
                string message = string.Format("GetChildren for property {2} failed for class {0}, object {1}", typeof(PersistentObjectType).Name, po, property);
                TriageException(he, message);
            }

            Contract.Assume(collection != null);
            IEnumerable<PropertyType> resultEnumerable = collection
                .Cast<PropertyType>()
                .Where(r => HasSufficientSecurity(r.GetType(), SecurityActionFlag.RETRIEVE));

            ICollection<PropertyType> result = new HashedSet<PropertyType>();
            foreach (PropertyType p in resultEnumerable)
            {
                result.Add(p);
            }

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.Debug(string.Format("GetChildren completed for class {0} ({1}) and property {2} ({3})", typeof(PersistentObjectType).Name, po, property, result));
            }

            return result;
        }

        public bool IsFlushable()
        {
            return true;
        }

        public void DoFlush()
        {
            if (IsFlushable() && Session.IsOpen && Session.IsDirty())
            {
                try
                {
                    Session.Flush();
                }
                catch (HibernateException he)
                {
                    TriageException(he, "Hibernate flush() failed.");
                }
            }
        }
    }
}