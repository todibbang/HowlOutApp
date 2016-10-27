using System;
using System.Collections.Generic;
using System.Text;
using HowlOut;
using HowlOut.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRendererAttribute(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer))]

namespace HowlOut.iOS.Renderers
{
	public class CustomTimePickerRenderer : TimePickerRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			CustomTimePicker timePicker = (CustomTimePicker)Element;

			if (timePicker != null)
			{
				SetBorderStyle(timePicker);
				SetTextColor(timePicker);


				Control.AdjustsFontSizeToFitWidth = true;
			}

			if (e.OldElement == null)
			{
				//Wire events
			}

			if (e.NewElement == null)
			{
				//Unwire events
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control == null)
			{
				return;
			}

			CustomTimePicker timePicker = (CustomTimePicker)Element;

			if (e.PropertyName == CustomTimePicker.TextColorProperty.PropertyName)
			{
				this.Control.TextColor = timePicker.TextColor.ToUIColor();
			}
		}

		void SetBorderStyle(CustomTimePicker timePicker)
		{
			this.Control.BorderStyle = (timePicker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;

		}

		void SetTextColor(CustomTimePicker timePicker)
		{
			this.Control.TextColor = timePicker.TextColor.ToUIColor();
		}
	}
}