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
using System.ServiceModel.Description;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Hosting
{
    public static class ServiceThrottleHelper
    {
        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public static void SetThrottle(this ServiceHost host, int maxCalls, int maxSessions, int maxInstances)
        {
            ServiceThrottlingBehavior throttle = new ServiceThrottlingBehavior();
            throttle.MaxConcurrentCalls = maxCalls;
            throttle.MaxConcurrentSessions = maxSessions;
            throttle.MaxConcurrentInstances = maxInstances;
            host.SetThrottle(throttle);
        }

        /// <summary>
        /// Can only call before opening the host
        /// </summary>
        public static void SetThrottle(this ServiceHost host, ServiceThrottlingBehavior serviceThrottle, bool overrideConfig)
        {
            if (host.State == CommunicationState.Opened)
            {
                throw new InvalidOperationException("Host is already opened");
            }
            ServiceThrottlingBehavior throttle = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (throttle == null)
            {
                host.Description.Behaviors.Add(serviceThrottle);
                return;
            }
            if (overrideConfig == false)
            {
                return;
            }
            host.Description.Behaviors.Remove(throttle);
            host.Description.Behaviors.Add(serviceThrottle);
        }

        /// <summary>
        /// Can only call before opening the host. Does not override config values if present
        /// </summary>
        public static void SetThrottle(this ServiceHost host, ServiceThrottlingBehavior serviceThrottle)
        {
            host.SetThrottle(serviceThrottle, false);
        }
    }
}
