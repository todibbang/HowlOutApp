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
		public void reloadView() { }
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
				try
				{
					List<string> fbf = await DependencyService.Get<SocialController>().getFacebookFriends();
					List<Profile> fbp = new List<Profile>();
					foreach (string s in fbf)
					{
						fbp.Add(new Profile() { ProfileId = s });
					}
					profilesThatCanBeAdded.AddRange(fbp);
				}
				catch (Exception exc) { }
			}
			//Event Conversation
			if (conversation.ModelType == ConversationModelType.Event)
			{
				var eve = await _dataManager.EventApiManager.GetEventById(conversation.ModelId);
				profilesThatCanBeAdded = eve.Attendees;
				if (eve.ProfileOwners != null)
				{
					foreach (Profile p in eve.ProfileOwners)
					{
						if (!profilesThatCanBeAdded.Exists(prof => prof.ProfileId == p.ProfileId)) profilesThatCanBeAdded.Add(p);
					}
				}
				else if (eve.GroupOwner != null && eve.GroupOwner.ProfileOwners != null)
				{
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

			profilesThatCanBeAdded.RemoveAll(p => p.ProfileId == App.StoredUserFacebookId);
			setup();
		}

		public InviteListView(Event eve, bool owner)
		{
			InitializeComponent();
			findPeopleToAddToEvent(eve, owner);

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

		private async void findPeopleToAddToEvent(Event eve, bool owner)
		{
			try
			{
				List<string> fbf = await DependencyService.Get<SocialController>().getFacebookFriends();
				List<Profile> fbp = new List<Profile>();
				foreach (string s in fbf)
				{
					if (!profilesThatCanBeAdded.Exists(p => p.ProfileId == s))
					{
						try
						{
							Profile pro = await _dataManager.ProfileApiManager.GetProfile(s);
							if (pro != null)
							{
								fbp.Add(pro);
							}
						}
						catch (Exception otherexc) { }
					}
				}
				profilesThatCanBeAdded.AddRange(fbp);
			}
			catch (Exception exc) { }
			

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
		}


		public InviteListView(Group grp, bool owner)
		{
			InitializeComponent();
			findPeopleToAddToGroup(grp, owner);

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

		private async void findPeopleToAddToGroup(Group grp, bool owner)
		{
			try
			{
				List<string> fbf = await DependencyService.Get<SocialController>().getFacebookFriends();
				List<Profile> fbp = new List<Profile>();
				foreach (string s in fbf)
				{
					if (!profilesThatCanBeAdded.Exists(p => p.ProfileId == s))
					{
						try
						{
							Profile pro = await _dataManager.ProfileApiManager.GetProfile(s);
							if (pro != null)
							{
								fbp.Add(pro);
							}
						}
						catch (Exception otherexc) { }
					}
				}
				profilesThatCanBeAdded.AddRange(fbp);
			}
			catch (Exception exc) { }


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
		}

		async void setup()
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
