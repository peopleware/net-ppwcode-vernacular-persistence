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
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Persistence.I.Dao;

namespace PPWCode.Vernacular.Persistence.I
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class AbstractAuditablePersistentObject :
        AbstractPersistentObject,
        IAuditableObject
    {
        protected AbstractAuditablePersistentObject()
        {
            Contract.Ensures(!PersistenceId.HasValue);

            Contract.Ensures(!CreatedAt.HasValue);
            Contract.Ensures(CreatedBy == null);
            Contract.Ensures(!LastModifiedAt.HasValue);
            Contract.Ensures(LastModifiedBy == null);
        }

        [DataMember]
        private DateTime? m_CreatedAt;

        [PPWAuditLogPropertyIgnore]
        public DateTime? CreatedAt
        {
            get { return m_CreatedAt; }
            set
            {
                if (m_CreatedAt != value)
                {
                    m_CreatedAt = value;
                    OnPropertyChanged("CreatedAt");
                }
            }
        }

        [DataMember]
        private string m_CreatedBy;

        [PPWAuditLogPropertyIgnore]
        public string CreatedBy
        {
            get { return m_CreatedBy; }
            set
            {
                if (m_CreatedBy != value)
                {
                    m_CreatedBy = value;
                    OnPropertyChanged("CreatedBy");
                }
            }
        }

        [DataMember]
        private DateTime? m_LastModifiedAt;

        [PPWAuditLogPropertyIgnore]
        public DateTime? LastModifiedAt
        {
            get { return m_LastModifiedAt; }
            set
            {
                if (m_LastModifiedAt != value)
                {
                    m_LastModifiedAt = value;
                    OnPropertyChanged("LastModifiedAt");
                }
            }
        }

        [DataMember]
        private string m_LastModifiedBy;

        [PPWAuditLogPropertyIgnore]
        public string LastModifiedBy
        {
            get { return m_LastModifiedBy; }
            set
            {
                if (m_LastModifiedBy != value)
                {
                    m_LastModifiedBy = value;
                    OnPropertyChanged("LastModifiedBy");
                }
            }
        }

        public override IDictionary<string, string> ReportingProperties()
        {
            IDictionary<string, string> reportingProperties = base.ReportingProperties();

            reportingProperties.Add("CreatedAt", CreatedAt != null ? CreatedAt.ToString() : "[null]");
            reportingProperties.Add("CreatedBy", CreatedBy);
            reportingProperties.Add("LastModifiedAt", LastModifiedAt != null ? LastModifiedAt.ToString() : "[null]");
            reportingProperties.Add("LastModifiedBy", LastModifiedBy);

            return reportingProperties;
        }
    }
}