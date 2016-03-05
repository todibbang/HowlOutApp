﻿using System;
using System.Net.Http;
using ModernHttpClient;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

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

			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/WithoutOwner/" + App.StoredUserFacebookId);

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

			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/Owner/" + App.StoredUserFacebookId);

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

			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/" + eventId);

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

		public async Task<bool> UpdateEvent(Event EventToUpdate)
		{
			if(EventToUpdate.EventId != null && EventToUpdate.EventId != "")
			{
				var uri = new Uri("https://howlout.gear.host/api/EventsAPI/"+EventToUpdate.EventId);

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
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/");

			try
			{
				System.Diagnostics.Debug.WriteLine("Trying");
				var json = JsonConvert.SerializeObject(eventToCreate);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await httpClient.PostAsync(uri, content);

				System.Diagnostics.Debug.WriteLine("Success ??? " + response.IsSuccessStatusCode);

				if (response.IsSuccessStatusCode)
				{
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
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/"+eventId);

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

		public async Task<ObservableCollection<Event>> SearchEvents(List<int> eventTypesId, Profile profile, double userLat, double userLong, double maxDistance)
		{
			ObservableCollection<Event> events = new ObservableCollection<Event>();

			var eventsIdString = "";
			for(int i = 0; i < eventTypesId.Count; i++)
			{
				eventsIdString += "eventTypesId=" + eventsIdString[i] + "&";
			}

			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/SearchEvent" + eventsIdString + "profileId=" + profile.ProfileId + 
				"&age=" + profile.Age + "&userLat="+userLat + "&userLong=" + userLong + "&maxDistance=" + maxDistance);

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
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/FollowEvent/" + eventId + "/" + profileId);

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

		public async Task<bool> UnfollowEvent(string eventId, string profileId)
		{
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/UnfollowEvent/" + eventId + "/" + profileId);

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

		public async Task<bool> AttendEvent(string eventId, string profileId)
		{
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/AttendEvent/" + eventId + "/" + profileId);

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

		public async Task<bool> UnattendEvent(string eventId, string profileId)
		{
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/UnattendEvent/" + eventId + "/" + profileId);

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

		public async Task<Event> AddCommentToEvent(string eventId, Comment comment)
		{
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/Comment/"+eventId);

			try
			{
				var json = JsonConvert.SerializeObject(comment);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await httpClient.PostAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					var retrievedEvent = JsonConvert.DeserializeObject<Event>(recievedContent);
					return retrievedEvent;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return null;
		}

		public async Task<Event> InviteToEvent(string eventId, List<string> profileIds)
		{
			var uri = new Uri("https://howlout.gear.host/api/EventsAPI/InviteToEvent/"+eventId);

			try
			{
				var json = JsonConvert.SerializeObject(profileIds);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await httpClient.PostAsync(uri, content);

				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					var retrievedEvent = JsonConvert.DeserializeObject<Event>(recievedContent);
					return retrievedEvent;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}

			return null;
		}

		public async Task<bool> DeclineEventInvite(string eventId, string profileId)
		{

			var uri = new Uri("https://howlout.gear.host/api/GroupApi/DeclineEventInvite?eventId="+eventId+"&profileId="+profileId);

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

