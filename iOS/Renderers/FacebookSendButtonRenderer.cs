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
	public class FacebookSendButtonRenderer : ViewRenderer<FacebookSendButton, UIView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FacebookSendButton> e)
		{
			FacebookSendButton element = (FacebookSendButton)Element;
			if (element == null) return;
			ShareLinkContent content = new ShareLinkContent();
			content.SetContentUrl(new NSUrl(element.Link));
			SendButton shareBtn = new SendButton(new CGRect(0, 0, 0, 0));
			shareBtn.SetShareContent(content);

			UIButton cBtn = new UIButton(new CGRect(0, 0, 61, 60));
			cBtn.Layer.BorderWidth = 2;
			cBtn.Layer.CornerRadius = 30;
			cBtn.Layer.BorderColor = Color.FromHex("#ff0b65ff").ToCGColor();

			cBtn.TouchUpInside += (sender, ef) =>
			{
				shareBtn.SendActionForControlEvents(UIControlEvent.TouchUpInside);
			};
			this.AddSubview(cBtn);
		}
	}
}
