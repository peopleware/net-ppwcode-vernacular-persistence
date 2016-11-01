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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class NHibernateSerializationContractBehaviorAttribute :
        Attribute,
        IContractBehavior
    {
        private static void ApplyDataContractSurrogate(ContractDescription contractDescription)
        {
            IEnumerable<DataContractSerializerOperationBehavior> dataContractSerializerOperationBehaviors = contractDescription
                .Operations
                .Select(operation => operation.Behaviors.Find<DataContractSerializerOperationBehavior>())
                .Where(dataContractBehavior => dataContractBehavior != null);
            foreach (DataContractSerializerOperationBehavior dataContractBehavior in dataContractSerializerOperationBehaviors)
            {
                dataContractBehavior.DataContractSurrogate = new NHibernateDataContractSurrogate();
            }
        }

        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            ApplyDataContractSurrogate(contractDescription);
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            ApplyDataContractSurrogate(contractDescription);
        }

        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }
    }
}
