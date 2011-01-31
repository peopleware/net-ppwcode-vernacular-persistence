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