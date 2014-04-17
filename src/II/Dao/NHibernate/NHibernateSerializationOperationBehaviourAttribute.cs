#region Using

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NHibernateSerializationOperationBehaviourAttribute
        : Attribute,
          IOperationBehavior
    {
        #region Private Helper

        private static void ApplyDataContractSurrogate(OperationDescription operationDescription)
        {
            DataContractSerializerOperationBehavior dataContractBehavior = operationDescription
                .Behaviors
                .Find<DataContractSerializerOperationBehavior>();
            if (dataContractBehavior != null)
            {
                dataContractBehavior.DataContractSurrogate = new NHibernateDataContractSurrogate();
            }
        }

        #endregion

        #region Implementation of IOperationBehavior

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            ApplyDataContractSurrogate(operationDescription);
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            ApplyDataContractSurrogate(operationDescription);
        }

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        #endregion
    }
}