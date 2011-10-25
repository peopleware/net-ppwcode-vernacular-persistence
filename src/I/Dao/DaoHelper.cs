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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Iesi.Collections.Generic;

using PPWCode.Util.OddsAndEnds.I.Extensions;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    public static class DaoHelper
    {
        #region fields

        private static readonly object s_Lock = new object();
        private static readonly Dictionary<Type, IEnumerable<Assembly>> s_Assemblies = new Dictionary<Type, IEnumerable<Assembly>>();
        private static readonly IDictionary<Type, IEnumerable<Type>> s_KnownTypes = new Dictionary<Type, IEnumerable<Type>>();
        private static readonly Type s_GenericUnboundType;

        #endregion

        #region Static Initialization

        static DaoHelper()
        {
            s_GenericUnboundType = typeof(HashedSet<>);
        }

        #endregion

        #region Property helpers

        public static void RegisterAssembly(Type service, IEnumerable<Assembly> assemblies)
        {
            lock (s_Lock)
            {
                s_Assemblies.Add(service, assemblies);
            }
        }

        #endregion

        #region Methods

        public static IEnumerable<Type> GetKnownTypes(Type service)
        {
            lock (s_Lock)
            {
                IEnumerable<Type> result;
                if (!s_KnownTypes.TryGetValue(service, out result))
                {
                    IEnumerable<Assembly> assemblies = s_Assemblies[service];
                    result = assemblies.GetKnownTypes(s_GenericUnboundType);
                    s_KnownTypes.Add(service, result);
                }
                return result ?? Enumerable.Empty<Type>();
            }
        }

        #endregion
    }
}
