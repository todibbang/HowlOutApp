using System;
using Xamarin.Forms;
using HowlOut;
using HowlOut.Droid;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.Views;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace HowlOut.Droid
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
				this.Control.SetTextColor(datePicker.TextColor.ToAndroid());
			}
		}

		void SetBorderStyle(CustomDatePicker datePicker)
		{
			//this.Control.BorderStyle = (datePicker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;

		}

		void SetTextColor(CustomDatePicker datePicker)
		{
			//this.Control.TextColor = datePicker.TextColor.ToUIColor();
		}

		void setFontSize(CustomDatePicker datePicker)
		{

		}
	}
}