/*
Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#region Using

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace PPWCode.Vernacular.Semantics.I
{
    /// <summary>
    /// Abstract class that supports things
    /// required by <see cref="ISemanticObject"/>.
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public abstract class AbstractSemanticObject :
        ISemanticObject
    {
        #region Contructor

        protected AbstractSemanticObject()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Initialize(false);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        #endregion

        #region Initializer

        protected virtual void Initialize(bool onDeserializing)
        {
            m_IsSerialized = onDeserializing;
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Triggers the PropertyChangedevent if this event is assigned
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            if (PropertyChanged != null)
            {
                // ReSharper disable PolymorphicFieldLikeEventInvocation
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                // ReSharper restore PolymorphicFieldLikeEventInvocation
            }
        }

        /// <summary>
        /// Is raised whenever a property is changed, part of the INotifyPropertyChanged interface.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ISemanticObject

        public override sealed bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override sealed int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            {
                sb.Append("{ ");
                sb.AppendFormat("HashCode = '{0}'", GetHashCode());

                foreach (PropertyInfo prop in GetType().GetProperties())
                {
                    sb.AppendFormat(", {0} = ", prop.Name);
                    object value;
                    try
                    {
                        value = prop.GetValue(this, null);
                    }
                    catch (Exception e)
                    {
                        value = e.GetBaseException().Message;
                    }
                    if (value == null)
                    {
                        sb.Append("[null]");
                    }
                    else if (value is string)
                    {
                        sb.AppendFormat("'{0}'", ((string)value));
                    }
                    else if (value is IEnumerable)
                    {
                        if (value is ICollection)
                        {
                            sb.AppendFormat("[{0} elements]", ((ICollection)value).Count);
                        }
                        else
                        {
                            sb.AppendFormat("[? elements]");
                        }
                    }
                    else if (value is AbstractSemanticObject)
                    {
                        sb.Append(((AbstractSemanticObject)value).LimitedToString());
                    }
                    else
                    {
                        sb.AppendFormat("'{0}'", value);
                    }
                }
                sb.Append(" }");
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method returns a limited string representation
        /// of this object, intended for debugging purposes.
        /// </summary>
        /// <remarks>
        /// <para>The method is used in <see cref="ToString"/>,
        /// when this refers to other <c>AbstractSemanticObject</c>
        /// instances via a simple reference.</para>
        /// <para>The default implementation of this method does
        /// not include representations of other objects, but only
        /// a representation of the type and the <see cref="GetHashCode"/>.</para>
        /// <para>Subclasses can override this method to return
        /// a simple string representation that better indentifies this
        /// object.</para>
        /// </remarks>
        public virtual string LimitedToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("{ ");
            sb.AppendFormat("HashCode = '{0}'", GetHashCode());
            sb.Append(" }");
            return sb.ToString();
        }

        private bool m_IsSerialized;

        public bool IsSerialized
        {
            get
            {
                return m_IsSerialized;
            }
        }

        #endregion

        #region Serialization

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize(true);
        }

        #endregion
    }
}