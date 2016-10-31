using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class InviteListView : ContentView
	{
		List<Profile> profilesAdded = new List<Profile>();
		List<Profile> profilesThatCanBeAdded = App.userProfile.Friends;
		DataManager _dataManager = new DataManager();

		public InviteListView(Conversation conversation, bool create)
		{
			InitializeComponent();
			if (create)
			{
				addBtn.Text = "Create";
			}
			else {
				foreach (Profile p in conversation.Profiles)
				{
					profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
				}
			}
			setup();
			addBtn.Clicked += async (sender, e) =>
			{
				App.coreView.returnToPreviousView();
				if (create)
				{
					profilesAdded.Add(App.userProfile);
					Conversation conv = await _dataManager.MessageApiManager.CreateConversations(profilesAdded);
					if (conv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(conv), "", null);
					}
				}
				else {
					Conversation newConv = await _dataManager.MessageApiManager.AddProfilesToConversation(conversation.ConversationID, profilesAdded);
					if (newConv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(newConv), "", null);
					}
				}
			};

		}
		public InviteListView(Event eve)
		{
			InitializeComponent();
			foreach (Profile p in eve.Attendees)
			{
				profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
			}
			setup();
			addBtn.Clicked += async (sender, e) =>
			{
				if (profilesAdded.Count > 0)
				{
					bool success = await _dataManager.EventApiManager.InviteProfilesToEvent(eve.EventId, profilesAdded);
					if (success)
					{
						App.coreView.returnToPreviousView();
					}
				}
			};
		}
		public InviteListView(Group grp)
		{
			InitializeComponent();
			foreach (Profile p in grp.Members)
			{
				profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
			}
			setup();
			addBtn.Clicked += async (sender, e) =>
			{
				if (profilesAdded.Count > 0)
				{
					bool success = await _dataManager.GroupApiManager.InviteAcceptDeclineLeaveGroup(grp.GroupId, profilesAdded, GroupApiManager.GroupHandlingType.Invite);
					if (success)
					{
						App.coreView.returnToPreviousView();
					}
				}
			};
		}
		public InviteListView(Organization org)
		{
			InitializeComponent();
			foreach (Profile p in org.Members)
			{
				profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
			}
			setup();
			addBtn.Clicked += async (sender, e) =>
			{
				foreach (Profile p in profilesAdded)
				{
					await _dataManager.OrganizationApiManager.AcceptInviteDeclineLeaveOrganization(org.OrganizationId, p.ProfileId, OrganizationApiManager.OrganizationHandlingType.Invite);
				}
				App.coreView.returnToPreviousView();
			};
		}

		void setup()
		{
			profilesToBeAdded.ItemsSource = profilesThatCanBeAdded;
			profilesToBeAdded.ItemSelected += OnPeopleToAddListItemSelected;
			addedToConversationList.ItemSelected += OnAddedPeopleListItemSelected;
			cancelBtn.Clicked += (sender, e) => { App.coreView.returnToPreviousView(); };
		}

		public void OnPeopleToAddListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (profilesToBeAdded.SelectedItem == null) { return; }
			var selectedProfile = profilesToBeAdded.SelectedItem as Profile;
			if (!profilesAdded.Exists(p => p.ProfileId == selectedProfile.ProfileId))
			{
				profilesAdded.Add(selectedProfile);
				addedToConversationList.ItemsSource = null;
				addedToConversationList.ItemsSource = profilesAdded;
			}
			profilesToBeAdded.SelectedItem = null;
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
