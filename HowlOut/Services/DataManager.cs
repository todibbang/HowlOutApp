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
		private EventApiManager eventApiManager;
		private ProfileApiManager profileApiManager;

        public DataManager ()
        {
            httpClient = new HttpClient(new NativeMessageHandler());
			eventApiManager = new EventApiManager (httpClient);
			profileApiManager = new ProfileApiManager (httpClient);
        }

		public Uri GetFacebookProfileImageUri(string facebookUserId)
        {
			return new Uri("https://graph.facebook.com/v2.5/"+facebookUserId+"/picture?height=300&width=300");
        }

		public async Task update()
		{
			updateSearch();
			updateManage();
			updateProfile();
		}
		public async Task updateSearch() {
			//App.coreView.searchEventList = await GetAllEvents ();
			App.coreView.searchEvent.updateList (await eventApiManager.GetAllEvents ());
		}
		public async Task updateManage() {
			App.coreView.manageEventList = await eventApiManager.GetEventsWithOwnerId ();
			App.coreView.manageEvent.updateList ();
		}
		public async Task updateProfile() {
			App.userProfile = await profileApiManager.GetProfileId(App.userProfile.ProfileId);
		}

		public async Task<ObservableCollection<Address>> AutoCompletionPlace(string input)
		{
			string path = "http://dawa.aws.dk/autocomplete?q=" + input;
			ObservableCollection<Address> addresses = new ObservableCollection<Address>();

			using (var client = new HttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(new Uri(path));

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					addresses = JsonConvert.DeserializeObject<ObservableCollection<Address>>(content);

					for (int i = 0; i < addresses.Count; i++) {
						System.Diagnostics.Debug.WriteLine ("forslagstekst: " + addresses [i].forslagstekst + " " + addresses[i].data.href);
					}
				}
				return addresses;
			}
		}

		public async Task<Position> GetCoordinates(string input)
		{
			string path = input;

			string adgangspunkt = "";

			using (var client = new HttpClient())
			{
				System.Diagnostics.Debug.WriteLine ("Trying to get new coords");

				Position position = new Position();
				HttpResponseMessage response = await client.GetAsync(new Uri(path));

				if (response.IsSuccessStatusCode)
				{
					System.Diagnostics.Debug.WriteLine ("Success getting new coords");

					adgangspunkt = await response.Content.ReadAsStringAsync();
					//adgangspunkt = JsonConvert.DeserializeObject<string>(content);

					System.Diagnostics.Debug.WriteLine (adgangspunkt);
					var substrings = Regex.Split(adgangspunkt, "koordinater");

					position = new Position (Convert.ToDouble(substrings [1].Substring (35, 16)), Convert.ToDouble(substrings [1].Substring (11, 16)) );
					//position.Latitude = Convert.ToDouble(substrings [1].Substring (11, 16));
					//position.Longitude = Convert.ToDouble(substrings [1].Substring (35, 16));

				}
				return position;
			}
		}
    }
}
