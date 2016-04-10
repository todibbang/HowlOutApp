using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace HowlOut
{
	public class GroupApiManager
	{
		private HttpClient httpClient;

		public GroupApiManager (HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task<List<Group>> GetAllGroups()
		{
			List<Group> groups = new List<Group>(); 

			var uri = new Uri("https://www.howlout.net/api/GroupApi/GetGroups");

			try { 
				var response = await httpClient.GetAsync(uri);
				if(response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					groups = JsonConvert.DeserializeObject<List<Group>>(content);
				}
			} catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return groups;
		}

		public async Task<List<Group>> GetGroupsFromName(string name)
		{
			List<Group> groups = new List<Group>(); 

			var uri = new Uri("https://www.howlout.net/api/GroupApi/GetFromName/"+name);

			try { 
				var response = await httpClient.GetAsync(uri);
				if(response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					groups = JsonConvert.DeserializeObject<List<Group>>(content);
				}
			} catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return groups;
		}

		public async Task<List<Comment>> GetGroupComments(string groupId)
		{
			List<Comment> comments = new List<Comment>(); 

			var uri = new Uri("https://www.howlout.net/api/GroupApi/Comment/"+groupId);

			try { 
				var response = await httpClient.GetAsync(uri);
				if(response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					comments = JsonConvert.DeserializeObject<List<Comment>>(content);
				}
			} catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return comments;
		}

		public async Task<List<Comment>> AddCommentToGroup(string groupId, Comment comment)
		{
			var uri = new Uri("https://www.howlout.net/api/GroupApi/Comment/"+groupId);

			try
			{
				var json = JsonConvert.SerializeObject(comment);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await httpClient.PostAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					var retrievedEvent = JsonConvert.DeserializeObject<List<Comment>>(recievedContent);
					return retrievedEvent;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return null;
		}

		public async Task<Group> GetGroupById(string groupId)
		{
			Group groupToRetrieve = new Group();

			var uri = new Uri("https://www.howlout.net/api/GroupApi/" + groupId);

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
				var uri = new Uri("https://www.howlout.net/api/GroupApi/"+groupToUpdate.GroupId);

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
			var uri = new Uri("https://www.howlout.net/api/GroupApi");

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
			var uri = new Uri("https://www.howlout.net/api/GroupApi/"+groupId);

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

		public async Task<bool> InviteToGroup(string groupId, List<string> profileIds)
		{
			var profileIdsAsString = "";


			for (int i = 0; i < profileIds.Count; i++) 
			{
				profileIdsAsString += "&profileIds=" + profileIds[i];
			}

			var uri = new Uri("https://www.howlout.net/api/GroupApi/InviteToGroup?groupId="+groupId+profileIdsAsString);

			try
			{
				var json = JsonConvert.SerializeObject("");
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
			return false;
		}

		public async Task<bool> JoinGroup(string groupId)
		{
			string profileId = App.userProfile.ProfileId;
			var uri = new Uri("https://www.howlout.net/api/GroupApi/JoinGroup?groupId="+groupId+"&profileId="+profileId);

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

			var uri = new Uri("https://www.howlout.net/api/GroupApi/DeclineGroupInvite?groupId="+groupId+"&profileId="+profileId);

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

			var uri = new Uri("https://www.howlout.net/api/GroupApi/LeaveGroup?groupId="+groupId+"&profileId="+profileId);

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

