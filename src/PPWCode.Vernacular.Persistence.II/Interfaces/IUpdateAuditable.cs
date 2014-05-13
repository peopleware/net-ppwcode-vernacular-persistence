using System;

namespace PPWCode.Vernacular.Persistence.II
{
    public interface IUpdateAuditable
    {
        DateTime? LastModifiedAt { get; set; }
        string LastModifiedBy { get; set; }
    }
}