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

			var exploreImage = new TapGestureRecognizer();
			exploreImage.Tapped += (sender, e) => 
			{
				App.coreView.setContentView(new SearchEvent(), 1);
			};
			exploreBtn.GestureRecognizers.Add(exploreImage);  

			var manageImage = new TapGestureRecognizer();
			manageImage.Tapped += (sender, e) => 
			{
				App.coreView.setContentView(new ManageEvent(), 2);
			};
			manageBtn.GestureRecognizers.Add(manageImage); 

			/*
			knapOne.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new SearchEvent(), 1);
			};
			knapTwo.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new ManageEvent(), 2);
			};
			*/
		}
	}
}

