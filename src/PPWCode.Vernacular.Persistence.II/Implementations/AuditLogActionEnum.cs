using System;

namespace PPWCode.Vernacular.Persistence.II
{
    [Flags]
    public enum AuditLogActionEnum
    {
        NONE = 0,
        CREATE = 1,
        UPDATE = 2,
        DELETE = 4,
        ALL = CREATE | UPDATE | DELETE,
    }
}