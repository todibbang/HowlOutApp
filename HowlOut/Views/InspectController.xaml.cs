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
		ListsAndButtons listMaker = new ListsAndButtons();
		private DataManager _dataManager = new DataManager();
		public CreateEvent createEventView;
		List<Comment> givenCommentList = new List<Comment>();
		List<Profile> profileList = new List<Profile>();

		public InspectController(Profile userProfile)
		{
			InitializeComponent();
			SetProfileInspect(userProfile);
		}

		async void SetProfileInspect(Profile userProfile)
		{
			if (userProfile.ProfileId == App.userProfile.ProfileId)
			{
				App.setOptionsGrid(optionGrid, new List<string> { "Friends", "Groups", "Organizations" }, new List<VisualElement> { profileGrid, groupGrid, orgGrid }, new List<Action> { 
					()=> {listMaker.createList(profileGrid, App.userProfile.Friends, null, null, null, null, null); },
					()=> {listMaker.createList(groupGrid, null, App.userProfile.Groups, null, null, null, null);} , 
					()=> {listMaker.createList(orgGrid, null, null, App.userProfile.Organizations, null, null, null);} }, null);
				App.coreView.GetLoggedInProfile();
				infoView.Content = new ProfileDesignView(userProfile, 200, false);
				App.coreView.topBar.setNavigationLabel(userProfile.Name, scrollView);
			}
			else {
				userProfile = await _dataManager.ProfileApiManager.GetProfile(userProfile.ProfileId);
				infoView.Content = new ProfileDesignView(userProfile, 200, false);
				if (_dataManager.IsProfileFriend(userProfile))
				{
					App.setOptionsGrid(optionGrid, new List<string> { "Friends", "Groups", "Events" }, new List<VisualElement> { profileGrid, groupGrid, eventsGrid }, new List<Action> { null, null, () => addEvents(userProfile) }, null);
				}
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
			profileList.Add(userGroup.ProfileOwner);
			if (userGroup.Members != null)
			{
				for (int i = 0; i < userGroup.Members.Count; i++)
				{
					profileList.Add(userGroup.Members[i]);
				}
			}
			listMaker.createList(profileGrid, profileList , null, null, null, null, null);
			App.setOptionsGrid(optionGrid, new List<string> { "Members", "Wall" }, new List<VisualElement> { profileGrid, wallGrid }, new List<Action> { null, null }, null);
			givenCommentList = userGroup.Comments;
			infoView.Content = new GroupDesignView(userGroup, 200);
			App.coreView.topBar.setNavigationLabel("Wolfpack " + userGroup.Name, scrollView);
		}


		public InspectController(Organization organization)
		{
			InitializeComponent();
			SetOrganizationInspect(organization);
		}

		async void SetOrganizationInspect(Organization organization)
		{
			listMaker.createList(profileGrid, organization.Members, null, null, null, null, null);
			infoView.Content = new OrganizationDesignView(organization, 200);
			App.coreView.topBar.setNavigationLabel("Wolfpack " + organization.Name, scrollView);
			//App.setOptionsGrid(optionGrid, new List<string> { "Events" }, new List<VisualElement> { eventsGrid }, new List<Action> { () => addEvents(userProfile.ProfileId) }, null);
		}

		public InspectController(Event eve)
		{
			InitializeComponent();
			SetEventInspect(eve);
		}

		async void SetEventInspect(Event eve)
		{
			eve = await _dataManager.EventApiManager.GetEventById(eve.EventId);
			if (!_dataManager.IsEventJoined(eve))
			{
				App.setOptionsGrid(optionGrid, new List<string> { "Attendees" }, new List<VisualElement> { profileGrid }, new List<Action> { null }, null);
			}
			else {
				App.setOptionsGrid(optionGrid, new List<string> { "Attendees", "Wall" }, new List<VisualElement> { profileGrid, wallGrid }, new List<Action> { null, null }, null);
				createWall(eve.Comments, MessageApiManager.CommentType.EventComment, eve.EventId);
			}

			if (eve.Attendees != null)
			{
				for (int i = 0; i < eve.Attendees.Count; i++)
				{
					profileList.Add(eve.Attendees[i]);
				}
			}
			if (_dataManager.IsEventYours(eve))
			{
				listMaker.createList(profileGrid, profileList, null, null, null, null, null);
			}
			else {
				listMaker.createList(profileGrid, profileList, null, null, null, null, null);
			}

			infoView.Content = new InspectEvent(eve, _dataManager.IsEventJoined(eve), scrollView);

			if (eve.ProfileOwner != null)
			{
				App.coreView.topBar.setNavigationLabel(eve.ProfileOwner.Name + "'s Event", scrollView);
			}
			else if (eve.OrganizationOwner != null)
			{
				App.coreView.topBar.setNavigationLabel(eve.OrganizationOwner.Name + "'s Event", scrollView);
			}
		}



		private void addEvents(Profile pro)
		{
			eventsGrid.Children.Clear();
			eventsGrid.Children.Add(new EventListView(pro));
		}

		private void createWall(List<Comment> comments, MessageApiManager.CommentType type, string id)
		{
			wallGrid.Children.Clear();
			wallGrid.Children.Add(new ConversationView(comments, type, id, wallGrid));
		}

		public ScrollView getScrollView()
		{
			return null;
		}
	}
}

