// Copyright 2010-2016 by PeopleWare n.v..
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

using PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Interfaces;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Implementations
{
    public class TimeProvider : ITimeProvider
    {
        private readonly DateTime m_UtcCreationDateTime;

        public TimeProvider()
        {
            m_UtcCreationDateTime = DateTime.UtcNow;
        }

        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public DateTime LocalNow
        {
            get { return DateTime.Now; }
        }

        public DateTime TransactionalUtcNow
        {
            get { return m_UtcCreationDateTime; }
        }

        public DateTime TransactionalLocalNow
        {
            get { return TransactionalUtcNow.ToLocalTime(); }
        }
    }
}
