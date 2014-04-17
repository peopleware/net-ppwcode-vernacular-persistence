#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

#endregion

namespace PPWCode.Vernacular.Persistence.II.Dao.NHibernate
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class NHibernateSerializationContractBehaviorAttribute :
        Attribute,
        IContractBehavior
    {
        #region Private Helpers

        private static void ApplyDataContractSurrogate(ContractDescription contractDescription)
        {
            IEnumerable<DataContractSerializerOperationBehavior> dataContractSerializerOperationBehaviors = contractDescription
                .Operations
                .Select(operation => operation.Behaviors.Find<DataContractSerializerOperationBehavior>())
                .Where(dataContractBehavior => dataContractBehavior != null);
            foreach (DataContractSerializerOperationBehavior dataContractBehavior in dataContractSerializerOperationBehaviors)
            {
                dataContractBehavior.DataContractSurrogate = new NHibernateDataContractSurrogate();
            }
        }

        #endregion

        #region Implementation of IContractBehavior

        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            ApplyDataContractSurrogate(contractDescription);
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            ApplyDataContractSurrogate(contractDescription);
        }

        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        #endregion
    }
}