using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class SelectBannerView : ContentView
	{
		public CreateEvent createEventView;
		public CreateGroup createGroupView;

		ObservableCollection<Button> bannerButtons = new ObservableCollection<Button>();

		List<string> avaliableBanners;

		List<string> partyBanners = new List<string>{"banner_blade.png", "banner_controller.png", "banner_donut.png", "banner_fodbold.png", "banner_golf.png", "banner_gravid.png", "banner_hjort.png", 
			"banner_kaffe.png", "banner_marked.png", "banner_publikum.png", "banner_scrabble.png", "banner_skak.png", "banner_skater.png", "banner_skovsoe.png", "banner_surf.png", "banner_vinglas.png",}; // "Blade.jpg", "Donut.jpg", "Fotografiapparat.jpg", "Gadelys.jpg",
			//"Grøntmarked.jpg", "Kaffebønner.jpg", "Kaffemaskine.jpg", "Motor.jpg", "Publikum koncert.jpg", "RebPaaStol.jpg", "Sejl.jpg",
			//"Skovsø.jpg", "Snedker.jpg", "surf tunnel.jpg", "Vinglas.jpg", "Zigaret.jpg"};

		public SelectBannerView ()
		{
			InitializeComponent ();


			avaliableBanners = new List<string>();
			avaliableBanners.AddRange(partyBanners);

			for (int i = 0; i < avaliableBanners.Count; i++) {
				Grid newGrid = new Grid () {
					RowSpacing = 0,
					ColumnSpacing = 0,
					Padding = 0,
					WidthRequest = App.coreView.Width,
					HeightRequest = App.coreView.Width * 0.56
				};
				newGrid.RowDefinitions.Add (new RowDefinition{ Height = App.coreView.Width  * 0.563 });
				newGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = App.coreView.Width });

				Button newBannerButton = new Button () { 
					Text = avaliableBanners [i],
					TextColor = Color.Transparent,
					HeightRequest = App.coreView.Width  * 0.563,
					BackgroundColor = Color.Transparent,
				};

				Image newImage = new Image () {
					Source = avaliableBanners [i],
				};

				newImage.Source = avaliableBanners [i];

				newGrid.Children.Add (newImage, 0, 0);
				newGrid.Children.Add (newBannerButton, 0, 0);



				bannerList.Children.Add (newGrid);
				bannerButtons.Add (newBannerButton);
				//bannerList.Children.Add (new StackLayout(){Padding=5});
			}

			foreach(Button button in bannerButtons) {
				button.Clicked += (sender, e) => {
					System.Diagnostics.Debug.WriteLine(button.Text + " ");
					//createEventView.newEvent.BannerName = button.Text;
					if (createEventView != null) { createEventView.setBanner(button.Text); }
					else if (createGroupView != null) { createGroupView.setBanner(button.Text); }
					//App.coreView.setContentView (createEventView, "CreateEvent");
					App.coreView.returnToPreviousView();
				};
			}

		}
	}
}

