// Copyright 2018 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace PPWCode.Vernacular.Persistence.III
{
    [Flags]
    public enum AuditLogActionEnum
    {
        /// <summary>
        ///     NONE: nothing will be logged.
        /// </summary>
        NONE = 0,

        /// <summary>
        ///     CREATE: log create operations.
        /// </summary>
        CREATE = 1,

        /// <summary>
        ///     UPDATE: log update operations.
        /// </summary>
        UPDATE = 2,

        /// <summary>
        ///     DELETE: log delete operations.
        /// </summary>
        DELETE = 4,

        /// <summary>
        ///     ALL: log create, update and delete operations.
        /// </summary>
        ALL = CREATE | UPDATE | DELETE
    }
}
