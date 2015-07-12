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

using NHibernate.Event;
using NHibernate.Persister.Entity;

using PPWCode.Util.OddsAndEnds.I.Security;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public class AuditEventListener :
        IPreUpdateEventListener,
        IPreInsertEventListener
    {
        private static void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
            {
                return;
            }

            state[index] = value;
        }

        public bool OnPreUpdate(PreUpdateEvent preUpdateEvent)
        {
            IAuditableObject audit = preUpdateEvent.Entity as IAuditableObject;
            if (audit == null)
            {
                return false;
            }

            DateTime time = DateTime.Now;
            string name = IdentityNameHelper.GetIdentityName();

            Set(preUpdateEvent.Persister, preUpdateEvent.State, "LastModifiedAt", time);
            Set(preUpdateEvent.Persister, preUpdateEvent.State, "LastModifiedBy", name);

            audit.LastModifiedAt = time;
            audit.LastModifiedBy = name;

            return false;
        }

        public bool OnPreInsert(PreInsertEvent preInsertEvent)
        {
            IAuditableObject audit = preInsertEvent.Entity as IAuditableObject;
            IInsertAuditableObject insertAuditOnly = preInsertEvent.Entity as IInsertAuditableObject;
            if (audit == null && insertAuditOnly == null)
            {
                return false;
            }

            DateTime time = DateTime.Now;
            string name = IdentityNameHelper.GetIdentityName();

            if (audit != null)
            {
                Set(preInsertEvent.Persister, preInsertEvent.State, "CreatedAt", time);
                Set(preInsertEvent.Persister, preInsertEvent.State, "CreatedBy", name);
                Set(preInsertEvent.Persister, preInsertEvent.State, "LastModifiedAt", time);
                Set(preInsertEvent.Persister, preInsertEvent.State, "LastModifiedBy", name);

                audit.CreatedAt = time;
                audit.CreatedBy = name;
                audit.LastModifiedAt = time;
                audit.LastModifiedBy = name;
            }

            if (insertAuditOnly != null)
            {
                Set(preInsertEvent.Persister, preInsertEvent.State, "CreatedAt", time);
                Set(preInsertEvent.Persister, preInsertEvent.State, "CreatedBy", name);

                insertAuditOnly.CreatedAt = time;
                insertAuditOnly.CreatedBy = name;
            }

            return false;
        }
    }
}