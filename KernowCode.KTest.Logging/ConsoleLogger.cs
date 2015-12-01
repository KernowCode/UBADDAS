using System;
using System.Collections.Generic;

namespace KernowCode.KTest.Logging
{
    public class ConsoleLogger : ILogger
    {
        private const int IndentSize = 2;
        private readonly int _reportLevel;
        private readonly ILoggerEntry _loggerEntry;
        private int _indentLevel = -1;

        public ConsoleLogger(ILoggerEntry consoleLoggerEnrtry, int reportLevel = int.MaxValue)
        {
            _reportLevel = reportLevel;
            _loggerEntry = consoleLoggerEnrtry;
        }

        #region ILogger Members

        public void SubSectionStart()
        {
            StepsStart();
        }

        public void SubSectionEnd()
        {
            StepsStop();
        }

        public void StepsStart()
        {
            _indentLevel++;
        }

        public void StepsStop()
        {
            _indentLevel--;
        }

        public void WriteObject(object content)
        {
            WriteLine(_loggerEntry.Render(content));
        }

        public void WriteLine(string content)
        {
            if (_reportLevel >= _indentLevel)
                Console.WriteLine(new string(' ', Math.Max(_indentLevel, 0)*IndentSize) + content);
        }

        public void WriteLine(string format, string[] arg)
        {
            Console.WriteLine(format, arg);
        }

        public IEnumerable<string> GetOutputLocation()
        {
            return new string[0];
        }

        public void WriteOutputLocations(IEnumerable<string> locations)
        {
            foreach (var location in locations)
                WriteLine("(also logged at: " + location + ")");
        }

        public void Dispose()
        {
        }

        #endregion
    }
}