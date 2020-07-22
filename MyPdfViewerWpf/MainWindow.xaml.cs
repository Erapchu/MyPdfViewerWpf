using MyPdfViewerWpf.PdfiumVewer.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyPdfViewerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private PdfSearchManager SearchManager { get; }

        public string InfoText { get; set; }
        public string SearchTerm { get; set; }

        public double ZoomPercent
        {
            get => Renderer.Zoom * 100;
            set => Renderer.SetZoom(value / 100);
        }
        public bool IsSearchOpen { get; set; }
        public int SearchMatchItemNo { get; set; }
        public int SearchMatchesCount { get; set; }
        public int Page
        {
            get => Renderer.PageNo + 1;
            set => Renderer.PageNo = Math.Min(Math.Max(value - 1, 0), Renderer.PageCount - 1);
        }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            Renderer.PropertyChanged += delegate
            {
                OnPropertyChanged(nameof(Page));
                OnPropertyChanged(nameof(ZoomPercent));
            };

            SearchManager = new PdfSearchManager(Renderer);
        }

        private void OpenPdf(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                Title = "Open PDF File"
            };

            if (dialog.ShowDialog() == true)
            {
                var bytes = File.ReadAllBytes(dialog.FileName);
                var mem = new MemoryStream(bytes);
                Renderer.Document?.Dispose();
                Renderer.OpenPdf(mem);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Renderer?.Dispose();
        }

        private void OnPrevPageClick(object sender, RoutedEventArgs e)
        {
            Renderer.PreviousPage();
        }
        private void OnNextPageClick(object sender, RoutedEventArgs e)
        {
            Renderer.NextPage();
        }

        private void OnFitWidth(object sender, RoutedEventArgs e)
        {
            Renderer.SetZoomMode(PdfViewerZoomMode.FitWidth);
        }
        private void OnFitHeight(object sender, RoutedEventArgs e)
        {
            Renderer.SetZoomMode(PdfViewerZoomMode.FitHeight);
        }

        private void OnZoomInClick(object sender, RoutedEventArgs e)
        {
            Renderer.ZoomIn();
        }

        private void OnZoomOutClick(object sender, RoutedEventArgs e)
        {
            Renderer.ZoomOut();
        }

        private void OnRotateLeftClick(object sender, RoutedEventArgs e)
        {
            Renderer.Counterclockwise();
        }

        private void OnRotateRightClick(object sender, RoutedEventArgs e)
        {
            Renderer.ClockwiseRotate();
        }

        private void OnContinuousModeClick(object sender, RoutedEventArgs e)
        {
            Renderer.PagesDisplayMode = PdfViewerPagesDisplayMode.ContinuousMode;
        }

        private void OnBookModeClick(object sender, RoutedEventArgs e)
        {
            Renderer.PagesDisplayMode = PdfViewerPagesDisplayMode.BookMode;
        }

        private void OnSinglePageModeClick(object sender, RoutedEventArgs e)
        {
            Renderer.PagesDisplayMode = PdfViewerPagesDisplayMode.SinglePageMode;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void OnTransparent(object sender, RoutedEventArgs e)
        {
            if ((Renderer.Flags & PdfRenderFlags.Transparent) != 0)
            {
                Renderer.Flags &= ~PdfRenderFlags.Transparent;
            }
            else
            {
                Renderer.Flags |= PdfRenderFlags.Transparent;
            }
        }

        private void OpenCloseSearch(object sender, RoutedEventArgs e)
        {
            IsSearchOpen = !IsSearchOpen;
            OnPropertyChanged(nameof(IsSearchOpen));
        }

        private void OnSearchTermKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }

        private void DoSearch(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            SearchMatchItemNo = 0;
            //SearchManager.MatchCase = MatchCaseCheckBox.IsChecked.GetValueOrDefault();
            //SearchManager.MatchWholeWord = WholeWordOnlyCheckBox.IsChecked.GetValueOrDefault();
            //SearchManager.HighlightAllMatches = HighlightAllMatchesCheckBox.IsChecked.GetValueOrDefault();
            //SearchMatchesTextBlock.Visibility = Visibility.Visible;

            if (!SearchManager.Search(SearchTerm))
            {
                MessageBox.Show(this, "No matches found.");
            }
            else
            {
                SearchMatchesCount = SearchManager.MatchesCount;
                // DisplayTextSpan(SearchMatches.Items[SearchMatchItemNo++].TextSpan);
            }

            if (!SearchManager.FindNext(true))
                MessageBox.Show(this, "Find reached the starting point of the search.");
        }

        private void DisplayTextSpan(PdfTextSpan span)
        {
            Page = span.Page + 1;
            Renderer.ScrollToVerticalOffset(span.Offset);
        }

        private void OnNextFoundClick(object sender, RoutedEventArgs e)
        {
            if (SearchMatchesCount > SearchMatchItemNo)
            {
                SearchMatchItemNo++;
                //DisplayTextSpan(SearchMatches.Items[SearchMatchItemNo - 1].TextSpan);
                SearchManager.FindNext(true);
            }
        }

        private void OnPrevFoundClick(object sender, RoutedEventArgs e)
        {
            if (SearchMatchItemNo > 1)
            {
                SearchMatchItemNo--;
                // DisplayTextSpan(SearchMatches.Items[SearchMatchItemNo - 1].TextSpan);
                SearchManager.FindNext(false);
            }
        }

        private void ToRtlClick(object sender, RoutedEventArgs e)
        {
            Renderer.IsRightToLeft = true;
        }

        private void ToLtrClick(object sender, RoutedEventArgs e)
        {
            Renderer.IsRightToLeft = false;
        }
    }
}
