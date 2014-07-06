// Copyright 2014 by PeopleWare n.v..
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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class PersistentObject<T>
        : IPersistentObject<T>,
          ICivilizedObject
        where T : IEquatable<T>
    {
        [DataMember]
        private T m_Id;

        protected PersistentObject(T id)
        {
            m_Id = id;
        }

        protected PersistentObject()
            : this(default(T))
        {
        }

        /// <summary>
        ///     A call to <see cref="ICivilizedObject.WildExceptions" />
        ///     returns an <see cref="CompoundSemanticException.IsEmpty" />
        ///     exception.
        /// </summary>
        [Pure]
        public virtual bool IsCivilized
        {
            get { return WildExceptions().IsEmpty; }
        }

        /// <summary>
        ///     Build a set of <see cref="CompoundSemanticException" /> instances
        ///     that tell what is wrong with this instance, with respect to
        ///     <em>being civilized</em>.
        /// </summary>
        /// <returns>
        ///     <para>
        ///         The result comes in the form of an <strong>unclosed</strong>
        ///         <see cref="CompoundSemanticException" />, of
        ///         which the set of element exceptions might be empty.
        ///     </para>
        ///     <para>This method should work in any state of the object.</para>
        ///     <para>
        ///         This method is public instead of
        ///         protected to make it more easy to describe to users what the business
        ///         rules for this type are.
        ///     </para>
        /// </returns>
        [Pure]
        public virtual CompoundSemanticException WildExceptions()
        {
            return new CompoundSemanticException();
        }

        /// <summary>
        ///     Call <see cref="ICivilizedObject.WildExceptions" />, and if the result
        ///     is not <see cref="CompoundSemanticException.IsEmpty" />,
        ///     close the exception and throw it.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method has no effects. If it ends nominally,
        ///         and if it throws an exception, no state is changed.
        ///     </para>
        ///     <para>
        ///         It is not <c>[Pure]</c> however, since it changes
        ///         the state of the exception to
        ///         <see cref="CompoundSemanticException.Closed" />.
        ///     </para>
        /// </remarks>
        public virtual void ThrowIfNotCivilized()
        {
            CompoundSemanticException cse = WildExceptions();

            if (cse != null && !cse.IsEmpty)
            {
                cse.Closed = true;
                throw cse;
            }
        }

        [Pure]
        public virtual T Id
        {
            get { return m_Id; }
        }

        [Pure]
        public virtual bool IsTransient
        {
            get { return EqualityComparer<T>.Default.Equals(Id, default(T)); }
        }

        [Pure]
        public virtual bool IsSame(IIdentity<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (IsTransient || other.IsTransient)
            {
                return false;
            }

            if (!EqualityComparer<T>.Default.Equals(Id, other.Id))
            {
                return false;
            }

            Type otherType = other.GetType();
            Type thisType = GetType();
            return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Type = '{0}' Id = '{1}'; HashCode = '{2}'", GetType().Name, Id, GetHashCode());

            IEnumerable<PropertyInfo> propertyInfos = GetType()
                .GetProperties()
                .Where(propertyInfo => propertyInfo.PropertyType.IsValueType);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object value;
                try
                {
                    value = propertyInfo.GetValue(this, null);
                }
                catch (Exception e)
                {
                    value = e.GetBaseException().Message;
                }

                sb.AppendFormat("; {0} = '{1}'", propertyInfo.Name, value);
            }

            return sb.ToString();
        }
    }
}