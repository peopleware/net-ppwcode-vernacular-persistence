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
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

#endregion

namespace PPWCode.Vernacular.Persistence.II.Dao.NHibernate
{
    /// <summary>
    /// <see cref="IUserType"/> for <see cref="Uri"/> instances,
    /// with support for wildcards in a &quot;Like&quot; criterium
    /// expression.
    /// </summary>
    /// <remarks>
    /// <para><see cref="Uri"/> instances are stored as canonical strings.</para>
    /// <para>We only support <see cref="Uri.IsAbsoluteUri">absolute URI's</see></para>
    /// </remarks>
    public class UriUserType : IUserType
    {
        /// <summary>
        /// We store our Uri in a single column in the database that can contain a string.
        /// </summary>
        public SqlType[] SqlTypes
        {
            get
            {
                SqlType[] types = new SqlType[1];
                types[0] = new SqlType(DbType.String);
                return types;
            }
        }

        /// <summary>
        /// This is a user type for <see cref="Uri"/> instances.
        /// </summary>
        public Type ReturnedType
        {
            get
            {
                return typeof(Uri);
            }
        }

        /// <summary>
        /// <see cref="Uri"/> implements <see cref="Uri.Equals(object)"/>
        /// it self by comparing the Uri's based
        /// on value so we use this implementation.
        /// </summary>
        public new bool Equals(object x, object y)
        {
            if (x == null)
            {
                return false;
            }
            return x.Equals(y);
        }

        /// <summary>
        /// <see cref="Uri"/> implements <see cref="Uri.GetHashCode"/>,
        /// so we use that.
        /// </summary>
        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        /// <summary>
        /// Turn a canonical representation of a URI, retrieved
        /// from the database, into a <see cref="Uri"/>.
        /// </summary>
        /// <returns>We throw an error if what we get from
        /// the database is not a canonical URI.</returns>
        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            //We get the string from the database using the NullSafeGet used to get strings
            string uriString = (string)NHibernateUtil.String.NullSafeGet(rs, names[0]);
            if (uriString == null)
            {
                return null;
            }
            if (! Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
            {
                throw new HibernateException("Trying to create a Uri from a string from the DB which is not a valid absolute Uri string: " + uriString);
            }
            //And save it in the Uri object. This would be the place to make sure that your string
            //is valid for use with the System.Uri class, but i will leave that to you
            Uri result = new Uri(uriString, UriKind.Absolute);
            return result;
        }

        public const string UriLikeWildcard = @"PPWCODE.ULWC";

        /// <summary>
        /// Write a <paramref name="value"/> that is a <see cref="Uri"/>
        /// into the <paramref name="cmd"/> at position <paramref name="index"/>
        /// as the canonical representation of the URI.
        /// </summary>
        /// <remarks>
        /// <para>This is also used by Hibernate for parameters in a &quot;Like&quot;
        /// query. In a &quot;Like&quot; query, we want to use a wild card.
        /// We recognize a <see cref="Uri"/> as being used as parameter in a
        /// &quot;Like&quot; if we find the wild card pattern in the
        /// <see cref="Uri"/> canonical string.</para>
        /// </remarks>
        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value != null && (! (value is Uri)))
            {
                throw new HibernateException("Trying to use UriUserType with " + value + ", which is not a Uri");
            }
            string canonicalString;
            if (value == null)
            {
                canonicalString = null;
            }
            else
            {
                Uri uri = (Uri)value;
                if (! uri.IsAbsoluteUri)
                {
                    throw new HibernateException("Trying to use UriUserTYpe with " + uri + ", which is not an absolute URI");
                }
                canonicalString = uri.AbsoluteUri;
                // canonical string is guaranteed not null
                if (canonicalString.Contains(UriLikeWildcard))
                {
                    // we are being used as a like parameter
                    canonicalString = canonicalString.Replace(@"%", @"[%]");
                    canonicalString = canonicalString.Replace(@"_", @"[_]");
                    canonicalString = canonicalString.Replace(@"?", @"[?]");
                    canonicalString = canonicalString.Replace(UriLikeWildcard, @"%");
                }
            }
            NHibernateUtil.String.NullSafeSet(cmd, canonicalString, index);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public bool IsMutable
        {
            get
            {
                return false;
            }
        }

        public object Replace(object original, object target, object owner)
        {
            //As our object is immutable we can just return the original
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            //Used for casching, as our object is immutable we can just return it as is
            return cached;
        }

        public object Disassemble(object value)
        {
            //Used for casching, as our object is immutable we can just return it as is
            return value;
        }
    }
}