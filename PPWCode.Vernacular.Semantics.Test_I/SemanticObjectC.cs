using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using PPWCode.Vernacular.Semantics.I;

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
        public SemanticObjectA A;
    }
}
