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

		public async Task<ObservableCollection<Event>> GetAllEvents()
		{
			ObservableCollection<Event> events = new ObservableCollection<Event>(); 

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/WithoutOwner/" + App.StoredUserFacebookId);

			try { 
				var response = await httpClient.GetAsync(uri);
				if(response.IsSuccessStatusCode)
				{
					System.Diagnostics.Debug.WriteLine("RECIEVED DATA!!!!!");
					var content = await response.Content.ReadAsStringAsync();
					events = JsonConvert.DeserializeObject<ObservableCollection<Event>>(content);
				}
			} catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("COULD NOT RECIEVE DATA!!!!!");
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return events;
		}

		public async Task<ObservableCollection<Event>> GetEventsWithOwnerId()
		{
			ObservableCollection<Event> events = new ObservableCollection<Event>();

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/Owner/" + App.StoredUserFacebookId+"?currentTime="+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

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

		public async Task<Event> GetEventById(string eventId)
		{
			Event eventToRetrieve = new Event();

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/" + eventId);

			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					eventToRetrieve = JsonConvert.DeserializeObject<Event>(content);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return eventToRetrieve;
		}

		public async Task<ObservableCollection<Comment>> GetEventComments(string eventId)
		{
			ObservableCollection<Comment> comments = new ObservableCollection<Comment>(); 

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/Comments/" + eventId);

			try { 
				var response = await httpClient.GetAsync(uri);
				if(response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					comments = JsonConvert.DeserializeObject<ObservableCollection<Comment>>(content);
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

		public async Task<ObservableCollection<Event>> SearchEvents()
		{
			string profileId = App.userProfile.ProfileId;
			double userLat = App.lastKnownPosition.Latitude;
			double userLong = App.lastKnownPosition.Longitude;


			ObservableCollection<Event> events = new ObservableCollection<Event>();

			var lat = userLat.ToString();
			var lon = userLong.ToString();

			string newLat = "";
			string newLon = "";

			//Regex.Replace(lat, "\\,", "\\.");
			//Regex.Replace(lon, "\\,", "\\.");

			//lat.Replace ("\\,", "\\.");
			//lon.Replace ("\\,", "\\.");

			for (int i = 0; i < lat.Length; i++) {
				if (lat [i].Equals ("\\,")) {
					newLat += "\\.";
				} else {
					newLat += lat [i];
				}
			}

			for (int i = 0; i < lon.Length; i++) {
				if (lon [i].Equals ("\\,")) {
					newLon += "\\.";
				} else {
					newLon += lon [i];
				}
			}

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/SearchEvent?profileId=" + profileId + 
				"&userLat="+newLat + "&userLong=" + newLon + "&currentTime="+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US")));


			if (lat.Contains (",")) {
				var newLatSplit = Regex.Split (newLat, "\\,");
				var newLonSplit = Regex.Split (newLon, "\\,");

				uri = new Uri("https://www.howlout.net/api/EventsAPI/SearchEvent?profileId=" + profileId + 
					"&userLat="+ newLatSplit[0] + "." + newLatSplit[1] + "&userLong=" + 
					newLonSplit[0] + "." + newLonSplit[1] + "&currentTime="+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", new CultureInfo("en-US")));
			} 
				

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
					if(eventToRetrieve.StartDate > DateTime.Now.AddHours(2).AddMinutes(5))
					{
						CrossLocalNotifications.Current.Show ("Event: " + eventToRetrieve.Title, "Is starting in 2 hours!", int.Parse(eventId), eventToRetrieve.StartDate.ToLocalTime().AddHours(-4));	
					}
					if(eventToRetrieve.StartDate > DateTime.Now.AddDays(1).AddMinutes(5))
					{
						CrossLocalNotifications.Current.Show ("Event: " + eventToRetrieve.Title, "Is starting in 1 day!", int.Parse(eventId + 1), eventToRetrieve.StartDate.ToLocalTime().AddHours(-2).AddDays(-1));	
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
					if(eventToRetrieve.StartDate.CompareTo(DateTime.Now.AddHours(2).AddMinutes(5)) > 0)
					{
						CrossLocalNotifications.Current.Show ("Event: " + eventToRetrieve.Title, "Is starting in 2 hours!", int.Parse(eventId), eventToRetrieve.StartDate.AddHours(-2));	
					}
					if(eventToRetrieve.StartDate.CompareTo(DateTime.Now.AddDays(1).AddMinutes(5)) > 0)
					{
						CrossLocalNotifications.Current.Show ("Event: " + eventToRetrieve.Title, "Is starting in 1 day!", int.Parse(eventId), eventToRetrieve.StartDate.AddDays(-1));	
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
	}
}

