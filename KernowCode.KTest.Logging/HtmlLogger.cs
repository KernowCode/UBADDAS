using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace KernowCode.KTest.Logging
{
    public class HtmlLogger : ILogger, IDisposable, ILogWithEmphasis, ILogWithImages, ILogWithExpandableSections
    {
        private readonly bool _createdFile;
        private readonly string _filePath;
        private string _emphasisTag = "strong";
        private IEnumerable<string> _emphasiseStartText = new List<string>();
        private IEnumerable<string> _expandStartText = new List<string>(); 
        private string _detailsStart;
        private Image _finalImage;
        private int _imageRelatedLineNumber;

        public HtmlLogger(string filePath)
        {            
            _filePath = filePath;
            string directory = Path.GetDirectoryName(filePath);            
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            CreateStylesheetAt(directory);
            File.Open(_filePath, FileMode.Create).Dispose();
            WriteHtml(
                "<!DOCTYPE html>",
                "<html>",
                "<head>",
                "<link rel='stylesheet' type='text/css' href='ktestoutput.css'>",                
                "</head>",
                "<body>");
            _createdFile = true;
        }

        private void CreateStylesheetAt(string directory) {
            var file = Path.Combine(directory,"ktestoutput.css");
            if (!File.Exists(file))
            {
                File.WriteAllLines(file, new[]{
                    "summary::-webkit-details-marker { display: none }",
                    "details details { margin-left: 1em; }",
                    "details div { margin-left: 1em; }",
                    "img { border:1px solid #021a40; }",
                    "img { max-width: 20%; }",
                    "img:hover { max-width: 80%; }",
                    "table { border-collapse: collapse; }",
                    "td { vertical-align: top; }",
                    ".steps { width: 33% }",
                    ".image { width: 66% }"
                });
            }
        }

        #region ILogger Members

        public void SubSectionStart()
        {                        
            _detailsStart = "<details>";
        }

        public void SubSectionEnd()
        {
            WriteHtml("</details>");
        }

        public void StepsStart()
        {
            var stepsStart = "<table><tr><td class='steps'>";
            _imageRelatedLineNumber = 0;
            if (string.IsNullOrWhiteSpace(_detailsStart))
                WriteHtml(stepsStart);
            else
                _detailsStart = stepsStart + _detailsStart;
        }

        public void StepsStop()
        {
            if (_imageRelatedLineNumber > 0)
            {
                SaveImage(_finalImage, null);
                _imageRelatedLineNumber = 0;
            }
            WriteHtml("</td></tr></table>");
        }

        public void WriteLine(string content)
        {
            OutputCorrectDetailsTagFor(content);
            content = Emphasise(content);
            _imageRelatedLineNumber++;
            content = AddBulletPointIfNeeded(content);
            WriteContent(content);
        }

        private string AddBulletPointIfNeeded(string content)
        {
            if (content.Length > 2 && content[1] == ' ')
            {
                content = "&bull;" + content.Substring(1);
            }
            return content;
        }

        private void OutputCorrectDetailsTagFor(string content)
        {
            var open = "";
            if (!string.IsNullOrWhiteSpace(_detailsStart))
            {
                if (_expandStartText.Any(x => content.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                    open = "open";
                WriteHtml(string.Format("<details {0}>", open));
            }
        }

        public void WriteLine(string format, string[] arg)
        {
            WriteLine(string.Format(format, arg));
        }

        public IEnumerable<string> GetOutputLocation()
        {
            return new[] {_filePath};
        }

        public void WriteOutputLocations(IEnumerable<string> locations)
        {
            foreach (var location in locations)
                WriteLine(string.Format("<small>Output: {0}</small>",location));
            WriteLine("");
        }

        public void Dispose()
        {
            if (_createdFile)
            {
                WriteHtml(
                    "<script>if (!(navigator.userAgent.toLowerCase().indexOf('chrome') > -1)) document.body.innerHTML += '<div><small>(Please view this document in Chrome)</small></div>';</script>");
                WriteHtml("</body>", "</html>");
            }
        }

        #endregion

        #region ILogWithEmphasis Members

        public void SetStartTextsToEmphasise(params string[] startText)
        {
            _emphasiseStartText = startText;
        }

        #endregion

        #region ILogWithImages Members

        public void SaveImage(Image image, Image image2)
        {
            _finalImage = image2;
            if (image != null)
            {
                string path = Path.Combine(Path.GetDirectoryName(_filePath), "Images");
                string name = Path.GetRandomFileName() + ".png";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                path = Path.Combine(path, name);
                image.Save(path, ImageFormat.Png);
                WriteHtml(string.Format("</td><td class='image'><div><img src='images/{0}'></div></td></tr><tr><td>", name));
            }
            _imageRelatedLineNumber = 0;
        }

        #endregion

        private void WriteHtml(params string[] lines)
        {
            File.AppendAllLines(_filePath, lines);
        }

        private void WriteContent(params string[] lines)
        {            
            string tag = GetTagName();
            lines = lines.Select(x => string.Format("<{0}>{1}</{0}>", tag, x)).ToArray();
            WriteHtml(lines);
        }

        private string GetTagName()
        {
            if (!string.IsNullOrWhiteSpace(_detailsStart))
            {
                _detailsStart = null;
                return "summary";
            }
            return "div";
        }

        private string Emphasise(string text)
        {
            string startText =
                _emphasiseStartText.FirstOrDefault(x => text.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
            if (startText != null)
                return string.Format("<{0}>{1}</{0}>{2}", _emphasisTag, startText, text.Substring(startText.Length));
            return text;
        }

        public void SetStartTextsToHaveSectionOpen(params string[] texts)
        {
            _expandStartText = texts;
        }
    }
}