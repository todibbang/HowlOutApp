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
		private Conversation conversation;
		private List<Comment> comments;
		private StackLayout wallLayout;
		List<Profile> profilesAdded;

		private MessageApiManager.CommentType type;
		private string ID;

		public ConversationView(List<Comment> coms, MessageApiManager.CommentType mT, string id, StackLayout layout)
		{
			InitializeComponent();
			comments = coms;
			entryTop.IsVisible = true;
			commentList.ItemSelected += OnItemSelected;
			type = mT;
			ID = id;
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
			ID = conv.ConversationID;

			UpdateList(true);
			postCommentButtonBottom.Clicked += async (sender, e) => { await PostNewComment(commentEntryBottom.Text); };
			setSize();

			peopleToAddConversationList.ItemSelected += OnPeopleToAddListItemSelected;
			addedToConversationList.ItemSelected += OnAddedPeopleListItemSelected;

			cancelCreateConversation.Clicked += (sender, e) => { AddToConversation.IsVisible = false; };
			createConversation.Clicked += async (sender, e) =>
			{
				Conversation newConv = await _dataManager.MessageApiManager.AddProfilesToConversation(conv.ConversationID, profilesAdded);
				if (newConv != null)
				{
					App.coreView.setContentViewWithQueue(new ConversationView(newConv), "", null);
					AddToConversation.IsVisible = false;
				}
			};


		}

		private async Task setSize()
		{
			await Task.Delay(10);
			outerGrid.HeightRequest = App.coreView.Height - (35 + 62 + 30);
			commentList.HeightRequest = outerGrid.HeightRequest - 80;
			App.coreView.topBar.showAddPeopleToConversationButton(true, this);
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
					Conversation cons = await _dataManager.MessageApiManager.WriteToConversation(ID, commentObj);
					if (cons != null)
					{
						success = true;
						comments = cons.Messages;
						UpdateList(false);
					}
				}
				else {

					List<Comment> coms = await _dataManager.MessageApiManager.CreateComment(ID, type, commentObj);
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
					comments = await _dataManager.MessageApiManager.GetComments(ID, type);
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
				commentList.HeightRequest = comments.Count * 130;
				listViewRow.Height = comments.Count * 130;
				wallLayout.HeightRequest = comments.Count * 130;
			}
			return true;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			commentList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}








		//Adds profiles to conversation
		public void ShowPeopleToAddToConversation()
		{
			profilesAdded = new List<Profile>();
			AddToConversation.IsVisible = true;
			peopleToAddConversationList.ItemsSource = App.userProfile.Friends;
			addedToConversationList.ItemsSource = null;

		}

		public void OnPeopleToAddListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (peopleToAddConversationList.SelectedItem == null) { return; }
			var selectedProfile = peopleToAddConversationList.SelectedItem as Profile;
			if (!profilesAdded.Exists(p => p.ProfileId == selectedProfile.ProfileId))
			{
				profilesAdded.Add(selectedProfile);
				addedToConversationList.ItemsSource = null;
				addedToConversationList.ItemsSource = profilesAdded;
			}
			peopleToAddConversationList.SelectedItem = null;
		}
		public void OnAddedPeopleListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (addedToConversationList.SelectedItem == null) { return; }
			var selectedProfile = addedToConversationList.SelectedItem as Profile;
			if (profilesAdded.Exists(p => p.ProfileId == selectedProfile.ProfileId))
			{
				profilesAdded.Remove(profilesAdded.Find(p => p.ProfileId == selectedProfile.ProfileId));
				addedToConversationList.ItemsSource = null;
				addedToConversationList.ItemsSource = profilesAdded;
			}
			addedToConversationList.SelectedItem = null;
		}
	}
}
