using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class TopBar : ContentView
	{
		public TopBar ()
		{
			InitializeComponent ();

			knapOne.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new FilterSearch(), false);
			};
			knapTwo.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new FilterSearch(), false);
			};
			knapThree.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new FilterSearch(), false);
			};
		}
	}
}

