using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class SelectBannerView : ContentView
	{
		public CreateEvent createEventView;

		ObservableCollection<Button> bannerButtons = new ObservableCollection<Button>();

		List<string> avaliableBanners;

		List<string> partyBanners = new List<string>{"Party.png", "Controller.jpeg", "Fodbold.jpeg", "Golf.jpeg", "Gravid.jpeg", "Hjort.jpeg", "Scrabble.jpeg", "Skak.jpeg", "Skater.jpeg", "Surf.jpeg" };

		public SelectBannerView ()
		{
			InitializeComponent ();


			avaliableBanners = new List<string>();
			avaliableBanners.AddRange(partyBanners);

			for (int i = 0; i < avaliableBanners.Count; i++) {
				Button newBannerButton = new Button () { 
					Image = avaliableBanners [i],
					WidthRequest = App.coreView.Width,
					HeightRequest = App.coreView.Width / 2
				};
				bannerList.Children.Add (newBannerButton);
				bannerButtons.Add (newBannerButton);
				bannerList.Children.Add (new StackLayout(){Padding=5});
			}

			foreach(Button button in bannerButtons) {
				button.Clicked += (sender, e) => {
					System.Diagnostics.Debug.WriteLine(button.Image.File + " ");
					createEventView.newEvent.Banner = button.Image.File;
					createEventView.setBanner(createEventView.newEvent.Banner);
					App.coreView.setContentView (createEventView, "CreateEvent");
				};
			}

		}
	}
}

