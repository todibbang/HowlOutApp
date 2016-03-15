using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class ListsAndButtons : ContentView
	{
		DataManager dataManager = new DataManager();

		public ListsAndButtons ()
		{
			InitializeComponent ();
		}



		public void createList(Grid grid, List<Profile> profiles, List<Group> groups, 
			ObservableCollection<Button> buttons, ObservableCollection<Button> acceptButtons, 
			ObservableCollection<Button> declineButtons, Profile userProfile, Button addNewButton)
		{
			int column = 0;
			int row = 0;
			int count = 0;
			int subjectNr = 0;
			bool addNew = false;

			if (profiles != null) {
				count = profiles.Count;
			} else {
				count = groups.Count;
			}



			if (userProfile != null) {
				if(userProfile.ProfileId == App.userProfile.ProfileId){
					count ++;
					addNew = true;
				}
			}

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




			for (int i = 0; i < count; i++) {
				if (column == 3) {
					column = 0;
					row++;
					grid.RowDefinitions.Add (new RowDefinition{ Height = 150 });
				}

				Grid cell = new Grid ();

				if (addNew) {
					if (profiles != null) {
						cell = AddNewGrid (userProfile.RecievedFriendRequests.Count);
					} else if (groups != null) {
						cell = AddNewGrid (userProfile.GroupsInviteTo.Count);
					}
					grid.Children.Add (cell, column, row);
					grid.Children.Add (addNewButton, column, row);
					addNew = false;
				} else {
					if (profiles != null) {
						cell = friendCellCreator (profiles [subjectNr], buttons, acceptButtons, declineButtons);
					} else if (groups != null) {
						cell = groupCellCreator (groups [subjectNr]);
					}
					grid.Children.Add (cell, column, row);
					subjectNr++;
				}
				column ++;
			}
			grid = newGrid;
		}

		private Button buttonCreator(int i)
		{

			Button button = new Button {
				Text = "" + i,
				TextColor = Color.Transparent,
				BackgroundColor = Color.Transparent,

			};
			return button;
		}

		private Grid friendCellCreator(Profile profile, ObservableCollection<Button> buttons, ObservableCollection<Button> acceptButtons, ObservableCollection<Button> declineButtons)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				RowDefinitions = {
					new RowDefinition{ Height = 10 },
					new RowDefinition{ Height = 85 },
					new RowDefinition{ Height = 40 },
					new RowDefinition{ Height = 50 },
				}
			};

			cellGrid.Children.Add (new ProfileDesignView(profile,null, 85, true), 0,1);

			Button newProfileButton = new Button ();
			buttons.Add (newProfileButton);
			cellGrid.Children.Add (newProfileButton, 0, 1);

			StackLayout stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal,};

			if (acceptButtons != null) {
				Button newAcceptButton = new Button { Text = "Accept" };
				acceptButtons.Add (newAcceptButton);
				stackLayout.Children.Add (newAcceptButton);
			}
			if (declineButtons != null) {
				Button newDeclineButton = new Button { Text = "Decline" };
				declineButtons.Add (newDeclineButton);
				stackLayout.Children.Add (newDeclineButton);
			}

			cellGrid.Children.Add (stackLayout, 0, 3);

			return cellGrid;
		}

		private Grid groupCellCreator(Group group)
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

			Button groupButton = new Button {
				HeightRequest = 85,
				WidthRequest = 85,
				BorderRadius = 42,
				BorderWidth = 6,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				Text = group.Members.Count + "",
			};
			cellGrid.Children.Add (groupButton, 0,1);

			cellGrid.Children.Add(new Label {
				Text = group.Name,
				TextColor = Color.Black,
				FontSize = 10,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			}, 0, 2);

			return cellGrid;
		}

		private Grid AddNewGrid(int i)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,

				ColumnDefinitions = {
					new ColumnDefinition{ Width = 60},
					new ColumnDefinition{ Width = 25},
				},
				RowDefinitions = {
					new RowDefinition{ Height = 10 },
					new RowDefinition{ Height = 25 },
					new RowDefinition{ Height = 60 },
					new RowDefinition{ Height = 40 },
				},
			};

			Button groupButton = new Button {
				HeightRequest = 85,
				WidthRequest = 85,
				BorderRadius = 42,
				BackgroundColor = Color.FromHex ("00E0A0"),
				HorizontalOptions = LayoutOptions.Center,
				Text = "+",
				TextColor = Color.White
			};
			cellGrid.Children.Add (groupButton, 0,2,1,3);

			if (i > 0) {
				Button notificationButton = new Button {
					HeightRequest = 25,
					WidthRequest = 25,
					BorderRadius = 12,
					BackgroundColor = Color.Red,
					HorizontalOptions = LayoutOptions.Center,
					Text = "" + i,
					TextColor = Color.White
				};
				cellGrid.Children.Add (notificationButton, 1, 2, 1, 2);
			}

			return cellGrid;
		}
	}
}