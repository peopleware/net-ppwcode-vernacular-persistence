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
using System.Security.Principal;
using System.ServiceModel;

using log4net;

using PPWCode.Vernacular.Exceptions.I;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers;

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    public abstract class ClientDao :
        IDisposable
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(ClientDao));

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Obj != null);
        }

        protected ClientDao(object obj, WindowsIdentity windowsIdentity)
        {
            Contract.Requires(obj != null);
            Contract.Ensures(obj == Obj);
            Contract.Ensures(windowsIdentity == WindowsIdentity);

            bool windowsIdentityOk =
                windowsIdentity == null
                || (windowsIdentity.IsAuthenticated
                    && (windowsIdentity.ImpersonationLevel == TokenImpersonationLevel.Impersonation
                        || windowsIdentity.ImpersonationLevel == TokenImpersonationLevel.Delegation));
            if (!windowsIdentityOk)
            {
                string msg;
                if (!windowsIdentity.IsAuthenticated)
                {
                    msg = string.Format(@"WindowsIdentity {0} cannot be used because it is not authenticated.", windowsIdentity.Name);
                }
                else
                {
                    msg = string.Format(
                        @"WindowsIdentity {0} cannot be used because it does not have the correct impersonation level ({1}).",
                        windowsIdentity.Name,
                        windowsIdentity.ImpersonationLevel);
                }

                s_Logger.Error(msg);
                throw new ProgrammingError(msg);
            }

            m_Obj = obj;
            m_WindowsIdentity = windowsIdentity;
            s_Logger.InfoFormat(@"WindowsIdentity set to {0}", m_WindowsIdentity != null ? m_WindowsIdentity.Name : "null");
        }

        protected ClientDao(object obj)
            : this(obj, null)
        {
            Contract.Requires(obj != null);
            Contract.Ensures(obj == Obj);
            Contract.Ensures(WindowsIdentity == null);
        }

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
                bool windowsIdentityOk =
                    value == null
                    || (value.IsAuthenticated
                        && (value.ImpersonationLevel == TokenImpersonationLevel.Impersonation
                            || value.ImpersonationLevel == TokenImpersonationLevel.Delegation));
                if (!windowsIdentityOk)
                {
                    string msg;
                    if (!value.IsAuthenticated)
                    {
                        msg = string.Format(@"WindowsIdentity {0} cannot be used because it is not authenticated.", value.Name);
                    }
                    else
                    {
                        msg = string.Format(
                            @"WindowsIdentity {0} cannot be used because it does not have the correct impersonation level ({1}).",
                            value.Name,
                            value.ImpersonationLevel);
                    }

                    s_Logger.Error(msg);
                    throw new ProgrammingError(msg);
                }

                CheckObjectAlreadyDisposed();
                m_WindowsIdentity = value;
                s_Logger.InfoFormat(@"WindowsIdentity set to {0}", m_WindowsIdentity != null ? m_WindowsIdentity.Name : "null");
            }
        }

        private void CloseProxy(ICommunicationObject co)
        {
            if (!m_Disposed)
            {
                CommunicationProxyHelper.Close(co);
            }
        }

        ~ClientDao()
        {
            if (m_Locker != null)
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
                s_Logger.Error(e);
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
                throw new ObjectAlreadyDisposedError(GetType().FullName);
            }
        }
    }
}