using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public partial class MapView : ContentView
	{
		public MapView (Event eve)
		{
			InitializeComponent ();

			setMap (eve);
		}

		public async void setMap(Event eve)
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;

			var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);

			System.Diagnostics.Debug.WriteLine ("Position Status: {0}", position.Timestamp);
			System.Diagnostics.Debug.WriteLine ("Position Latitude: {0}", position.Latitude);
			System.Diagnostics.Debug.WriteLine ("Position Longitude: {0}", position.Longitude);

			var map = new Map(
				MapSpan.FromCenterAndRadius(
					new Position(eve.Latitude,eve.Longitude), Distance.FromMiles(0.1))) {
				IsShowingUser = true,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var pin = new Pin
			{
				Type = PinType.Place,
				//Position = new Position(eve.Latitude, eve.Longitude),
				Position = new Position(eve.Latitude,eve.Longitude),
				Label = eve.Title,
				Address = eve.PositionName,
			};

			map.Pins.Add (pin);



			mapLayout.Children.Add(map);
		}

		public static double distance(double lat1, double lon1, double lat2, double lon2) {
			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 60 * 1.1515;
			//if (unit == 'K') {
				dist = dist * 1.609344;
			//} else if (unit == 'N') {
			//	dist = dist * 0.8684;
			//}
			return (dist);
		}

		private static double deg2rad(double deg) {
			return (deg * Math.PI / 180.0);
		}

		private static double rad2deg(double rad) {
			return (rad / Math.PI * 180.0);
		}
	}
}

