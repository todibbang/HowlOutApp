﻿using System;
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

		public async Task<bool> CreateProfile(Profile profile)
		{
			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI");

			try
			{
				System.Diagnostics.Debug.WriteLine("Trying");
				var json = JsonConvert.SerializeObject(profile);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					System.Diagnostics.Debug.WriteLine("Success");
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failing");
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return false;
		}

		public async Task<bool> SetSearchSettings(string profileId, SearchSettings searchSettings)
		{
			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/SearchReference/"+profileId);

			try
			{
				var json = JsonConvert.SerializeObject(searchSettings);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failing");
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return false;
		}

		public async Task<List<Profile>> GetProfilesFromName(string name)
		{
			List<Profile> profiles = new List<Profile>(); 

			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/GetFromName/" + name);

			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					profiles = JsonConvert.DeserializeObject<List<Profile>>(content);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return profiles;
		}

		public async Task<Profile> GetProfile(string profileId)
		{
			Profile profile = new Profile();

			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/" + profileId);

			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					profile = JsonConvert.DeserializeObject<Profile>(content);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return profile;
		}

		public async Task<ObservableCollection<Event>> GetEventsInvitedTo()
		{
			string profileId = App.userProfile.ProfileId;
			ObservableCollection<Event> events = new ObservableCollection<Event>();

			//4/10/2016 7:12:43 PM - virker !! 
			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/GetEventsInvitedTo/" + profileId + "?currentTime="+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					events = JsonConvert.DeserializeObject<ObservableCollection<Event>>(content);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return events;
		}

		public async Task<ObservableCollection<Event>> GetEventsFollowed()
		{
			string profileId = App.userProfile.ProfileId;

			ObservableCollection<Event> events = new ObservableCollection<Event>();

			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/GetEventsFollowed/" + profileId + "?currentTime="+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					events = JsonConvert.DeserializeObject<ObservableCollection<Event>>(content);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return events;
		}

		public async Task<Profile> GetLoggedInProfile(string profileId)
		{
			Profile profile = new Profile();

			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/Me?profileId=" + profileId);

			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					profile = JsonConvert.DeserializeObject<Profile>(content);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return profile;
		}

		public async Task<bool> RequestFriend(string profileRequestToId, string profileRequestFromId)
		{
			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/RequestFriend?profileRequestToId="+profileRequestToId+"&profileRequestFromId="+profileRequestFromId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return false;
		}

		public async Task<bool> AcceptFriend(string profileAcceptedId, string profileRequestedId)
		{
			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/AcceptFriend?profileAcceptedId="+profileAcceptedId+"&profileRequestedId="+profileRequestedId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return false;
		}

		public async Task<bool> DeclineFriendRequest(string profileRequestToId, string profileRequestFromId)
		{
			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/DeclineFriendRequest?profileRequestToId="+profileRequestToId+"&profileRequestFromId="+profileRequestFromId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return false;
		}

		public async Task<bool> RemoveFriend(string profileRequestToId, string profileRequestFromId)
		{
			var uri = new Uri("https://www.howlout.net/api/ProfilesAPI/RemoveFriend?profileRequestToId="+profileRequestToId+"&profileRequestFromId="+profileRequestFromId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return false;
		}
	}
}

