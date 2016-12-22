using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class InviteListView : ContentView, ViewModelInterface
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
						App.coreView.setContentViewWithQueue(new ConversationView(conv));
					}
				};
			}
			else {
				addBtn.Clicked += async (sender, e) =>
				{
					App.coreView.returnToPreviousView();
					Conversation newConv = await _dataManager.MessageApiManager.AddProfilesToConversation(conversation.ConversationID, profilesAdded);
					if (newConv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(newConv));
					}
				};
			}

		}

		public void viewInFocus(UpperBar bar)
		{
			bar.setNavigationLabel("Invite To Organization", null);
			bar.setNavigationLabel("Invite To Group", null);
			bar.setNavigationLabel("Invite To Event", null);
			bar.setNavigationLabel("Add People To Conversation", null);
			bar.setNavigationLabel("Create New Conversation", null);
		}

		public void viewExitFocus() { }

		public ContentView getContentView() { return this; }

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
				if (eve.ProfileOwners != null) {
					foreach (Profile p in eve.GroupOwner.ProfileOwners)
					{
						if (!profilesThatCanBeAdded.Exists(prof => prof.ProfileId == p.ProfileId)) profilesThatCanBeAdded.Add(p);
					}
				}
			}
			//Group Conversation
			if (conversation.ModelType == ConversationModelType.Group)
			{
				var grp = await _dataManager.GroupApiManager.GetGroup(conversation.ModelId);
				profilesThatCanBeAdded = grp.Members;
				if (grp.ProfileOwners != null)
				{
					foreach (Profile p in grp.ProfileOwners)
					{
						if (!profilesThatCanBeAdded.Exists(prof => prof.ProfileId == p.ProfileId)) profilesThatCanBeAdded.Add(p);
					}
				}
			}

			//Organization Conversation 
			/*
			if (conversation.ModelType == ConversationModelType.Organization)
			{
				var org = await _dataManager.OrganizationApiManager.GetOrganization(conversation.ModelId);
				profilesThatCanBeAdded = org.Members;
			} */

			profilesThatCanBeAdded.RemoveAll(p => p.ProfileId == App.StoredUserFacebookId);
			setup();
		}

		public InviteListView(Event eve, bool owner)
		{
			InitializeComponent();
			if (owner)
			{
				foreach (Profile p in eve.ProfileOwners)
				{
					profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
				}
			}
			else {
				foreach (Profile p in eve.Attendees)
				{
					profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
				}
			}

			setup();
			addBtn.Clicked += async (sender, e) =>
			{
				if (profilesAdded.Count > 0)
				{
					bool success = false;
					if (owner)
					{
						success = await _dataManager.EventApiManager.InviteToEventAsOwner(eve.EventId, profilesAdded);
					}
					else {
						success = await _dataManager.EventApiManager.InviteProfilesToEvent(eve.EventId, profilesAdded);
					}
					if (success)
					{
						App.coreView.returnToPreviousView();
					}
					else {
						App.coreView.displayAlertMessage("Error", "Error inviting.", "Ok");
					}
				}
			};
		}
		public InviteListView(Group grp, bool owner)
		{
			InitializeComponent();
			if (owner)
			{
				foreach (Profile p in grp.ProfileOwners)
				{
					profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
				}
			}
			else {
				foreach (Profile p in grp.Members)
				{
					profilesThatCanBeAdded.Remove(profilesThatCanBeAdded.Find(r => r.ProfileId == p.ProfileId));
				}
			}

			setup();
			addBtn.Clicked += async (sender, e) =>
			{
				if (profilesAdded.Count > 0)
				{
					bool success = false;
					if (owner)
					{
						await _dataManager.GroupApiManager.InviteToGroupAsOwner(grp.GroupId, profilesAdded);
					}
					else {
						await _dataManager.GroupApiManager.InviteDeclineToGroup(grp.GroupId, true, profilesAdded);
					}
					if (success)
					{
						App.coreView.returnToPreviousView();
					}
					else {
						App.coreView.displayAlertMessage("Error", "Error inviting.", "Ok");
					}
				}
			};

		}
		/*
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

		} */

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
