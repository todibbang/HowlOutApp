using System;
using System.Net.Http;
using ModernHttpClient;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Plugin.LocalNotifications;
using System.Text.RegularExpressions;
using System.Globalization;

namespace HowlOut
{
	public class EventApiManager
	{
		private HttpClient httpClient;

		public EventApiManager (HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task<List<Event>> SearchEvents(string searchWord)
		{
			double userLat = App.lastKnownPosition.Latitude;
			double userLong = App.lastKnownPosition.Longitude;
			List<Event> events = new List<Event>();

			var lat = userLat.ToString();
			var lon = userLong.ToString();
			string newLat = "";
			string newLon = "";

			for (int i = 0; i < lat.Length; i++) {
				if (lat[i].Equals("\\,")) { newLat += "\\."; }
				else { newLat += lat[i]; }
			}

			for (int i = 0; i < lon.Length; i++) {
				if (lon[i].Equals("\\,")) { newLon += "\\."; }
				else { newLon += lon[i]; }
			}

			var uri = "/search?userLat=" + newLat + "&userLong=" + newLon + "&currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));

			if (lat.Contains(",")) {
				var newLatSplit = Regex.Split(newLat, "\\,");
				var newLonSplit = Regex.Split(newLon, "\\,");
				uri = "/search?userLat=" + newLatSplit[0] + "." + newLatSplit[1] + "&userLong=" +
					newLonSplit[0] + "." + newLonSplit[1] + "&currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
			}

			if (!string.IsNullOrWhiteSpace(searchWord))
			{
				searchWord = searchWord.Replace("#", "%23");
				uri += "&searchWord=" + searchWord;
			}

			events = await GetEventsServerCall(uri);
			return events;
		}

		public async Task<List<Event>> GetEventsAttending()
		{
			var uri ="/owner/" + App.StoredUserFacebookId + "?currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
			return await GetEventsServerCall(uri);
		}

		public async Task<List<Event>> GetEndedEvents()
		{
			var uri = "/getEndedEvents?currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
			return await GetEventsServerCall(uri);
		}

		public async Task<Event> GetEventById(string eventId)
		{
			if (string.IsNullOrWhiteSpace(eventId)) return null;
			var uri = "/" + eventId;
			List<Event> events = await GetEventsServerCall(uri);
			if (events == null || events.Count == 0 || events[0] == null) return null;
			return events[0];
		}

		public async Task<List<Event>> GetEventsProfilesAttending(bool joinedOrFollowing, List<Profile> profiles)
		{
			var uri = "/eventsFromProfileIds?joined=" + joinedOrFollowing + "&currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
			for (int i = 0; i < profiles.Count; i++) {
				if(profiles[i] != null) uri += "&profileIds=" + profiles[i].ProfileId;
			}
			return await GetEventsServerCall(uri);
		}

		public async Task<List<Event>> GetEventsForGroups(List<Group> groups)
		{
			if (groups != null && groups.Count > 0)
			{
				var uri = "/eventsFromGroupIds?currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
				for (int i = 0; i < groups.Count; i++)
				{
					uri += "&groupIds=" + groups[i].GroupId;
				}
				return await GetEventsServerCall(uri);
			}
			return null;
		}

		public async Task<Event> CreateEditEvent(Event eventToCreate)
		{
			var uri = "";
			var content = JsonConvert.SerializeObject(eventToCreate);
			List<Event> events = await PostEventServerCall(uri, content);
			if (events != null && events.Count > 0)
			{
				if (events[0].StartDate > DateTime.Now.AddHours(2).AddMinutes(1))
				{
					CrossLocalNotifications.Current.Show("Event: " + events[0].Title, events[0].Title + " is starting in 2 hours!", (int.Parse(events[0].EventId) * 2), events[0].StartDate.ToLocalTime().AddHours(-2));
				}
				if (events[0].StartDate > DateTime.Now.AddDays(1).AddMinutes(1))
				{
					CrossLocalNotifications.Current.Show("Event: " + events[0].Title, events[0].Title + " is starting in 1 day!", (int.Parse(events[0].EventId) * 2) + 1, events[0].StartDate.ToLocalTime().AddDays(-1));
				}
				return events[0];
			}
			return null;
		}

		public async Task<bool> DeleteEvent(string eventId)
		{
			var uri = new Uri(App.serverUri + "event/" + eventId);
			try {
				var response = await httpClient.DeleteAsync(uri);
				if (response.IsSuccessStatusCode) 
				{ 
					return true; 
				}
				else { 
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK"); 
				}
			} catch (Exception ex) 
			{ 
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return false;
		}

		public async Task<bool> AttendOrTrackEvent(string eventId, bool attendOrLeave, bool joinOrUnjoin)
		{
			var uri = "/joinOrTrack/"+eventId+"/"+ App.StoredUserFacebookId + "?attend="+attendOrLeave + "&join="+joinOrUnjoin;
			return await PutEventServerCall(uri);
		}

		public async Task<bool> InviteProfilesToEvent(string eventId, List<Profile> profiles)
		{
			var uri = "/invite/"+eventId+"?profileIds=" + profiles[0].ProfileId;
			for (int i = 1; i < profiles.Count; i++) {
				uri += "&profileIds=" + profiles[i].ProfileId;
			}
			uri += "&profileSenderId=" + App.StoredUserFacebookId;
			return await PutEventServerCall(uri);
		}

		public async Task<bool> AcceptDeclineLeaveEventAsOwner(string id, OwnerHandlingType handlingType)
		{
			var uri = "/AcceptDeclineLeaveEventAsOwner?eventId=" + id + "&handlingType=" + handlingType;
			App.coreView.updateHomeView();
			return await PutEventServerCall(uri);
		}

		public async Task<bool> InviteToEventAsOwner(string id, List<Profile> profiles)
		{
			var uri = "/inviteToEventAsOwner?eventId=" + id;
			foreach (Profile p in profiles)
			{
				uri += "&profileIds=" + p.ProfileId;
			}
			App.coreView.updateHomeView();
			return await PutEventServerCall(uri);
		}

		public async Task<List<Event>> GetEventsServerCall(string uri)
		{
			List<Event> events = new List<Event>();
			var recievedContent = "";
			try
			{
				var response = await httpClient.GetAsync(new Uri(App.serverUri + "event" + uri));
				if (response.IsSuccessStatusCode) { 
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						events = JsonConvert.DeserializeObject<List<Event>>(recievedContent);
					}
					catch (Exception ex)
					{
						Event eve = JsonConvert.DeserializeObject<Event>(recievedContent);
						events.Add(eve);
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
			return events;
		}

		public async Task<bool> PutEventServerCall(string uri)
		{
			try
			{
				var response = await httpClient.PutAsync(new Uri(App.serverUri + "event" + uri), new StringContent(""));
				System.Diagnostics.Debug.WriteLine(await response.Content.ReadAsStringAsync());
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

		public async Task<List<Event>> PostEventServerCall(string uri, string content)
		{
			List<Event> events = new List<Event>();
			var recievedContent = "";
			try
			{
				var response = await httpClient.PostAsync(new Uri(App.serverUri + "event" + uri), new StringContent(content, Encoding.UTF8, "application/json"));
				System.Diagnostics.Debug.WriteLine(await response.Content.ReadAsStringAsync());
				if (response.IsSuccessStatusCode)
				{
					recievedContent = await response.Content.ReadAsStringAsync();
					try
					{
						events = JsonConvert.DeserializeObject<List<Event>>(recievedContent);
					}
					catch (Exception ex)
					{
						Event eve = JsonConvert.DeserializeObject<Event>(recievedContent);
						events.Add(eve);
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
			return events;
		}
	}
}

