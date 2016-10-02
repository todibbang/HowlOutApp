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

		Button profilesButton = new Button {Text= "Friends", BackgroundColor= Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor=App.HowlOut, FontSize = 16};
		Button groupsButton = new Button { Text = "WolfPacks", BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.HowlOut, FontSize = 16 };
		Button eventsButton = new Button { Text = "Events", BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.HowlOut, FontSize = 16 };
		Button wallButton = new Button { Text = "Wall", BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.HowlOut, FontSize = 16 };

		public InspectController(Profile userProfile, Group userGroup, Event eventObject)
		{
			InitializeComponent();
			setInfo(userProfile, userGroup, eventObject);

			eventsButton.Clicked += (sender, e) =>
			{
				eventsGrid.Children.Add(new EventView(10));
			};


			postCommentButton.Clicked += (sender, e) =>
			{
				PostNewComment(userGroup, eventObject, commentEntry.Text);
			};
		}

		private async void PostNewComment(Group group, Event eve, string comment)
		{
			if (!string.IsNullOrWhiteSpace(comment))
			{
				//TODO changed this to recieve comment object instead of event
				var commentObj = new Comment
				{
					Content = comment,
					SenderID = App.StoredUserFacebookId,
					DateAndTime = DateTime.Now.ToLocalTime()
				};

				List<Comment> updatedComments = null;
				if (group != null)
				{
					updatedComments = await _dataManager.GroupApiManager.AddCommentToGroup(group.GroupId, commentObj);
				}
				else if (eve != null)
				{
					updatedComments = await _dataManager.EventApiManager.AddCommentToEvent(eve.EventId, commentObj);
				}


				if (updatedComments != null)
				{
					createWall(updatedComments);
					commentEntry.Text = "";
				}
				else {
					await App.coreView.displayAlertMessage("Comment Not Posted", "An error happened and the comment was not posted, try again.", "Ok");
				}
			}
		}

		private void createWall(List<Comment> comments)
		{
			if (comments != null)
			{
				while (WallList.Children.Count != 0)
				{
					WallList.Children.RemoveAt(0);
				}
				for (int i = comments.Count - 1; i > -1; i--)
				{
					WallList.Children.Add(new InspectManageComment(comments[i]));
				}
			}
		}

		private async void setInfo(Profile userProfile, Group userGroup, Event eventObject)
		{
			

			usersPhoto.Source = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=150&width=150";
			if (userProfile != null)
			{
				if (userProfile.ProfileId == App.userProfile.ProfileId)
				{
					App.setOptionsGrid(optionGrid, new List<Button> { profilesButton, groupsButton }, new List<VisualElement>{profileGrid, groupGrid});
					App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile(App.userProfile.ProfileId);
					userProfile = App.userProfile;
				}
				else {
					userProfile = await _dataManager.ProfileApiManager.GetProfile(userProfile.ProfileId);
					if (_dataManager.IsProfileFriend(userProfile))
					{
						App.setOptionsGrid(optionGrid, new List<Button> { profilesButton, groupsButton, eventsButton}, new List<VisualElement> { profileGrid, groupGrid, eventsGrid});
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
				App.setOptionsGrid(optionGrid, new List<Button> { profilesButton, wallButton, eventsButton }, new List<VisualElement> { profileGrid, wallGrid, eventsGrid });
				profilesButton.Text = "Members";
				givenCommentList = userGroup.Comments;
				infoView.Content = new ProfileDesignView(null, userGroup, null, 200, ProfileDesignView.Design.Inspect, ProfileDesignView.Show.Group, false);
				App.coreView.topBar.setNavigationLabel("Wolf pack " + userGroup.Name, scrollView);
			}
			else if (eventObject != null)
			{

				profilesButton.Text = "Attendees";

				eventObject = await _dataManager.EventApiManager.GetEventById(eventObject.EventId);
				if (!_dataManager.IsEventJoined(eventObject))
				{
					App.setOptionsGrid(optionGrid, new List<Button> { profilesButton }, new List<VisualElement> { profileGrid });
				}
				else {
					App.setOptionsGrid(optionGrid, new List<Button> { profilesButton, wallButton }, new List<VisualElement> { profileGrid, wallGrid });
				}

				if (eventObject.Attendees != null)
				{
					foreach (Profile p in eventObject.RequestingToJoin)
					{
						profileList.Add(p);
					}
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
			createWall(givenCommentList);
		}





		public ScrollView getScrollView()
		{
			return scrollView;
		}
	}
}

