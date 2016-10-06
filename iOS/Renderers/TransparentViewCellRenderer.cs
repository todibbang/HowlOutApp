using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using HowlOut.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(TransparentViewCellRenderer))]


namespace HowlOut.iOS
{
	public class TransparentViewCellRenderer : ViewCellRenderer
	{
		public TransparentViewCellRenderer()
		{

		}

		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			if (cell != null) cell.BackgroundColor = UIColor.Clear;
			return cell;
		}
	}
}
