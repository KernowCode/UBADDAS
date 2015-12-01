using System;

namespace KernowCode.KTest.Logging
{
    public interface ILoggerEntry
    {
        string Render(object entry);
        Type LogsType { get; }
    }
}