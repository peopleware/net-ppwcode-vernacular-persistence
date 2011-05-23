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
using System.Diagnostics;
using System.ServiceModel.Dispatcher;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.GenericInterceptor
{
    public abstract class GenericInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker m_PreviousInvoker;

        public GenericInvoker(IOperationInvoker previousInvoker)
        {
            Debug.Assert(previousInvoker != null);

            m_PreviousInvoker = previousInvoker;
        }

        public virtual object[] AllocateInputs()
        {
            return m_PreviousInvoker.AllocateInputs();
        }

        /// <summary>
        /// Exceptions here will abort the call
        /// </summary>
        /// <returns></returns>
        protected abstract void PreInvoke(object instance, object[] inputs);

        /// <summary>
        /// Always called, even if operation had an exception
        /// </summary>
        /// <returns></returns>
        protected abstract void PostInvoke(object instance, object returnedValue, object[] outputs, Exception exception);

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            PreInvoke(instance, inputs);
            object returnedValue = null;
            object[] outputParams = new object[] { };
            Exception exception = null;
            try
            {
                returnedValue = m_PreviousInvoker.Invoke(instance, inputs, out outputParams);
                outputs = outputParams;
                return returnedValue;
            }
            catch (Exception operationException)
            {
                exception = operationException;
                throw;
            }
            finally
            {
                PostInvoke(instance, returnedValue, outputParams, exception);
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            PreInvoke(instance, inputs);
            return m_PreviousInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            object returnedValue = null;
            object[] outputParams = { };
            Exception exception = null;

            try
            {
                returnedValue = m_PreviousInvoker.InvokeEnd(instance, out outputs, result);
                outputs = outputParams;
                return returnedValue;
            }
            catch (Exception operationException)
            {
                exception = operationException;
                throw;
            }
            finally
            {
                PostInvoke(instance, returnedValue, outputParams, exception);
            }
        }

        public bool IsSynchronous
        {
            get { return m_PreviousInvoker.IsSynchronous; }
        }
    }
}