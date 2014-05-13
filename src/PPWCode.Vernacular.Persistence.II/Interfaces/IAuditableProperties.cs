namespace PPWCode.Vernacular.Persistence.II
{
    public interface IAuditableProperties
        : IInsertAuditableProperties,
          IUpdateAuditableProperties
    {
    }
}