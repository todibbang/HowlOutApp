using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace HowlOut
{
	public class OrganizationApiManager
	{
		private HttpClient httpClient;

		public OrganizationApiManager(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task<Organization> GetOrganization(string id)
		{
			var uri = "/" + id;
			List<Organization> organizations = await GetOrganizationServerCall(uri);
			return organizations[0];
		}

		public async Task<List<Organization>> GetOrganizationsFromName(string name)
		{
			var uri = "/organizationsFromName/" + name;
			return await GetOrganizationServerCall(uri);
		}

		public async Task<Organization> CreateEditOrganization(Organization grp)
		{
			var uri = "/" + App.StoredUserFacebookId;
			var content = JsonConvert.SerializeObject(grp);
			List<Organization> organizations = await PostOrganizationServerCall(uri, content);
			if (organizations != null && organizations.Count > 0)
			{
				return organizations[0];
			}
			return null;
		}

		public async Task<bool> DeleteOrganization(string id)
		{
			var uri = new Uri(App.serverUri + "organization/" + id);
			try
			{
				var response = await httpClient.DeleteAsync(uri);
				if (response.IsSuccessStatusCode) { return true; }
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

		public async Task<bool> AcceptInviteDeclineLeaveOrganization(string OrgId, string ProId, OrganizationHandlingType handlingType)
		{
			var uri = "/acceptInviteDeclineLeaveOrganization?organizationId=" + OrgId + "&profileId=" + ProId + "&handlingType="+handlingType;
			App.coreView.GetLoggedInProfile();
			return await PutOrganizationServerCall(uri);
		}

		public enum OrganizationHandlingType
		{
			Invite, Decline, Accept, Leave
		}

		public async Task<bool> TrackOrganization(string OrgId, bool followUnfollow)
		{
			var uri = "/track?organizationId=" + OrgId + "&profileId=" + App.StoredUserFacebookId + "&follow=" + followUnfollow;
			return await PutOrganizationServerCall(uri);
		}

		public async Task<List<Organization>> GetOrganizationServerCall(string uri)
		{
			List<Organization> organizations = new List<Organization>();
			var recievedContent = "";
			try
			{
				var response = await httpClient.GetAsync(new Uri(App.serverUri + "organization" + uri));
				if (response.IsSuccessStatusCode) 
				{
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						organizations = JsonConvert.DeserializeObject<List<Organization>>(recievedContent);
					}
					catch (Exception ex)
					{
						Organization org = JsonConvert.DeserializeObject<Organization>(recievedContent);
						organizations.Add(org);
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
			return organizations;
		}

		public async Task<bool> PutOrganizationServerCall(string uri)
		{
			try
			{
				var response = await httpClient.PutAsync(new Uri(App.serverUri + "organization" + uri), new StringContent(""));
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

		public async Task<List<Organization>> PostOrganizationServerCall(string uri, string content)
		{
			List<Organization> organizations = new List<Organization>();
			var recievedContent = "";
			try
			{
				var response = await httpClient.PostAsync(new Uri(App.serverUri + "organization" + uri), new StringContent(content, Encoding.UTF8, "application/json"));
				if (response.IsSuccessStatusCode)
				{
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						organizations = JsonConvert.DeserializeObject<List<Organization>>(recievedContent);
					}
					catch (Exception ex)
					{
						Organization org = JsonConvert.DeserializeObject<Organization>(recievedContent);
						organizations.Add(org);
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
			return organizations;
		}

	}
}

