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
using System.Data;

using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public class UriUserType : IUserType
    {
     public SqlType[] SqlTypes
     {
         get
         {
             //We store our Uri in a single column in the database that can contain a string
             SqlType[] types = new SqlType[1];
             types[0] = new SqlType(DbType.String);
             return types;
         }
     }

      public Type ReturnedType
      {
          get { return typeof(Uri); }
      }

      public new bool Equals(object x, object y)
      {
          //Uri implements Equals it self by comparing the Uri's based
          // on value so we use this implementation
          if (x == null)
          {
              return false;
          }
          else
          {
              return x.Equals(y);
          }
      }

      public int GetHashCode(object x)
      {
          //Again URL itself implements GetHashCode so we use that
          return x.GetHashCode();
      }

      public object NullSafeGet(IDataReader rs, string[] names, object owner)
      {
          //We get the string from the database using the NullSafeGet used to get strings
          string uriString = (string) NHibernateUtil.String.NullSafeGet(rs, names[0]);

          //And save it in the Uri object. This would be the place to make sure that your string
          //is valid for use with the System.Uri class, but i will leave that to you
          Uri result = new Uri(uriString);
          return result;
      }

      public void NullSafeSet(IDbCommand cmd, object value, int index)
      {
          Uri uri = value as Uri;
          //Set the value using the NullSafeSet implementation for string from NHibernateUtil
          if (uri == null)
          {
              NHibernateUtil.String.NullSafeSet(cmd, null, index);
              return;
          }
          //ToString called on Uri instance
          value = uri.OriginalString;
          NHibernateUtil.String.NullSafeSet(cmd, value, index);
      }

      public object DeepCopy(object value)
      {
          Uri uri = value as Uri;
          // We deep copy the uri by creating a new instance with the same contents
          if (uri == null)
          {
              return null;
          }
          return new Uri(uri.OriginalString);
      }

      public bool IsMutable
      {
          get { return false; }
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
