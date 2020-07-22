using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPdfViewerWpf.PdfiumVewer.Net
{
    [Flags]
    public enum PdfViewerPagesDisplayMode
    {
        SinglePageMode = 1,
        BookMode = 2,
        ContinuousMode = 4
    }
}
