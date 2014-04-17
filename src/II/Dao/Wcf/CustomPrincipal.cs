#region Using

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading;

#endregion

namespace PPWCode.Vernacular.Persistence.II.Dao.Wcf
{
    public sealed class CustomPrincipal : IPrincipal
    {
        private readonly WindowsIdentity m_Identity;
        private HashSet<string> m_HashedRoles;
        private static readonly bool s_UseSecurity;
        private static readonly IDictionary<string, HashSet<string>> s_RoleCache = new Dictionary<string, HashSet<string>>();
        private static readonly object s_Lock = new object();

        static CustomPrincipal()
        {
            object valueFromConfig = ConfigurationManager.AppSettings["UseSecurity"];
            s_UseSecurity = valueFromConfig != null ? Convert.ToBoolean(valueFromConfig) : true;
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

        #region IPrincipal Members

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

        #endregion
    }
}