using System;
using System.Runtime.InteropServices;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AuditLogAttribute : Attribute
    {
        public AuditLogAttribute()
        {
            AuditLogAction = AuditLogActionEnum.NONE;
        }

        public AuditLogActionEnum AuditLogAction { get; set; }
    }
}