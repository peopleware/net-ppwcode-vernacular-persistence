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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Event;
using PPWCode.Util.OddsAndEnds.II.Security;
using PPWCode.Vernacular.Exceptions.II;

#endregion

namespace PPWCode.Vernacular.Persistence.II.Dao.NHibernate
{
    public class AuditLogEventListner :
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

        private static readonly Dictionary<Type, AuditLogItem> s_DomainTypes;
        private static readonly object s_DomainTypesSyncObj;

        static AuditLogEventListner()
        {
            s_DomainTypes = new Dictionary<Type, AuditLogItem>();
            s_DomainTypesSyncObj = new object();
        }

        private static string GetStringValueFromStateArray(object[] stateArray, int position)
        {
            var value = stateArray[position];
            return value == null || value.ToString() == string.Empty ? "<null>" : value.ToString();
        }

        #region IPostUpdateEventListener Members

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            AuditLogItem ali = AuditLogItem.Find(@event.Entity.GetType());
            if ((ali.AuditLogAction & PPWAuditLogActionEnum.UPDATE) == PPWAuditLogActionEnum.UPDATE)
            {
                string identityName = IdentityNameHelper.GetIdentityName();
                DateTime now = DateTime.Now;
                string entityName = @event.Entity.GetType().Name;

                if (@event.OldState == null)
                {
                    throw new ProgrammingError(
                        String.Format(
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
                                logs.Add(new AuditLog
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

        #endregion

        #region IPostInsertEventListener Members

        public void OnPostInsert(PostInsertEvent @event)
        {
            AuditLogItem ali = AuditLogItem.Find(@event.Entity.GetType());
            if ((ali.AuditLogAction & PPWAuditLogActionEnum.CREATE) == PPWAuditLogActionEnum.CREATE)
            {
                string identityName = IdentityNameHelper.GetIdentityName();
                DateTime now = DateTime.Now;
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
                            logs.Add(new AuditLog
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

        #endregion

        #region IPostDeleteEventListener Members

        public void OnPostDelete(PostDeleteEvent @event)
        {
            AuditLogItem ali = AuditLogItem.Find(@event.Entity.GetType());
            if ((ali.AuditLogAction & PPWAuditLogActionEnum.DELETE) == PPWAuditLogActionEnum.DELETE)
            {
                string identityName = IdentityNameHelper.GetIdentityName();
                DateTime now = DateTime.Now;
                string entityName = @event.Entity.GetType().Name;

                using (ISession session = @event.Session.GetSession(EntityMode.Poco))
                {
                    session.Save(new AuditLog
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

        #endregion
    }
}
