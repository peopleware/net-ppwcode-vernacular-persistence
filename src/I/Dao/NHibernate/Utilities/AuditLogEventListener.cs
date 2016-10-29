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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;

using PPWCode.Vernacular.Exceptions.I;
using PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Interfaces;

using Environment = System.Environment;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Utilities
{
    public abstract class AuditLogEventListener :
        IRegisterEventListener,
        IPostUpdateEventListener,
        IPostInsertEventListener,
        IPostDeleteEventListener
    {
        private class AuditLogItem
        {
            public PPWAuditLogActionEnum AuditLogAction { get; private set; }

            public Dictionary<string, PPWAuditLogActionEnum> Properties { get; private set; }

            private AuditLogItem()
            {
                AuditLogAction = PPWAuditLogActionEnum.NONE;
                Properties = null;
            }

            public static AuditLogItem Find(Type t)
            {
                lock (s_DomainTypesSyncObj)
                {
                    AuditLogItem result;
                    if (t != typeof(AuditLog))
                    {
                        if (!s_DomainTypes.TryGetValue(t, out result))
                        {
                            result = new AuditLogItem();
                            PPWAuditLogAttribute auditLogAttribute = t.GetCustomAttributes(true).OfType<PPWAuditLogAttribute>().FirstOrDefault();
                            if (auditLogAttribute != null)
                            {
                                result.AuditLogAction = auditLogAttribute.AuditLogAction;
                                result.Properties = new Dictionary<string, PPWAuditLogActionEnum>();
                                foreach (PropertyInfo pi in t.GetProperties().Where(o => o.CanWrite))
                                {
                                    PPWAuditLogPropertyIgnoreAttribute auditLogPropertyIgnore = pi.GetCustomAttributes(true).OfType<PPWAuditLogPropertyIgnoreAttribute>().FirstOrDefault();
                                    result.Properties.Add(pi.Name, auditLogPropertyIgnore != null ? auditLogPropertyIgnore.AuditLogAction : PPWAuditLogActionEnum.NONE);
                                }
                            }

                            s_DomainTypes.Add(t, result);
                        }
                    }
                    else
                    {
                        result = new AuditLogItem();
                    }

                    return result;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (obj == this)
                {
                    return true;
                }

                AuditLogItem dt = obj as AuditLogItem;
                if (dt == null)
                {
                    return false;
                }

                return AuditLogAction == dt.AuditLogAction
                       && Properties.SequenceEqual(dt.Properties);
            }

            public override int GetHashCode()
            {
                int result = 14;
                result = (29 * result) + AuditLogAction.GetHashCode();
                result = (29 * result) + Properties.GetHashCode();
                return result;
            }
        }

        private static readonly object s_DomainTypesSyncObj;
        private static readonly Dictionary<Type, AuditLogItem> s_DomainTypes;

        private readonly IIdentityProvider m_IdentityProvider;
        private readonly ITimeProvider m_TimeProvider;
        private readonly bool m_UseUtc;

        static AuditLogEventListener()
        {
            s_DomainTypes = new Dictionary<Type, AuditLogItem>();
            s_DomainTypesSyncObj = new object();
        }

        protected AuditLogEventListener(IIdentityProvider identityProvider, ITimeProvider timeProvider, bool useUtc)
        {
            Contract.Requires(identityProvider != null);
            Contract.Requires(timeProvider != null);
            Contract.Ensures(IdentityProvider == identityProvider);
            Contract.Ensures(TimeProvider == timeProvider);

            m_IdentityProvider = identityProvider;
            m_TimeProvider = timeProvider;
            m_UseUtc = useUtc;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(IdentityProvider != null);
            Contract.Invariant(TimeProvider != null);
        }

        protected IIdentityProvider IdentityProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<IIdentityProvider>() != null);

                return m_IdentityProvider;
            }
        }

        protected ITimeProvider TimeProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<ITimeProvider>() != null);

                return m_TimeProvider;
            }
        }

        public bool UseUtc
        {
            get { return m_UseUtc; }
        }

        public void Register(Configuration cfg)
        {
            cfg.EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[] { this }
                .Concat(cfg.EventListeners.PostUpdateEventListeners)
                .ToArray();
            cfg.EventListeners.PostInsertEventListeners = new IPostInsertEventListener[] { this }
                .Concat(cfg.EventListeners.PostInsertEventListeners)
                .ToArray();
            cfg.EventListeners.PostDeleteEventListeners = new IPostDeleteEventListener[] { this }
                .Concat(cfg.EventListeners.PostDeleteEventListeners)
                .ToArray();
        }

        private static string GetStringValueFromStateArray(object[] stateArray, int position)
        {
            var value = stateArray[position];
            return value == null || value.ToString() == string.Empty ? "<null>" : value.ToString();
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            AuditLogItem ali = AuditLogItem.Find(@event.Entity.GetType());
            if ((ali.AuditLogAction & PPWAuditLogActionEnum.UPDATE) == PPWAuditLogActionEnum.UPDATE)
            {
                DateTime now = UseUtc ? TimeProvider.UtcNow : TimeProvider.LocalNow;
                string identityName = IdentityProvider.IdentityName;
                if (identityName == null)
                {
                    throw new InvalidOperationException("Unknown IdentityName");
                }

                string entityName = @event.Entity.GetType().Name;

                if (@event.OldState == null)
                {
                    throw new ProgrammingError(
                              string.Format(
                                  "No old state available for entity type '{1}'.{0}Make sure you're loading it into Session before modifying and saving it.",
                                  Environment.NewLine,
                                  entityName));
                }

                List<AuditLog> logs = new List<AuditLog>();
                int[] dirtyFieldIndexes = @event.Persister.FindDirty(@event.State, @event.OldState, @event.Entity, @event.Session);
                foreach (int dirtyFieldIndex in dirtyFieldIndexes)
                {
                    string oldValue = GetStringValueFromStateArray(@event.OldState, dirtyFieldIndex);
                    string newValue = GetStringValueFromStateArray(@event.State, dirtyFieldIndex);

                    if (oldValue != newValue)
                    {
                        string propertyName = @event.Persister.PropertyNames[dirtyFieldIndex];
                        PPWAuditLogActionEnum auditLogAction;
                        if (ali.Properties.TryGetValue(propertyName, out auditLogAction))
                        {
                            if ((auditLogAction & PPWAuditLogActionEnum.UPDATE) == PPWAuditLogActionEnum.NONE)
                            {
                                logs.Add(
                                    new AuditLog
                                    {
                                        EntryType = "U",
                                        EntityName = entityName,
                                        EntityId = (long?)@event.Id,
                                        PropertyName = propertyName,
                                        OldValue = oldValue,
                                        NewValue = newValue,
                                        CreatedBy = identityName,
                                        CreatedAt = now,
                                    });
                            }
                        }
                    }
                }

                if (logs.Count > 0)
                {
                    using (ISession session = @event.Session.GetSession(EntityMode.Poco))
                    {
                        logs.ForEach(o => session.Save(o));
                        session.Flush();
                    }
                }
            }
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            AuditLogItem ali = AuditLogItem.Find(@event.Entity.GetType());
            if ((ali.AuditLogAction & PPWAuditLogActionEnum.CREATE) == PPWAuditLogActionEnum.CREATE)
            {
                DateTime now = UseUtc ? TimeProvider.UtcNow : TimeProvider.LocalNow;
                string identityName = IdentityProvider.IdentityName;
                if (identityName == null)
                {
                    throw new InvalidOperationException("Unknown IdentityName");
                }

                string entityName = @event.Entity.GetType().Name;

                List<AuditLog> logs = new List<AuditLog>();
                int length = @event.State.Count();
                for (int fi = 0; fi < length; fi++)
                {
                    string newValue = GetStringValueFromStateArray(@event.State, fi);

                    string propertyName = @event.Persister.PropertyNames[fi];
                    PPWAuditLogActionEnum auditLogAction;
                    if (ali.Properties.TryGetValue(propertyName, out auditLogAction))
                    {
                        if ((auditLogAction & PPWAuditLogActionEnum.CREATE) == PPWAuditLogActionEnum.NONE)
                        {
                            logs.Add(
                                new AuditLog
                                {
                                    EntryType = "I",
                                    EntityName = entityName,
                                    EntityId = (long?)@event.Id,
                                    PropertyName = propertyName,
                                    OldValue = null,
                                    NewValue = newValue,
                                    CreatedBy = identityName,
                                    CreatedAt = now,
                                });
                        }
                    }
                }

                if (logs.Count > 0)
                {
                    using (ISession session = @event.Session.GetSession(EntityMode.Poco))
                    {
                        logs.ForEach(o => session.Save(o));
                        session.Flush();
                    }
                }
            }
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            AuditLogItem ali = AuditLogItem.Find(@event.Entity.GetType());
            if ((ali.AuditLogAction & PPWAuditLogActionEnum.DELETE) == PPWAuditLogActionEnum.DELETE)
            {
                DateTime now = UseUtc ? TimeProvider.UtcNow : TimeProvider.LocalNow;
                string identityName = IdentityProvider.IdentityName;
                if (identityName == null)
                {
                    throw new InvalidOperationException("Unknown IdentityName");
                }

                string entityName = @event.Entity.GetType().Name;

                using (ISession session = @event.Session.GetSession(EntityMode.Poco))
                {
                    session.Save(
                        new AuditLog
                        {
                            EntryType = "D",
                            EntityName = entityName,
                            EntityId = (long?)@event.Id,
                            CreatedBy = identityName,
                            CreatedAt = now,
                        });
                    session.Flush();
                }
            }
        }
    }
}
