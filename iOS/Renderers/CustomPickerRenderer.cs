using System;
using System.Collections.Generic;
using System.Text;
using HowlOut;
using HowlOut.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRendererAttribute(typeof(CustomPicker), typeof(CustomPickerRenderer))]

namespace HowlOut.iOS.Renderers
{
	public class CustomPickerRenderer : PickerRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			CustomPicker Picker = (CustomPicker)Element;

			if (Picker != null)
			{
				SetBorderStyle(Picker);
				SetTextColor(Picker);

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

			CustomPicker Picker = (CustomPicker)Element;

			if (e.PropertyName == CustomPicker.TextColorProperty.PropertyName)
			{
				this.Control.TextColor = Picker.TextColor.ToUIColor();
			}
		}

		void SetBorderStyle(CustomPicker Picker)
		{
			this.Control.BorderStyle = (Picker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;

		}

		void SetTextColor(CustomPicker Picker)
		{
			this.Control.TextColor = Picker.TextColor.ToUIColor();
		}

		void setFontSize(CustomPicker Picker)
		{

		}
	}
}