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
using System.Diagnostics.Contracts;

using log4net;

using NHibernate;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public abstract class AbstractNHibernateDao :
        IDao
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(AbstractNHibernateDao));

        private readonly object m_Locker = new object();

        private bool m_Disposed;

        private ISession m_Session;

        public ISession Session
        {
            get { return m_Session; }
            set { m_Session = value; }
        }

        public bool IsOperational
        {
            get
            {
                Contract.Ensures(Contract.Result<bool>() ? (Session != null) : true);
                return m_Session != null;
            }
        }

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
    }
}