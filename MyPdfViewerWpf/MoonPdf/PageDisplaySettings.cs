using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPdfViewerWpf
{
	internal class PageDisplaySettings
	{
		public int ImagesPerRow { get; set; }
		public double HorizontalOffsetBetweenPages { get; set; }
		public ViewType ViewType { get; set; }
		public float ZoomFactor { get; set; }
		public ImageRotation Rotation { get; set; }

		public PageDisplaySettings(int imagesPerRow, ViewType viewType, double horizontalOffsetBetweenPages, ImageRotation rotation = ImageRotation.None, float zoomFactor = 1.0f)
		{
			this.ImagesPerRow = imagesPerRow;
			this.ZoomFactor = zoomFactor;
			this.ViewType = viewType;
			this.HorizontalOffsetBetweenPages = viewType == ViewType.SinglePage ? 0 : horizontalOffsetBetweenPages;
			this.Rotation = rotation;
		}
	}

	public enum ImageRotation
	{
		None,
		Rotate90,
		Rotate180,
		Rotate270
	}

	public enum ViewType
	{
		SinglePage,
		Facing,
		BookView
	}
}
