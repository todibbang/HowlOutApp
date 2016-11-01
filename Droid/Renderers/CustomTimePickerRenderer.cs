using System;
using Xamarin.Forms;
using HowlOut;
using HowlOut.Droid;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.Views;

[assembly: ExportRenderer(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer))]
namespace HowlOut.Droid
{
	public class CustomTimePickerRenderer : TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			CustomTimePicker datePicker = (CustomTimePicker)Element;

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

			CustomTimePicker datePicker = (CustomTimePicker)Element;

			if (e.PropertyName == CustomTimePicker.TextColorProperty.PropertyName)
			{
				this.Control.SetTextColor(datePicker.TextColor.ToAndroid());
			}
		}

		void SetBorderStyle(CustomTimePicker datePicker)
		{
			//this.Control.BorderStyle = (datePicker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;

		}

		void SetTextColor(CustomTimePicker datePicker)
		{
			//this.Control.TextColor = datePicker.TextColor.ToUIColor();
		}

		void setFontSize(CustomTimePicker datePicker)
		{

		}
	}
}