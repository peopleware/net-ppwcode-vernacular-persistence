// Copyright 2010-2016 by PeopleWare n.v..
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
using System.Diagnostics.Contracts;
using System.Threading;

namespace PPWCode.Vernacular.Persistence.I.Dao
{
    [ContractClass(typeof(IPPWSecurityContract))]
    public interface IPPWSecurity
    {
        /// <summary>
        ///     Checks if the CurrentPrincipal has enough security for Action "securityActionFlag" on Type "type".
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <param name="securityActionFlag">The intended action.</param>
        /// <returns>A <see cref="bool" /> indicating whether the current user has enough rights.</returns>
        bool HasSufficientSecurity(Type type, SecurityActionFlag securityActionFlag);

        /// <summary>
        ///     Checks if the CurrentPrincipal has enough security for Action "securityActionFlag" on Type "type".
        ///     If not it will throw an exception of type <see cref="DaoSecurityException" />.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <param name="securityActionFlag">The intended action.</param>
        void CheckSecurity(Type type, SecurityActionFlag securityActionFlag);
    }

    /// <exclude />
    /// <summary>The contract class for <see cref="IPPWSecurity" />.</summary>
    [ContractClassFor(typeof(IPPWSecurity))]
    public abstract class IPPWSecurityContract :
        IPPWSecurity
    {
        bool IPPWSecurity.HasSufficientSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            Contract.Assume(
                Thread.CurrentPrincipal.Identity != null
                && Thread.CurrentPrincipal.Identity.IsAuthenticated);

            return default(bool);
        }

        void IPPWSecurity.CheckSecurity(Type type, SecurityActionFlag securityActionFlag)
        {
            Contract.Assume(
                Thread.CurrentPrincipal.Identity != null
                && Thread.CurrentPrincipal.Identity.IsAuthenticated);
        }
    }
}
