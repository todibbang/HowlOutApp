using System;
using System.Collections.Generic;
using System.Text;
using HowlOut;
using HowlOut.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRendererAttribute(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]

namespace HowlOut.iOS.Renderers
{
	public class CustomDatePickerRenderer : DatePickerRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			CustomDatePicker datePicker = (CustomDatePicker)Element;

			if (datePicker != null)
			{
				SetBorderStyle(datePicker);
				SetTextColor(datePicker);

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

			CustomDatePicker datePicker = (CustomDatePicker)Element;

			if (e.PropertyName == CustomDatePicker.TextColorProperty.PropertyName)
			{
				this.Control.TextColor = datePicker.TextColor.ToUIColor();
			}
		}

		void SetBorderStyle(CustomDatePicker datePicker)
		{
			this.Control.BorderStyle = (datePicker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;

		}

		void SetTextColor(CustomDatePicker datePicker)
		{
			this.Control.TextColor = datePicker.TextColor.ToUIColor();
		}

		void setFontSize(CustomDatePicker datePicker)
		{
			
		}
	}
}