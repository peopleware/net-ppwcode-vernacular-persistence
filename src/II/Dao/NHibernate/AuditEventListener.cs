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

using System;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using PPWCode.Util.OddsAndEnds.II.Security;

namespace PPWCode.Vernacular.Persistence.II.Dao.NHibernate
{
    public class AuditEventListener :
        IPreUpdateEventListener,
        IPreInsertEventListener
    {
        #region IPreUpdateEventListener Members

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

        #endregion

        #region IPreInsertEventListener Members

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

        #endregion

        private static void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
            {
                return;
            }
            state[index] = value;
        }
    }
}
