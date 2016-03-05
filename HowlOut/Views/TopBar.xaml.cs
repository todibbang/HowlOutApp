using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class TopBar : ContentView
	{
		DataManager dataManager = new DataManager();

		public TopBar ()
		{
			InitializeComponent ();

			var backImage = new TapGestureRecognizer();

			backImage.Tapped += async (sender, e) => 
			{
				await backBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await backBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.returnToPreviousView();
			};
				
			backBtn.GestureRecognizers.Add(backImage); 

			back.Clicked += (sender, e) =>
			{
				App.coreView.returnToPreviousView();
			};

			updateButton.Clicked += (sender, e) =>
			{
				dataManager.update();
			};

			howlOut.Clicked += (sender, e) => 
			{
				App.coreView.setContentView (new UserProfile (App.userProfile, null, null, false, false), "UserProfile");
			};
		}
	}
}

