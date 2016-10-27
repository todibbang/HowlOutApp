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
					infoLayout.Children.Add(new Line());
					infoLayout.IsVisible = true;
					infoLayout.Children.Add(new Label() { Text = "  Friends", FontSize = 12 });
					infoLayout.Children.Add(new ListsAndButtons(userProfile.Friends, null, null, true));
				}

				if (userProfile.Groups.Count > 0)
				{
					infoLayout.Children.Add(new Line());
					infoLayout.IsVisible = true;
					infoLayout.Children.Add(new Label() { Text = "  Groups", FontSize = 12 });
					infoLayout.Children.Add(new ListsAndButtons(null, userProfile.Groups, null, true));
				}

				if (userProfile.Organizations.Count > 0)
				{
					infoLayout.Children.Add(new Line());
					infoLayout.IsVisible = true;
					infoLayout.Children.Add(new Label() { Text = "  Organizations", FontSize = 12 });
					infoLayout.Children.Add(new ListsAndButtons(null, null, userProfile.Organizations, true));
				}
			}
			else if (App.userProfile.Friends.Exists(p => p.ProfileId == userProfile.ProfileId))
			{
				if (userProfile.Friends.Count > 0)
				{
					infoLayout.Children.Add(new Line());
					infoLayout.IsVisible = true;
					infoLayout.Children.Add(new Label() { Text = "  Friends", FontSize = 12 });
					infoLayout.Children.Add(new ListsAndButtons(userProfile.Friends, null, null, true));
				}
				moreLayout.IsVisible = true;
				moreLayout.Children.Add(new EventListView(userProfile));
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
					infoLayout.Children.Add(new Line());
					infoLayout.IsVisible = true;
					infoLayout.Children.Add(new Label() { Text = "  Members", FontSize = 12 });
					infoLayout.Children.Add(new ListsAndButtons(userGroup.Members, null, null, true));
				}
				infoLayout.Children.Add(new Line());
				moreLayout.IsVisible = true;
				moreLayout.Children.Add(new EventListView(userGroup));
				StackLayout wall = new StackLayout();
				wall.Children.Add(new ConversationView(userGroup.Comments, MessageApiManager.CommentType.GroupComment, userGroup.GroupId, wall));
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
			App.coreView.topBar.setNavigationLabel("Wolfpack " + organization.Name, scrollView);

			if (organization.Members.Exists(p => p.ProfileId == App.userProfile.ProfileId))
			{
				if (organization.Members.Count > 0)
				{
					infoLayout.Children.Add(new Line());
					infoLayout.IsVisible = true;
					infoLayout.Children.Add(new Label() { Text = "  Members", FontSize = 12 });
					infoLayout.Children.Add(new ListsAndButtons(organization.Members, null, null, true));
				}
				infoLayout.Children.Add(new Line());
				moreLayout.IsVisible = true;
				moreLayout.Children.Add(new EventListView(organization));
				StackLayout wall = new StackLayout();
				wall.Children.Add(new ConversationView(organization.Comments, MessageApiManager.CommentType.OrganizationComment, organization.OrganizationId, wall));
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
				infoLayout.Children.Add(new Line());
				infoLayout.IsVisible = true;
				infoLayout.Children.Add(new Label() { Text = "  Attendees", FontSize = 12 });
				infoLayout.Children.Add(new ListsAndButtons(eve.Attendees, null, null, true));
			}
			if (_dataManager.IsEventJoined(eve))
			{
				moreLayout.IsVisible = true;
				StackLayout wall = new StackLayout();
				wall.Children.Add(new ConversationView(eve.Comments, MessageApiManager.CommentType.EventComment, eve.EventId, wall));
				moreLayout.Children.Add(wall);
			}
		}

		public ScrollView getScrollView()
		{
			return null;
		}
	}
}

