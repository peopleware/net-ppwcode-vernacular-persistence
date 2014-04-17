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

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using PPWCode.Vernacular.Persistence.II.Dao.Wcf.Helpers.GenericInterceptor;

#endregion

namespace PPWCode.Vernacular.Persistence.II.Dao.NHibernate
{
    public class NHibernateContextAttribute
        : ServiceInterceptorBehaviorAttribute,
          IContractBehavior
    {
        public string SessionFactory { get; set; }

        #region IContractBehavior Members

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

        #endregion

        #region Overrides of ServiceInterceptorBehaviorAttribute

        protected override OperationInterceptorBehaviorAttribute CreateOperationInterceptor()
        {
            return new NHibernateFlushSessionOperationInterceptor.NHibernateFlushSessionOperationAttribute();
        }

        #endregion
    }
}