using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace HowlOut
{
	public class MessageApiManager
	{
		private HttpClient httpClient;

		public MessageApiManager(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task<List<Comment>> GetComments(string id, CommentType commentType)
		{
			var uri = "/" + id + "?commentType=" + commentType;
			List<Comment> comments = new List<Comment>();
			var recievedContent = "";
			try
			{
				var response = await httpClient.GetAsync(new Uri(App.serverUri + "message/comment" + uri));
				if (response.IsSuccessStatusCode) { 
					recievedContent = await response.Content.ReadAsStringAsync(); 
					comments = JsonConvert.DeserializeObject<List<Comment>>(recievedContent);
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}

			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
				Comment comm = JsonConvert.DeserializeObject<Comment>(recievedContent);
				comments.Add(comm);
			}
			return comments;
		}

		public async Task<List<Conversation>> GetConversations(string modelId, ConversationModelType modelType)
		{
			List<Conversation> conversations = new List<Conversation>(); 
			string uri = App.serverUri + "message/conversation/";
			if (modelType != ConversationModelType.Profile) uri += modelId + "?modelType=" + modelType;
			else uri += "getAll";
			try
			{
				var response = await httpClient.GetAsync(new Uri(uri));
				if (response.IsSuccessStatusCode) { 
					var recievedContent = await response.Content.ReadAsStringAsync(); 
					return conversations = JsonConvert.DeserializeObject<List<Conversation>>(recievedContent);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<Conversation> GetOneConversation(string id)
		{
			Conversation conversation = new Conversation();

			try
			{
				var response = await httpClient.GetAsync(new Uri(App.serverUri + "message/conversation/getOne/" + id));
				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					return conversation = JsonConvert.DeserializeObject<Conversation>(recievedContent);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<List<Comment>> CreateComment(string id, CommentType commentType, Comment comment)
		{
			var content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			try {
				var response = await httpClient.PostAsync(new Uri(App.serverUri + "message/comment/" + id + "?commentType="+ commentType), content);
				if (response.IsSuccessStatusCode) { 
					var recievedContent = await response.Content.ReadAsStringAsync();
					var list = JsonConvert.DeserializeObject<List<Comment>>(recievedContent);
					return list;
				}
			}
			catch (Exception ex) { System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message); }
			return null;
		}

		public async Task<Conversation> CreateConversations(ConversationModelType modelType, List<Profile> profiles, string modelTypeId, string title)
		{
			var uri = "?modelType="+modelType;
			for (int i = 0; i < profiles.Count; i++)
			{
				uri += "&profileIds=" + profiles[i].ProfileId;
			}
			if (!string.IsNullOrWhiteSpace(modelTypeId) && modelType != ConversationModelType.Profile) uri += "&modelTypeId=" + modelTypeId;
			if (!string.IsNullOrWhiteSpace(title)) uri += "&title=" + title;
			try {
				var response = await httpClient.PostAsync(new Uri(App.serverUri + "message/conversation" + uri), new StringContent(""));
				if (response.IsSuccessStatusCode) { 
					var recievedContent = await response.Content.ReadAsStringAsync(); 
					return JsonConvert.DeserializeObject<Conversation>(recievedContent);
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex) { System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message); }
			return null;
		}

		public async Task<Conversation> WriteToConversation(string id, Comment comment)
		{
			var content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

			try
			{
				var response = await httpClient.PutAsync(new Uri(App.serverUri + "message/conversation/writeToConversation/" + id), content);
				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<Conversation>(recievedContent);
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex) { System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message); }
			return null;
		}

		public async Task<Conversation> AddProfilesToConversation(string id, List<Profile> profiles)
		{
			var uri = App.serverUri + "message/conversation/addToConversation?conversationId=" + id;
			for (int i = 0; i < profiles.Count; i++)
			{
				uri += "&profileIds=" + profiles[i].ProfileId;
			}
			try
			{
				var response = await httpClient.PutAsync(new Uri(uri), new StringContent(""));
				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<Conversation>(recievedContent);
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex) { System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message); }
			return null;
		}

		public async Task<bool> leaveConversation(string id)
		{
			var uri = App.serverUri + "message/conversation/LeaveConversation?conversationId=" + id;
			try
			{
				var response = await httpClient.PutAsync(new Uri(uri), new StringContent(""));
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
				else
				{
					await App.coreView.displayAlertMessage("Connection Error", "Trouble Connecting To Server", "OK");
				}
			}
			catch (Exception ex) { System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message); }
			return false;
		}

		public enum CommentType
		{
			GroupComment, EventComment, OrganizationComment, Converzation
		}


		public async Task<List<Notification>> GetNotifications()
		{
			List<Notification> notifications = new List<Notification>();

			try
			{
				var response = await httpClient.GetAsync(new Uri(App.serverUri + "message/inAppNotification/" + App.StoredUserFacebookId));
				if (response.IsSuccessStatusCode)
				{
					var recievedContent = await response.Content.ReadAsStringAsync();
					return notifications = JsonConvert.DeserializeObject<List<Notification>>(recievedContent);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<bool> SetNotificationSeen(string id)
		{
			try
			{
				var response = await httpClient.PutAsync(new Uri(App.serverUri + "message/inAppNotification/setSeen/" + id + "?isSeen=true"), new StringContent("", Encoding.UTF8, "application/json"));
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

