// Copyright 2018 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace PPWCode.Vernacular.Persistence.III
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class PersistentObject<T>
        : CivilizedObject,
          IPersistentObject<T>
        where T : IEquatable<T>
    {
        [DataMember(Name = "Id")]
        private T _id;

        protected PersistentObject(T id)
        {
            _id = id;
        }

        protected PersistentObject()
            : this(default(T))
        {
        }

        public virtual T Id
            => _id;

        public virtual bool IsTransient
            => EqualityComparer<T>.Default.Equals(Id, default(T));

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
            // REMARK: do not change this to ToLogString() !!
            //  ToLogString() calls itself "obj.ToString()", which causes an infinite loop
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
