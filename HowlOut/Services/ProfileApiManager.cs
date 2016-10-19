using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace HowlOut
{
	public class ProfileApiManager
	{
		private HttpClient httpClient;

		public ProfileApiManager (HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task<Profile> GetProfile(string id)
		{
			var uri = "/"+id;
			List<Profile> profiles = await GetProfilesServerCall(uri);
			return profiles[0];
		}

		public async Task<Profile> GetLoggedInProfile()
		{
			var uri = "/me?profileId=" + App.StoredUserFacebookId;
			List<Profile> profiles = await GetProfilesServerCall(uri);
			return profiles[0];
		}

		public async Task<List<Profile>> GetProfilesFromName(string name)
		{
			var uri = "/profilesFromName/" + name;
			return await GetProfilesServerCall(uri);
		}


		public async Task<bool> CreateUpdateProfile(Profile pro, bool create)
		{
			var uri = "?create="+create;
			Profile newPro = new Profile() { Name = pro.Name, Description = pro.Description, ProfileId = pro.ProfileId, ImageSource = pro.ImageSource };
			var content = JsonConvert.SerializeObject(newPro);
			var recievedContent = "";
			try
			{
				var response = await httpClient.PostAsync(new Uri(App.serverUri + "profile" + uri), new StringContent(content, Encoding.UTF8, "application/json"));
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return false;
		}

		public async Task<bool> updateSearchSettings(SearchSettings settings)
		{
			var uri = "/searchPreference/" + App.StoredUserFacebookId;
			var content = JsonConvert.SerializeObject(settings);
			List<Profile> profiles = await PostProfileServerCall(uri, content);
			return true;
		}

		public async Task<bool> RequestDeclineAcceptUnfriend(string profileFriendId, bool acceptOrDecline)
		{
			var uri = "/requestDeclineAcceptUnfriend?profileSignedInId=" + App.StoredUserFacebookId + "&profileFriendId=" + profileFriendId + "&acceptOrRequest=" + acceptOrDecline;
			return await PutProfileServerCall(uri);
		}

		public async Task<List<Profile>> GetProfilesServerCall(string uri)
		{
			List<Profile> profiles = new List<Profile>();
			var recievedContent = "";
			try {
				var response = await httpClient.GetAsync(new Uri(App.serverUri + "profile" + uri));
				if (response.IsSuccessStatusCode)
				{
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						return profiles = JsonConvert.DeserializeObject<List<Profile>>(recievedContent);
					}
					catch (Exception ex)
					{
						Profile pro = JsonConvert.DeserializeObject<Profile>(recievedContent);
						profiles.Add(pro);
					}
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return profiles;
		}

		public async Task<bool> PutProfileServerCall(string uri)
		{
			try {
				var response = await httpClient.PutAsync(new Uri(App.serverUri + "profile" + uri), new StringContent(""));
				if (response.IsSuccessStatusCode) 
				{ 
					return true; 
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex) 
			{ 
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message); 
			}
			return false;
		}

		public async Task<List<Profile>> PostProfileServerCall(string uri, string content)
		{
			List<Profile> profiles = new List<Profile>();
			var recievedContent = "";
			try {
				var response = await httpClient.PostAsync(new Uri(App.serverUri + "profile" + uri), new StringContent(content, Encoding.UTF8, "application/json"));
				if (response.IsSuccessStatusCode) 
				{ 
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						return profiles = JsonConvert.DeserializeObject<List<Profile>>(recievedContent);
					}
					catch (Exception ex)
					{
						Profile pro = JsonConvert.DeserializeObject<Profile>(recievedContent);
						profiles.Add(pro);
					}
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return profiles;
		}
	}
}

