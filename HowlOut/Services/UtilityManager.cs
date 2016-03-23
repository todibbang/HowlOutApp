using System;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;
using System.Collections.Generic;

namespace HowlOut
{
	public class UtilityManager
	{
		HttpClient httpClient;
		ProfileApiManager profileManager;
		EventApiManager eventApiManager;
		GroupApiManager groupManager;

		public UtilityManager ()
		{
			httpClient = new HttpClient(new NativeMessageHandler());
			profileManager = new ProfileApiManager (httpClient);
			eventApiManager= new EventApiManager (httpClient);
			groupManager = new GroupApiManager (httpClient);
		}

		public async void updateLastKnownPosition()
		{
			if (CrossGeolocator.Current.IsGeolocationEnabled && CrossGeolocator.Current.IsGeolocationAvailable) {
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;
				var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);
				App.lastKnownPosition = new Position (position.Latitude, position.Longitude);
				System.Diagnostics.Debug.WriteLine ("Last Known Position: " + App.lastKnownPosition.Latitude + "" + App.lastKnownPosition.Longitude);
			}
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
			map.Pins.Clear ();
			var pin = new Pin
			{
				Type = PinType.Place,
				Position = new Position(pos.Latitude,pos.Longitude),
				Label = label,
				Address = address,
			};
			map.Pins.Add (pin);
		}

		public string distance(Position position1, Position position2) {

			double lat1 = position1.Latitude;
			double lon1 = position1.Longitude;
			double lat2 = position2.Latitude;
			double lon2 = position2.Longitude;
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

			/*
			string distance = "";
			if (dist < 1)
				distance = "less than 1 km away";
			else {
				int Dist = (int)dist;
				distance = Dist + " km away";
			}
			*/		


			return (((int)dist) + "");
		}

		private double deg2rad(double deg) {
			return (deg * Math.PI / 180.0);
		}

		private double rad2deg(double rad) {
			return (rad / Math.PI * 180.0);
		}

		public string getTime(DateTime time)
		{
			string hour = time.Hour.ToString();
			if (hour.Length == 1) {
				hour = "0" + time.Hour.ToString ();
			}

			string minute = time.Minute.ToString();
			if(minute.Length == 1) minute = "0" + time.Minute.ToString();

			string newTime = hour + ":" + minute;

			return newTime;
		}

		public List<string> setTime(DateTime time) {
			var theTimeNow = DateTime.Now;
			var timeBetween = time - theTimeNow;
			string number;
			string describer;

			if (timeBetween.TotalDays < 1) {
				number = timeBetween.Hours + "";
				describer = "hour";
			} else if (timeBetween.TotalDays < 7) {
				number = (time.TimeOfDay + "").Substring(0, 5);
				describer = (time.DayOfWeek + "").Substring(0, 3);
			} else {
				number = time.Day + "";
				describer = time.ToString("MMMM") + "";
			}

			return new List<string> () { number, describer };
		}


	}
}

