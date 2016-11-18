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
			conversationSetup(conversation);
			if (create)
			{
				addBtn.Text = "Create";
				addBtn.Clicked += async (sender, e) =>
				{
					App.coreView.returnToPreviousView();
					profilesAdded.Add(App.userProfile);
					Conversation conv = await _dataManager.MessageApiManager.CreateConversations(conversation.ModelType, profilesAdded, conversation.ModelId, "");
					if (conv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(conv), "", null);
					}
				};
				App.coreView.topBar.setNavigationLabel("Create New Conversation", null);
			}
			else {
				addBtn.Clicked += async (sender, e) =>
				{
					App.coreView.returnToPreviousView();
					Conversation newConv = await _dataManager.MessageApiManager.AddProfilesToConversation(conversation.ConversationID, profilesAdded);
					if (newConv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(newConv), "", null);
					}
				};
				App.coreView.topBar.setNavigationLabel("Add People To Conversation", null);
			}

		}

		async void conversationSetup(Conversation conversation)
		{
			//Normal Conversation
			if (conversation.ModelType == ConversationModelType.Profile)
			{
				profilesThatCanBeAdded = App.userProfile.Friends;
			}
			//Event Conversation
			if (conversation.ModelType == ConversationModelType.Event)
			{
				var eve = await _dataManager.EventApiManager.GetEventById(conversation.ModelId);
				profilesThatCanBeAdded = eve.Attendees;
				if (eve.ProfileOwner != null && !profilesThatCanBeAdded.Exists(prof => prof.ProfileId == eve.ProfileOwner.ProfileId)) profilesThatCanBeAdded.Add(eve.ProfileOwner);
				else if (eve.OrganizationOwner != null)
				{
					foreach (Profile p in eve.OrganizationOwner.Members)
					{
						if(!profilesThatCanBeAdded.Exists(prof => prof.ProfileId == p.ProfileId))profilesThatCanBeAdded.Add(p);
					}
				}
			}
			//Group Conversation
			if (conversation.ModelType == ConversationModelType.Group)
			{
				var grp = await _dataManager.GroupApiManager.GetGroup(conversation.ModelId);
				profilesThatCanBeAdded = grp.Members;
				if (grp.ProfileOwner != null && !profilesThatCanBeAdded.Exists(prof => prof.ProfileId == grp.ProfileOwner.ProfileId)) profilesThatCanBeAdded.Add(grp.ProfileOwner);
				else if (grp.OrganizationOwner != null && grp.OrganizationOwner.Members != null && grp.OrganizationOwner.Members.Count > 0)
				{
					foreach (Profile p in grp.OrganizationOwner.Members)
					{
						if (!profilesThatCanBeAdded.Exists(prof => prof.ProfileId == p.ProfileId))profilesThatCanBeAdded.Add(p);
					}
				}
			}

			//Organization Conversation
			if (conversation.ModelType == ConversationModelType.Organization)
			{
				var org = await _dataManager.OrganizationApiManager.GetOrganization(conversation.ModelId);
				profilesThatCanBeAdded = org.Members;
			}

			profilesThatCanBeAdded.RemoveAll(p => p.ProfileId == App.StoredUserFacebookId);
			setup();
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
			App.coreView.topBar.setNavigationLabel("Invite To Event", null);
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
					bool success = await _dataManager.GroupApiManager.InviteDeclineToGroup(grp.GroupId, true, profilesAdded);
					if (success)
					{
						App.coreView.returnToPreviousView();
					}
				}
			};
			App.coreView.topBar.setNavigationLabel("Invite To Group", null);
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
					await _dataManager.OrganizationApiManager.InviteToOrganization(org.OrganizationId, p);
				}
				App.coreView.returnToPreviousView();
			};
			App.coreView.topBar.setNavigationLabel("Invite To Organization", null);
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
