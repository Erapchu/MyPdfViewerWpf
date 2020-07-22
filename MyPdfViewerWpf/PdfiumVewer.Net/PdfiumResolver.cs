using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPdfViewerWpf.PdfiumVewer.Net
{
    public class PdfiumResolver
    {
        public static event PdfiumResolveEventHandler Resolve;

        private static void OnResolve(PdfiumResolveEventArgs e)
        {
            Resolve?.Invoke(null, e);
        }

        internal static string GetPdfiumFileName()
        {
            var e = new PdfiumResolveEventArgs();
            OnResolve(e);
            return e.PdfiumFileName;
        }
    }
}
