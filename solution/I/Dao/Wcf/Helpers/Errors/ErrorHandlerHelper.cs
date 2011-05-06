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

#region Using

using System;
using System.Linq;
using System.ServiceModel.Channels;

using log4net;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors
{
    public static class ErrorHandlerHelper
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(ErrorHandlerHelper));

        public static void LogError(Exception error)
        {
            LogError(error, null);
        }

        public static void LogError(Exception error, MessageFault fault)
        {
            string message = CreateLogbookentry(error, fault).ToString();
            s_Logger.Error(message, error);
        }

        private static ExceptionLogbookEntry CreateLogbookentry(Exception error, MessageFault fault)
        {
            string typeName;
            string methodName;

            string assemblyName = typeName = methodName = "Unknown";

            if (error.TargetSite != null)
            {
                assemblyName = error.TargetSite.Module.Assembly.GetName().Name;
                methodName = error.TargetSite.Name;
                typeName = error.TargetSite.DeclaringType.Name;
            }

            string fileName = GetFileName(error);
            int lineNumber = GetLineNumber(error);
            string exceptionName = error.GetType().ToString();
            string exceptionMessage = error.Message;
            string providedFault = String.Empty;
            string providedMessage = String.Empty;

            if (fault != null)
            {
                providedFault = fault.Code.Name;
                providedMessage = fault.Reason.Translations[0].Text;
            }
            return new ExceptionLogbookEntry(assemblyName, fileName, lineNumber, typeName, methodName, exceptionName, exceptionMessage, providedFault, providedMessage);
        }

        private static string GetFileName(Exception error)
        {
            if (error.StackTrace == null)
            {
                return "Unavailable";
            }
            int originalLineIndex = error.StackTrace.IndexOf(":line");
            if (originalLineIndex == -1)
            {
                return "Unavailable";
            }
            string originalLine = error.StackTrace.Substring(0, originalLineIndex);
            string[] sections = originalLine.Split('\\');
            return sections[sections.Length - 1];
        }

        private static int GetLineNumber(Exception error)
        {
            if (error.StackTrace == null)
            {
                return 0;
            }
            string[] sections = error.StackTrace.Split(' ');
            int index = sections.TakeWhile(section => !section.EndsWith(":line")).Count();
            if (index == sections.Length)
            {
                return 0;
            }
            string lineNumber = sections[index + 1];
            int number;
            try
            {
                number = Convert.ToInt32(lineNumber.Substring(0, lineNumber.Length - 2));
            }
            catch (FormatException)
            {
                number = Convert.ToInt32(lineNumber);
            }

            return number;
        }
    }
}
