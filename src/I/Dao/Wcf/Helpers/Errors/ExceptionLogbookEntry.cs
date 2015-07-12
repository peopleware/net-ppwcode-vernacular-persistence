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
using System.Diagnostics;
using System.Reflection;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors
{
    public struct ExceptionLogbookEntry
    {
        public ExceptionLogbookEntry(string assemblyName, string fileName, int lineNumber, string typeName, string methodName, string exceptionName, string exceptionMessage, string providedFault, string providedMessage, string eventDescription)
            : this(assemblyName, fileName, lineNumber, typeName, methodName, exceptionName, exceptionMessage, providedFault, providedMessage)
        {
            Event = eventDescription;
        }

        public ExceptionLogbookEntry(string assemblyName, string fileName, int lineNumber, string typeName, string methodName, string exceptionName, string exceptionMessage)
            : this(assemblyName, fileName, lineNumber, typeName, methodName, exceptionName, exceptionMessage, string.Empty, string.Empty)
        {
        }

        public ExceptionLogbookEntry(string assemblyName, string fileName, int lineNumber, string typeName, string methodName, string exceptionName, string exceptionMessage, string providedFault, string providedMessage)
        {
            MachineName = Environment.MachineName;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            HostName = entryAssembly == null
                           ? Process.GetCurrentProcess().MainModule.ModuleName
                           : entryAssembly.GetName().Name;
            AssemblyName = assemblyName;
            FileName = fileName;
            LineNumber = lineNumber;
            TypeName = typeName;
            MemberAccessed = methodName;
            Date = DateTime.Now.ToShortDateString();
            Time = DateTime.Now.ToLongTimeString();
            ExceptionName = exceptionName;
            ExceptionMessage = exceptionMessage;
            ProvidedFault = providedFault;
            ProvidedMessage = providedMessage;
            Event = string.Empty;
        }

        public readonly string MachineName;
        public readonly string HostName;
        public readonly string AssemblyName;
        public readonly string FileName;
        public readonly int LineNumber;
        public readonly string TypeName;
        public readonly string MemberAccessed;
        public readonly string Date;
        public readonly string Time;
        public readonly string ExceptionName;
        public readonly string ExceptionMessage;
        public readonly string ProvidedFault;
        public readonly string ProvidedMessage;
        public readonly string Event;

        public override string ToString()
        {
            return
                string.Format(
                    "MachineName: {1}{0}" +
                    "HostName: {2}{0}" +
                    "AssemblyName: {3}{0}" +
                    "FileName: {4}{0}" +
                    "LineNumber: {5}{0}" +
                    "TypeName: {6}{0}" +
                    "MemberAccessed: {7}{0}" +
                    "Date: {8}{0}" +
                    "Time: {9}{0}" +
                    "ExceptionName: {10}{0}" +
                    "ExceptionMessage: {11}{0}" +
                    "ProvidedFault: {12}{0}" +
                    "ProvidedMessage: {13}{0}" +
                    "Event: {14}{0}",
                    Environment.NewLine,
                    MachineName,
                    HostName,
                    AssemblyName,
                    FileName,
                    LineNumber,
                    TypeName,
                    MemberAccessed,
                    Date,
                    Time,
                    ExceptionName,
                    ExceptionMessage,
                    ProvidedFault,
                    ProvidedMessage,
                    Event);
        }
    }
}