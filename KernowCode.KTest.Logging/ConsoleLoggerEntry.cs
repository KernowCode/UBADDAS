using System;

namespace KernowCode.KTest.Logging
{
    internal class ConsoleLoggerEntry : ILoggerEntry
    {
        public string Render(object entry)
        {
            return entry.ToString();
        }

        public Type LogsType { get { return typeof (string); } }
    }
}