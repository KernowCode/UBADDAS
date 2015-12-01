using System;
using System.Collections.Generic;

namespace KernowCode.KTest.Logging
{
    public class LoggerEntry : ILoggerEntry
    {
        private List<ILoggerEntry> _loggerEntry = new List<ILoggerEntry>();

        public void Add(ILoggerEntry loggerEntry)
        {
            _loggerEntry.Add(loggerEntry);
        }

        public string Render(object entry)
        {
            foreach (var loggerEntry in _loggerEntry)
            {
                if (entry.GetType() == loggerEntry.LogsType)
                    return loggerEntry.Render(entry);
            }
            return "";
        }

        public Type LogsType { get { return typeof (object); } }
    }
}