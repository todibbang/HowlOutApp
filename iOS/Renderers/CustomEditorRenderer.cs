using System; using System.Collections.Generic; using System.Text; using HowlOut; using HowlOut.iOS.Renderers; using UIKit; using Xamarin.Forms; using Xamarin.Forms.Platform.iOS;  [assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))] namespace HowlOut.iOS.Renderers {
	public class CustomEditorRenderer : EditorRenderer
	{
		private string Placeholder { get; set; }

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			var element = this.Element as CustomEditor;

			if (Control != null && element != null)
			{
				Placeholder = element.Placeholder;

				if (Control.Text == "" || Control.Text == Placeholder)
				{ 					Control.TextColor = App.PlaceHolderColor.ToUIColor();
					Control.Text = Placeholder; 				}
				else { 					Control.TextColor = App.NormalTextColor.ToUIColor(); 				}
				//Control.Layer.BorderColor = Color.White.ToCGColor();
				Control.Layer.BorderWidth = 0; 				Control.Layer.CornerRadius = 4; 				Control.TextAlignment = UITextAlignment.Left;

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
		}
	} }