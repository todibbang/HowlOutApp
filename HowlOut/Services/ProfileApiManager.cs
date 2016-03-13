using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

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

