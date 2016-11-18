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

			setTopbar();

			conversationScrollView.Scrolled += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine(conversationScrollView.ScrollY);
				if (conversationScrollView.ScrollY > 0 && conversationScrollView.ScrollY < 100 && !scrollDelay)
				{
					DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
				}

				if (conversationScrollView.ScrollY < 10 && !scrollDelay)
				{
					conversationInfo.IsVisible = true;
				}
			};

			App.coreView.viewdConversation = this;


		}

		async void setConversationInfo(Conversation c)
		{
			if (c.Profiles != null)
			{
				foreach (Profile p in c.Profiles)
				{
					if (p.ProfileId != App.StoredUserFacebookId)
					{
						if (c.Profiles.Count > 2)
						{
							conversationInfoLabel.Text += p.Name.Split(' ')[0] + ", ";
						}
						else {
							conversationInfoLabel.Text += p.Name + ", ";
						}

						if (conversationInfoImage.Source == null && c.ModelType == ConversationModelType.Profile)
						{
							conversationInfoImage.Source = p.ImageSource;
						}
					}
				}
			}

			TapGestureRecognizer t = new TapGestureRecognizer();


			if (c.ModelType == ConversationModelType.Event)
			{
				t.Tapped += (sender, e) =>
				{
					App.coreView.GoToSelectedEvent(c.ModelId);
				};

				Event eve = await _dataManager.EventApiManager.GetEventById(c.ModelId);
				conversationInfoImage.Source = eve.ImageSource;
				conversationInfoModelLabel.Text = "Event: " + eve.Title;
			}
			else if (c.ModelType == ConversationModelType.Group)
			{
				t.Tapped += (sender, e) =>
				{
					App.coreView.GoToSelectedGroup(c.ModelId);
				};

				Group grp = await _dataManager.GroupApiManager.GetGroup(c.ModelId);
				conversationInfoImage.Source = grp.ImageSource;
				conversationInfoModelLabel.Text = "Group: " + grp.Name;
			}
			else if (c.ModelType == ConversationModelType.Organization)
			{
				t.Tapped += (sender, e) =>
				{
					App.coreView.GoToSelectedOrganization(c.ModelId);
				};

				Organization org = await _dataManager.OrganizationApiManager.GetOrganization(c.ModelId);
				conversationInfoImage.Source = org.ImageSource;
				conversationInfoModelLabel.Text = "Organization: " + org.Name;
			}
			else {
				t.Tapped += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new ListsAndButtons(c.Profiles, null,null, false, false), "", null);
				};
			}

			conversationInfoProfilesButton.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new ListsAndButtons(c.Profiles, null, null, false, false), "", null);
			};

			conversationInfoImage.GestureRecognizers.Add(t);
			conversationInfo.IsVisible = true;
		}

		/*
		private async Task setInfoBar(Conversation conv)
		{
			if (conv.ModelType != ConversationModelType.Profile)
			{
				App.coreView.GoToSelectedEvent()

				if (conv.ModelType == ConversationModelType.Event)
				{
					ContentView cv = new GroupDesignView(groups[i], height, design);
				}

				modelTypeView.Children.Add
			}
		} */

		private async Task setTopbar()
		{
			await Task.Delay(10);
			App.coreView.topBar.showAddPeopleToConversationButton(true, this);
		}

		private async Task setSize(bool add)
		{
			await Task.Delay(10);
			outerGrid.HeightRequest = App.coreView.Height - (137);
			commentList.HeightRequest = outerGrid.HeightRequest - 80;
			conversationList.HeightRequest = outerGrid.HeightRequest - 80;
			commentListLayout.HeightRequest = commentListLayout.Height;
			conversationListLayout.HeightRequest = conversationListLayout.Height;

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
			conversationList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}
	}
}
