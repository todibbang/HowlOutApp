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

		public async Task<List<Event>> SearchEvents()
		{
			string profileId = App.userProfile.ProfileId;
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

			var uri = "/search?profileId=" + profileId +
				"&userLat=" + newLat + "&userLong=" + newLon + "&currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));

			if (lat.Contains(",")) {
				var newLatSplit = Regex.Split(newLat, "\\,");
				var newLonSplit = Regex.Split(newLon, "\\,");
				uri = "/search?profileId=" + profileId +
					"&userLat=" + newLatSplit[0] + "." + newLatSplit[1] + "&userLong=" +
					newLonSplit[0] + "." + newLonSplit[1] + "&currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
			}
			events = await GetEventsServerCall(uri);
			return events;
		}

		public async Task<List<Event>> GetEventsAttending()
		{
			var uri ="/owner/" + App.StoredUserFacebookId + "?currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
			return await GetEventsServerCall(uri);
		}

		public async Task<Event> GetEventById(string eventId)
		{
			var uri = "/" + eventId;
			List<Event> events = await GetEventsServerCall(uri);
			return events[0];
		}

		public async Task<List<Event>> GetEventsProfilesAttending(bool joinedOrFollowing, List<Profile> profiles)
		{
			var uri = "/eventsFromProfileIds?joined=" + joinedOrFollowing + "&currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
			for (int i = 0; i < profiles.Count; i++) {
				uri += "&profileIds=" + profiles[i].ProfileId;
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

		public async Task<List<Event>> GetEventsForOrgs(List<Organization> orgs)
		{
			if (orgs != null && orgs.Count > 0)
			{
				var uri = "/eventsFromOrganizationIds?currentTime=" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
				for (int i = 0; i < orgs.Count; i++)
				{
					uri += "&organizationIds=" + orgs[i].OrganizationId;
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
					CrossLocalNotifications.Current.Show("Event: " + events[0].Title, events[0].Title + " is starting in 2 hours!", int.Parse(events[0].EventId), events[0].StartDate.ToLocalTime().AddHours(-2));
				}
				if (events[0].StartDate > DateTime.Now.AddDays(1).AddMinutes(1))
				{
					CrossLocalNotifications.Current.Show("Event: " + events[0].Title, events[0].Title + " is starting in 1 day!", int.Parse(events[0].EventId + 1), events[0].StartDate.ToLocalTime().AddDays(-1));
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



		/*
		public async Task<bool> GetEventsWithOwnerId(String id)
		{
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/Owner/" + id + "?currentTime="+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US")));
			return await GetEventServerCall(uri);
		}



		public async Task<List<Comment>> GetEventComments(string eventId)
		{
			List<Comment> comments = new List<Comment>(); 

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/Comments/" + eventId);

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

		public async Task<bool> UpdateEvent(Event EventToUpdate)
		{
			if(EventToUpdate.EventId != null && EventToUpdate.EventId != "")
			{
				var uri = new Uri("https://www.howlout.net/api/EventsAPI/"+EventToUpdate.EventId);

				try
				{
					var json = JsonConvert.SerializeObject(EventToUpdate);
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

		public async Task<Event> CreateEvent(EventDBO eventToCreate)
		{
			Event eventToRetrieve = new Event ();
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/");

			try
			{
				System.Diagnostics.Debug.WriteLine("Trying");
				var json = JsonConvert.SerializeObject(eventToCreate);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync(uri, content);

				System.Diagnostics.Debug.WriteLine("Suchcess ??? " + response.IsSuccessStatusCode);

				if (response.IsSuccessStatusCode)
				{
					System.Diagnostics.Debug.WriteLine("Ass ??? ");
					var recievedContent = await response.Content.ReadAsStringAsync();
					eventToRetrieve = JsonConvert.DeserializeObject<Event>(recievedContent);
					System.Diagnostics.Debug.WriteLine("New Event ID: " + eventToRetrieve.EventId);

					if (eventToRetrieve.StartDate > DateTime.Now.AddHours(2).AddMinutes(1))
					{
						CrossLocalNotifications.Current.Show("Event: " + eventToRetrieve.Title, eventToRetrieve.Title + " is starting in 2 hours!", int.Parse(eventToRetrieve.EventId), eventToRetrieve.StartDate.ToLocalTime().AddHours(-2));
					}
					if (eventToRetrieve.StartDate > DateTime.Now.AddDays(1).AddMinutes(1))
					{
						CrossLocalNotifications.Current.Show("Event: " + eventToRetrieve.Title, eventToRetrieve.Title + " is starting in 1 day!", int.Parse(eventToRetrieve.EventId + 1), eventToRetrieve.StartDate.ToLocalTime().AddDays(-1));
					}

					return eventToRetrieve;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failing");
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return null;
		}

		public async Task<bool> DeleteEvent(string eventId)
		{
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/"+eventId);

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



		public async Task<bool> FollowEvent(string eventId, string profileId)
		{
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/FollowEvent/" + eventId + "/" + profileId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					var eventToRetrieve = JsonConvert.DeserializeObject<Event>(recievedContent);
					//if(eventToRetrieve.StartDate.CompareTo(DateTime.Now.AddHours(2).AddMinutes(5)) > 0)

					if (eventToRetrieve.StartDate > DateTime.Now.AddHours(2).AddMinutes(1))
					{
						CrossLocalNotifications.Current.Show("Event: " + eventToRetrieve.Title, eventToRetrieve.Title + " is starting in 2 hours!", int.Parse(eventToRetrieve.EventId), eventToRetrieve.StartDate.ToLocalTime().AddHours(-2));
					}
					if (eventToRetrieve.StartDate > DateTime.Now.AddDays(1).AddMinutes(1))
					{
						CrossLocalNotifications.Current.Show("Event: " + eventToRetrieve.Title, eventToRetrieve.Title + " is starting in 1 day!", int.Parse(eventToRetrieve.EventId + 1), eventToRetrieve.StartDate.ToLocalTime().AddDays(-1));
					}


					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return false;
		}

		public async Task<bool> UnfollowEvent(string eventId, string profileId)
		{
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/UnfollowEvent/" + eventId + "/" + profileId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					var eventToRetrieve = JsonConvert.DeserializeObject<Event>(recievedContent);
					CrossLocalNotifications.Current.Cancel(int.Parse(eventToRetrieve.EventId));
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return false;
		}

		public async Task<bool> AttendEvent(string eventId, string profileId)
		{
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/AttendEvent/" + eventId + "/" + profileId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					var eventToRetrieve = JsonConvert.DeserializeObject<Event>(recievedContent);
					if (eventToRetrieve.StartDate > DateTime.Now.AddHours(2).AddMinutes(1))
					{
						CrossLocalNotifications.Current.Show("Event: " + eventToRetrieve.Title, eventToRetrieve.Title + " is starting in 2 hours!", int.Parse(eventToRetrieve.EventId), eventToRetrieve.StartDate.ToLocalTime().AddHours(-2));
					}
					if (eventToRetrieve.StartDate > DateTime.Now.AddDays(1).AddMinutes(1))
					{
						CrossLocalNotifications.Current.Show("Event: " + eventToRetrieve.Title, eventToRetrieve.Title + " is starting in 1 day!", int.Parse(eventToRetrieve.EventId + 1), eventToRetrieve.StartDate.ToLocalTime().AddDays(-1));
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return false;
		}

		public async Task<bool> UnattendEvent(string eventId, string profileId)
		{
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/UnattendEvent/" + eventId + "/" + profileId);

			try
			{
				var content = new StringContent("");
				var response = await httpClient.PutAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					var eventToRetrieve = JsonConvert.DeserializeObject<Event>(recievedContent);
					CrossLocalNotifications.Current.Cancel(int.Parse(eventToRetrieve.EventId));
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return false;
		}

		public async Task<List<Comment>> AddCommentToEvent(string eventId, Comment comment)
		{
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/Comment/"+eventId);

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

		public async Task<bool> InviteToEvent(string eventId, List<string> profileIds)
		{
			var profileIdsAsString = "";

			for (int i = 0; i < profileIds.Count; i++) 
			{
				if (i == 0) 
				{
					profileIdsAsString += "?";
				} 
				else 
				{
					profileIdsAsString += "&";
				}
				profileIdsAsString += "profileIds=" + profileIds[i];
			}

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/InviteToEvent/" + eventId + profileIdsAsString);

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

		public async Task<bool> DeclineEventInvite(string eventId, string profileId)
		{

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/DeclineEventInvite?eventId="+eventId+"&profileId="+profileId);

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



		public async Task<bool> AcceptJoinRequest(string profileId, bool accept)
		{
			
			return true;
		}
		*/



	}
}

