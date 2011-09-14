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
using System.Diagnostics.Contracts;
using System.Reflection;

using log4net;

using NHibernate;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public abstract class AbstractNHibernateDao :
        IDao
    {
        #region Fields

        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(AbstractNHibernateDao));

        #endregion

        #region Session

        private ISession m_Session;

        public ISession Session
        {
            get { return m_Session; }
            set { m_Session = value; }
        }

        #endregion

        #region IDao Members

        public bool IsOperational
        {
            get
            {
                Contract.Ensures(Contract.Result<bool>() ? (Session != null) : true);
                return m_Session != null;
            }
        }

        #endregion

        #region IDisposable Members

        private readonly object m_Locker = new object();
        private bool m_Disposed;

        ~AbstractNHibernateDao()
        {
            lock (m_Locker)
            {
                SafeCleanup();
            }
        }

        protected bool Disposed
        {
            get
            {
                lock (m_Locker)
                {
                    return m_Disposed;
                }
            }
        }

        public void Dispose()
        {
            lock (m_Locker)
            {
                if (!m_Disposed)
                {
                    SafeCleanup();
                    m_Disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        private void SafeCleanup()
        {
            try
            {
                Cleanup();
            }
            catch (Exception e)
            {
                s_Logger.Error(e);
            }
        }

        protected virtual void Cleanup()
        {
        }

        #endregion
    }
}
