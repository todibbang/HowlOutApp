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

			back.Clicked += (sender, e) =>
			{
				App.coreView.returnToPreviousView();
			};
			knapTwo.Clicked += (sender, e) =>
			{
			};
			knapThree.Clicked += (sender, e) =>
			{
			};
		}
	}
}

