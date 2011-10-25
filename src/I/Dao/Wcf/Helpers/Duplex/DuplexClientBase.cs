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
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Duplex
{
    public abstract class DuplexClientBase<T, C> : DuplexClientBase<T> where T : class
    {
        static DuplexClientBase()
        {
            VerifyCallback();
        }

        internal static void VerifyCallback()
        {
            Type contractType = typeof(T);
            Type callbackType = typeof(C);
            object[] attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (attributes.Length != 1)
            {
                throw new InvalidOperationException(String.Format("Type of {0} is not a service contract", contractType));
            }
            ServiceContractAttribute serviceContractAttribute = attributes[0] as ServiceContractAttribute;
            if (callbackType != serviceContractAttribute.CallbackContract)
            {
                throw new InvalidOperationException(String.Format("Type of {0} is not configured as callback contract for {1}", callbackType, contractType));
            }
        }

        protected DuplexClientBase(InstanceContext<C> context)
            : base(context.Context)
        {
        }

        protected DuplexClientBase(InstanceContext<C> context, string endpointName)
            : base(context.Context, endpointName)
        {
        }

        protected DuplexClientBase(InstanceContext<C> context, Binding binding, EndpointAddress remoteAddress)
            : base(context.Context, binding, remoteAddress)
        {
        }

        protected DuplexClientBase(InstanceContext<C> context, string endpointName, EndpointAddress remoteAddress)
            : base(context.Context, endpointName, remoteAddress)
        {
        }

        protected DuplexClientBase(InstanceContext<C> context, string endpointName, string remoteAddress)
            : base(context.Context, endpointName, remoteAddress)
        {
        }

        protected DuplexClientBase(C callback)
            : base(callback)
        {
        }

        protected DuplexClientBase(C callback, string endpointName)
            : base(callback, endpointName)
        {
        }

        protected DuplexClientBase(C callback, Binding binding, EndpointAddress remoteAddress)
            : base(callback, binding, remoteAddress)
        {
        }

        protected DuplexClientBase(C callback, string endpointName, EndpointAddress remoteAddress)
            : base(callback, endpointName, remoteAddress)
        {
        }

        protected DuplexClientBase(C calback, string endpointName, string remoteAddress)
            : base(calback, endpointName, remoteAddress)
        {
        }
    }
}
