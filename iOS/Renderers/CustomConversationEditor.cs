using System;
using System.Collections.Generic;
using System.Text;
using HowlOut;
using HowlOut.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;

[assembly: ExportRenderer(typeof(ConversationEditor), typeof(CustomConversationEditor))]
namespace HowlOut.iOS.Renderers
{
	public class CustomConversationEditor : EditorRenderer
	{
		private string Placeholder { get; set; }

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			var element = this.Element as ConversationEditor;

			if (Control != null && element != null)
			{
				Placeholder = element.Placeholder;

				if (Control.Text == "" || Control.Text == Placeholder)
				{
					Control.TextColor = App.PlaceHolderColor.ToUIColor();
					Control.Text = Placeholder;
				}
				else {
					Control.TextColor = App.NormalTextColor.ToUIColor();
				}
				//Control.Layer.BorderColor = Color.White.ToCGColor();
				Control.Layer.BorderWidth = 0;
				Control.Layer.CornerRadius = 4;
				Control.TextAlignment = UITextAlignment.Left;

				Control.ShouldBeginEditing += (UITextView textView) =>
				{
					if (textView.Text == Placeholder)
					{
						textView.Text = "";
						textView.TextColor = App.NormalTextColor.ToUIColor(); // Text Color
					}

					return true;
				};

				Control.ShouldEndEditing += (UITextView textView) =>
				{
					if (textView.Text == "")
					{
						textView.Text = Placeholder;
						textView.TextColor = App.PlaceHolderColor.ToUIColor(); // Placeholder Color
					}

					return true;
				};
			}

			var toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, Control.Frame.Size.Width, 44.0f));

			toolbar.Items = new[]
			{
				new UIBarButtonItem("Close", UIBarButtonItemStyle.Plain, delegate { Control.ResignFirstResponder(); }),
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				new UIBarButtonItem("Send", UIBarButtonItemStyle.Plain, delegate {
					App.coreView.viewdConversation.pushPostNewComment();
				})
			};

			this.Control.InputAccessoryView = toolbar;
		}
	}
}