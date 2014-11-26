using System;
using System.Collections.Generic;

namespace KernowCode.KTest.Logging
{
    public interface ILogger : IDisposable
    {
        void SubSectionStart();
        void SubSectionEnd();
        void StepsStart();
        void StepsStop();
        void WriteLine(string content);
        void WriteLine(string format, string[] arg);
        IEnumerable<string> GetOutputLocation();
        void WriteOutputLocations(IEnumerable<string> locations);
    }
}