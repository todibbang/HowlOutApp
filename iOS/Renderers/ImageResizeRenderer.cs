using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using UIKit;
using HowlOut.iOS;

using System.Drawing;
using UIKit;
using CoreGraphics;

[assembly: Dependency(typeof(ImageResizeRenderer))]

namespace HowlOut.iOS
{
	public class ImageResizeRenderer : ImageResizer
	{
		public byte[] ResizeImage(byte[] imageData, float size)
		{
			return ResizeImageIOS(imageData, size);
		}

		public byte[] ResizeImageIOS(byte[] imageData, float size)
		{
			UIImage originalImage = ImageFromByteArray(imageData);
			System.Diagnostics.Debug.Write("originalImage.Size.Height"+ originalImage.Size.Height + ", " + originalImage.Size.Width);
			UIImageOrientation orientation = originalImage.Orientation;

			float width = size;
			float height = ((float)originalImage.Size.Height / (float)originalImage.Size.Width) * size;
			System.Diagnostics.Debug.Write("new size" + width + ", " + height);
			//create a 24bit RGB image
			using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
												 (int)width, (int)height, 8,
												 (int)(4 * width), CGColorSpace.CreateDeviceRGB(),
												 CGImageAlphaInfo.PremultipliedFirst))
			{

				RectangleF imageRect = new RectangleF(0, 0, width, height);

				// draw the image
				context.DrawImage(imageRect, originalImage.CGImage);

				UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

				// save the image as a jpeg
				return resizedImage.AsJPEG().ToArray();
			}
		}

		public UIKit.UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			UIKit.UIImage image;
			try
			{
				image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
			}
			catch (Exception e)
			{
				Console.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}
	}
}
