using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPdfViewerWpf.PdfiumVewer.Net
{
    public class PdfMatch
    {
        public string Text { get; }
        public PdfTextSpan TextSpan { get; }
        public int Page { get; }

        public PdfMatch(string text, PdfTextSpan textSpan, int page)
        {
            Text = text;
            TextSpan = textSpan;
            Page = page;
        }
    }
}
