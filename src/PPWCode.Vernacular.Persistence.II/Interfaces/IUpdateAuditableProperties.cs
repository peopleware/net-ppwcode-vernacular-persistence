using System.Diagnostics.Contracts;

namespace PPWCode.Vernacular.Persistence.II
{
    [ContractClass(typeof(IUpdateAuditablePropertiesContract))]
    public interface IUpdateAuditableProperties
    {
        string LastModifiedAtPropertyName { get; }
        string LastModifiedByPropertyName { get; }
    }

    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof(IUpdateAuditableProperties))]
    public abstract class IUpdateAuditablePropertiesContract : IUpdateAuditableProperties
    {
        public string LastModifiedAtPropertyName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return default(string);
            }
        }

        public string LastModifiedByPropertyName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return default(string);
            }
        }
    }
}