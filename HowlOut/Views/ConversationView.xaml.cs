using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace HowlOut
{
	public partial class ConversationView : ContentView
	{
		private DataManager _dataManager;
		private Conversation conversation;
		private bool direction;

		public ConversationView(Conversation conv, bool dir)
		{
			InitializeComponent();
			conversation = conv;
			_dataManager = new DataManager();
			direction = dir;
			entryTop.IsVisible = direction;
			entryBottom.IsVisible = !direction;

			UpdateList();
			postCommentButtonTop.Clicked += async (sender, e) => { await PostNewComment(commentEntryTop.Text); };
			postCommentButtonBottom.Clicked += async (sender, e) => { await PostNewComment(commentEntryBottom.Text); };
			commentList.ItemSelected += OnItemSelected;
		}

		private async Task PostNewComment(string comment)
		{
			if (!string.IsNullOrWhiteSpace(comment))
			{
				//TODO changed this to recieve comment object instead of event
				var commentObj = new Comment
				{
					Content = comment,
					SenderID = App.StoredUserFacebookId,
					DateAndTime = DateTime.Now.ToLocalTime()
				};
				conversation.Comments.Add(commentObj);
				bool success = await UpdateList();
				if (success) {
					commentEntryTop.Text = ""; 
					commentEntryBottom.Text = "";
				} else {
					await App.coreView.displayAlertMessage("Comment Not Posted", "An error happened and the comment was not posted, try again.", "Ok");
				}
			}
		}

		public async Task<bool> UpdateList()
		{
			loading.IsVisible = true;
			List<Comment> comList = new List<Comment>();
			//comList = await _dataManager.EventApiManager.GetEventComments("");
			comList = conversation.Comments;
			if (direction) {
				comList = comList.OrderByDescending(c => c.DateAndTime).ToList();
			} else {
				comList = comList.OrderBy(c => c.DateAndTime).ToList();
			}

			commentList.ItemsSource = comList;
			commentList.IsRefreshing = false;
			loading.IsVisible = false;
			await Task.Delay(10);
			if (!direction)
			{
				commentList.ScrollTo(commentList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, true);
			}
			else {
				commentList.ScrollTo(commentList.ItemsSource.OfType<Comment>().First(), ScrollToPosition.Start, true);
				commentList.HeightRequest = comList.Count * 150;
			}
			return true;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			commentList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}
	}
}
