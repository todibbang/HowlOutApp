using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace HowlOut
{
	public partial class ConversationView : ContentView, ViewModelInterface
	{
		private DataManager _dataManager = new DataManager();
		public Conversation conversation;
		string title;

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
			commentView.IsVisible = true;
			comments = coms;
			entryTop.IsVisible = true;
			commentList.ItemSelected += OnItemSelected;
			type = mT;
			ConversationId = id;
			wallLayout = layout;

			UpdateList(true);
			postCommentButtonTop.Clicked += async (sender, e) => { await PostNewComment(commentEntryTop.Text); };
			setSize(false);
		}

		public ConversationView(Conversation conv)
		{
			InitializeComponent();
			conversationScrollView.IsVisible = true;
			setConversationInfo(conv);
			conversation = conv;
			entryBottom.IsVisible = true;
			conversationList.ItemSelected += OnItemSelected;
			type = MessageApiManager.CommentType.Converzation;
			ConversationId = conv.ConversationID;

			UpdateList(true);
			setSize(true);



			bool scrollDelay = false;
			commentEntryBottom.Focused += async (sender, e) =>
			{
				conversationList.TranslationY = 90;
				conversationScrollView.HeightRequest = 1000;
				conversationInfo.IsVisible = false;
				try
				{
					await Task.Delay(1);
					if (conversationList.ItemsSource.OfType<Comment>().Last() != null)
					{
						conversationList.ScrollTo(conversationList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, true);
					}
				}
				catch (Exception ex) { }

				scrollDelay = true;
				await Task.Delay(200);
				scrollDelay = false;

			};
			commentEntryBottom.Unfocused += (sender, e) =>
			{
				conversationList.TranslationY = 0;
			};

			conversationScrollView.Scrolled += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine(conversationScrollView.ScrollY);
				if (conversationScrollView.ScrollY > 0 && conversationScrollView.ScrollY < 100 && !scrollDelay)
				{
					DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
				}

				if (conversationScrollView.ScrollY < 10 && !scrollDelay)
				{
					if (conversation.ModelType != ConversationModelType.Profile)
					{
						conversationInfo.IsVisible = true;
					}
				}
			};
			App.coreView.viewdConversation = this;
		}

		public async void viewInFocus(UpperBar bar)
		{
			if (conversation == null) return;

			title = "";

			if (conversation.Profiles != null)
			{
				foreach (Profile p in conversation.Profiles)
				{
					if (p.ProfileId != App.StoredUserFacebookId)
					{
						if (conversation.Profiles.Count > 2)
						{
							title += p.Name.Split(' ')[0] + ", ";
						}
						else {
							title += p.Name;
						}

						if (conversationInfoImage.Source == null && conversation.ModelType == ConversationModelType.Profile)
						{
							conversationInfoImage.Source = p.ImageSource;
						}
					}
				}

				if (conversation.Profiles.Count > 2)
				{
					title = title.Substring(0, title.Length-2);
				}
			}
			if (!string.IsNullOrWhiteSpace(conversation.Title))
			{
				title = conversation.Title;
			}

			App.coreView.topBar.setRightButton("ic_menu.png").Clicked += async (sender, e) =>
			{
				List<Action> actions = new List<Action>();
				List<string> titles = new List<string>();
				List<string> images = new List<string>();

				actions.Add(() => { App.coreView.setContentViewWithQueue(new InviteListView(conversation, false)); });
				titles.Add("Invite");
				images.Add("ic_add_profiles.png");

				actions.Add(async() => {
					editTitleGrid.IsVisible = !editTitleGrid.IsVisible;

				});
				titles.Add("Change Title");
				images.Add("ic_title.png");

				actions.Add(async () =>
				{
					bool success = await _dataManager.MessageApiManager.leaveConversation(conversation.ConversationID);
					if (success) { App.coreView.returnToPreviousView(); }
					else { await App.coreView.displayAlertMessage("Error", "Error", "Ok"); }
				});
				titles.Add("Leave");
				images.Add("ic_settings.png");

				await App.coreView.DisplayOptions(actions, titles, images);
			};

			bar.setNavigationlabel(title).Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new ListsAndButtons(conversation.Profiles, null, false, false));
			};

			if (type == MessageApiManager.CommentType.Converzation)
			{
				try
				{
					await Task.Delay(2);
					conversationList.ScrollTo(conversationList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, true);
				}
				catch (Exception e) {}
			}
		}

		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }

		async void setConversationInfo(Conversation c)
		{
			TapGestureRecognizer t = new TapGestureRecognizer();

			conversationInfo.IsVisible = true;
			if (c.ModelType == ConversationModelType.Event)
			{
				conversationInfoProfilesButton.Clicked += (sender, e) =>
				{
					App.coreView.GoToSelectedEvent(c.ModelId);
				};

				Event eve = await _dataManager.EventApiManager.GetEventById(c.ModelId);
				conversationInfoImage.Source = eve.ImageSource;
				conversationInfoModelLabel.Text = "Event: " + eve.Title;
			}
			else if (c.ModelType == ConversationModelType.Group)
			{
				conversationInfoProfilesButton.Clicked += (sender, e) =>
				{
					App.coreView.GoToSelectedGroup(c.ModelId);
				};

				Group grp = await _dataManager.GroupApiManager.GetGroup(c.ModelId);
				conversationInfoImage.Source = grp.ImageSource;
				conversationInfoModelLabel.Text = "Group: " + grp.Name;
			}
			else {
				conversationInfo.IsVisible = false;
				conversationInfoProfilesButton.IsVisible = false;
			}

			closeEditTitle.Clicked += (sender, e) =>
			{
				editTitleGrid.IsVisible = !editTitleGrid.IsVisible;
			};
			editTitleButton.Clicked += async (sender, e) =>
			{
				await _dataManager.MessageApiManager.EditConversationTitle(conversation.ConversationID, editTitleEntry.Text);
				c = await _dataManager.MessageApiManager.GetOneConversation(c.ConversationID);
				App.coreView.setContentViewReplaceCurrent(new ConversationView(c), 1);
			};

		}

		private async Task setSize(bool add)
		{
			await Task.Delay(10);
			outerGrid.HeightRequest = App.coreView.Height;// - (62);
			//commentList.HeightRequest = outerGrid.HeightRequest - 80;
			//conversationList.HeightRequest = outerGrid.HeightRequest - 80;
			//commentListLayout.HeightRequest = commentListLayout.Height;
			//conversationListLayout.HeightRequest = conversationListLayout.Height;

			//listHeight = commentList.HeightRequest;
			//listHeight = commentList.HeightRequest;
			if(add)App.coreView.addConversationViewToActiveList(this);
		}

		public async void pushPostNewComment()
		{
			await PostNewComment(commentEntryBottom.Text);
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
					DateAndTime = DateTime.Now.ToLocalTime(),
					ImageSource = App.userProfile.ImageSource
				};

				bool success = false;

				comments.Add(commentObj);
				UpdateList(false);

				commentEntryTop.Text = "";
				commentEntryBottom.Text = "";

				//commentEntryBottom.

				if (type == MessageApiManager.CommentType.Converzation)
				{
					Conversation cons = await _dataManager.MessageApiManager.WriteToConversation(ConversationId, commentObj);
					if (cons != null)
					{
						success = true;
						comments = cons.Messages;
					}
				}
				else {

					List<Comment> coms = await _dataManager.MessageApiManager.CreateComment(ConversationId, type, commentObj);
					if (coms != null)
					{
						success = true;
						comments = coms;
					}
				}

				if (success) {
					
				} else {
					await App.coreView.displayAlertMessage("Comment Not Posted", "An error happened and the comment was not posted, try again.", "Ok");
				}
			}
		}

		public async Task<bool> UpdateList(bool update)
		{
			if (update)
			{
				if (type == MessageApiManager.CommentType.Converzation)
				{
					conversation = await _dataManager.MessageApiManager.GetOneConversation(ConversationId);
					comments = conversation.Messages;
				}
				else {
					comments = await _dataManager.MessageApiManager.GetComments(ConversationId, type);
				}
			}

			if (type == MessageApiManager.CommentType.Converzation) {
				comments = comments.OrderBy(c => c.DateAndTime).ToList();
				conversationList.ItemsSource = comments;
				conversationList.IsRefreshing = false;
			} else {
				comments = comments.OrderByDescending(c => c.DateAndTime).ToList();
				commentList.ItemsSource = comments;
				commentList.IsRefreshing = false;
			}


			await Task.Delay(10);
			if (type == MessageApiManager.CommentType.Converzation)
			{
				conversationList.ScrollTo(conversationList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, false);
			}
			else {
				commentList.HeightRequest = comments.Count * 120 + 100;
				listViewRow.Height = comments.Count * 120 + 100;
				wallLayout.HeightRequest = comments.Count * 120 + 100;
				commentList.ScrollTo(commentList.ItemsSource.OfType<Comment>().First(), ScrollToPosition.Start, true);
			}
			return true;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			commentList.SelectedItem = null;
			conversationList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}
	}
}
