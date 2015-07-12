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
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.I;

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    [Serializable]
    public class DaoSecurityException : SemanticException
    {
        public DaoSecurityException()
            : base()
        {
        }

        public DaoSecurityException(string message)
            : base(message)
        {
        }

        public DaoSecurityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DaoSecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public DaoSecurityException(Type checkedType, SecurityActionFlag requestedSecurityAction)
            : base(string.Format("Access denied for action '{0}' on type '{1}'", requestedSecurityAction, checkedType.Name))
        {
            Contract.Requires(checkedType != null);
            Contract.Ensures(checkedType == CheckedType);
            Contract.Ensures(requestedSecurityAction == RequestedSecurityAction);

            CheckedType = checkedType;
            RequestedSecurityAction = requestedSecurityAction;
        }

        public Type CheckedType
        {
            get { return (Type)Data["CheckedType"]; }
            private set { Data["CheckedType"] = value; }
        }

        public SecurityActionFlag RequestedSecurityAction
        {
            get { return (SecurityActionFlag)Data["RequestedSecurityAction"]; }
            private set { Data["RequestedSecurityAction"] = value; }
        }
    }
}