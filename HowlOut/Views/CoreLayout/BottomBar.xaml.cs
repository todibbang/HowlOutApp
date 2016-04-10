using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class BottomBar : ContentView
	{
		DataManager _dataManager;

		public BottomBar ()
		{
			InitializeComponent ();
			_dataManager = new DataManager ();

			/*
			var exploreImage = new TapGestureRecognizer();
			exploreImage.Tapped += async (sender, e) => 
			{
				await exploreBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await exploreBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentView(new EventView(), "Event");
			};
			exploreBtn.GestureRecognizers.Add(exploreImage);  
			*/

			meBtn.Clicked += (sender, e) => 
			{
				App.coreView.setContentView (2);
			};

			eventBtn.Clicked += (sender, e) => 
			{
				App.coreView.setContentView (1);
			};
		}
	}
}

