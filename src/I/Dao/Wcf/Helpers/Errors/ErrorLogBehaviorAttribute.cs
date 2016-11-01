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
using System.ServiceModel.Dispatcher;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ErrorLogBehaviorAttribute :
        Attribute,
        IErrorHandler,
        IServiceBehavior
    {
        protected Type ServiceType { get; set; }

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase host)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase host)
        {
            ServiceType = description.ServiceType;
            foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(this);
            }
        }

        bool IErrorHandler.HandleError(Exception error)
        {
            ErrorHandlerHelper.LogError(error);
            return false;
        }

        void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }
    }
}
