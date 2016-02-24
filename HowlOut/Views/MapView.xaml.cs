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

		public MapView (Position position)
		{
			InitializeComponent ();
			ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };
			map.MoveToRegion (MapSpan.FromCenterAndRadius (new Position (position.Latitude, position.Longitude), Distance.FromKilometers (20)));
			mapLayout.Children.Add(map);
			map.Tapped += (sender, e) => 
			{
				getResults(map.tapPosition, map);
			};

			selectButton.Clicked += (sender, e) => 
			{
				App.coreView.setContentView (createEventView, 0);
			};
		}

		public MapView (Event eve)
		{
			InitializeComponent ();
			ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };

			UtilityManager utilityManager = new UtilityManager ();
			utilityManager.setMapForEvent (eve, map, mapLayout);



			map.Tapped += (sender, e) => 
			{
				
				getResults(map.tapPosition, map);


			};

		}

		public async void getResults(Position position, ExtMap map)
		{
			tappedAddress="";

			var possibleAddresses = await getAddressFromPosition.GetAddressesForPositionAsync (position);
			foreach (var address in possibleAddresses) {
				
				tappedAddress += address;
			}
			tappedAddress = Regex.Replace(tappedAddress, @"\r\n?|\n", " ");


			map.Pins.Clear();
			var pin = new Pin
			{
				Type = PinType.Place,
				Position = new Position(map.tapPosition.Latitude, map.tapPosition.Longitude),
				Label = tappedAddress + "",
				Address = tappedAddress + "",
			};
			map.Pins.Add (pin);

			if (createEventView != null) {
				createEventView.newEvent.Latitude = map.tapPosition.Latitude;
				createEventView.newEvent.Longitude = map.tapPosition.Longitude;
				createEventView.newEvent.PositionName = tappedAddress;
				createEventView.locationButton.Text = tappedAddress;
			}
		}
	}
}

