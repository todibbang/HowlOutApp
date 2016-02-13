using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class BottomBar : ContentView
	{
		public BottomBar ()
		{
			InitializeComponent ();

			knapOne.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new SearchEvent());
			};
			knapTwo.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new ManageEvent());
			};
		}
	}
}

