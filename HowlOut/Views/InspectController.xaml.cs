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
		private DataManager _dataManager = new DataManager();
		public ConversationView conversationView = null;

		public InspectController(Profile userProfile)
		{
			InitializeComponent();
			SetProfileInspect(userProfile);
		}

		async void SetProfileInspect(Profile userProfile)
		{
			userProfile = await _dataManager.ProfileApiManager.GetProfile(userProfile.ProfileId);
			infoView.Content = new ProfileDesignView(userProfile, 200, false, GenericDesignView.Design.ShowAll);
			App.coreView.topBar.setNavigationLabel(userProfile.Name, scrollView);

			if (userProfile.ProfileId == App.userProfile.ProfileId)
			{
				if (userProfile.Friends.Count > 0)
				{
					addNewElement(new ListsAndButtons(userProfile.Friends, null, null, true), "Friends");
				}

				if (userProfile.Groups.Count > 0)
				{
					addNewElement(new ListsAndButtons(null, userProfile.Groups, null, true), "Groups");
				}

				if (userProfile.Organizations.Count > 0)
				{
					addNewElement(new ListsAndButtons(null, null, userProfile.Organizations, true), "Organizations");
				}
			}
			else if (App.userProfile.Friends.Exists(p => p.ProfileId == userProfile.ProfileId))
			{
				if (userProfile.Friends.Count > 0)
				{
					addNewElement(new ListsAndButtons(userProfile.Friends, null, null, true), "Friends");
				}
				addNewElement(new EventListView(userProfile), "Events");
			}

		}


		public InspectController(Group userGroup)
		{
			InitializeComponent();
			SetGroupInspect(userGroup);
		}

		async void SetGroupInspect(Group userGroup)
		{
			userGroup = await _dataManager.GroupApiManager.GetGroup(userGroup.GroupId);
			infoView.Content = new GroupDesignView(userGroup, 200, GenericDesignView.Design.ShowAll);
			App.coreView.topBar.setNavigationLabel("Group " + userGroup.Name, scrollView);

			if (_dataManager.AreYouGroupMember(userGroup))
			{
				if (userGroup.Members.Count > 0)
				{
					addNewElement(new ListsAndButtons(userGroup.Members, null, null, true), "Members");
				}
				addNewElement(new EventListView(userGroup), "Events");
				moreLayout.IsVisible = true;
				StackLayout wall = new StackLayout();
				conversationView = new ConversationView(userGroup.Comments, MessageApiManager.CommentType.GroupComment, userGroup.GroupId, wall);
				wall.Children.Add(conversationView);
				moreLayout.Children.Add(wall);
			}
		}


		public InspectController(Organization organization)
		{
			InitializeComponent();
			SetOrganizationInspect(organization);
		}

		async void SetOrganizationInspect(Organization organization)
		{
			organization = await _dataManager.OrganizationApiManager.GetOrganization(organization.OrganizationId);
			infoView.Content = new OrganizationDesignView(organization, 200, GenericDesignView.Design.ShowAll);
			App.coreView.topBar.setNavigationLabel("Group " + organization.Name, scrollView);

			if (organization.Members.Exists(p => p.ProfileId == App.userProfile.ProfileId))
			{
				if (organization.Members.Count > 0)
				{
					addNewElement(new ListsAndButtons(organization.Members, null, null, true), "Members");
				}
				addNewElement(new EventListView(organization), "Events");
				moreLayout.IsVisible = true;
				StackLayout wall = new StackLayout();
				conversationView = new ConversationView(organization.Comments, MessageApiManager.CommentType.OrganizationComment, organization.OrganizationId, wall);
				wall.Children.Add(conversationView);
				moreLayout.Children.Add(wall);
			}
		}

		public InspectController(Event eve)
		{
			InitializeComponent();
			SetEventInspect(eve);
		}

		async void SetEventInspect(Event eve)
		{
			eve = await _dataManager.EventApiManager.GetEventById(eve.EventId);
			infoView.Content = new InspectEvent(eve, _dataManager.IsEventJoined(eve), scrollView);

			if (_dataManager.IsEventJoined(eve))
			{
				moreLayout.IsVisible = true;
				StackLayout wall = new StackLayout();
				conversationView = new ConversationView(eve.Comments, MessageApiManager.CommentType.EventComment, eve.EventId, wall);
				wall.Children.Add(conversationView);
				moreLayout.Children.Add(wall);
			}


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
				addNewElement(new ListsAndButtons(eve.Attendees, null, null, true), "Attendees");
			}
		}

		void addNewElement(View element, string Title)
		{
			Grid grid = new Grid() { RowSpacing=0, BackgroundColor = Color.White};
			element.BackgroundColor = App.HowlOutBackground;
			grid.RowDefinitions.Add(new RowDefinition { Height = 1});
			grid.RowDefinitions.Add(new RowDefinition { Height = 40});
			grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			grid.Children.Add(new Line(), 0, 0);
			grid.Children.Add(new Label() { Text = "  " + Title, VerticalTextAlignment = TextAlignment.Center, TranslationY = 5 },0,1);
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

