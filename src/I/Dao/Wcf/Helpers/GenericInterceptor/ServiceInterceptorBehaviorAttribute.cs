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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using log4net;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.GenericInterceptor
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ServiceInterceptorBehaviorAttribute
        : Attribute,
          IServiceBehavior
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(ServiceInterceptorBehaviorAttribute));

        protected abstract OperationInterceptorBehaviorAttribute CreateOperationInterceptor();

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase host)
        {
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
            {
                foreach (var operation in endpoint.Contract.Operations)
                {
                    IOperationBehavior operationBehavior = CreateOperationInterceptor();
                    if (!operation.Behaviors.Contains(operationBehavior.GetType()))
                    {
                        operation.Behaviors.Add(operationBehavior);

                        if (s_Logger.IsDebugEnabled)
                        {
                            s_Logger.DebugFormat(
                                "Added operationbehavior of type '{0}' onto method '{1}.{2}'",
                                operationBehavior.GetType().Name,
                                operation.DeclaringContract.Name,
                                operation.Name);
                        }
                    }
                }
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
