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
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public class NHibernateSecurityStatelessCrudDao :
        NHibernateStatelessCrudDao,
        IPPWSecurity
    {
        private static readonly Dictionary<Type, List<PPWSecurityActionAttribute>> s_SecurityAction;

        private static readonly object s_SecurityActionSyncObj;

        static NHibernateSecurityStatelessCrudDao()
        {
            s_SecurityAction = new Dictionary<Type, List<PPWSecurityActionAttribute>>();
            s_SecurityActionSyncObj = new object();
        }

        bool IPPWSecurity.HasSufficientSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            lock (s_SecurityActionSyncObj)
            {
                List<PPWSecurityActionAttribute> securityList;
                if (!s_SecurityAction.TryGetValue(type, out securityList))
                {
                    securityList = type
                        .GetCustomAttributes(false)
                        .OfType<PPWSecurityActionAttribute>()
                        .ToList();
                    s_SecurityAction.Add(type, securityList);
                }

                IPrincipal principal;
                if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    principal = Thread.CurrentPrincipal;
                }
                else
                {
                    WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
                    if (windowsIdentity != null && windowsIdentity.IsAuthenticated)
                    {
                        principal = new WindowsPrincipal(windowsIdentity);
                    }
                    else
                    {
                        return false;
                    }
                }

                return securityList
                    .Exists(s =>
                            (s.SecurityAction & securityActionFlag) == securityActionFlag
                            && principal.IsInRole(s.Role));
            }
        }

        void IPPWSecurity.CheckSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            if (!((IPPWSecurity)this).HasSufficientSecurity(type, securityActionFlag))
            {
                throw new DaoSecurityException(type, securityActionFlag);
            }
        }
    }
}