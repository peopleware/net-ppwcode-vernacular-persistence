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
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

using Iesi.Collections.Generic;

using NHibernate.Collection;
using NHibernate.Proxy;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    public class NHibernateDataContractSurrogate :
        IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            object result = obj;

            INHibernateProxy proxy = obj as INHibernateProxy;
            if (proxy != null)
            {
                ILazyInitializer li = proxy.HibernateLazyInitializer;
                result = li.IsUninitialized ? null : li.GetImplementation();
            }

            IPersistentCollection persistentCollection = obj as IPersistentCollection;
            if (persistentCollection != null)
            {
                Type genericType = obj.GetType();
                if (genericType.IsGenericType)
                {
                    Type unboundedType = typeof(HashedSet<>);
                    Type[] boundedTypes = genericType.GetGenericArguments();
                    if (boundedTypes.Length == 1)
                    {
                        Type boundedType = unboundedType.MakeGenericType(boundedTypes[0]);
                        result = Activator.CreateInstance(boundedType);
                    }
                }
                result = persistentCollection.WasInitialized ? persistentCollection.Entries(null) : result;
            }
            return result;
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            return null;
        }

        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            return null;
        }

        public Type GetDataContractType(Type type)
        {
            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            return obj;
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }

        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            return typeDeclaration;
        }

        #endregion
    }
}
