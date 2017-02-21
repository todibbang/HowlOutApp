using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class InspectController : ContentView, ViewModelInterface
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		private DataManager _dataManager = new DataManager();
		public ConversationView conversationView = null;
		bool userProfilerBeingViewed = false;
		string Title;
		Profile pro;
		Event eve;
		Group grp;

		public InspectController(Profile userProfile)
		{
			InitializeComponent();
			SetProfileInspect(userProfile);
			scrollView.Scrolled += (sender, e) => { if (scrollView.ScrollY < -150) { reloadView(); } };
		}

		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			if (eve != null)
			{
				addEventMenu(eve, ub);
			}
			else if (grp != null)
			{
				addGroupMenu(grp, ub);
			}
			ub.setNavigationlabel(Title);
			ub.setPadding();
			return ub;
		}

		public void viewExitFocus()
		{
			//App.coreView.topBar.hideAll();
		}
		public void reloadView()
		{
			if (pro != null)
			{
				this.Content = new InspectController(pro);
			}
			else if (eve != null)
			{
				this.Content = new InspectController(eve);
			}
			else if (grp != null)
			{
				this.Content = new InspectController(grp);
			}
		}

		public ContentView getContentView() { return this; }

		async void SetProfileInspect(Profile userProfile)
		{
			pro = userProfile;
			try
			{
				System.Diagnostics.Debug.WriteLine("ID: " + userProfile.ProfileId);
				Title = userProfile.Name;
				if (userProfile.ProfileId == App.StoredUserFacebookId)
				{
					await _dataManager.ProfileApiManager.GetLoggedInProfile();
					userProfile = App.userProfile;
				}
				else {
					userProfile = await _dataManager.ProfileApiManager.GetProfile(userProfile.ProfileId);
				}
				infoView.Content = new ProfileDesignView(userProfile, 200, false, GenericDesignView.Design.ShowAll);
				infoView.Padding = new Thickness(0, 70, 0, 0);

				if (userProfile.ProfileId == App.userProfile.ProfileId)
				{
					userProfilerBeingViewed = true;
					if (userProfile.RecievedFriendRequests != null && userProfile.RecievedFriendRequests.Count > 0)
					{
						addNewElement(new ListsAndButtons(userProfile.RecievedFriendRequests, null, true, true), "Friend Requests", userProfile.RecievedFriendRequests.Count);
					}
					if (userProfile.GroupsInviteToAsOwner != null && userProfile.GroupsInviteToAsOwner.Count > 0)
					{
						addNewElement(new ListsAndButtons(null, userProfile.GroupsInviteToAsOwner, true, true), "Groups Invited To As Owner", userProfile.GroupsInviteToAsOwner.Count);
					}
					if (userProfile.GroupsInviteTo != null && userProfile.GroupsInviteTo.Count > 0)
					{
						addNewElement(new ListsAndButtons(null, userProfile.GroupsInviteTo, true, true), "Groups Invited To", userProfile.GroupsInviteTo.Count);
					}

					addNewElement(new ListsAndButtons(userProfile.Friends, null, true, false), "Friends", userProfile.Friends.Count);
					userProfile.GroupsOwned.AddRange(userProfile.Groups);
					addNewElement(new ListsAndButtons(null, userProfile.GroupsOwned, true, false), "Groups", userProfile.Groups.Count);
					//addNewElement(new ListsAndButtons(null, null, userProfile.Organizations, true, false), "Organizations", userProfile.Organizations.Count);
					addNewElement(new EventListView(userProfile), "Events", 0);
				}
				else if (App.userProfile.Friends.Exists(p => p.ProfileId == userProfile.ProfileId))
				{
					if (userProfile.Friends.Count > 0)
					{
						addNewElement(new ListsAndButtons(userProfile.Friends, null, true, false), "Friends", userProfile.Friends.Count);
					}
					addNewElement(new EventListView(userProfile), "Events", 0);
				}

				if (userProfile.ProfileId != App.userProfile.ProfileId)
				{
					App.coreView.IsLoading(true);
					message.IsVisible = true;
					var msImg = new TapGestureRecognizer();
					msImg.Tapped += async (sender, e) =>
					{
						Conversation conv = await _dataManager.MessageApiManager.CreateConversations(ConversationModelType.Profile, new List<Profile>() { App.userProfile, userProfile }, "", "");
						App.coreView.setContentViewWithQueue(new ConversationView(conv));
						/*
						Conversation conv = App.coreView.conversatios.conList.Find(c => c.Profiles.Count == 2 && c.Profiles.Exists(p => p.ProfileId == userProfile.ProfileId));
						if (conv == null)
						{
							conv = await _dataManager.MessageApiManager.CreateConversations(ConversationModelType.Profile, new List<Profile>() { App.userProfile, userProfile }, "", "");
						}
						if (conv != null)
						{
							App.coreView.setContentViewWithQueue(new ConversationView(conv), "", null);
						}
						*/
					};
					message.GestureRecognizers.Add(msImg);
					App.coreView.IsLoading(false);
				}
			}
			catch (Exception ex)
			{
				App.rootPage.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView();
			}
			App.coreView.slideInView();
		}

		public InspectController(Group userGroup)
		{
			InitializeComponent();
			if (userGroup != null)
			{
				Title = userGroup.Name;
				SetGroupInspect(userGroup);
				App.notificationController.setUpdateSeen(userGroup.GroupId, NotificationModelType.Group);
			}
			scrollView.Scrolled += (sender, e) => { if (scrollView.ScrollY < -150) { reloadView(); } };
		}

		async void SetGroupInspect(Group userGroup)
		{
			try
			{
				this.grp = userGroup;
				userGroup = await _dataManager.GroupApiManager.GetGroup(userGroup.GroupId);
				this.grp = userGroup;
				//addGroupMenu(userGroup);

				infoView.Content = new GroupDesignView(userGroup, 200, GenericDesignView.Design.ShowAll);
				infoView.Padding = new Thickness(0, 70, 0, 0);

				if (_dataManager.AreYouGroupMember(userGroup))
				{
					if (userGroup.ProfilesRequestingToJoin.Count > 0)
					{
						addNewElement(new ListsAndButtons(userGroup.ProfilesRequestingToJoin, null, null, userGroup), "Requesting To Join", userGroup.ProfilesRequestingToJoin.Count);
					}

					addNewElement(new ListsAndButtons(userGroup.ProfileOwners, null, true, false), "Owners", userGroup.ProfileOwners.Count);
					if (userGroup.Members.Count > 0)
					{
						addNewElement(new ListsAndButtons(userGroup.Members, null, true, false), "Members", userGroup.Members.Count);
					}

					if (userGroup.NumberOfActiveEvents > 0) addNewElement(new EventListView(userGroup), "Events", userGroup.NumberOfActiveEvents);
					StackLayout wall = new StackLayout();
					conversationView = new ConversationView(userGroup.Comments, MessageApiManager.CommentType.GroupComment, userGroup.GroupId, wall);
					wall.Children.Add(conversationView);
					addWallElement(wall, "Wall", userGroup.GroupId, ConversationModelType.Group);
				}
			}
			catch (Exception ex)
			{
				App.rootPage.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView();
			}
			App.coreView.slideInView();
		}

		/*
		public InspectController(Organization organization)
		{
			InitializeComponent();
			if (organization != null)
			{
				Title = organization.Name;
				SetOrganizationInspect(organization);
				_dataManager.setUpdateSeen(organization.OrganizationId, NotificationModelType.Organization);
			}
		}

		async void SetOrganizationInspect(Organization organization)
		{
			try
			{
				organization = await _dataManager.OrganizationApiManager.GetOrganization(organization.OrganizationId);
				infoView.Content = new OrganizationDesignView(organization, 200, GenericDesignView.Design.ShowAll);


				if (organization.Members.Exists(p => p.ProfileId == App.userProfile.ProfileId))
				{
					if (organization.Members.Count > 0)
					{
						addNewElement(new ListsAndButtons(organization.Members, null, null, true, false), "Members", organization.Members.Count);
					}
					if(organization.NumberOfActiveEvents > 0)addNewElement(new EventListView(organization), "Events", organization.NumberOfActiveEvents);
					StackLayout wall = new StackLayout();
					conversationView = new ConversationView(organization.Comments, MessageApiManager.CommentType.OrganizationComment, organization.OrganizationId, wall);
					wall.Children.Add(conversationView);
					addWallElement(wall, "Wall", organization.OrganizationId, ConversationModelType.Organization);
				}
			}
			catch (Exception ex)
			{
				App.rootPage.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView();
			}
		}*/

		public InspectController(Event eve)
		{
			this.Resources = App.resourceDictionary;
			InitializeComponent();
			if (eve != null)
			{
				Title = eve.Title;
				SetEventInspect(eve);
				App.notificationController.setUpdateSeen(eve.EventId, NotificationModelType.Event);
			}
			scrollView.Scrolled += (sender, e) => { if (scrollView.ScrollY < -150) { reloadView(); } };
		}

		async void SetEventInspect(Event eve)
		{
			try
			{
				this.eve = eve;
				eve = await _dataManager.EventApiManager.GetEventById(eve.EventId);
				this.eve = eve;
				await Task.Delay(10);
				infoView.Content = new InspectEvent(eve, _dataManager.IsEventJoined(eve), scrollView);

				//addEventMenu(eve);




				if (eve.Attendees.Count > 0)
				{
					addNewElement(new ListsAndButtons(eve.Attendees, null, true, false), "Attendees", eve.Attendees.Count);
				}

				if (_dataManager.IsEventJoined(eve) || _dataManager.IsEventYours(eve))
				{
					StackLayout wall = new StackLayout();
					conversationView = new ConversationView(eve.Comments, MessageApiManager.CommentType.EventComment, eve.EventId, wall);
					wall.Children.Add(conversationView);
					addWallElement(wall, "Wall", eve.EventId, ConversationModelType.Event);
				}
			}
			catch (Exception ex)
			{
				App.rootPage.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView();
			}
			//App.coreView.slideInView();
		}

		async void addEventMenu(Event eve, UpperBar ub)
		{
			ub.setRightButton("ic_more_vert_white.png").Clicked += async (sender, e) =>
			{
				List<Action> actions = new List<Action>();
				List<string> titles = new List<string>();
				List<string> images = new List<string>();

				actions.Add(() => { _dataManager.AttendTrackEvent(eve.EventId, !eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId), false); });
				titles.Add(eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId) == true ? "Unfollow" : "Follow");
				images.Add("ic_paw.png");

				actions.Add(() => { App.coreView.DisplayShare(eve); });
				titles.Add("Share");
				images.Add("ic_share.png");

				if (_dataManager.IsEventYours(eve))
				{
					actions.Add(() => { App.coreView.setContentViewWithQueue(new InviteListView(eve, false)); });
					titles.Add("Invite Attendees");
					images.Add("ic_add_profiles.png");

					if (eve.GroupOwner == null)
					{
						actions.Add(() => { App.coreView.setContentViewWithQueue(new InviteListView(eve, true)); });
						titles.Add("Invite Owners");
						images.Add("ic_add_profiles.png");
					}

					actions.Add(() => { App.coreView.setContentViewWithQueue(new CreateEvent(eve, false)); });
					titles.Add("Edit");
					images.Add("ic_edit.png");

					if (eve.ProfileOwners != null && eve.ProfileOwners.Count > 1)
					{
						actions.Add(async () =>
						{
							await _dataManager.EventApiManager.AcceptDeclineLeaveEventAsOwner(eve.EventId, OwnerHandlingType.Leave);
							App.coreView.reloadCurrentView();
						});
						titles.Add("Leave");
						images.Add("ic_settings.png");
					}
					else if (_dataManager.IsEventJoined(eve))
					{
						actions.Add(async () =>
						{
							await App.coreView._dataManager.AttendTrackEvent(eve.EventId, false, true);
						});
						titles.Add("Leave Event");
						images.Add("ic_manage.png");
					}
				}
				else {

					if (_dataManager.IsEventJoined(eve))
					{
						actions.Add(() => { App.coreView.setContentViewWithQueue(new InviteListView(eve, false)); });
						titles.Add("Invite Attendees");
						images.Add("ic_add_profiles.png");

						actions.Add(() => { App.coreView._dataManager.AttendTrackEvent(eve.EventId, false, true); });
						titles.Add("Leave Event");
						images.Add("ic_manage.png");
					}
				}
				await  App.coreView.DisplayOptions(actions, titles, images, optiongrid);
			};
		}

		async void addGroupMenu(Group grp, UpperBar ub)
		{
			ub.setRightButton("ic_more_vert_white.png").Clicked += async (sender, e) =>
			{
				List<Action> actions = new List<Action>();
				List<string> titles = new List<string>();
				List<string> images = new List<string>();

				if (grp.ProfileOwners.Exists(p => p.ProfileId == App.userProfile.ProfileId))
				{
					actions.Add(() => { App.coreView.setContentViewWithQueue(new CreateGroup(grp, false)); });
					titles.Add("Edit");
					images.Add("ic_settings.png");

					actions.Add(() => { App.coreView.setContentViewWithQueue(new InviteListView(grp, false)); });
					titles.Add("Invite Members");
					images.Add("ic_settings.png");

					actions.Add(() => { App.coreView.setContentViewWithQueue(new InviteListView(grp, true)); });
					titles.Add("Invite Owners");
					images.Add("ic_settings.png");

					if (grp.ProfileOwners.Count > 1)
					{
						actions.Add(async () =>
						{
							await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroupAsOwner(grp.GroupId, OwnerHandlingType.Leave);
							App.coreView.reloadCurrentView();
						});
						titles.Add("Leave");
						images.Add("ic_settings.png");
					}

					//await App.coreView.DisplayOptions(actions, titles, images);
					await App.coreView.DisplayOptions(actions, titles, images, optiongrid);
				}
				else if (grp.Members.Exists(p => p.ProfileId == App.userProfile.ProfileId))
				{
					actions.Add(async () =>
					{
						bool success = await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(grp.GroupId, GroupApiManager.GroupHandlingType.Leave);
						if (success) { App.coreView.reloadCurrentView(); }
						else { await App.rootPage.displayAlertMessage("Error", "Error", "Ok"); }
					});
					titles.Add("Leave");
					images.Add("ic_settings.png");

					//await App.coreView.DisplayOptions(actions, titles, images);
					await App.coreView.DisplayOptions(actions, titles, images, optiongrid);
				}
			};
		}





		void addNewElement(View element, string Title, int amount)
		{
			Grid grid = new Grid() { RowSpacing = 0, BackgroundColor = Color.White };
			element.BackgroundColor = App.HowlOutBackground;
			grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 40 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.Children.Add(new Line(), 0, 0);

			StackLayout sll = new StackLayout() { Orientation = StackOrientation.Horizontal };
			string title = "";
			if (amount == 0) title = "  " + Title;
			else title = "  " + Title + " (" + amount + ")";
			Label lb = new Label() { Text = title, VerticalTextAlignment = TextAlignment.Center, TranslationY = 5, HorizontalOptions = LayoutOptions.StartAndExpand, FontAttributes = FontAttributes.Bold };
			sll.Children.Add(lb);

			StackLayout slb = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
			Button k = new Button() { BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.EndAndExpand };
			slb.Children.Add(k);
			k.Clicked += (sender, e) =>
			{
				if (element != null)
				{
					element.IsVisible = !element.IsVisible;
					if (element.IsVisible) lb.FontAttributes = FontAttributes.Bold;
					else lb.FontAttributes = FontAttributes.None;
				}
			};

			if (userProfilerBeingViewed && !Title.Contains(" ") && !Title.Contains("Events"))
			{
				Button b = new Button() { TextColor = App.HowlOut, Text = "find more " + Title.ToLower() + "  ", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.EndAndExpand, BackgroundColor = Color.Transparent };
				b.Clicked += (sender, e) =>
				{
					int i = 0;
					if (Title == "Groups") i = 1;
					if (Title == "Organizations") i = 2;
					App.coreView.setContentViewWithQueue(new FindNewFriendsView(i));
				};
				slb.Children.Add(b);
			}
			grid.Children.Add(sll, 0, 1);
			grid.Children.Add(slb, 0, 1);
			grid.Children.Add(new Line(), 0, 2);
			grid.Children.Add(element, 0, 3);
			infoLayout.IsVisible = true;
			infoLayout.Children.Add(grid);
		}

		void addWallElement(View element, string Title, string conversationID, ConversationModelType conversationtype)
		{
			Grid grid = new Grid() { RowSpacing = 0, BackgroundColor = Color.White };
			element.BackgroundColor = App.HowlOutBackground;
			grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 40 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			grid.Children.Add(new Line(), 0, 0);
			StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal };
			sl.Children.Add(new Label() { Text = "  " + Title + "   ", VerticalTextAlignment = TextAlignment.Center, TranslationY = 5, HorizontalOptions = LayoutOptions.StartAndExpand });
			Button b = new Button() { TextColor = App.HowlOut, Text = "View Tools  ", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.EndAndExpand };

			b.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new YourConversations(conversationtype, conversationID, 0));
			};

			sl.Children.Add(b);
			grid.Children.Add(sl, 0, 1);
			grid.Children.Add(new Line(), 0, 2);
			grid.Children.Add(element, 0, 3);
			infoLayout.IsVisible = true;
			infoLayout.Children.Add(grid);
		}
		public ScrollView getScrollView()
		{
			return null;
		}
	}
}

