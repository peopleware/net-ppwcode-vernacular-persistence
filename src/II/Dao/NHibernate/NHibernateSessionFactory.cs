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
using System.Reflection;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public static class NHibernateSessionFactory
    {
        #region fields

        private static readonly object s_Lock = new object();
        private static readonly Dictionary<string, ISessionFactory> s_SessionFactories = new Dictionary<string, ISessionFactory>();

        #endregion

        #region Private helpers

        public static Configuration CreateConfiguration(HashSet<Assembly> assemblies)
        {
            Configuration cfg = new Configuration();

            cfg.EventListeners.PreUpdateEventListeners = new IPreUpdateEventListener[]
            {
                new AuditEventListener(),
            };
            cfg.EventListeners.PreInsertEventListeners = new IPreInsertEventListener[]
            {
                new AuditEventListener(),
            };
            cfg.EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[]
            {
                new AuditLogEventListner(),
            };
            cfg.EventListeners.PostInsertEventListeners = new IPostInsertEventListener[]
            {
                new AuditLogEventListner(),
            };
            cfg.EventListeners.PostDeleteEventListeners = new IPostDeleteEventListener[]
            {
                new AuditLogEventListner(),
            };

            if (assemblies == null)
            {
                assemblies = new HashSet<Assembly>();
            }
            assemblies.Add(typeof(AuditLog).Assembly);
            foreach (Assembly assembly in assemblies)
            {
                cfg.AddAssembly(assembly);
            }

            return cfg.Configure();
        }

        #endregion

        public static ISessionFactory CreateSessionFactory(IEnumerable<KeyValuePair<string, string>> properties, HashSet<Assembly> assemblies)
        {
            Configuration cfg = CreateConfiguration(assemblies);
            foreach (KeyValuePair<string, string> item in properties)
            {
                if (cfg.Properties.ContainsKey(item.Key))
                {
                    if (string.IsNullOrEmpty(item.Value))
                    {
                        cfg.Properties.Remove(item.Key);
                    }
                    else
                    {
                        cfg.SetProperty(item.Key, item.Value);
                    }
                }
                else
                {
                    cfg.Properties.Add(item);
                }
            }
            return cfg.BuildSessionFactory();
        }

        public static ISessionFactory CreateSessionFactory(string connectionStringName, IDictionary<string, string> properties, HashSet<Assembly> assemblies)
        {
            lock (s_Lock)
            {
                ISessionFactory result;
                if (!s_SessionFactories.TryGetValue(connectionStringName, out result))
                {
                    if (properties == null)
                    {
                        properties = new Dictionary<string, string>();
                    }
                    properties.Add("connection.connection_string_name", connectionStringName);
                    result = CreateSessionFactory(properties, assemblies);
                    s_SessionFactories.Add(connectionStringName, result);
                }
                return result;
            }
        }

        public static ISessionFactory GetSessionFactory(string connectionStringName)
        {
            lock (s_Lock)
            {
                return s_SessionFactories[connectionStringName];
            }
        }
    }
}