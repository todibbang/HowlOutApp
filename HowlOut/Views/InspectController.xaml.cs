using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.ObjectModel;
using System.Net.Http;
using ModernHttpClient;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class InspectController : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		private DataManager _dataManager = new DataManager();
		public ConversationView conversationView = null;
		bool userProfilerBeingViewed = false;

		public InspectController(Profile userProfile)
		{
			InitializeComponent();
			SetProfileInspect(userProfile);
		}

		async void SetProfileInspect(Profile userProfile)
		{
			try {
				if (userProfile.ProfileId == App.StoredUserFacebookId) { userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile(); }
				else { userProfile = await _dataManager.ProfileApiManager.GetProfile(userProfile.ProfileId); }
				infoView.Content = new ProfileDesignView(userProfile, 200, false, GenericDesignView.Design.ShowAll);
				App.coreView.topBar.setNavigationLabel(userProfile.Name, scrollView);

				if (userProfile.ProfileId == App.userProfile.ProfileId)
				{
					userProfilerBeingViewed = true;
					addNewElement(new ListsAndButtons(userProfile.Friends, null, null, true, false), "Friends");
					addNewElement(new ListsAndButtons(null, userProfile.Groups, null, true, false), "Groups");
					addNewElement(new ListsAndButtons(null, null, userProfile.Organizations, true, false), "Organizations");
					if (userProfile.RecievedFriendRequests.Count > 0)
					{
						addNewElement(new ListsAndButtons(userProfile.RecievedFriendRequests, null, null, true, true), "Friend Requests");
					}
					if (userProfile.GroupsInviteTo.Count > 0)
					{
						addNewElement(new ListsAndButtons(null, userProfile.GroupsInviteTo, null, true, true), "Groups Invited To");
					}
					if (userProfile.OrganizationsInviteTo.Count > 0)
					{
						addNewElement(new ListsAndButtons(null,null, userProfile.OrganizationsInviteTo,  true, true), "Organizations Invited To");
					}

				}
				else if (App.userProfile.Friends.Exists(p => p.ProfileId == userProfile.ProfileId))
				{
					if (userProfile.Friends.Count > 0)
					{
						addNewElement(new ListsAndButtons(userProfile.Friends, null, null, true, false), "Friends");
					}
					addNewElement(new EventListView(userProfile), "Events");
				}

				if (userProfile.ProfileId != App.userProfile.ProfileId)
				{
					App.coreView.IsLoading(true);
					message.IsVisible = true;
					var msImg = new TapGestureRecognizer();
					msImg.Tapped += async (sender, e) =>
					{
						Conversation conv = await _dataManager.MessageApiManager.CreateConversations(ConversationModelType.Profile, new List<Profile>() { App.userProfile, userProfile }, "", "");
						App.coreView.setContentViewWithQueue(new ConversationView(conv), "", null);
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
				App.coreView.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView();
			}
		}

		public InspectController(Group userGroup)
		{
			InitializeComponent();
			SetGroupInspect(userGroup);
			_dataManager.setUpdateSeen(userGroup.GroupId, NotificationModelType.Group);
		}

		async void SetGroupInspect(Group userGroup)
		{
			try
			{
				userGroup = await _dataManager.GroupApiManager.GetGroup(userGroup.GroupId);
				infoView.Content = new GroupDesignView(userGroup, 200, GenericDesignView.Design.ShowAll);
				App.coreView.topBar.setNavigationLabel("Group " + userGroup.Name, scrollView);

				if (_dataManager.AreYouGroupMember(userGroup))
				{
					List<Profile> members = new List<Profile>();
					if (userGroup.ProfileOwner != null) members.Add(userGroup.ProfileOwner);
					foreach (Profile p in userGroup.Members)
					{
						members.Add(p);
					}
					addNewElement(new ListsAndButtons(members, null, null, true, false), "Members");
					if (userGroup.ProfilesRequestingToJoin.Count > 0)
					{
						addNewElement(new ListsAndButtons(userGroup.ProfilesRequestingToJoin, null, null, null, userGroup, null), "Requesting To Join");
					}

					if (userGroup.NumberOfActiveEvents > 0) addNewElement(new EventListView(userGroup), "Events");
					StackLayout wall = new StackLayout();
					conversationView = new ConversationView(userGroup.Comments, MessageApiManager.CommentType.GroupComment, userGroup.GroupId, wall);
					wall.Children.Add(conversationView);
					addWallElement(wall, "Wall", userGroup.GroupId, ConversationModelType.Group);
				}
			}catch (Exception ex) {
				App.coreView.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView(); 
			}
		}


		public InspectController(Organization organization)
		{
			InitializeComponent();
			SetOrganizationInspect(organization);
			_dataManager.setUpdateSeen(organization.OrganizationId, NotificationModelType.Organization);
		}

		async void SetOrganizationInspect(Organization organization)
		{
			try
			{
				organization = await _dataManager.OrganizationApiManager.GetOrganization(organization.OrganizationId);
				infoView.Content = new OrganizationDesignView(organization, 200, GenericDesignView.Design.ShowAll);
				App.coreView.topBar.setNavigationLabel("Organization " + organization.Name, scrollView);

				if (organization.Members.Exists(p => p.ProfileId == App.userProfile.ProfileId))
				{
					if (organization.Members.Count > 0)
					{
						addNewElement(new ListsAndButtons(organization.Members, null, null, true, false), "Members");
					}
					if(organization.NumberOfActiveEvents > 0)addNewElement(new EventListView(organization), "Events");
					StackLayout wall = new StackLayout();
					conversationView = new ConversationView(organization.Comments, MessageApiManager.CommentType.OrganizationComment, organization.OrganizationId, wall);
					wall.Children.Add(conversationView);
					addWallElement(wall, "Wall", organization.OrganizationId, ConversationModelType.Organization);
				}
			}
			catch (Exception ex)
			{
				App.coreView.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView();
			}
		}

		public InspectController(Event eve)
		{
			InitializeComponent();
			SetEventInspect(eve);
			_dataManager.setUpdateSeen(eve.EventId, NotificationModelType.Event);
		}

		async void SetEventInspect(Event eve)
		{
			try
			{
				eve = await _dataManager.EventApiManager.GetEventById(eve.EventId);
				infoView.Content = new InspectEvent(eve, _dataManager.IsEventJoined(eve), scrollView);

				if (eve.ProfileOwner != null)
				{
					App.coreView.topBar.setNavigationLabel(eve.ProfileOwner.Name + "'s Event", scrollView);
				}
				else if (eve.OrganizationOwner != null)
				{
					App.coreView.topBar.setNavigationLabel(eve.OrganizationOwner.Name + "'s Event", scrollView);
				}

				if (eve.Attendees.Count > 0)
				{
					addNewElement(new ListsAndButtons(eve.Attendees, null, null, true, false), "Attendees");
				}

				if (_dataManager.IsEventJoined(eve))
				{
					StackLayout wall = new StackLayout();
					conversationView = new ConversationView(eve.Comments, MessageApiManager.CommentType.EventComment, eve.EventId, wall);
					wall.Children.Add(conversationView);
					addWallElement(wall, "Wall", eve.EventId, ConversationModelType.Event);
				}
			}
			catch (Exception ex)
			{
				App.coreView.displayAlertMessage("Error", "Error loading content", "Ok");
				App.coreView.returnToPreviousView();
			}
		}

		void addNewElement(View element, string Title)
		{
			Grid grid = new Grid() { RowSpacing = 0, BackgroundColor = Color.White };
			element.BackgroundColor = App.HowlOutBackground;
			grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 40 });
			grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			grid.Children.Add(new Line(), 0, 0);
			StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal };
			sl.Children.Add(new Label() { Text = "  " + Title, VerticalTextAlignment = TextAlignment.Center, TranslationY = 5, HorizontalOptions = LayoutOptions.StartAndExpand });
			if (userProfilerBeingViewed && !Title.Contains(" "))
			{
				Button b = new Button() { TextColor = App.HowlOut, Text = "find more " + Title.ToLower() + "  ", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.EndAndExpand };
				b.Clicked += (sender, e) =>
				{
					int i = 0;
					if (Title == "Groups") i = 1;
					if (Title == "Organizations") i = 2;
						App.coreView.setContentViewWithQueue(new FindNewFriendsView(i), "", null);
				};
				sl.Children.Add(b);

			}
			grid.Children.Add(sl, 0, 1);
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
			sl.Children.Add(new Label() { Text = "  " + Title, VerticalTextAlignment = TextAlignment.Center, TranslationY = 5, HorizontalOptions = LayoutOptions.StartAndExpand });
			Button b = new Button() { TextColor = App.HowlOut, Text = "View conversations  ", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.EndAndExpand };
			b.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new YourConversations(conversationtype, conversationID), "", null);
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

