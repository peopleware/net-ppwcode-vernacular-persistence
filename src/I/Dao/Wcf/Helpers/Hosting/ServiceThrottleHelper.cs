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
using System.ServiceModel;
using System.ServiceModel.Description;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Hosting
{
    public static class ServiceThrottleHelper
    {
        /// <summary>
        ///     Configures the throttling behavior of the host.
        /// </summary>
        /// <param name="host">The given host.</param>
        /// <param name="maxCalls">The maximum number of concurrent calls.</param>
        /// <param name="maxSessions">The maximum number of concurrent sessions.</param>
        /// <param name="maxInstances">The maximum number of concurrent instances.</param>
        /// <remarks>
        ///     Can only call before opening the host.
        /// </remarks>
        public static void SetThrottle(this ServiceHost host, int maxCalls, int maxSessions, int maxInstances)
        {
            ServiceThrottlingBehavior throttle = new ServiceThrottlingBehavior();
            throttle.MaxConcurrentCalls = maxCalls;
            throttle.MaxConcurrentSessions = maxSessions;
            throttle.MaxConcurrentInstances = maxInstances;
            host.SetThrottle(throttle);
        }

        /// <summary>
        ///     Applies the given <see cref="ServiceThrottlingBehavior" /> to the given <see cref="ServiceHost" />.
        ///     If a throttling behavior is already present, it is only overridden if allowed by the given
        ///     <paramref name="overrideConfig" /> setting.
        /// </summary>
        /// <param name="host">The given host.</param>
        /// <param name="serviceThrottle">The given service throttling behavior.</param>
        /// <param name="overrideConfig">Whether an existing behavior can be overridden.</param>
        /// <remarks>
        ///     Can only call before opening the host.
        /// </remarks>
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
        ///     Applies the given <see cref="ServiceThrottlingBehavior" /> to the given <see cref="ServiceHost" />.
        /// </summary>
        /// <param name="host">The given host.</param>
        /// <param name="serviceThrottle">The given service throttling behavior.</param>
        /// <remarks>
        ///     Can only call before opening the host. Does not override config values if present.
        /// </remarks>
        public static void SetThrottle(this ServiceHost host, ServiceThrottlingBehavior serviceThrottle)
        {
            host.SetThrottle(serviceThrottle, false);
        }
    }
}