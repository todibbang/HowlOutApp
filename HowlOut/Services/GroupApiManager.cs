using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace HowlOut
{
	public class GroupApiManager
	{
		private HttpClient httpClient;

		public GroupApiManager ()
		{
			this.httpClient = httpClient;
		}

		public async Task<ObservableCollection<Group>> GetAllGroups()
		{
			ObservableCollection<Group> groups = new ObservableCollection<Group>(); 

			var uri = new Uri("https://howlout.gear.host/api/GroupApi/GetGroups");

			try { 
				var response = await httpClient.GetAsync(uri);
				if(response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					groups = JsonConvert.DeserializeObject<ObservableCollection<Group>>(content);
				}
			} catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return groups;
		}

		public async Task<Group> GetGroupById(string groupId)
		{
			Group groupToRetrieve = new Group();

			var uri = new Uri("https://howlout.gear.host/api/GroupApi/" + groupId);

			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					groupToRetrieve = JsonConvert.DeserializeObject<Group>(content);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return groupToRetrieve;
		}

		public async Task<bool> UpdateGroup(Group groupToUpdate)
		{
			if(groupToUpdate.GroupId != null && groupToUpdate.GroupId != "")
			{
				var uri = new Uri("https://howlout.gear.host/api/GroupApi/"+groupToUpdate.GroupId);

				try
				{
					var json = JsonConvert.SerializeObject(groupToUpdate);
					var content = new StringContent(json, Encoding.UTF8, "application/json");

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
			}

			return false;
		}

		public async Task<Group> CreateGroup(GroupDBO groupToCreate)
		{
			Group groupReturned = new Group ();
			var uri = new Uri("https://howlout.gear.host/api/GroupApi");

			try
			{
				var json = JsonConvert.SerializeObject(groupToCreate);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					groupReturned = JsonConvert.DeserializeObject<Group>(recievedContent);
					return groupReturned;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failing");
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return null;
		}

		public async Task<bool> DeleteGroup(string groupId)
		{
			var uri = new Uri("https://howlout.gear.host/api/GroupApi/"+groupId);

			try
			{
				var response = await httpClient.DeleteAsync(uri);

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

		public async Task<bool> InviteToGroup(string groupId, string profileId)
		{

			var uri = new Uri("https://howlout.gear.host/api/GroupApi/InviteToGroup?groupId="+groupId+"&profileId="+profileId);

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

		public async Task<bool> JoinGroup(string groupId, string profileId)
		{

			var uri = new Uri("https://howlout.gear.host/api/GroupApi/JoinGroup?groupId="+groupId+"&profileId="+profileId);

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

		public async Task<bool> DeclineGroupInvite(string groupId, string profileId)
		{

			var uri = new Uri("https://howlout.gear.host/api/GroupApi/DeclineGroupInvite?groupId="+groupId+"&profileId="+profileId);

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

		public async Task<bool> LeaveGroup(string groupId, string profileId)
		{

			var uri = new Uri("https://howlout.gear.host/api/GroupApi/LeaveGroup?groupId="+groupId+"&profileId="+profileId);

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

