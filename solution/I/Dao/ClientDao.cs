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
using System.Security.Principal;
using System.ServiceModel;

using log4net;

using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    public abstract class ClientDao :
        IDisposable
    {
        #region Fields

        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(ClientDao));

        #endregion

        #region Invariant

        [ContractInvariantMethod]
        // ReSharper disable UnusedMember.Local
        private void ObjectInvariant()
        {
            Contract.Invariant(Obj != null);
        }

        // ReSharper restore UnusedMember.Local

        #endregion

        #region Constructor

        protected ClientDao(object obj, WindowsIdentity windowsIdentity)
        {
            Contract.Requires(obj != null);
            Contract.Requires(windowsIdentity == null
                              || (windowsIdentity.IsAuthenticated
                                  && (windowsIdentity.ImpersonationLevel == TokenImpersonationLevel.Impersonation
                                      || windowsIdentity.ImpersonationLevel == TokenImpersonationLevel.Delegation)));
            Contract.Ensures(obj == Obj);
            Contract.Ensures(windowsIdentity == WindowsIdentity);

            m_Obj = obj;
            m_WindowsIdentity = windowsIdentity;
        }

        protected ClientDao(object obj) : this(obj, null)
        {
            Contract.Requires(obj != null);
            Contract.Ensures(obj == Obj);
            Contract.Ensures(WindowsIdentity == null);
        }

        #endregion

        #region Properties

        private readonly object m_Obj;

        [Pure]
        public object Obj
        {
            get
            {
                CheckObjectAlreadyDisposed();
                return m_Obj;
            }
        }

        private WindowsIdentity m_WindowsIdentity;

        [Pure]
        public WindowsIdentity WindowsIdentity
        {
            get
            {
                CheckObjectAlreadyDisposed();
                return m_WindowsIdentity;
            }
            set
            {
                Contract.Requires(value == null
                                  || (value.IsAuthenticated
                                      && (value.ImpersonationLevel == TokenImpersonationLevel.Impersonation
                                          || value.ImpersonationLevel == TokenImpersonationLevel.Delegation)));

                CheckObjectAlreadyDisposed();
                m_WindowsIdentity = value;
            }
        }


        #endregion

        #region Protected Methods

        private void CloseProxy(ICommunicationObject co)
        {
            if (!m_Disposed)
            {
                CommunicationProxyHelper.Close(co);
            }
        }

        #endregion

        #region Implementation of IDisposable

        ~ClientDao()
        {
            lock (m_Locker)
            {
                if (!m_Disposed)
                {
                    s_Logger.Warn("Code smell: Call dispose directly instead of relying on the finalizer.");
                    SafeCleanup();
                }
            }
        }

        private readonly object m_Locker = new object();
        private bool m_Disposed;

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
                s_Logger.Error(MethodBase.GetCurrentMethod().Name, e);
            }
        }

        protected virtual void Cleanup()
        {
            if (Obj == null)
            {
                return;
            }
            ICommunicationObject co = Obj as ICommunicationObject;
            if (co != null)
            {
                CloseProxy(co);
            }
            IDisposable disposableObj = Obj as IDisposable;
            if (disposableObj != null)
            {
                disposableObj.Dispose();
            }
        }

        protected void CheckObjectAlreadyDisposed()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #endregion
    }
}
