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
using System.Diagnostics.Contracts;
using System.ServiceModel;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors
{
    public static class DebugHelper
    {
        public const bool IncludeExceptionDetailInFaults =
#if DEBUG
 true;
#else
 false;
#endif

        public static Exception ExtractException(this FaultException<ExceptionDetail> fault)
        {
            return ExtractException(fault.Detail);
        }
        private static Exception ExtractException(ExceptionDetail detail)
        {
            Contract.Requires(detail != null);
            Contract.Requires(Type.GetType(detail.Type) != null, "Make sure this assembly contains the definition of the custom exception.");
            Contract.Requires(Type.GetType(detail.Type).IsSubclassOf(typeof(Exception)), "FaultException<T> where T is a subclass of Exception.");
            Contract.Requires(Type.GetType(detail.Type).GetConstructor(new[] { typeof(string), typeof(Exception), }) != null, "Exception type does not have suitable constructor.");

            Exception innerException;
            if (detail.InnerException != null)
            {
                innerException = ExtractException(detail.InnerException);
            }
            else
            {
                innerException = null;
            }
            Type type = Type.GetType(detail.Type);
            Exception exception = Activator.CreateInstance(type, detail.Message, innerException) as Exception;

            return exception;
        }
    }
}
