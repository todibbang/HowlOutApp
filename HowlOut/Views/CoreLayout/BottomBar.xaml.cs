using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class BottomBar : ContentView
	{
		DataManager _dataManager;
		private StandardButton standardButton = new StandardButton();

		public BottomBar ()
		{
			InitializeComponent ();
			_dataManager = new DataManager ();

			//MeButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Transparent, "", 0));
			EventButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Transparent, "", 0));


			var createImage = new TapGestureRecognizer();
			createImage.Tapped += async (sender, e) =>
			{
				App.coreView.setContentView(0);
			};
			create.GestureRecognizers.Add(createImage);

			var exploreImage = new TapGestureRecognizer();
			exploreImage.Tapped += async (sender, e) => 
			{
				App.coreView.setContentView(1);
			};
			search.GestureRecognizers.Add(exploreImage);  

			events.Clicked += (sender, e) => 
			{
				App.coreView.setContentView (2);
			};

			var friendsImage = new TapGestureRecognizer();
			friendsImage.Tapped += async (sender, e) =>
			{
				App.coreView.setContentView(3);
			};
			friends.GestureRecognizers.Add(friendsImage);

			var socialImage = new TapGestureRecognizer();
			socialImage.Tapped += async (sender, e) =>
			{
				App.coreView.setContentView(4);
			};
			me.GestureRecognizers.Add(socialImage);
		}
	}
}

