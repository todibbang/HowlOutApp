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

			MeButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Transparent, "", 0));
			EventButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Transparent, "", 0));


			var exploreImage = new TapGestureRecognizer();
			exploreImage.Tapped += async (sender, e) => 
			{
				App.coreView.setContentViewWithQueue(new MapTest(), "");
			};
			exploreBtn.GestureRecognizers.Add(exploreImage);  

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

