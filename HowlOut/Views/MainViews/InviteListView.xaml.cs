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
			App.coreView.IsLoading(true);
			conversationSetup(conversation);
			if (create)
			{
				toolBoxOptionsLayout.IsVisible = true;
				titleOptionLayout.IsVisible = true;

				addBtn.Text = "Create";
				addBtn.Clicked += async (sender, e) =>
				{
					profilesAdded.Add(App.userProfile);
					App.coreView.IsLoading(true);
					App.coreView.returnToPreviousView();

					if (toolBoxOptions.SelectedIndex == 1)
					{

						Conversation conv = new Conversation()
						{
							ModelType = ConversationModelType.Profile,
							ConversationID = "12344567",
							Messages = new List<Comment>(),
							LastMessage = null,
							ModelId = "23456",
							Profiles = profilesAdded,
							Title = "ExpenShare Test",
							SubType = ConversationSubType.ExpenShare,
							subTypeDictionary = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>()
						};

						foreach (Profile p in conv.Profiles)
						{
							conv.subTypeDictionary.Add(p.ProfileId, new List<Tuple<string, string, string, StatusOptions>>());
						}

						App.coreView.setContentViewWithQueue(new ConversationView(conv));
					}
					else if (toolBoxOptions.SelectedIndex == 2)
					{


						var dick = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>();
						dick.Add("Options", new List<Tuple<string, string, string, StatusOptions>>()
						{
							Tuple.Create("0","0","", StatusOptions.Confirmed),
							//Tuple.Create("2","0","bla", StatusOptions.Confirmed),
							//Tuple.Create("3","0","bla bla", StatusOptions.Confirmed),
							//Tuple.Create("4","0","bla bla bla", StatusOptions.Confirmed),
						});

						dick.Add("OptionSettings", new List<Tuple<string, string, string, StatusOptions>>()
						{
							Tuple.Create("0","0","", StatusOptions.Confirmed),
						});

						foreach (Profile p in profilesAdded)
						{
							dick.Add(p.ProfileId, new List<Tuple<string, string, string, StatusOptions>>()
						{

						});
						}

						Conversation conv = new Conversation()
						{
							ModelType = ConversationModelType.Profile,
							ConversationID = "12344568",
							Messages = new List<Comment>(),
							LastMessage = null,
							ModelId = "234567",
							Profiles = profilesAdded,
							Title = "Doodle Test",
							SubType = ConversationSubType.Doodle,
							subTypeDictionary = dick,
						};

						App.coreView.setContentViewWithQueue(new DoodleView(conv));
					}

					else if (toolBoxOptions.SelectedIndex == 3)
					{
						var proList = new List<Profile>();
						proList.AddRange(profilesAdded);

						Conversation conv = new Conversation { subTypeDictionary = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>(), Profiles = proList};
						conv.subTypeDictionary.Add("ToDoList", new List<Tuple<string, string, string, StatusOptions>>());
						conv.subTypeDictionary.Add("ToDoListInfo", new List<Tuple<string, string, string, StatusOptions>>());
						conv.subTypeDictionary["ToDoList"].Add( Tuple.Create("", "0", "", StatusOptions.NotStarted));
						conv.subTypeDictionary["ToDoListInfo"].Add(Tuple.Create("1", "0", "", StatusOptions.NotStarted));

						App.coreView.setContentViewWithQueue(new ToDoListView(conv));
					}
					else 
					{
						Conversation conv = await _dataManager.MessageApiManager.CreateConversations(conversation.ModelType, profilesAdded, conversation.ModelId, titleOption.Text);
						if (conv != null)
						{
							App.coreView.setContentViewWithQueue(new ConversationView(conv));
						}

					}
					App.coreView.IsLoading(false);
				};
			}
			else {
				addBtn.Clicked += async (sender, e) =>
				{
					App.coreView.IsLoading(true);
					App.coreView.returnToPreviousView();
					Conversation newConv = await _dataManager.MessageApiManager.AddProfilesToConversation(conversation.ConversationID, profilesAdded);
					if (newConv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(newConv));
					}
					App.coreView.IsLoading(false);
				};
			}

		}
		public void reloadView() { }
		public void viewInFocus(UpperBar bar)
		{
			bar.setNavigationlabel("Invite To Organization");
			bar.setNavigationlabel("Invite To Group");
			bar.setNavigationlabel("Invite To Event");
			bar.setNavigationlabel("Add People To Conversation");
			bar.setNavigationlabel("Create New Conversation");
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
					profilesThatCanBeAdded.Clear();
					profilesThatCanBeAdded.AddRange(App.userProfile.Friends);
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
			App.coreView.IsLoading(true);
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
				profilesThatCanBeAdded.Clear();
				profilesThatCanBeAdded.AddRange(App.userProfile.Friends);
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
			App.coreView.IsLoading(true);
			findPeopleToAddToGroup(grp, owner);

			addBtn.Clicked += async (sender, e) =>
			{

				if (profilesAdded.Count > 0)
				{
					bool success = false;
					if (owner)
					{
						success = await _dataManager.GroupApiManager.InviteToGroupAsOwner(grp.GroupId, profilesAdded);
					}
					else {
						success = await _dataManager.GroupApiManager.InviteDeclineToGroup(grp.GroupId, true, profilesAdded);
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
				profilesThatCanBeAdded.Clear();
				profilesThatCanBeAdded.AddRange(App.userProfile.Friends);
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
			for (int p = 0; p < profilesThatCanBeAdded.Count; p++)
			{
				if (string.IsNullOrEmpty(profilesThatCanBeAdded[p].Name) || string.IsNullOrEmpty(profilesThatCanBeAdded[p].ImageSource))
				{
					profilesThatCanBeAdded[p] = await _dataManager.ProfileApiManager.GetProfile(profilesThatCanBeAdded[p].ProfileId);
				}
			}
			App.coreView.IsLoading(false);

			profilesToBeAdded.ItemsSource = profilesThatCanBeAdded;
			profilesToBeAdded.ItemSelected += OnPeopleToAddListItemSelected;
			addedToConversationList.ItemSelected += OnAddedPeopleListItemSelected;
			cancelBtn.Clicked += (sender, e) =>
			{
				App.coreView.returnToPreviousView();
			};
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
