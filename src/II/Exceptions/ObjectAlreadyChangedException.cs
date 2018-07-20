﻿// Copyright 2014 by PeopleWare n.v..
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
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable]
    public class ObjectAlreadyChangedException : SemanticException
    {
        public ObjectAlreadyChangedException(string message, string entityName, object identifier)
            : this(message, entityName, identifier, null)
        {
        }

        public ObjectAlreadyChangedException(string message, string entityName, object identifier, Exception innerException)
            : base(message, innerException)
        {
            EntityName = entityName;
            Identifier = identifier;
        }

        protected ObjectAlreadyChangedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public object EntityName
        {
            get { return Data["EntityName"]; }
            private set { Data["EntityName"] = value; }
        }

        public object Identifier
        {
            get { return Data["Identifier"]; }
            private set { Data["Identifier"] = value; }
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            ObjectAlreadyChangedException otherObjectAlreadyChangedException = other as ObjectAlreadyChangedException;
            return result
                   && otherObjectAlreadyChangedException != null
                   && EntityName == otherObjectAlreadyChangedException.EntityName
                   && Identifier == otherObjectAlreadyChangedException.Identifier;
        }

        public override string ToString()
        {
            return string.Format(@"Type: {0}; Sender={1}", GetType().Name, Identifier);
        }
    }
}