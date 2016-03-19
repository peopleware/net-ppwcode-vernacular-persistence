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

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using log4net;

using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.GenericInterceptor;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public class NHibernateContextAttribute
        : ServiceInterceptorBehaviorAttribute,
          IContractBehavior
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(NHibernateContextAttribute));

        public string SessionFactory { get; set; }

        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceContextInitializers.Add(new NHibernateContextInitializer(SessionFactory));
        }

        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        protected override OperationInterceptorBehaviorAttribute CreateOperationInterceptor()
        {
            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.DebugFormat(
                    "Requested to create an OperationInterceptor of type '{0}' with SessionFactory '{1}'.",
                    typeof(NHibernateFlushSessionOperationInterceptor.NHibernateFlushSessionOperationAttribute).Name,
                    SessionFactory);
            }

            return new NHibernateFlushSessionOperationInterceptor.NHibernateFlushSessionOperationAttribute();
        }
    }
}