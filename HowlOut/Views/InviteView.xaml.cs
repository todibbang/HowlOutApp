using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Net.Http;
using ModernHttpClient;
using ImageCircle.Forms.Plugin.Abstractions;

namespace HowlOut
{
	public partial class InviteView : ContentView
	{
		EventApiManager eventApiManager= new EventApiManager (new HttpClient(new NativeMessageHandler()));

		ListsAndButtons listMaker = new ListsAndButtons();
		DataManager dataManager = new DataManager();

		ObservableCollection <Button> friendButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> acceptButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> declineButtons = new ObservableCollection <Button>();
		Button friendRequestButton = new Button ();


		public InviteView (Profile userProfile, Group userGroup, Event eventObject, List<Profile> profilesToSelectFrom)
		{
			InitializeComponent ();

			Dictionary<Profile, string> profilesNotToInvite = new Dictionary<Profile, string> { };
			ObservableCollection <Profile> profilesToInvite = new ObservableCollection <Profile>();

			profileGrid.IsVisible = true;

			if(userProfile != null){
				for (int e = 0; e < userProfile.Friends.Count; e++) {
					profilesNotToInvite.Add (userProfile.Friends [e], userProfile.Friends [e].ProfileId);
				}
			} else if (userGroup != null) { 
				for (int e = 0; e < userGroup.Members.Count; e++) {
					profilesNotToInvite.Add (userGroup.Members [e], userGroup.Members [e].ProfileId);
				}
			} else if (eventObject != null) { 
				for (int e = 0; e < eventObject.Attendees.Count; e++) {
					profilesNotToInvite.Add (eventObject.Attendees [e], eventObject.Attendees [e].ProfileId);
				}
			}

			for (int i = profilesToSelectFrom.Count - 1; i > -1; i--) {
				if (profilesNotToInvite.ContainsValue (profilesToSelectFrom [i].ProfileId)) {
					profilesToSelectFrom.Remove (profilesToSelectFrom [i]);
				}
			}

			listMaker.createList (profileGrid, profilesToSelectFrom, null, friendButtons, acceptButtons, declineButtons, userProfile, friendRequestButton);
			int counter = 0;
			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					Profile profile = null;
					if (userProfile != null) {
						profile = profilesToSelectFrom[counter];
					} 
					else if(userGroup != null) { 
						profile = profilesToSelectFrom[counter];
					} 
					else if(eventObject != null) { 
						profile = profilesToSelectFrom[counter];
					}
					System.Diagnostics.Debug.WriteLine(profile.Name + "2");
					App.coreView.setContentView (new UserProfile (profile, null, null), "UserProfile");
				};
				counter++;
			}

			inviteButton.Clicked += (sender, e) => 
			{

				System.Diagnostics.Debug.WriteLine("Blykaman");

				if(userGroup != null) {
					sendInviteToGroup(userGroup, profilesToInvite);
				} else if(eventObject != null) {
					sendInviteToEvent(eventObject, profilesToInvite);
				}

			};
		}


		private async void sendInviteToEvent(Event eve, ObservableCollection<Profile> profiles)
		{
			List <string> IdsToInvite = new List<string> ();

			for (int i = 0; i < profiles.Count; i++) {
				IdsToInvite.Add (profiles[i].ProfileId);
			}

			/*
			for (int i = 0; i < groupsToInvite.Count; i++) {
				for (int e = 0; e < groupsToInvite [i].Members.Count; e++) {
					IdsToInvite.Add (groupsToInvite [i].Members [e].ProfileId);
				}
			}
			*/
			await eventApiManager.InviteToEvent(eve.EventId, IdsToInvite);
		}

		private async void sendInviteToGroup(Group group, ObservableCollection<Profile> profiles)
		{
			GroupApiManager groupManager = new GroupApiManager ();
			List <string> IdsToInvite = new List<string> ();

			for (int i = 0; i < profiles.Count; i++) {
				IdsToInvite.Add (profiles[i].ProfileId);
			}
			//await groupManager.InviteToEvent(group.GroupId, IdsToInvite);
		}

		public ObservableCollection<Button> createList(Grid grid, List<Profile> profiles)
		{
			int count = 0;
			count = profiles.Count;

			ObservableCollection<Button> buttons = new ObservableCollection<Button> ();

			Grid newGrid = new Grid {
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto }
				},
				RowDefinitions = {
					new RowDefinition{ Height = 150 },
					new RowDefinition{ Height = 150 },
					new RowDefinition{ Height = 150 }
				},
				RowSpacing=0,
				ColumnSpacing=0,
			};

			int subjectNr = 0;
			int column = 0;
			int row = 0;

			for (int i = 0; i < count; i++) {
				if (column == 3) {
					column = 0;
					row++;
					grid.RowDefinitions.Add (new RowDefinition{ Height = 150 });
				}

				Grid cell = new Grid ();
				Button button = new Button ();

				cell = friendCellCreator (profiles [subjectNr]);
				button = buttonCreator (subjectNr);
				friendButtons.Add (button);

				//adds the whole cell to the grid
				grid.Children.Add (cell, column, row);
				grid.Children.Add (button, column, row);

				column ++;
				subjectNr++;
			}
			grid = newGrid;

			return buttons;
		}

		private Button buttonCreator(int i)
		{

			Button button = new Button {
				Text = "" + i,
				TextColor = Color.Transparent,
				BackgroundColor = Color.Transparent,
				BorderWidth=4,
				BorderColor = Color.Red,

			};
			return button;
		}

		private Grid friendCellCreator(Profile profile)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				RowDefinitions = {
					new RowDefinition{ Height = 10 },
					new RowDefinition{ Height = 85 },
					new RowDefinition{ Height = 40 },
				}
			};

			//adds the users profile picture
			var profilePicUri = dataManager.GetFacebookProfileImageUri(profile.ProfileId);
			//var profilePicUri = dataManager.GetFacebookProfileImageUri(App.StoredUserFacebookId);
			CircleImage profilePicture = new CircleImage {
				HeightRequest = 85,
				WidthRequest = 85,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				Source = ImageSource.FromUri (profilePicUri)
			};
			cellGrid.Children.Add (profilePicture, 0,1);

			cellGrid.Children.Add(new Label {
				Text = profile.Name + ", " + profile.Age,
				TextColor = Color.Black,
				FontSize = 10,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			}, 0, 2);

			return cellGrid;
		}

	}
}

