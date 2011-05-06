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
using System.Configuration;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Bindings
{
    public static class QueuedServiceHelper
    {
        public static void VerifyQueues()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(config);

            foreach (ChannelEndpointElement endpointElement in sectionGroup.Client.Endpoints)
            {
                if (endpointElement.Binding == "netMsmqBinding")
                {
                    string queue = GetQueueFromUri(endpointElement.Address);

                    if (MessageQueue.Exists(queue) == false)
                    {
                        MessageQueue.Create(queue, true);
                    }
                }
            }
        }

        public static void VerifyQueue<T>(string endpointName) where T : class
        {
            using (ChannelFactory<T> factory = new ChannelFactory<T>(endpointName))
            {
                factory.Endpoint.VerifyQueue();
            }
        }

        public static void VerifyQueue<T>() where T : class
        {
            VerifyQueue<T>(string.Empty);
        }

        public static void VerifyQueue(this ServiceEndpoint endpoint)
        {
            if (endpoint.Binding is NetMsmqBinding)
            {
                string queue = GetQueueFromUri(endpoint.Address.Uri);

                if (MessageQueue.Exists(queue) == false)
                {
                    MessageQueue.Create(queue, true);
                }
                NetMsmqBinding binding = endpoint.Binding as NetMsmqBinding;
                if (binding.DeadLetterQueue == DeadLetterQueue.Custom)
                {
                    string DLQ = GetQueueFromUri(binding.CustomDeadLetterQueue);
                    if (MessageQueue.Exists(DLQ) == false)
                    {
                        MessageQueue.Create(DLQ, true);
                    }
                }
            }
        }

        public static void PurgeQueue(ServiceEndpoint endpoint)
        {
            if (endpoint.Binding is NetMsmqBinding)
            {
                string queueName = GetQueueFromUri(endpoint.Address.Uri);

                if (MessageQueue.Exists(queueName) == true)
                {
                    using (MessageQueue queue = new MessageQueue(queueName))
                    {
                        queue.Purge();
                    }
                }
            }
        }

        internal static string GetQueueFromUri(Uri uri)
        {
            string queue = String.Empty;

            if (uri.Segments[1] == @"private/")
            {
                queue = @".\private$\" + uri.Segments[2];
            }
            else
            {
                queue = uri.Host;
                foreach (string segment in uri.Segments)
                {
                    if (segment == "/")
                    {
                        continue;
                    }
                    string localSegment = segment;
                    if (segment[segment.Length - 1] == '/')
                    {
                        localSegment = segment.Remove(segment.Length - 1);
                    }
                    queue += @"\";
                    queue += localSegment;
                }
            }
            return queue;
        }
    }
}
