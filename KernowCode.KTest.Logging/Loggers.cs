using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KernowCode.KTest.Logging
{
    public class Loggers : ILogger, ILogWithEmphasis, ILogWithImages, ILogWithExpandableSections
    {
        public const string MidAlignSeparator = "<>";
        private readonly IList<ILogger> _loggers;
        private bool _NotDisposed = true;
        private string _cacheContent = "";

        public Loggers(IList<ILogger> loggers)
        {
            _loggers = loggers;
            WriteOutputLocations(GetOutputLocation());
        }

        #region ILogger Members

        public void SubSectionStart()
        {
            WithLoggers(x => x.SubSectionStart());
        }

        public void SubSectionEnd()
        {
            WithLoggers(x => x.SubSectionEnd());
        }

        public void StepsStart()
        {
            WithLoggers(x => x.StepsStart());
        }

        public void StepsStop()
        {
            WithLoggers(x => x.StepsStop());
        }

        public void WriteLine(string content)
        {
            content = PrependCachedContent(content);
            foreach (ILogger logger in _loggers)
            {
                string loggerContent = HandleMidAlign(content, logger);
                logger.WriteLine(loggerContent);
            }
        }

        public void WriteLine(string format, string[] arg)
        {
            WriteLine(string.Format(format, arg));
        }

        public IEnumerable<string> GetOutputLocation()
        {
            var outputLocations = new List<string>();
            WithLoggers(l =>
                {
                    if (l.GetOutputLocation() != null)
                        outputLocations.AddRange(l.GetOutputLocation());
                });
            return outputLocations;
        }

        public void WriteOutputLocations(IEnumerable<string> locations)
        {            
            WithLoggers(l=> l.WriteOutputLocations(locations.Except(l.GetOutputLocation())));
            WithLoggers(l=> l.WriteLine(""));
        }

        public void Dispose()
        {            
            if (_NotDisposed)
            {
                foreach (ILogger logger in _loggers)
                    logger.Dispose();
                _NotDisposed = false;
            }
        }

        #endregion

        #region ILogWithEmphasis Members

        public void SetStartTextsToEmphasise(params string[] startTexts)
        {
            foreach (ILogger logger in _loggers)
                if (logger is ILogWithEmphasis)
                    ((ILogWithEmphasis) logger).SetStartTextsToEmphasise(startTexts);
        }

        #endregion

        #region ILogWithImages Members

        public void SaveImage(Image image, Image image2)
        {
            foreach (ILogger logger in _loggers)
                if (logger is ILogWithImages)
                    ((ILogWithImages) logger).SaveImage(image, image2);
        }

        #endregion

        ~Loggers()
        {
            Dispose();
        }

        public void SetStartTextsToHaveSectionOpen(params string[] texts)
        {
            foreach (ILogger logger in _loggers)
                if (logger is ILogWithExpandableSections)
                    ((ILogWithExpandableSections)logger).SetStartTextsToHaveSectionOpen(texts);
        }

        public void Write(string content)
        {
            _cacheContent += content;
        }

        private void WithLoggers(Action<ILogger> action)
        {
            foreach (ILogger logger in _loggers)
            {
                action(logger);
            }
        }

        private string PrependCachedContent(string content)
        {
            if (!string.IsNullOrWhiteSpace(_cacheContent))
            {
                content = _cacheContent + content;
                _cacheContent = "";
            }
            return content;
        }

        private string HandleMidAlign(string content, ILogger logger)
        {
            if (logger is ILogWithMidAlign) return content;
            return content.Replace(MidAlignSeparator, " ");
        }
    }
}