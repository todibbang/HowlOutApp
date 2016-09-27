using System; using System.Collections.Generic; using System.Text; using HowlOut; using HowlOut.iOS.Renderers; using UIKit; using Xamarin.Forms; using Xamarin.Forms.Platform.iOS;  [assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]  namespace HowlOut.iOS.Renderers {
	public class CustomEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				Control.Layer.BorderWidth = 1;
				Control.Layer.BorderColor = Color.White.ToCGColor(); 				Control.TextAlignment = UITextAlignment.Left;
			}
		}
	} }