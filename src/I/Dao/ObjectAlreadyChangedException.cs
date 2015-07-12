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
    public class ObjectAlreadyChangedException :
        SemanticException
    {
        public ObjectAlreadyChangedException()
        {
        }

        public ObjectAlreadyChangedException(string message)
            : base(message)
        {
        }

        public ObjectAlreadyChangedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ObjectAlreadyChangedException(object sender)
            : base(null, null)
        {
            Contract.Requires(sender != null);
            Contract.Ensures(Sender == sender);

            Sender = sender;
        }

        protected ObjectAlreadyChangedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public object Sender
        {
            get { return Data["Sender"]; }
            private set { Data["Sender"] = value; }
        }
    }
}