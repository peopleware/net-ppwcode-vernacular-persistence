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

using System.Collections.Generic;
using System.Diagnostics.Contracts;

using PPWCode.Vernacular.Semantics.I;

#endregion

namespace PPWCode.Vernacular.Semantics.Test_I
{
    public class SemanticObjectA : AbstractSemanticObject
    {
        [ContractInvariantMethod]
        private void TypeInvariants()
        {
            Contract.Invariant(Bs != null);
            Contract.Invariant(Contract.ForAll(Bs, b => b.A == this));
        }

        public SemanticObjectA()
        {
            Contract.Ensures(Bs.Count == 0);
        }

        private readonly List<SemanticObjectB> m_Bs = new List<SemanticObjectB>();

        public IList<SemanticObjectB> Bs
        {
            get
            {
                return new List<SemanticObjectB>(m_Bs);
            }
        }

        internal void AddB(SemanticObjectB b)
        {
            Contract.Requires(b != null);
            Contract.Requires(b.A == this);
            Contract.Ensures(Bs.Contains(b));

            if (!m_Bs.Contains(b))
            {
                m_Bs.Add(b);
            }
        }

        internal void RemoveB(SemanticObjectB b)
        {
            Contract.Ensures(! Bs.Contains(b));

            m_Bs.Remove(b);
        }

        /// <summary>
        /// Closing the loop.
        /// This could create a loop, as it is downstream, but not a collection
        /// </summary>
        public SemanticObjectC C { get; set; }
    }
}