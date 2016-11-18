using System;
using Xamarin.Forms;

namespace HowlOut
{
	public class CropImage : View
	{
		#region MaxWidthProperty
		public static readonly BindableProperty MaxWidthProperty = BindableProperty.Create<CropImage, double>(p => p.MaxWidth, default(double));
		public double MaxWidth
		{
			get
			{
				return (double)GetValue(MaxWidthProperty);
			}
			set
			{
				SetValue(MaxWidthProperty, value);
			}
		}
		#endregion

		#region MaxHeightProperty
		public static readonly BindableProperty MaxHeightProperty = BindableProperty.Create<CropImage, double>(p => p.MaxHeight, default(double));
		public double MaxHeight
		{
			get
			{
				return (double)GetValue(MaxHeightProperty);
			}
			set
			{
				SetValue(MaxHeightProperty, value);
			}
		}
		#endregion

		#region SourceProperty
		public static readonly BindableProperty SourceProperty = BindableProperty.Create<IconView, string>(p => p.Source, default(string));
		public string Source
		{
			get
			{
				return (string)GetValue(SourceProperty);
			}
			set
			{
				SetValue(SourceProperty, value);
			}
		}
		#endregion
	}
}
