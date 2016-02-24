using System;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public class UtilityManager
	{
		public UtilityManager ()
		{
		}



		public async Task getCurrentUserPosition(Position pos)
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;

			var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);

			pos = new Position (position.Latitude, position.Longitude);


		}

		public async void setMapForEvent(Event eve, ExtMap map, StackLayout mapLayout)
		{
			System.Diagnostics.Debug.WriteLine ("testPhase: " + eve.Latitude + ", " + eve.Longitude);

			map.MoveToRegion (
				MapSpan.FromCenterAndRadius (
					new Position (eve.Latitude, eve.Longitude), Distance.FromKilometers (0.5)));


			var pin = new Pin
			{
				Type = PinType.Place,
				Position = new Position(eve.Latitude,eve.Longitude),
				Label = eve.Title,
				Address = eve.PositionName,
			};

			map.Pins.Add (pin);
			mapLayout.Children.Add(map);
		}

		public double distance(double lat1, double lon1, double lat2, double lon2) {
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

		private double deg2rad(double deg) {
			return (deg * Math.PI / 180.0);
		}

		private double rad2deg(double rad) {
			return (rad / Math.PI * 180.0);
		}
	}
}

