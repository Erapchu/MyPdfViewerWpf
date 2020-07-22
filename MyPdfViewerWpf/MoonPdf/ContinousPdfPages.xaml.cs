using MyPdfViewerWpf;
using MyPdfViewerWpf.Virtualizing;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for ContinousPdfPages.xaml
    /// </summary>
    public partial class ContinousPdfPages : UserControl
    {
		private ScrollViewer scrollViewer;
		private CustomVirtualizingPanel virtualPanel;
		private PdfImageProvider imageProvider;
		private VirtualizingCollection<IEnumerable<PdfImage>> virtualizingPdfPages;

		public ContinousPdfPages()
		{
			InitializeComponent();

			//this.SizeChanged += ContinuousMoonPdfPanel_SizeChanged;
		}

		/*void ContinuousMoonPdfPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(this);
		}

		public void Load(IPdfSource source, string password = null)
		{
			this.virtualPanel = VisualTreeHelperEx.FindChild<CustomVirtualizingPanel>(this);
			this.scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(this);
			this.virtualPanel.PageRowBounds = this.parent.PageRowBounds.Select(f => f.SizeIncludingOffset).ToArray();
			this.imageProvider = new PdfImageProvider(source, this.parent.TotalPages,
										new PageDisplaySettings(this.parent.GetPagesPerRow(), this.parent.ViewType, this.parent.HorizontalMargin, this.parent.Rotation),
										password: password);

			if (this.parent.ZoomType == ZoomType.Fixed)
				this.CreateNewItemsSource();
			else if (this.parent.ZoomType == ZoomType.FitToHeight)
				this.ZoomToHeight();
			else if (this.parent.ZoomType == ZoomType.FitToWidth)
				this.ZoomToWidth();

			if (this.scrollViewer != null)
			{
				this.scrollViewer.Visibility = System.Windows.Visibility.Visible;
				this.scrollViewer.ScrollToTop();
			}
		}

		public void Unload()
		{
			this.scrollViewer.Visibility = System.Windows.Visibility.Collapsed;
			this.scrollViewer.ScrollToHorizontalOffset(0);
			this.scrollViewer.ScrollToVerticalOffset(0);
			this.imageProvider = null;

			if (this.virtualizingPdfPages != null)
			{
				this.virtualizingPdfPages.CleanUpAllPages();
				this.virtualizingPdfPages = null;
			}

			this.itemsControl.ItemsSource = null;
		}

		private void CreateNewItemsSource()
		{
			var pageTimeout = TimeSpan.FromSeconds(2);

			if (this.virtualizingPdfPages != null)
				this.virtualizingPdfPages.CleanUpAllPages();

			this.virtualizingPdfPages = new AsyncVirtualizingCollection<IEnumerable<PdfImage>>(this.imageProvider, this.parent.GetPagesPerRow(), pageTimeout);
			this.itemsControl.ItemsSource = this.virtualizingPdfPages;
		}

		#region Zoom specific code
		public float CurrentZoom
		{
			get
			{
				if (this.imageProvider != null)
					return this.imageProvider.Settings.ZoomFactor;

				return 1.0f;
			}
		}

		public void ZoomToWidth()
		{
			var scrollBarWidth = this.scrollViewer.ComputedVerticalScrollBarVisibility == System.Windows.Visibility.Visible ? SystemParameters.VerticalScrollBarWidth : 0;
			scrollBarWidth += 2; // Magic number, sorry :)

			ZoomInternal((this.ActualWidth - scrollBarWidth) / this.parent.PageRowBounds.Max(f => f.SizeIncludingOffset.Width));
		}

		public void ZoomToHeight()
		{
			var scrollBarHeight = this.scrollViewer.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible ? SystemParameters.HorizontalScrollBarHeight : 0;

			ZoomInternal((this.ActualHeight - scrollBarHeight) / this.parent.PageRowBounds.Max(f => f.SizeIncludingOffset.Height));
		}

		public void ZoomIn()
		{
			ZoomInternal(this.CurrentZoom + this.parent.ZoomStep);
		}

		public void ZoomOut()
		{
			ZoomInternal(this.CurrentZoom - this.parent.ZoomStep);
		}

		public void Zoom(double zoomFactor)
		{
			this.ZoomInternal(zoomFactor);
		}

		private void ZoomInternal(double zoomFactor)
		{
			if (zoomFactor > this.parent.MaxZoomFactor)
				zoomFactor = this.parent.MaxZoomFactor;
			else if (zoomFactor < this.parent.MinZoomFactor)
				zoomFactor = this.parent.MinZoomFactor;

			var yOffset = this.scrollViewer.VerticalOffset;
			var xOffset = this.scrollViewer.HorizontalOffset;
			var zoom = this.CurrentZoom;

			if (Math.Abs(Math.Round(zoom, 2) - Math.Round(zoomFactor, 2)) == 0.0)
				return;

			this.virtualPanel.PageRowBounds = this.parent.PageRowBounds.Select(f => new Size(f.Size.Width * zoomFactor + f.HorizontalOffset, f.Size.Height * zoomFactor + f.VerticalOffset)).ToArray();
			this.imageProvider.Settings.ZoomFactor = (float)zoomFactor;

			this.CreateNewItemsSource();

			this.scrollViewer.ScrollToHorizontalOffset((xOffset / zoom) * zoomFactor);
			this.scrollViewer.ScrollToVerticalOffset((yOffset / zoom) * zoomFactor);
		}
		#endregion

		public void GotoPreviousPage()
		{
			if (this.scrollViewer == null)
				return;

			var currentPageIndex = GetCurrentPageIndex(this.parent.ViewType);

			if (currentPageIndex == 0)
				return;

			var startIndex = PageHelper.GetVisibleIndexFromPageIndex(currentPageIndex - 1, this.parent.ViewType);
			var verticalOffset = this.virtualPanel.GetVerticalOffsetByItemIndex(startIndex);
			this.scrollViewer.ScrollToVerticalOffset(verticalOffset);
		}

		public void GotoNextPage()
		{
			var nextIndex = PageHelper.GetNextPageIndex(GetCurrentPageIndex(this.parent.ViewType), this.parent.TotalPages, this.parent.ViewType);

			if (nextIndex == -1)
				return;

			GotoPage(nextIndex + 1);
		}

		public void GotoPage(int pageNumber)
		{
			if (this.scrollViewer == null)
				return;

			var startIndex = PageHelper.GetVisibleIndexFromPageIndex(pageNumber - 1, this.parent.ViewType);
			var verticalOffset = this.virtualPanel.GetVerticalOffsetByItemIndex(startIndex);
			this.scrollViewer.ScrollToVerticalOffset(verticalOffset);
		}

		public int GetCurrentPageIndex(ViewType viewType)
		{
			if (this.scrollViewer == null)
				return 0;

			var pageIndex = this.virtualPanel.GetItemIndexByVerticalOffset(this.scrollViewer.VerticalOffset);

			if (pageIndex > 0)
			{
				if (viewType == ViewType.Facing)
					pageIndex *= 2;
				else if (viewType == ViewType.BookView)
					pageIndex = (pageIndex * 2) - 1;
			}

			return pageIndex;
		}

		ScrollViewer ScrollViewer
		{
			get { return this.scrollViewer; }
		}

		UserControl Instance
		{
			get { return this; }
		}*/
    }
}
