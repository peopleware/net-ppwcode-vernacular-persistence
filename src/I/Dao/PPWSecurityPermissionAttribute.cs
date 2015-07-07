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
using System.Runtime.InteropServices;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    [Serializable, Flags, ComVisible(true)]
    public enum SecurityActionFlag : byte
    {
        /// <summary>
        /// Create
        /// </summary>
        CREATE = 0x01,
        /// <summary>
        /// Retrieve
        /// </summary>
        RETRIEVE = 0x02,
        /// <summary>
        /// Update
        /// </summary>
        UPDATE = 0x04,
        /// <summary>
        /// Delete
        /// </summary>
        DELETE = 0x08,
        /// <summary>
        /// AllPermissions
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
