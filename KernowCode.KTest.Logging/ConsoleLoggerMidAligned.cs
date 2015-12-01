using System;
using System.Collections.Generic;

namespace KernowCode.KTest.Logging
{
    public class ConsoleLoggerMidAligned : ILogger, ILogWithMidAlign
    {
        private const int IndentSize = 2;
        private readonly int _leftSectionPadding;
        private readonly int _reportLevel;
        private int _indentLevel = -1;
        private readonly ILoggerEntry _loggerEntry = new ConsoleLoggerEntry();

        public ConsoleLoggerMidAligned(int leftSectionPadding, int reportLevel = int.MaxValue)
        {
            _leftSectionPadding = leftSectionPadding;
            _reportLevel = reportLevel;
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
                Console.WriteLine(HelpCreateLine(content));
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

        private string HelpCreateLine(string content)
        {
            string line = "";
            if (content.Contains(Loggers.MidAlignSeparator))
                line = FormatMidAlign(content, line);
            else
                line = new string(' ', _leftSectionPadding + 1) + content;
            line = AddIndentationTo(line);
            return line;
        }

        private string AddIndentationTo(string line)
        {
            return new string(' ', Math.Max(_indentLevel, 0)*IndentSize) + line;
        }

        private string FormatMidAlign(string content, string line)
        {
            string[] contentParts = content.Split(Loggers.MidAlignSeparator.ToCharArray(),
                                                  StringSplitOptions.None | StringSplitOptions.RemoveEmptyEntries);
            string contentToLeft = contentParts[0];
            string contentToRight = contentParts[1];
            var padding = new string(' ', _leftSectionPadding - contentToLeft.Length);
            line = padding + contentToLeft + " " + contentToRight;
            return line;
        }
    }
}