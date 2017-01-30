using System; using System.Collections.Generic; using System.Text; using HowlOut; using HowlOut.iOS.Renderers; using UIKit; using Xamarin.Forms; using Xamarin.Forms.Platform.iOS; using CoreGraphics;  [assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]  namespace HowlOut.iOS.Renderers {
	public class CustomEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);  			var toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, Control.Frame.Size.Width, 44.0f));  			toolbar.Items = new[] 			{ 				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), 				new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, delegate {  Control.ResignFirstResponder(); }) 			}; 			this.Control.InputAccessoryView = toolbar;

			if (this.Control != null)
			{
				this.Control.Layer.BorderWidth = 0; 				this.Control.Layer.BorderColor = App.HowlOutBackground.ToCGColor(); 				this.Control.TextAlignment = UITextAlignment.Left;
			}
		}
	} }