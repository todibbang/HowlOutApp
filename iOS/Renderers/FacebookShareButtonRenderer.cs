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
	public class FacebookShareButtonRenderer : ViewRenderer<FacebookShareButton, UIView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FacebookShareButton> e)
		{
			FacebookShareButton element = (FacebookShareButton)Element;
			if (element == null) return;
			ShareLinkContent content = new ShareLinkContent();
			content.SetContentUrl(new NSUrl(element.Link));
			ShareButton shareBtn = new ShareButton(new CGRect(0, 0, 0, 0));
			shareBtn.SetShareContent(content);

			UIButton cBtn = new UIButton(new CGRect(0, 0, 61, 60));
			cBtn.Layer.BorderWidth = 2;
			cBtn.Layer.CornerRadius = 30;
			cBtn.Layer.BorderColor = Color.FromHex("#ff2e4587").ToCGColor();

			cBtn.TouchUpInside += (sender, ef) =>
			{
				shareBtn.SendActionForControlEvents(UIControlEvent.TouchUpInside);
			};
			this.AddSubview(cBtn);
		}
		/*
		public FacebookShareButtonRenderer()
		{
			ShareLinkContent content = new ShareLinkContent();
			content.SetContentUrl(new NSUrl("https://developers.facebook.com"));

			//ShareButton shareBtn = new ShareButton(new CGRect(width / 2 - 110, height / 1.1, 220, 46));
			ShareButton shareBtn = new ShareButton(new CGRect(0, 0, 220, 46));

			shareBtn.SetShareContent(content);

			shareBtn.BackgroundColor = Color.Silver.ToUIColor();

			this.AddSubview(shareBtn);
		} */
	}
}
