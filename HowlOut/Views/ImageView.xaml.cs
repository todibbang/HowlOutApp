using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ImageView : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		public ImageView(string source)
		{
			InitializeComponent();
			image.Source = source;
			TapGestureRecognizer tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) =>
			{
				App.coreView.returnToPreviousView();
			};
			image.GestureRecognizers.Add(tgr);
		}
	}
}
