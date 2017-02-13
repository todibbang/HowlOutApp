using System;
using System.ComponentModel;
 
using HowlOut;
using HowlOut.iOS;
 
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Drawing;
 
[assembly: ExportRenderer (typeof (CustomLabel), typeof (CustomLabelRenderer))]
 
namespace HowlOut.iOS
{
	public class CustomLabelRenderer : LabelRenderer
	{
		// Override the OnElementChanged method so
		// we can tweak this renderer post-initial setup
		protected override void OnElementChanged(
			ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			var label = Control as UILabel;
			if (label != null)
			{
				label.AdjustsFontSizeToFitWidth = true;
				label.MinimumFontSize = 10;
				label.Lines = 3;
				label.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
				label.LineBreakMode = UILineBreakMode.Clip;
			}
		}
	}
}