using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Text.RegularExpressions;

namespace HowlOut
{
	public partial class MapView : ContentView
	{
		public Geocoder getAddressFromPosition = new Geocoder ();
		public String tappedAddress = "";
		public CreateEvent createEventView;
		UtilityManager utilityManager = new UtilityManager ();

		ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };
		Button selectButton = new Button ();
		Entry searchEntry = new Entry ();

		public MapView (Position position)
		{
			InitializeComponent ();
			mapLayout.VerticalOptions = LayoutOptions.FillAndExpand;
			utilityManager.setMapForEvent(position, map, mapLayout);
			selectButton.Text = "selectButton";
			selectButton.BackgroundColor = Color.White;
			selectLayout.Children.Add (selectButton);
			searchEntry.Text = "searchEntry";
			searchLayout.Children.Add (searchEntry);

			map.Tapped += (sender, e) => {
				getResults(map.tapPosition, map);
			};

			selectButton.Clicked += (sender, e) => {
				App.coreView.setContentView (createEventView, 0);
			};
		}

		public MapView (Event eve)
		{
			InitializeComponent ();
			utilityManager.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
			utilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.PositionName);
		}

		public async void getResults(Position position, ExtMap map)
		{
			tappedAddress="";
			var possibleAddresses = await getAddressFromPosition.GetAddressesForPositionAsync (position);
			foreach (var address in possibleAddresses) { tappedAddress += address; }
			tappedAddress = Regex.Replace(tappedAddress, @"\r\n?|\n", " ");

			map.Pins.Clear();
			utilityManager.setPin (new Position (map.tapPosition.Latitude, map.tapPosition.Longitude), map, tappedAddress, tappedAddress);

			if (createEventView != null) {
				createEventView.newEvent.Latitude = map.tapPosition.Latitude;
				createEventView.newEvent.Longitude = map.tapPosition.Longitude;
				createEventView.newEvent.PositionName = tappedAddress;
				createEventView.locationButton.Text = tappedAddress;
			}
			searchEntry.Text = tappedAddress;
		}
	}
}

