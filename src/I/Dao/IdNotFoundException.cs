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

using PPWCode.Vernacular.Exceptions.I;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    [Serializable]
    public class IdNotFoundException :
        SemanticException
    {
        public IdNotFoundException()
        {
        }

        public IdNotFoundException(string message)
            : base(message)
        {
        }

        public IdNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IdNotFoundException(Type persistentObjectType, long? id)
            : base(null, null)
        {
            Contract.Requires(persistentObjectType != null);

            Contract.Ensures(PersistenObjectType == persistentObjectType);
            Contract.Ensures(PersistenceId == id);

            PersistenceId = id;
            PersistenObjectType = persistentObjectType;
        }

        protected IdNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type PersistenObjectType
        {
            get
            {
                return Data["PersistenObjectType"] as Type;
            }
            private set
            {
                Data["PersistenObjectType"] = value;
            }
        }

        public long? PersistenceId
        {
            get
            {
                return Data["PersistenceId"] as long?;
            }
            private set
            {
                Data["PersistenceId"] = value;
            }
        }
    }
}
