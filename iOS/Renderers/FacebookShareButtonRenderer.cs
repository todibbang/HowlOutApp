using HowlOut.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using HowlOut;
using Facebook.ShareKit;
using CoreGraphics;
using Foundation;

using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

[assembly: ExportRenderer(typeof(FacebookShareButton), typeof(FacebookShareButtonRenderer))]

namespace HowlOut.iOS.Renderers
{
	public class FacebookShareButtonRenderer : ViewRenderer
	{
		public FacebookShareButtonRenderer()
		{
			//base.ViewDidLoad();

			int height = AppDelegate.height;
			int width = AppDelegate.width;

			ShareLinkContent content = new ShareLinkContent();
			content.SetContentUrl(new NSUrl("https://developers.facebook.com"));

			//ShareButton shareBtn = new ShareButton(new CGRect(width / 2 - 110, height / 1.1, 220, 46));
			ShareButton shareBtn = new ShareButton(new CGRect(0, 0, 100, 44));
			shareBtn.SetShareContent(content);

			this.AddSubview(shareBtn);
		}

	}
}
