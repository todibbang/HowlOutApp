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

		public InspectController(Profile userProfile, Group userGroup, Event eventObject)
		{
			InitializeComponent();
			setInfo(userProfile, userGroup, eventObject);
		}

		private async void setInfo(Profile userProfile, Group userGroup, Event eventObject)
		{
			if (userProfile != null)
			{
				if (userProfile.ProfileId == App.userProfile.ProfileId)
				{
					App.setOptionsGrid(optionGrid, new List<string> { "Friends", "Wolfpacks" }, new List<VisualElement>{profileGrid, groupGrid}, new List<Action> { null, null });
					App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile(App.userProfile.ProfileId);
					userProfile = App.userProfile;
				}
				else {
					userProfile = await _dataManager.ProfileApiManager.GetProfile(userProfile.ProfileId);
					if (_dataManager.IsProfileFriend(userProfile))
					{
						App.setOptionsGrid(optionGrid, new List<string> { "Friends", "Wolfpacks", "Events" }, new List<VisualElement> { profileGrid, groupGrid, eventsGrid }, new List<Action> {null, null, ()=> addEvents(userProfile.ProfileId)});
					}
				}
				listMaker.createList(profileGrid, userProfile.Friends, null, ListsAndButtons.ListType.Normal, null, null);
				listMaker.createList(groupGrid, null, userProfile.Groups, ListsAndButtons.ListType.Normal, null, null);

				infoView.Content = new ProfileDesignView(userProfile, null, null, 200, ProfileDesignView.Design.Inspect, ProfileDesignView.Show.Profile, false);
				App.coreView.topBar.setNavigationLabel(userProfile.Name, scrollView);
			}
			else if (userGroup != null)
			{
				userGroup = await _dataManager.GroupApiManager.GetGroupById(userGroup.GroupId);
				profileList.Add(userGroup.Owner);
				if (userGroup.Members != null)
				{
					for (int i = 0; i < userGroup.Members.Count; i++)
					{
						profileList.Add(userGroup.Members[i]);
					}
				}
				listMaker.createList(profileGrid, profileList, null, ListsAndButtons.ListType.Normal, null, null);
				App.setOptionsGrid(optionGrid, new List<string> { "Members", "Wall" }, new List<VisualElement> { profileGrid, wallGrid }, new List<Action> { null, null});
				givenCommentList = userGroup.Comments;
				infoView.Content = new ProfileDesignView(null, userGroup, null, 200, ProfileDesignView.Design.Inspect, ProfileDesignView.Show.Group, false);
				App.coreView.topBar.setNavigationLabel("Wolfpack " + userGroup.Name, scrollView);
			}
			else if (eventObject != null)
			{
				eventObject = await _dataManager.EventApiManager.GetEventById(eventObject.EventId);
				if (!_dataManager.IsEventJoined(eventObject))
				{
					App.setOptionsGrid(optionGrid, new List<string> { "Attendees" }, new List<VisualElement> { profileGrid }, new List<Action> { null });
				}
				else {
					App.setOptionsGrid(optionGrid, new List<string> { "Attendees", "Wall" }, new List<VisualElement> { profileGrid, wallGrid }, new List<Action> { null, null });
				}

				if (eventObject.Attendees != null)
				{
					for (int i = 0; i < eventObject.Attendees.Count; i++)
					{
						profileList.Add(eventObject.Attendees[i]);
					}
				}
				if (_dataManager.IsEventYours(eventObject))
				{
					listMaker.createList(profileGrid, profileList, null, ListsAndButtons.ListType.EventAttendeesSeenAsOwner, null, null);
				}
				else {
					listMaker.createList(profileGrid, profileList, null, ListsAndButtons.ListType.Normal, null, null);
				}

				infoView.Content = new InspectEvent(eventObject, _dataManager.IsEventJoined(eventObject), scrollView);

				if (eventObject.Owner != null)
				{
					App.coreView.topBar.setNavigationLabel(eventObject.Owner.Name + "'s Event", scrollView);
				}
				else if (eventObject.OrganisationOwner != null)
				{
					App.coreView.topBar.setNavigationLabel(eventObject.OrganisationOwner.Name + "'s Event", scrollView);
				}
			}
			Conversation conv = new Conversation()
			{
				Comments = givenCommentList,
				ConversationID = "123467",
			};



			createWall(conv);
		}

		private void addEvents(string id)
		{
			eventsGrid.Children.Clear();
			eventsGrid.Children.Add(new EventView(10, id));
		}

		private void createWall(Conversation conv)
		{
			wallGrid.Children.Clear();
			wallGrid.Children.Add(new ConversationView(conv, true));
		}

		public ScrollView getScrollView()
		{
			return scrollView;
		}
	}
}

