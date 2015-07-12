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
using System.Runtime.InteropServices;

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    [Serializable, Flags, ComVisible(true)]
    public enum SecurityActionFlag : byte
    {
        /// <summary>
        ///     The security action flag for Create.
        /// </summary>
        CREATE = 0x01,

        /// <summary>
        ///     The security action flag for Retrieve.
        /// </summary>
        RETRIEVE = 0x02,

        /// <summary>
        ///     The security action flag for Update.
        /// </summary>
        UPDATE = 0x04,

        /// <summary>
        ///     The security action flag for Delete.
        /// </summary>
        DELETE = 0x08,

        /// <summary>
        ///     The security action flag for everything together.
        /// </summary>
        ALL_PERMISSIONS = CREATE | RETRIEVE | UPDATE | DELETE,
    }

    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false), ComVisible(true)]
    public class PPWSecurityActionAttribute : Attribute
    {
        public SecurityActionFlag SecurityAction { get; set; }

        public string Role { get; set; }
    }
}