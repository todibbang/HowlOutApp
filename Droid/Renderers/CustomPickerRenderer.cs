using System;
using Xamarin.Forms;
using HowlOut;
using HowlOut.Droid;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.Views;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace HowlOut.Droid
{
	public class CustomPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			CustomPicker datePicker = (CustomPicker)Element;

			if (datePicker != null)
			{
				SetBorderStyle(datePicker);
				SetTextColor(datePicker);
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

			CustomPicker datePicker = (CustomPicker)Element;

			if (e.PropertyName == CustomPicker.TextColorProperty.PropertyName)
			{
				this.Control.SetTextColor(datePicker.TextColor.ToAndroid());
			}
		}

		void SetBorderStyle(CustomPicker datePicker)
		{
			//this.Control.BorderStyle = (datePicker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;

		}

		void SetTextColor(CustomPicker datePicker)
		{
			//this.Control.TextColor = datePicker.TextColor.ToUIColor();
		}

		void setFontSize(CustomPicker datePicker)
		{

		}
	}
}