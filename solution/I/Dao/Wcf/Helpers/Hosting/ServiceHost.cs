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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Bindings;
using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Hosting
{
    public class ServiceHost<T> : ServiceHost
    {
        private class ErrorHandlerBehavior :
            IServiceBehavior,
            IErrorHandler
        {
            private readonly IErrorHandler m_ErrorHandler;
            public ErrorHandlerBehavior(IErrorHandler errorHandler)
            {
                m_ErrorHandler = errorHandler;
            }

            void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase host)
            {
            }

            void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
            {
            }

            void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase host)
            {
                foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
                {
                    dispatcher.ErrorHandlers.Add(this);
                }
            }

            bool IErrorHandler.HandleError(Exception error)
            {
                return m_ErrorHandler.HandleError(error);
            }

            void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                m_ErrorHandler.ProvideFault(error, version, ref fault);
            }
        }

        private readonly List<IServiceBehavior> m_ErrorHandlers = new List<IServiceBehavior>();

        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public void AddErrorHandler(IErrorHandler errorHandler)
        {
            if (State == CommunicationState.Opened)
            {
                throw new InvalidOperationException("Host is already opened");
            }
            IServiceBehavior errorHandlerBehavior = new ErrorHandlerBehavior(errorHandler);

            m_ErrorHandlers.Add(errorHandlerBehavior);
        }

        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public void AddErrorHandler()
        {
            AddErrorHandler(new ErrorLogBehaviorAttribute());
        }

        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public void EnableMetadataExchange()
        {
            EnableMetadataExchange(true);
        }

        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public void EnableMetadataExchange(bool enableHttpGet)
        {
            if (State == CommunicationState.Opened)
            {
                throw new InvalidOperationException("Host is already opened");
            }

            ServiceMetadataBehavior metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();

            if (metadataBehavior == null)
            {
                metadataBehavior = new ServiceMetadataBehavior();
                Description.Behaviors.Add(metadataBehavior);

                if (BaseAddresses.Any(uri => uri.Scheme == "http"))
                {
                    metadataBehavior.HttpGetEnabled = enableHttpGet;
                }

                if (BaseAddresses.Any(uri => uri.Scheme == "https"))
                {
                    metadataBehavior.HttpsGetEnabled = enableHttpGet;
                }
            }
            AddAllMexEndPoints();
        }
        public void AddAllMexEndPoints()
        {
            foreach (Uri baseAddress in BaseAddresses)
            {
                BindingElement bindingElement = null;
                switch (baseAddress.Scheme)
                {
                    case "net.tcp":
                        {
                            bindingElement = new TcpTransportBindingElement();
                            break;
                        }
                    case "net.pipe":
                        {
                            bindingElement = new NamedPipeTransportBindingElement();
                            break;
                        }
                    case "http":
                        {
                            bindingElement = new HttpTransportBindingElement();
                            break;
                        }
                    case "https":
                        {
                            bindingElement = new HttpsTransportBindingElement();
                            break;
                        }
                    default:
                        break;
                }
                if (bindingElement != null)
                {
                    Binding binding = new CustomBinding(bindingElement);
                    AddServiceEndpoint(typeof(IMetadataExchange), binding, "MEX");
                }
            }
        }

        public bool HasMexEndpoint
        {
            get
            {
                return Description.Endpoints.Any(endpoint => endpoint.Contract.ContractType == typeof(IMetadataExchange));
            }
        }

        protected override void OnOpening()
        {
            foreach (IServiceBehavior behavior in m_ErrorHandlers)
            {
                Description.Behaviors.Add(behavior);
            }

            foreach (ServiceEndpoint endpoint in Description.Endpoints)
            {
                endpoint.VerifyQueue();
            }
            base.OnOpening();
        }

        protected override void OnClosing()
        {
            PurgeQueues();
            base.OnClosing();
        }

        [Conditional("DEBUG")]
        private void PurgeQueues()
        {
            foreach (ServiceEndpoint endpoint in Description.Endpoints)
            {
                QueuedServiceHelper.PurgeQueue(endpoint);
            }
        }

        /// <summary>
        /// Can only call after opening the host
        /// </summary>
        public ServiceThrottle Throttle
        {
            get
            {
                if (State != CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is not opened");
                }

                ChannelDispatcher dispatcher = OperationContext.Current.Host.ChannelDispatchers[0] as ChannelDispatcher;
                return dispatcher.ServiceThrottle;
            }
        }

        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public bool IncludeExceptionDetailInFaults
        {
            set
            {
                if (State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is already opened");
                }
                ServiceBehaviorAttribute debuggingBehavior = Description.Behaviors.Find<ServiceBehaviorAttribute>();
                debuggingBehavior.IncludeExceptionDetailInFaults = value;
            }
            get
            {
                ServiceBehaviorAttribute debuggingBehavior = Description.Behaviors.Find<ServiceBehaviorAttribute>();
                return debuggingBehavior.IncludeExceptionDetailInFaults;
            }
        }

        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public bool SecurityAuditEnabled
        {
            get
            {
                ServiceSecurityAuditBehavior securityAudit = Description.Behaviors.Find<ServiceSecurityAuditBehavior>();
                if (securityAudit != null)
                {
                    return securityAudit.MessageAuthenticationAuditLevel == AuditLevel.SuccessOrFailure &&
                           securityAudit.ServiceAuthorizationAuditLevel == AuditLevel.SuccessOrFailure;
                }
                return false;
            }
            set
            {
                if (State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is already opened");
                }
                ServiceSecurityAuditBehavior securityAudit = Description.Behaviors.Find<ServiceSecurityAuditBehavior>();
                if (securityAudit == null && value)
                {
                    securityAudit = new ServiceSecurityAuditBehavior
                    {
                        MessageAuthenticationAuditLevel = AuditLevel.SuccessOrFailure,
                        ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure
                    };
                    Description.Behaviors.Add(securityAudit);
                }
            }
        }

        public ServiceHost()
            : base(typeof(T))
        {
        }

        public ServiceHost(params string[] baseAddresses)
            : base(typeof(T), Convert(baseAddresses))
        {
        }

        public ServiceHost(params Uri[] baseAddresses)
            : base(typeof(T), baseAddresses)
        {
        }

        public ServiceHost(T singleton, params string[] baseAddresses)
            : base(singleton, Convert(baseAddresses))
        {
        }

        public ServiceHost(T singleton)
            : base(singleton)
        {
        }

        public ServiceHost(T singleton, params Uri[] baseAddresses)
            : base(singleton, baseAddresses)
        {
        }

        public virtual T Singleton
        {
            get
            {
                if (SingletonInstance == null)
                {
                    return default(T);
                }
                return (T)SingletonInstance;
            }
        }

        private static Uri[] Convert(IEnumerable<string> baseAddresses)
        {
            return baseAddresses
                .Select(address => new Uri(address))
                .ToArray();
        }
    }
}
