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

using log4net;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers
{
    public static class CommunicationProxyHelper
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(CommunicationProxyHelper));

        public static void Close(ICommunicationObject co)
        {
            if (co == null)
            {
                return;
            }

            try
            {
                switch (co.State)
                {
                    case CommunicationState.Closed:
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Faulted:
                        co.Abort();
                        break;
                    case CommunicationState.Created:
                    case CommunicationState.Opened:
                    case CommunicationState.Opening:
                        co.Close();
                        break;
                    default:
                        co.Abort();
                        break;
                }
            }
            catch (Exception e)
            {
                s_Logger.Error(co, e);
                // ReSharper disable EmptyGeneralCatchClause
                try
                {
                    co.Abort();
                }
                catch
                {
                }
                // ReSharper restore EmptyGeneralCatchClause
            }
        }
    }
}