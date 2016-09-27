﻿
using System;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using System.ComponentModel;
using HowlOut;
using Xamarin.Forms;
using HowlOut.Droid;
using Android.Graphics;
using Android.Graphics.Drawables;

[assembly: ExportRendererAttribute(typeof(IconView), typeof(IconViewRenderer))]





namespace HowlOut.Droid
{
	public class IconViewRenderer : ViewRenderer<IconView, ImageView>
	{
		private bool _isDisposed;

		public IconViewRenderer()
		{
			base.AutoPackage = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}
			_isDisposed = true;
			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<IconView> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null)
			{
				SetNativeControl(new ImageView(Context));
			}
			UpdateBitmap(e.OldElement);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == IconView.SourceProperty.PropertyName)
			{
				UpdateBitmap(null);
			}
			else if (e.PropertyName == IconView.ForegroundProperty.PropertyName)
			{
				UpdateBitmap(null);
			}
		}

		private void UpdateBitmap(IconView previous = null)
		{
			if (!_isDisposed)
			{
				var d = Resources.GetDrawable(Element.Source).Mutate();
				d.SetColorFilter(new LightingColorFilter(Element.Foreground.ToAndroid(), Element.Foreground.ToAndroid()));
				d.Alpha = Element.Foreground.ToAndroid().A;
				Control.SetImageDrawable(d);
				((IVisualElementController)Element).NativeSizeChanged();
			}
		}
	}
}
