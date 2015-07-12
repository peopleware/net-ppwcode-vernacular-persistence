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
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf
{
    public sealed class CustomPrincipal : IPrincipal
    {
        private static readonly bool s_UseSecurity;
        private static readonly IDictionary<string, HashSet<string>> s_RoleCache = new Dictionary<string, HashSet<string>>();
        private static readonly object s_Lock = new object();
        private readonly WindowsIdentity m_Identity;
        private HashSet<string> m_HashedRoles;

        static CustomPrincipal()
        {
            object valueFromConfig = ConfigurationManager.AppSettings["UseSecurity"];
            s_UseSecurity = valueFromConfig != null ? Convert.ToBoolean(valueFromConfig) : true;
        }

        private static HashSet<string> GetHashedRoles(WindowsIdentity id)
        {
            lock (s_Lock)
            {
                HashSet<string> result;
                if (!s_RoleCache.TryGetValue(id.Name, out result))
                {
                    result = new HashSet<string>();
                    if (id.Groups != null)
                    {
                        IdentityReferenceCollection identityReferenceCollection = id.Groups.Translate(typeof(NTAccount));
                        if (identityReferenceCollection != null)
                        {
                            IEnumerable<string> groups =
                                identityReferenceCollection
                                    .AsEnumerable()
                                    .Select(x => x.Value);
                            foreach (string @group in groups)
                            {
                                string[] items = @group.Split('\\');
                                result.Add(items.Length != 2 ? @group : items[1]);
                            }
                        }
                    }

                    s_RoleCache.Add(id.Name, result);
                }

                return result;
            }
        }

        public CustomPrincipal(IIdentity identity)
        {
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            if (windowsIdentity == null || !windowsIdentity.IsAuthenticated)
            {
                throw new ArgumentException(@"Only authenticated windows identities are supported.");
            }

            m_Identity = windowsIdentity;
        }

        // helper method for easy access (without casting)
        public static CustomPrincipal Current
        {
            get { return Thread.CurrentPrincipal as CustomPrincipal; }
        }

        // cache roles for subsequent requests
        private void EnsureRoles()
        {
            if (m_HashedRoles == null)
            {
                m_HashedRoles = GetHashedRoles(m_Identity);
            }
        }

        IIdentity IPrincipal.Identity
        {
            get { return m_Identity; }
        }

        bool IPrincipal.IsInRole(string role)
        {
            if (s_UseSecurity)
            {
                EnsureRoles();
                bool result = m_HashedRoles.Contains(role);
                return result;
            }

            return true;
        }
    }
}