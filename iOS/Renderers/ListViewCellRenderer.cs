/*

using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;
using HowlOut.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ListViewCellRenderer))]

namespace HowlOut.iOS
{
	public class ListViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell uitv, UITableView tv)
		{
			var cell = base.GetCell(item, uitv, tv);
			if (cell != null)
			{
				cell.SelectedBackgroundView = new UIView() { BackgroundColor = UIColor.Blue };
				cell.BackgroundColor = UIColor.Black; // Set whatever native properties you need
			}
			tv.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			return cell;
		}
	}
}
*/