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
		private DataManager _dataManager = new DataManager();
		public Conversation conversation;
		List<Comment> comments;
		private StackLayout wallLayout;
		List<Profile> profilesAdded;

		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		private double listHeight;

		public MessageApiManager.CommentType type;
		public string ConversationId;

		public ConversationView(List<Comment> coms, MessageApiManager.CommentType mT, string id, StackLayout layout)
		//public ConversationView(List<Comment> coms, MessageApiManager.CommentType mT, string id)
		{
			InitializeComponent();
			comments = coms;
			entryTop.IsVisible = true;
			commentList.ItemSelected += OnItemSelected;
			type = mT;
			ConversationId = id;
			wallLayout = layout;

			UpdateList(true);
			postCommentButtonTop.Clicked += async (sender, e) => { await PostNewComment(commentEntryTop.Text); };
			setSize();
		}


		public ConversationView(Conversation conv)
		{
			InitializeComponent();
			conversation = conv;
			entryBottom.IsVisible = true;
			commentList.ItemSelected += OnItemSelected;
			type = MessageApiManager.CommentType.Converzation;
			ConversationId = conv.ConversationID;

			UpdateList(true);
			postCommentButtonBottom.Clicked += async (sender, e) => {
				await PostNewComment(commentEntryBottom.Text); 
			};
			setSize();

			commentEntryBottom.Focused += (sender, e) =>
			{
				bottomInputLifter.Height = 200;
				listLayout.HeightRequest = listHeight - 200;
				commentList.HeightRequest = listHeight - 200;
			};
			commentEntryBottom.Unfocused += (sender, e) =>
			{
				bottomInputLifter.Height = 0;
				listLayout.HeightRequest = listHeight;
				commentList.HeightRequest = listHeight;
			};
			App.coreView.topBar.showAddPeopleToConversationButton(true, this);
		}

		private async Task setSize()
		{
			await Task.Delay(10);
			outerGrid.HeightRequest = App.coreView.Height - (35 + 62 + 30);
			commentList.HeightRequest = outerGrid.HeightRequest - 80;
			listLayout.HeightRequest = commentList.Height;
			listHeight = commentList.HeightRequest;
			App.coreView.addConversationViewToActiveList(this);
		}

		private async Task PostNewComment(string comment)
		{
			if (!string.IsNullOrWhiteSpace(comment))
			{
				//TODO changed this to recieve comment object instead of event
				var commentObj = new Comment
				{
					Content = comment,
					SenderId = App.StoredUserFacebookId,
					DateAndTime = DateTime.Now.ToLocalTime()
				};

				bool success = false;

				if (type == MessageApiManager.CommentType.Converzation)
				{
					Conversation cons = await _dataManager.MessageApiManager.WriteToConversation(ConversationId, commentObj);
					if (cons != null)
					{
						success = true;
						comments = cons.Messages;
						UpdateList(false);
					}
				}
				else {

					List<Comment> coms = await _dataManager.MessageApiManager.CreateComment(ConversationId, type, commentObj);
					if (coms != null)
					{
						success = true;
						comments = coms;
						UpdateList(false);
					}
				}

				if (success) {
					commentEntryTop.Text = ""; 
					commentEntryBottom.Text = "";
				} else {
					await App.coreView.displayAlertMessage("Comment Not Posted", "An error happened and the comment was not posted, try again.", "Ok");
				}
			}
		}

		public async Task<bool> UpdateList(bool update)
		{
			loading.IsVisible = true;
			if (update)
			{
				if (type == MessageApiManager.CommentType.Converzation)
				{
					comments = conversation.Messages;
				}
				else {
					comments = await _dataManager.MessageApiManager.GetComments(ConversationId, type);
				}
			}

			if (type == MessageApiManager.CommentType.Converzation) {
				comments = comments.OrderBy(c => c.DateAndTime).ToList();
			} else {
				comments = comments.OrderByDescending(c => c.DateAndTime).ToList();
			}

			commentList.ItemsSource = comments;
			commentList.IsRefreshing = false;
			loading.IsVisible = false;
			await Task.Delay(10);
			if (type == MessageApiManager.CommentType.Converzation)
			{
				commentList.ScrollTo(commentList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, true);
			}
			else {
				commentList.ScrollTo(commentList.ItemsSource.OfType<Comment>().First(), ScrollToPosition.Start, true);
				commentList.HeightRequest = comments.Count * 120 + 100;
				listViewRow.Height = comments.Count * 120 + 100;
				wallLayout.HeightRequest = comments.Count * 120 + 100;
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
