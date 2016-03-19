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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;

using log4net;

using NHibernate.Hql.Ast.ANTLR;

using PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.GenericInterceptor;

namespace PPWCode.Vernacular.Persistence.I.Dao.Wcf.Helpers.Errors
{
    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags", Justification = ".Net code")]
    public class AuditLogOperationInterceptor
        : GenericInvoker
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(AuditLogOperationInterceptor));
        private readonly AuditLevel m_AuditLevel;

        public AuditLogOperationInterceptor(IOperationInvoker previousInvoker, AuditLevel auditLevel)
            : base(previousInvoker)
        {
            m_AuditLevel = auditLevel;

            if (s_Logger.IsDebugEnabled)
            {
                s_Logger.DebugFormat(
                    "Creating new instance of type '{0}' with (previousInvoker: '{1}', auditLevel: '{2}')",
                    typeof(AuditLogOperationInterceptor).Name,
                    previousInvoker.GetType().Name,
                    auditLevel);
            }
        }

        /// <inheritdoc />
        /// <remarks>Exceptions here will abort the call.</remarks>
        protected override void PreInvoke(object instance, object[] inputs)
        {
        }

        /// <inheritdoc />
        /// <remarks>Always called, even if operation had an exception.</remarks>
        protected override void PostInvoke(object instance, object returnedValue, object[] outputs, Exception exception)
        {
            switch (m_AuditLevel)
            {
                case AuditLevel.None:
                    break;

                case AuditLevel.Success:
                {
                    if (exception == null)
                    {
                        LogAdditionalInfo(null, false);
                    }

                    break;
                }

                case AuditLevel.Failure:
                {
                    if (exception != null)
                    {
                        LogAdditionalInfo(exception, !(exception is SemanticException));
                    }

                    break;
                }

                case AuditLevel.SuccessOrFailure:
                {
                    LogAdditionalInfo(exception, exception != null && !(exception is SemanticException));
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void LogAdditionalInfo(Exception exception, bool logAsError)
        {
            OperationContext operationContext = OperationContext.Current;
            if (operationContext != null)
            {
                AuthorizationContext authorizationContext = operationContext.ServiceSecurityContext.AuthorizationContext;

                string service = operationContext.Channel.LocalAddress.Uri.ToString();
                string actionName = operationContext.IncomingMessageHeaders != null ? operationContext.IncomingMessageHeaders.Action : string.Empty;
                string identity = GetIdentityNamesFromContext(authorizationContext);
                StringBuilder sb = new StringBuilder();
                sb
                    .AppendLine()
                    .AppendFormat("Service: {0}", service)
                    .AppendLine()
                    .AppendFormat("Action: {0}", actionName)
                    .AppendLine()
                    .AppendFormat("ClientIdentity: {0}", identity)
                    .AppendLine()
                    .AppendFormat("AuthorizationContext: {0}", authorizationContext.Id)
                    .AppendLine();

                string message = sb.ToString();

                if (logAsError)
                {
                    if (exception != null)
                    {
                        s_Logger.Error(message, exception);
                    }
                    else
                    {
                        s_Logger.Error(message);
                    }
                }
                else
                {
                    if (exception != null)
                    {
                        s_Logger.Info(message, exception);
                    }
                    else
                    {
                        s_Logger.Info(message);
                    }
                }
            }
        }

        private static string GetIdentityNamesFromContext(AuthorizationContext authContext)
        {
            if (authContext == null)
            {
                return string.Empty;
            }

            StringBuilder str = new StringBuilder(256);
            foreach (ClaimSet claimSet in authContext.ClaimSets)
            {
                // Windows
                WindowsClaimSet windows = claimSet as WindowsClaimSet;
                if (windows != null)
                {
                    if (str.Length > 0)
                    {
                        str.Append(", ");
                    }

                    AppendIdentityName(str, windows.WindowsIdentity);
                }
                else
                {
                    // X509
                    X509CertificateClaimSet x509 = claimSet as X509CertificateClaimSet;
                    if (x509 != null)
                    {
                        if (str.Length > 0)
                        {
                            str.Append(", ");
                        }

                        AppendCertificateIdentityName(str, x509.X509Certificate);
                    }
                }
            }

            if (str.Length <= 0)
            {
                List<IIdentity> identities = null;
                object obj;
                if (authContext.Properties.TryGetValue("Identities", out obj))
                {
                    identities = obj as List<IIdentity>;
                }

                if (identities != null)
                {
                    foreach (IIdentity identity in identities.Where(identity => identity != null))
                    {
                        if (str.Length > 0)
                        {
                            str.Append(", ");
                        }

                        AppendIdentityName(str, identity);
                    }
                }
            }

            return str.Length <= 0 ? string.Empty : str.ToString();
        }

        private static void AppendIdentityName(StringBuilder str, IIdentity identity)
        {
            string name = identity.Name;

            str.Append(string.IsNullOrEmpty(name) ? "<null>" : name);

            WindowsIdentity windows = identity as WindowsIdentity;
            if (windows != null && windows.User != null)
            {
                str.Append("; ");
                str.Append(windows.User);
            }
        }

        private static void AppendCertificateIdentityName(StringBuilder str, X509Certificate2 certificate)
        {
            string value = certificate.SubjectName.Name;
            if (string.IsNullOrEmpty(value))
            {
                value = certificate.GetNameInfo(X509NameType.DnsName, false);
                if (string.IsNullOrEmpty(value))
                {
                    value = certificate.GetNameInfo(X509NameType.SimpleName, false);
                    if (string.IsNullOrEmpty(value))
                    {
                        value = certificate.GetNameInfo(X509NameType.EmailName, false);
                        if (string.IsNullOrEmpty(value))
                        {
                            value = certificate.GetNameInfo(X509NameType.UpnName, false);
                        }
                    }
                }
            }

            // Same format as X509Identity
            str.Append(string.IsNullOrEmpty(value) ? "<x509>" : value);
            str.Append("; ");
            str.Append(certificate.Thumbprint);
        }
    }
}