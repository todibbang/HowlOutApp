using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class VoteItemTemplate : ContentView
	{
		public VoteItemTemplate()
		{
			InitializeComponent();
			var tap = new TapGestureRecognizer();
			tap.Tapped += (sender, e) =>
			{
				if (thisGrid.BackgroundColor == Color.Transparent) thisGrid.BackgroundColor = App.HowlOutFade;
				else thisGrid.BackgroundColor = Color.Transparent;
			};
			clicked.GestureRecognizers.Add(tap);
		}
	}
}
