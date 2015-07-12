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
    [Flags]
    public enum PPWAuditLogActionEnum
    {
        /// <summary>
        ///     The None action flag.
        /// </summary>
        NONE = 0,

        /// <summary>
        ///     The Insert action flag.
        /// </summary>
        CREATE = 1,

        /// <summary>
        ///     The Update action flag.
        /// </summary>
        UPDATE = 2,

        /// <summary>
        ///     The Delete action flag.
        /// </summary>
        DELETE = 4,

        /// <summary>
        ///     All is a collection of all existing action flags.
        /// </summary>
        ALL = CREATE | UPDATE | DELETE,
    }

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PPWAuditLogAttribute : Attribute
    {
        public PPWAuditLogActionEnum AuditLogAction { get; set; }

        public PPWAuditLogAttribute()
        {
            AuditLogAction = PPWAuditLogActionEnum.NONE;
        }
    }
}