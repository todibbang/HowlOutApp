using System;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public class UtilityManager
	{
		public static Position lastKnownPosition = new Position();

		public UtilityManager ()
		{
		}

		public async void updateLastKnownPosition()
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;
			var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);
			lastKnownPosition = new Position (position.Latitude, position.Longitude);
			System.Diagnostics.Debug.WriteLine ("Last Known Position: " + lastKnownPosition.Latitude + "" + lastKnownPosition.Longitude);
		}

		public Position getCurrentUserPosition()
		{
			return lastKnownPosition;
		}

		public async void setMapForEvent(Position pos, ExtMap map, StackLayout mapLayout)
		{
			map.MoveToRegion (
				MapSpan.FromCenterAndRadius (
					new Position (pos.Latitude, pos.Longitude), Distance.FromKilometers (0.5)));
			mapLayout.Children.Add(map);
		}

		public async void setPin(Position pos, ExtMap map, String label, String address)
		{
			var pin = new Pin
			{
				Type = PinType.Place,
				Position = new Position(pos.Latitude,pos.Longitude),
				Label = label,
				Address = address,
			};
			map.Pins.Add (pin);
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

