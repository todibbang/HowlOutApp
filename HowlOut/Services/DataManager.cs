using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace HowlOut
{
	public class DataManager
	{
		private HttpClient httpClient;
		public EventApiManager EventApiManager { get; set; }
		public ProfileApiManager ProfileApiManager { get; set; }
		public GroupApiManager GroupApiManager { get; set; }
		public UtilityManager UtilityManager { get; set; }

		public DataManager ()
		{
			httpClient = new HttpClient (new NativeMessageHandler ());
			EventApiManager = new EventApiManager (httpClient);
			ProfileApiManager = new ProfileApiManager (httpClient);
			GroupApiManager = new GroupApiManager (httpClient);
			UtilityManager = new UtilityManager ();
		}

		public Uri GetFacebookProfileImageUri (string facebookUserId)
		{
			return new Uri ("https://graph.facebook.com/v2.5/" + facebookUserId + "/picture?height=300&width=300");
		}

		public async Task update ()
		{
			App.userProfile = await ProfileApiManager.GetLoggedInProfile (App.StoredUserFacebookId);
		}



		public async Task<ObservableCollection<Address>> AutoCompletionPlace (string input)
		{
			string path = "https://dawa.aws.dk/autocomplete?q=" + input;
			ObservableCollection<Address> addresses = new ObservableCollection<Address> ();

			try { 
				var response = await httpClient.GetAsync (new Uri (path));
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					addresses = JsonConvert.DeserializeObject<ObservableCollection<Address>> (content);

					for (int i = 0; i < addresses.Count; i++) {
						System.Diagnostics.Debug.WriteLine ("forslagstekst: " + addresses [i].forslagstekst + " " + addresses [i].data.href);
					}
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine ("COULD NOT RECIEVE DATA!!!!!");
				System.Diagnostics.Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}

			return addresses;

		}

		public async Task<Position> GetCoordinates (string input)
		{
			input = Regex.Replace (input, "http", "https");

			string path = input;

			string adgangspunkt = "";

			Position position = new Position ();

			try { 
				var response = await httpClient.GetAsync (new Uri (path));

				System.Diagnostics.Debug.WriteLine ("Success getting new coords");

				if (response.IsSuccessStatusCode) {
					System.Diagnostics.Debug.WriteLine ("Success getting new coords");

					adgangspunkt = await response.Content.ReadAsStringAsync ();
					//adgangspunkt = JsonConvert.DeserializeObject<string>(content);

					System.Diagnostics.Debug.WriteLine (adgangspunkt);
					var substrings = Regex.Split (adgangspunkt, "koordinater");

					position = new Position (Convert.ToDouble (substrings [1].Substring (35, 16)), Convert.ToDouble (substrings [1].Substring (11, 16)));
					//position.Latitude = Convert.ToDouble(substrings [1].Substring (11, 16));
					//position.Longitude = Convert.ToDouble(substrings [1].Substring (35, 16));
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine ("COULD NOT RECIEVE DATA!!!!!");
				System.Diagnostics.Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}


			return position;
		}

	}
}
