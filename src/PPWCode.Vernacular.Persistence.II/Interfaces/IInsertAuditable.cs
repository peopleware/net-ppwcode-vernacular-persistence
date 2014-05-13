using System;

namespace PPWCode.Vernacular.Persistence.II
{
    public interface IInsertAuditable
    {
        DateTime? CreatedAt { get; set; }
        string CreatedBy { get; set; }
    }
}