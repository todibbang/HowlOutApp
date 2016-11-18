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

[assembly: ExportRenderer(typeof(FacebookSendButton), typeof(FacebookSendButtonRenderer))]

namespace HowlOut.iOS.Renderers
{
	public class FacebookSendButtonRenderer : ViewRenderer
	{
		public FacebookSendButtonRenderer()
		{
			//base.ViewDidLoad();

			int height = AppDelegate.height;
			int width = AppDelegate.width;

			ShareLinkContent content = new ShareLinkContent();
			content.SetContentUrl(new NSUrl("https://developers.facebook.com"));

			//ShareButton shareBtn = new ShareButton(new CGRect(width / 2 - 110, height / 1.1, 220, 46));
			SendButton shareBtn = new SendButton(new CGRect(0, 0, 100, 44));

			shareBtn.BackgroundColor = UIColor.White;
			shareBtn.TintColor = UIColor.White;

			shareBtn.Layer.BackgroundColor = Color.White.ToCGColor();
			shareBtn.Layer.ShadowColor = Color.White.ToCGColor();

			shareBtn.Layer.CornerRadius = 22f;
			shareBtn.Layer.BorderWidth = 0;
			//shareBtn.Layer.BorderColor = App.HowlOut.ToCGColor();

			shareBtn.SetShareContent(content);

			this.AddSubview(shareBtn);
		}

	}
}
