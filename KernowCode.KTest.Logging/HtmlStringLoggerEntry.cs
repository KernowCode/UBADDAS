using System;

namespace KernowCode.KTest.Logging
{
    public class HtmlStringLoggerEntry : ILoggerEntry
    {
        public string Render(object entry)
        {
            return entry.ToString();            
        }

        public Type LogsType { get { return typeof(string); } }
    }
}