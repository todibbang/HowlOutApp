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

		public async Task<Group> GetGroup(string id)
		{
			var uri = "/"+id;
			List<Group> groups = await GetGroupServerCall(uri);
			return groups[0];
		}

		public async Task<List<Group>> GetGroupsFromName(string name)
		{
			var uri = "/groupsFromName/" + name;
			return await GetGroupServerCall(uri);
		}


		public async Task<Group> CreateEditGroup(Group grp)
		{
			var uri = "";
			var content = JsonConvert.SerializeObject(grp);
			List<Group> groups = await PostGroupServerCall(uri, content);
			return groups[0];
		}

		public async Task<bool> DeleteGroup(string eventId)
		{
			var uri = new Uri(App.serverUri + "group/" + eventId);
			try
			{
				var response = await httpClient.DeleteAsync(uri);
				if (response.IsSuccessStatusCode) 
				{ 
					return true; 
				}
				else { 
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK"); 
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return false;
		}

		public async Task<bool> InviteAcceptDeclineLeaveGroup(string id, List<Profile> profiles, GroupHandlingType handlingType)
		{
			var uri = "/inviteAcceptDeclineLeaveGroup?groupId=" + id;
			for (int i = 0; i < profiles.Count; i++)
			{
				uri += "&profileIds=" + profiles[i].ProfileId;
			}
			uri += "&handlingType="+handlingType;
			return await PutGroupServerCall(uri);
		}

		public async Task<List<Group>> GetGroupServerCall(string uri)
		{
			List<Group> groups = new List<Group>();
			var recievedContent = "";
			try
			{
				var response = await httpClient.GetAsync(new Uri(App.serverUri + "group" + uri));
				if (response.IsSuccessStatusCode)
				{
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						groups = JsonConvert.DeserializeObject<List<Group>>(recievedContent);
					}
					catch (Exception ex)
					{
						Group grp = JsonConvert.DeserializeObject<Group>(recievedContent);
						groups.Add(grp);
					}
				}
				else {
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return groups;
		}

		public async Task<bool> PutGroupServerCall(string uri)
		{
			try
			{
				var response = await httpClient.PutAsync(new Uri(App.serverUri + "group" + uri), new StringContent(""));
				if (response.IsSuccessStatusCode) 
				{ 
					return true; 
				}
				else { 
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK"); 
				}
			}
			catch (Exception ex) 
			{ 
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message); 
			}
			return false;
		}

		public async Task<List<Group>> PostGroupServerCall(string uri, string content)
		{
			List<Group> groups = new List<Group>();
			var recievedContent = "";
			try
			{
				var response = await httpClient.PostAsync(new Uri(App.serverUri + "group" + uri), new StringContent(content, Encoding.UTF8, "application/json"));
				if (response.IsSuccessStatusCode) {
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						groups = JsonConvert.DeserializeObject<List<Group>>(recievedContent);
					} 
					catch (Exception ex)
					{
						Group grp = JsonConvert.DeserializeObject<Group>(recievedContent);
						groups.Add(grp);
					}
				}
				else { 
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK"); 
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return groups;
		}

		public enum GroupHandlingType
		{
			Invite, Decline, Accept, Leave, Request
		}
	}
}

