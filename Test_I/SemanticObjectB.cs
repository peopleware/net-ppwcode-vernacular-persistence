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
    public class SemanticObjectB : AbstractSemanticObject
    {
        [ContractInvariantMethod]
        private void TypeInvariants()
        {
            Contract.Invariant(A != null ? A.Bs.Contains(this) : true);
            Contract.Invariant(Cs != null);
            Contract.Invariant(Contract.ForAll(Cs, c => c.B == this));
        }

        public SemanticObjectB()
        {
            Contract.Ensures(IntProperty == 0);
            Contract.Ensures(StringProperty == null);
            Contract.Ensures(A == null);
            Contract.Ensures(Cs.Count == 0);
        }

        private int m_IntProperty;

        public int IntProperty
        {
            get
            {
                return m_IntProperty;
            }
            set
            {
                if (m_IntProperty != value)
                {
                    m_IntProperty = value;
                    OnPropertyChanged("IntProperty");
                }
            }
        }

        private string m_StringProperty;

        public string StringProperty
        {
            get
            {
                return m_StringProperty;
            }
            set
            {
                if (m_StringProperty != value)
                {
                    m_StringProperty = value;
                    OnPropertyChanged("StringProperty");
                }
            }
        }

        private SemanticObjectA m_A;

        public SemanticObjectA A
        {
            get
            {
                return m_A;
            }
            set
            {
                Contract.Ensures(Contract.OldValue(A) != null && Contract.OldValue(A) != value
                                     ? ! Contract.OldValue(A).Bs.Contains(this) : true);
                Contract.Ensures(A == value);

                if (m_A != null)
                {
                    m_A.RemoveB(this);
                }
                m_A = value;
                if (m_A != null)
                {
                    m_A.AddB(this);
                }
            }
        }

        private readonly List<SemanticObjectC> m_Cs = new List<SemanticObjectC>();

        public IList<SemanticObjectC> Cs
        {
            get
            {
                return new List<SemanticObjectC>(m_Cs);
            }
        }

        internal void AddC(SemanticObjectC c)
        {
            Contract.Requires(c != null);
            Contract.Requires(c.B == this);
            Contract.Ensures(Cs.Contains(c));

            if (!m_Cs.Contains(c))
            {
                m_Cs.Add(c);
            }
        }

        internal void RemoveC(SemanticObjectC c)
        {
            Contract.Ensures(!Cs.Contains(c));

            m_Cs.Remove(c);
        }

        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>().Contains("IntProperty"));
            Contract.Ensures(Contract.Result<string>().Contains("StringProperty"));
            Contract.Ensures(Contract.Result<string>().Contains("A"));
            Contract.Ensures(Contract.Result<string>().Contains("Cs"));

            return base.ToString();
        }
    }
}