using System;

namespace PPWCode.Vernacular.Persistence.II
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}