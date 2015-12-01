using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace KernowCode.KTest.Logging
{
    public class HtmlLogger : ILogger, ILogWithEmphasis, ILogWithImages, ILogWithExpandableSections
    {
        private readonly bool _createdFile;
        private readonly string _filePath;
        private string _emphasisTag = "strong";
        private string _emphasisClass = "class='badge badge-success'";
        private IEnumerable<string> _emphasiseStartText = new List<string>();
        private IEnumerable<string> _expandStartText = new List<string>();
        private string _detailsStart;
        private Image _finalImage;
        private int _imageRelatedLineNumber;
        private readonly ILoggerEntry _loggerEntry;

        public HtmlLogger(ILoggerEntry loggerEntry, string filePath)
        {
            _filePath = filePath;
            _loggerEntry = loggerEntry;
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            CreateStylesheetAt(directory);
            File.Open(_filePath, FileMode.Create).Dispose();
            WriteHtml(
                "<!DOCTYPE html>",
                "<html>",
                "<head>",
                "<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.min.css'>",
                "<script src='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/js/bootstrap.min.js'></script>",
                "<link rel='stylesheet' type='text/css' href='ktestoutput.css'>",
                "</head>",
                "<body>",
                "<script>if (!(navigator.userAgent.toLowerCase().indexOf('chrome') > -1)) document.body.innerHTML += '<div><small>(Please view this document in Chrome)</small></div>';</script>",
                "<script>",
                "  document.addEventListener('DOMContentLoaded', function() {",                
                "    var notifyParentOfError = function(parent, classNames) {",
                "      if (parent) {",
                "        if (parent.tagName === 'DETAILS') {	    ",
                "          var badge = parent.getElementsByClassName('badge')[0];",
                "          badge.className = classNames;",
                "        }",
                "        notifyParentOfError(parent.parentElement, classNames);",
                "      }",
                "    }",
                "    var slows = document.getElementsByClassName('slow-pass');    ",
                "    for (var i = 0; i < slows.length; i++) {",
                "      notifyParentOfError(slows[i].parentElement, 'badge badge-warning');",
                "    }",
                "    var errors = document.getElementsByClassName('fail');    ",
                "    for (var i = 0; i < errors.length; i++) {",
                "      notifyParentOfError(errors[i].parentElement, 'badge badge-danger');",
                "    }",
                "  }, false);",
                "</script>"
                );
            _createdFile = true;
        }

        private void CreateStylesheetAt(string directory)
        {
            var file = Path.Combine(directory, "ktestoutput.css");
            if (!File.Exists(file))
            {
                File.WriteAllLines(file,
                                   new[]
                                       {
                                           ".badge-danger { background-color: #d43f3a; }",
                                           ".badge-warning { background-color: #d58512; }",
                                           ".badge-success { background-color: #398439; }",
                                           ".badge-info { background-color: #269abc; }",
                                           ".badge-inverse { background-color: #333333; }",
                                           "* { padding-top: 1px; padding-right: 1px; padding-bottom: 1px; padding-left: 1px; }",
                                           "* { font-family: Arial, Helvetica, sans-serif; }",
                                           "summary::-webkit-details-marker { display: none }",
                                           "details details { margin-left: 1em; }",
                                           "details div { margin-left: 1em; }",
                                           "table { border-collapse: collapse; }",
                                           "td { vertical-align: top; }",
                                           ".steps { width: 35% }",
                                           ".image { width: 65% }",
                                           "img { border:1px solid #021a40; }",
                                           ".fail { color: red; }",
                                           ".fail-hint { display: block; }",
                                           ".fail-exception { display: block; }",
                                           ".fail-hint::before { content: 'AN ERROR OCCURED -'; display:block; }",
                                           ".fail-exception::before { content: 'EXCEPTION -'; display:block; }",
                                           @"span.pass:before {content:'\2713';display:inline-block;color:green;padding:0 5px 0 0;}",
                                           @"span.fail:before {content:'\2717';display:inline-block;color:red;padding:0 5px 0 0;}",                                           
                                           "@media screen {",
                                           " span.slow-pass { background-color: DarkOrange; }",
                                           " span.slow-pass:after {content:'*';display:inline-block;color:DarkOrange;}",
                                           " img {-webkit-transition: width 0.25s; transition: width 0.25s; }",
                                           " img { width: 20%; }",
                                           " tr:hover img { width: 80%; }",
                                           " .a-value { display: none }",
                                           " div:hover .a-value { display: initial; }",
                                           "}",
                                           "@media print {",
                                           " * { font-size: xx-small; } tr { page-break-inside:avoid; }",
                                           "} "                                           
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
            var stepsStart = "<table class='table'><tr><td class='steps'>";
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

        public void WriteObject(object content)
        {
            WriteLine(_loggerEntry.Render(content));
        }

        public void WriteLine(string content)
        {
            OutputCorrectDetailsTagFor(content);
            content = Emphasise(content);
            _imageRelatedLineNumber++;            
            WriteContent(content);
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
                WriteLine(string.Format("<small>Output: {0}</small>", location));
            WriteLine("");
        }

        public void Dispose()
        {
            if (_createdFile)
            {
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
            _finalImage = image2 ?? image;
            var currentImage = image;
            if (currentImage != null)
            {
                string path = Path.Combine(Path.GetDirectoryName(_filePath), "Images");
                string name = Path.GetRandomFileName() + ".png";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                path = Path.Combine(path, name);
                currentImage.Save(path, ImageFormat.Png);
                WriteHtml(string.Format("</td><td class='image'><div><img src='images/{0}'/></div></td></tr><tr><td>",
                                        name));
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
                return string.Format("<{0} {1}>{2}</{0}>{3}", _emphasisTag, _emphasisClass, startText, text.Substring(startText.Length));
            return text;
        }

        public void SetStartTextsToHaveSectionOpen(params string[] texts)
        {
            _expandStartText = texts;
        }

    }
}