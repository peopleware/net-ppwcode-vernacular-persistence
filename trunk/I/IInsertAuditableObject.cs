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
using System.ComponentModel;
using System.Diagnostics.Contracts;

using PPWCode.Vernacular.Exceptions.I;

#endregion

namespace PPWCode.Vernacular.Persistence.I
{
    [ContractClass(typeof(IInsertAuditableObjectContract))]
    public interface IInsertAuditableObject :
        IPersistentObject
    {
        DateTime? CreatedAt { get; set; }
        string CreatedBy { get; set; }
    }

    /// <exclude />
    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IInsertAuditableObject))]
    public abstract class IInsertAuditableObjectContract :
        IInsertAuditableObject
    {
        #region IAuditableObject Members

        public DateTime? CreatedAt
        {
            get { return default(DateTime?); }
            set { Contract.Ensures(CreatedAt == value); }
        }

        public string CreatedBy
        {
            get { return default(string); }
            set { Contract.Ensures(CreatedBy == value); }
        }

        #endregion

        #region IPersistentObject Members

        public abstract long? PersistenceId { get; set; }

        public abstract bool IsTransient { get; }

        public abstract bool HasSamePersistenceId(IPersistentObject other);

        #endregion

        #region ISemanticObject Members

        public abstract bool IsSerialized { get; }

        #endregion

        #region INotifyPropertyChanged Members

#pragma warning disable

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore

        #endregion

        #region Implementation of IRousseauObject

        public abstract bool IsCivilized();
        public abstract CompoundSemanticException WildExceptions();
        public abstract void ThrowIfNotCivilized();

        #endregion
    }
}
