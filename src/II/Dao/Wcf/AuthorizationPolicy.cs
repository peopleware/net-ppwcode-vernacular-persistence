#region Using

using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;

using PPWCode.Vernacular.Exceptions.I;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf
{
    public class AuthorizationPolicy :
        IAuthorizationPolicy
    {
        private Guid m_ID = Guid.NewGuid();

        // this method gets called after the authentication stage
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            // get the authenticated client identity
            IIdentity client = GetClientIdentity(evaluationContext);

            // set the custom principal
            evaluationContext.Properties["Principal"] = new CustomPrincipal(client);

            return true;
        }

        private static IIdentity GetClientIdentity(EvaluationContext evaluationContext)
        {
            object obj;
            if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
            {
                throw new SecurityException(@"No Identity found");
            }

            IList<IIdentity> identities = obj as IList<IIdentity>;
            if (identities == null || identities.Count <= 0)
            {
                throw new SecurityException(@"No Identity found");
            }

            return identities[0];
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        public string Id
        {
            get { return m_ID.ToString(); }
        }
    }
}