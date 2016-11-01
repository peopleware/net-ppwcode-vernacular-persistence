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

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Event.Default;

using PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Interfaces;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Utilities.EventListenerFixes
{
    [Serializable]
    public class DefaultAutoFlushEventListenerEx
        : DefaultAutoFlushEventListener,
          IRegisterEventListener
    {
        private static readonly IInternalLogger s_Log = LoggerProvider.LoggerFor(typeof(DefaultAutoFlushEventListenerEx));

        protected override void PerformExecutions(IEventSource session)
        {
            if (s_Log.IsDebugEnabled)
            {
                s_Log.Debug("executing flush");
            }

            try
            {
                session.ConnectionManager.FlushBeginning();

                // IMPL NOTE : here we alter the flushing flag of the persistence context to allow
                // during-flush callbacks more leniency in regards to initializing proxies and
                // lazy collections during their processing.
                // For more information, see HHH-2763 / NH-1882
                session.PersistenceContext.Flushing = true;

                // we need to lock the collection caches before
                // executing entity inserts/updates in order to
                // account for bidi associations
                session.ActionQueue.PrepareActions();
                session.ActionQueue.ExecuteActions();
            }
            catch (HibernateException he)
            {
                if (s_Log.IsErrorEnabled)
                {
                    s_Log.Error("Could not synchronize database state with session", he);
                }

                throw;
            }
            finally
            {
                session.PersistenceContext.Flushing = false;
                session.ConnectionManager.FlushEnding();
            }
        }

        public void Register(Configuration cfg)
        {
            cfg.EventListeners.AutoFlushEventListeners = new IAutoFlushEventListener[] { this };
        }
    }
}
