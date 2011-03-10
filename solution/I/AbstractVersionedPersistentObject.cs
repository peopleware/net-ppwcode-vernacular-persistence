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
using System.Runtime.Serialization;

using PPWCode.Vernacular.Persistence.I.Dao;

#endregion

namespace PPWCode.Vernacular.Persistence.I
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class AbstractVersionedPersistentObject :
        AbstractPersistentObject,
        IVersionedPersistentObject
    {
        #region Constructor

        protected AbstractVersionedPersistentObject()
        {
            Contract.Ensures(!PersistenceId.HasValue);
            Contract.Ensures(!PersistenceVersion.HasValue);
        }

        #endregion

        #region IVersionedPersistentObject Members

        [DataMember]
        private int? m_PersistenceVersion;

        [PPWAuditLogPropertyIgnore]
        public int? PersistenceVersion
        {
            get
            {
                return m_PersistenceVersion;
            }
            set
            {
                m_PersistenceVersion = value;
            }
        }

        [Pure]
        public bool HasSamePersistenceVersion(IVersionedPersistentObject other)
        {
            return other == null
                       ? false
                       : !other.PersistenceVersion.HasValue || !PersistenceVersion.HasValue
                             ? false
                             : PersistenceVersion.Value == other.PersistenceVersion.Value;
        }

        #endregion
    }
}
