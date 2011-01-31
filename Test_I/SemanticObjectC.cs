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

using System.Diagnostics.Contracts;

using PPWCode.Vernacular.Semantics.I;

#endregion

namespace PPWCode.Vernacular.Semantics.Test_I
{
    public class SemanticObjectC : AbstractSemanticObject
    {
        [ContractInvariantMethod]
        private void TypeInvariants()
        {
            Contract.Invariant(B != null ? B.Cs.Contains(this) : true);
        }

        public SemanticObjectC()
        {
            Contract.Ensures(B == null);
        }

        private SemanticObjectB m_B;

        public SemanticObjectB B
        {
            get
            {
                return m_B;
            }
            set
            {
                Contract.Ensures(Contract.OldValue(B) != null && Contract.OldValue(B) != value
                                     ? ! Contract.OldValue(B).Cs.Contains(this) : true);
                Contract.Ensures(B == value);

                if (m_B != null)
                {
                    m_B.RemoveC(this);
                }
                m_B = value;
                if (m_B != null)
                {
                    m_B.AddC(this);
                }
            }
        }

        /// <summary>
        /// Closing the loop.
        /// </summary>
        public SemanticObjectA A { get; set; }

        public override string LimitedToString()
        {
            return "TEST";
        }
    }
}