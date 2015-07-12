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

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using NHibernate;

using PPWCode.Util.OddsAndEnds.I.Extensions;

using Spring.Context.Support;

using ISession = NHibernate.ISession;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public class NHibernateContextInitializer
        : IInstanceContextInitializer
    {
        private readonly string m_SessionFactory;

        public NHibernateContextInitializer()
        {
        }

        public NHibernateContextInitializer(string sessionFactory)
        {
            m_SessionFactory = sessionFactory;
        }

        public void Initialize(InstanceContext instanceContext, Message message)
        {
            ISessionFactory sessionFactory = ContextRegistry
                .GetContext()
                .GetObject<ISessionFactory>(m_SessionFactory ?? @"NHibernateSessionFactory");
            ISession session = sessionFactory != null
                                   ? sessionFactory.OpenSession()
                                   : null;
            NHibernateContextExtension nHibernateContextExtension = new NHibernateContextExtension(session);
            instanceContext.Extensions.Add(nHibernateContextExtension);
        }
    }
}