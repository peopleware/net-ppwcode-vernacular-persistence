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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Duplex;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Hosting
{
    public static class InProcFactory
    {
        private struct HostRecord
        {
            public HostRecord(ServiceHost host, string address)
            {
                m_Host = host;
                m_Address = new EndpointAddress(address);
            }

            // ReSharper disable InconsistentNaming
            public readonly ServiceHost m_Host;
            public readonly EndpointAddress m_Address;
            // ReSharper restore InconsistentNaming
        }

        private static readonly Uri s_BaseAddress = new Uri("net.pipe://localhost/" + Guid.NewGuid());

        private static readonly Binding s_Binding;

        private static readonly Dictionary<Type, HostRecord> s_Hosts = new Dictionary<Type, HostRecord>();
        private static readonly Dictionary<Type, ServiceThrottlingBehavior> s_Throttles = new Dictionary<Type, ServiceThrottlingBehavior>();
        private static readonly Dictionary<Type, object> s_Singletons = new Dictionary<Type, object>();

        static InProcFactory()
        {
            NetNamedPipeBinding binding;
            try
            {
                binding = new NetNamedPipeBinding("InProcFactory");
            }
            catch
            {
                binding = new NetNamedPipeBinding();
            }

            binding.TransactionFlow = true;
            s_Binding = binding;
            AppDomain.CurrentDomain.ProcessExit +=
                (sender, e) =>
                {
                    foreach (HostRecord hostRecord in s_Hosts.Values)
                    {
                        hostRecord.m_Host.Close();
                    }
                };
        }

        /// <summary>
        ///     Can only call SetThrottle() before creating any instance of the service.
        /// </summary>
        /// <typeparam name="S">Service type.</typeparam>
        /// <param name="throttle">Throttle to use.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetThrottle<S>(ServiceThrottlingBehavior throttle)
        {
            if (s_Throttles.ContainsKey(typeof(S)))
            {
                s_Throttles[typeof(S)] = throttle;
            }
            else
            {
                s_Throttles.Add(typeof(S), throttle);
            }
        }

        /// <summary>
        ///     Can only call SetThrottle() before creating any instance of the service.
        /// </summary>
        /// <typeparam name="S">Service type.</typeparam>
        /// <param name="maxCalls">The maximum number of concurrent calls.</param>
        /// <param name="maxSessions">The maximum number of concurrent sessions.</param>
        /// <param name="maxInstances">The maximum number of concurrent instances.</param>
        public static void SetThrottle<S>(int maxCalls, int maxSessions, int maxInstances)
        {
            ServiceThrottlingBehavior throttle = new ServiceThrottlingBehavior
                                                 {
                                                     MaxConcurrentCalls = maxCalls,
                                                     MaxConcurrentSessions = maxSessions,
                                                     MaxConcurrentInstances = maxInstances,
                                                 };
            SetThrottle<S>(throttle);
        }

        /// <summary>
        ///     Can only call SetSingleton() before creating any instance of the service.
        /// </summary>
        /// <typeparam name="S">Service type.</typeparam>
        /// <param name="singleton">The given singleton.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetSingleton<S>(S singleton)
        {
            if (s_Singletons.ContainsKey(typeof(S)))
            {
                s_Singletons[typeof(S)] = singleton;
            }
            else
            {
                s_Singletons.Add(typeof(S), singleton);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static I CreateInstance<S, I>()
            where I : class
            where S : class, I
        {
            HostRecord hostRecord = GetHostRecord<S, I>();
            ChannelFactory<I> factory = new ChannelFactory<I>(s_Binding, hostRecord.m_Address);
            return factory.CreateChannel();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static I CreateInstance<S, I>(Action<ServiceEndpoint> serviceEndPointAction, Action<ChannelFactory<I>> channelfactoryAction)
            where I : class
            where S : class, I
        {
            HostRecord hostRecord = GetHostRecord<S, I>(serviceEndPointAction);
            ChannelFactory<I> factory = new ChannelFactory<I>(s_Binding, hostRecord.m_Address);
            if (channelfactoryAction != null)
            {
                channelfactoryAction.Invoke(factory);
            }

            if (serviceEndPointAction != null)
            {
                serviceEndPointAction(factory.Endpoint);
            }

            return factory.CreateChannel();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static I CreateInstance<S, I, C>(InstanceContext<C> context)
            where I : class
            where S : class, I
        {
            HostRecord hostRecord = GetHostRecord<S, I>();
            return DuplexChannelFactory<I, C>.CreateChannel(context, s_Binding, hostRecord.m_Address);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static I CreateInstance<S, I, C>(C callback)
            where I : class
            where S : class, I
        {
            DuplexClientBase<I, C>.VerifyCallback();
            InstanceContext<C> context = new InstanceContext<C>(callback);
            return CreateInstance<S, I, C>(context);
        }

        private static HostRecord GetHostRecord<S, I>(Action<ServiceEndpoint> serviceEndPointAction)
            where I : class
            where S : class, I
        {
            HostRecord hostRecord;
            if (s_Hosts.ContainsKey(typeof(S)))
            {
                hostRecord = s_Hosts[typeof(S)];
            }
            else
            {
                ServiceHost<S> host;
                if (s_Singletons.ContainsKey(typeof(S)))
                {
                    S singleton = s_Singletons[typeof(S)] as S;
                    host = new ServiceHost<S>(singleton, s_BaseAddress);
                }
                else
                {
                    host = new ServiceHost<S>(s_BaseAddress);
                }

                string address = s_BaseAddress + Guid.NewGuid().ToString();

                hostRecord = new HostRecord(host, address);
                s_Hosts.Add(typeof(S), hostRecord);
                ServiceEndpoint serviceEndpoint = host.AddServiceEndpoint(typeof(I), s_Binding, address);
                if (serviceEndPointAction != null)
                {
                    serviceEndPointAction(serviceEndpoint);
                }

                if (s_Throttles.ContainsKey(typeof(S)))
                {
                    host.SetThrottle(s_Throttles[typeof(S)]);
                }

                host.Open();
            }

            return hostRecord;
        }

        private static HostRecord GetHostRecord<S, I>()
            where I : class
            where S : class, I
        {
            return GetHostRecord<S, I>(null);
        }

        public static void CloseProxy<I>(I instance) where I : class
        {
            ICommunicationObject proxy = instance as ICommunicationObject;
            if (proxy == null)
            {
                return;
            }

            switch (proxy.State)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                    proxy.Close();
                    break;
                case CommunicationState.Closing:
                case CommunicationState.Closed:
                case CommunicationState.Faulted:
                    proxy.Abort();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}