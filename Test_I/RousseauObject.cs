using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using PPWCode.Vernacular.Exceptions.I;
using PPWCode.Vernacular.Semantics.I;

namespace PPWCode.Vernacular.Semantics.Test_I
{
    public class RousseauObject : AbstractRousseauObject
    {
        [ContractInvariantMethod]
        private void TypeInvariants()
        {
        }

        public RousseauObject()
        {
        }

        public bool MockWild { get; set; }

        public override CompoundSemanticException WildExceptions()
        {
            CompoundSemanticException cpe = base.WildExceptions();
            if (MockWild)
            {
                cpe.AddElement(new SemanticException("TEST"));
            }
            return cpe;
        }
    }
}
