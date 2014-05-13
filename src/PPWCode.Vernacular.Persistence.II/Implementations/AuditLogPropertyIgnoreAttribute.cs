using System;
using System.Runtime.InteropServices;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AuditLogPropertyIgnoreAttribute : Attribute
    {
        public AuditLogPropertyIgnoreAttribute()
        {
            AuditLogAction = AuditLogActionEnum.ALL;
        }

        public AuditLogActionEnum AuditLogAction { get; set; }
    }
}