using System;
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

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/Owner/" + App.StoredUserFacebookId);

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

		public async Task<ObservableCollection<Event>> SearchEvents(string profileId, double userLat, double userLong)
		{
			ObservableCollection<Event> events = new ObservableCollection<Event>();

			var uri = new Uri("https://www.howlout.net/api/EventsAPI/SearchEvent?profileId=" + profileId + 
				"&userLat="+userLat + "&userLong=" + userLong);

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
			var uri = new Uri("https://www.howlout.net/api/EventsAPI/Comment/"+eventId);

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

