using System.Diagnostics.Contracts;

namespace PPWCode.Vernacular.Persistence.II
{
    [ContractClass(typeof(IInsertAuditablePropertiesContract))]
    public interface IInsertAuditableProperties
    {
        string CreatedAtPropertyName { get; }
        string CreatedByPropertyName { get; }
    }

    // ReSharper disable once InconsistentNaming
    [ContractClassFor(typeof(IInsertAuditableProperties))]
    public abstract class IInsertAuditablePropertiesContract : IInsertAuditableProperties
    {
        public string CreatedAtPropertyName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return default(string);
            }
        }

        public string CreatedByPropertyName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return default(string);
            }
        }
    }
}