using System;
using System.ComponentModel;
using HowlOut;
using Xamarin.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using HowlOut.iOS;
using CoreGraphics;
using System.Drawing;
using System.Threading.Tasks;

[assembly: ExportRendererAttribute(typeof(CropImage), typeof(CustomCropImageRenderer))]



namespace HowlOut.iOS
{
	public class CustomCropImageRenderer : ViewRenderer<CropImage, UIImageView>
	{
		private bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing && base.Control != null)
			{
				UIImage image = base.Control.Image;
				UIImage uIImage = image;
				if (image != null)
				{
					uIImage.Dispose();
					uIImage = null;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CropImage> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null)
			{
				UIImageView uIImageView = new UIImageView(CGRect.Empty)
				{
					ContentMode = UIViewContentMode.ScaleAspectFit,
					ClipsToBounds = true
				};
				SetNativeControl(uIImageView);
			}
			if (e.NewElement != null)
			{
				SetImage(e.OldElement);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == IconView.SourceProperty.PropertyName)
			{
				SetImage(null);
			}
			else if (e.PropertyName == IconView.ForegroundProperty.PropertyName)
			{
				SetImage(null);
			}
		}

		private async void SetImage(CropImage previous = null)
		{
			if (previous == null)
			{
				/*
				await Task.Delay(100);

				var uiImage = new UIImage(Element.Source);
				//uiImage = uiImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
				//Control.TintColor = Element.Foreground.ToUIColor();


				//var sourceWidth = Element.Width;
				//var SourceHeight = Element.Height;

				var sourceWidth = Element.Width;
				var SourceHeight = Element.Height;

				//var sourceSize = sourceImage.Size;
				var maxResizeFactor = Math.Max(Element.MaxWidth / sourceWidth, Element.MaxHeight / SourceHeight);


				//if (maxResizeFactor > 1) return sourceImage;


				double width = maxResizeFactor * sourceWidth;
				var height = maxResizeFactor * SourceHeight;
				UIGraphics.BeginImageContextWithOptions(new SizeF((float)width, (float)height), false, 2.0f);
				uiImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
				Control.Image = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
*/


				/*
				var uiImage = new UIImage(Element.Source);
				await Task.Delay(100);
				var sourceSize = uiImage.Size;
				var maxResizeFactor = Math.Max(Element.MaxWidth / sourceSize.Width, Element.MaxHeight / sourceSize.Height);


				//if (maxResizeFactor > 1) return sourceImage;


				double width = maxResizeFactor * sourceSize.Width * 2;
				//var height = maxResizeFactor * sourceSize.Height * 2;
				float height = 80f;
				UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
				uiImage.Draw(new RectangleF(0, 0, (float)width, (float)height));

				*/

				if (Element.Source == null)
				{
					return;
				}

				await Task.Delay(10);
				double originalWidth = Element.Width;
				double originalHeight = Element.Height;


				var uiImage = new UIImage(Element.Source);
				var imgSize = uiImage.Size;

				var maxResizeFactor = Math.Max(originalWidth / imgSize.Width, originalHeight / imgSize.Height);

				double width = maxResizeFactor * imgSize.Width;
				var height = maxResizeFactor * imgSize.Height;
				UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
				uiImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
				uiImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();


				imgSize = uiImage.Size;


				UIGraphics.BeginImageContext(new SizeF((float)originalWidth, (float)originalHeight));
				var context = UIGraphics.GetCurrentContext();
				var clippedRect = new RectangleF(0, 0, (float)originalWidth, (float)originalHeight);
				context.ClipToRect(clippedRect);
				var drawRect = new RectangleF(0f, -40f, (float)imgSize.Width, (float)imgSize.Height);
				uiImage.Draw(drawRect);



				Control.Image = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();






				//Control.Image = resultImage;
				/*
				if (!_isDisposed)
				{
					((IVisualElementController)Element).NativeSizeChanged();
				}
				*/
			}
		}






		public CustomCropImageRenderer()
		{
			
		}






	}
}
