using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HowlOut
{
    public class DataManager
    {
        HttpClient httpClient;

        public DataManager ()
        {
            httpClient = new HttpClient(new NativeMessageHandler());
        }

        public Uri GetFacebookProfileImageUri(string profileFacebookId)
        {
            return new Uri("https://graph.facebook.com/" + profileFacebookId + "/picture");
        }

		public async Task<ObservableCollection<Event>> GetAllEvents()
        {
			ObservableCollection<Event> events = new ObservableCollection<Event>(); 

            var uri = new Uri("https://howlout.gear.host/api/EventsAPI");

            try { 
                var response = await httpClient.GetAsync(uri);
                if(response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
					events = JsonConvert.DeserializeObject<ObservableCollection<Event>>(content);
                }
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
            }
            return events;
        }

        public async Task<List<Event>> GetEventsWithOwnerId()
        {
            List<Event> events = new List<Event>();

            var uri = new Uri("https://howlout.gear.host/api/EventsAPI/Owner/" + App.StoredUserFacebookId);

            try
            {
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    events = JsonConvert.DeserializeObject<List<Event>>(content);
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

        public async Task<bool> CreateEvent(Event eventToCreate)
        {
            var uri = new Uri("https://howlout.gear.host/api/EventsAPI");

            try
            {
                var json = JsonConvert.SerializeObject(eventToCreate);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(uri, content);

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

        public async Task<bool> DeleteEvent(string eventId)
        {
            var uri = new Uri("https://howlout.gear.host/api/EventsAPI"+eventId);

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
    }
}
