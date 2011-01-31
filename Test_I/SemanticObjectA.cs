using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using PPWCode.Vernacular.Semantics.I;

namespace PPWCode.Vernacular.Semantics.Test_I
{
    public class SemanticObjectA : AbstractSemanticObject
    {
        [ContractInvariantMethod]
        private void TypeInvariants()
        {
            Contract.Invariant(Bs != null);
            Contract.Invariant(Contract.ForAll<SemanticObjectB>(Bs, b => b.A == this));
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

        // this could create a loop, as it is downstream, but not a collection
        ///// <summary>
        ///// Closing the loop.
        ///// </summary>
        //public SemanticObjectC C { get; set; }
    }
}
