using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class ListsAndButtons : ContentView
	{
		DataManager _dataManager = new DataManager();

		public ListsAndButtons ()
		{
			InitializeComponent ();
		}

		public void createList(Grid grid, List<Profile> profiles, List<Group> groups, 
			Button addNewButton, string gridStyle, Event eventInvitingTo)
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

			if (addNewButton != null) {
					count ++;
					addNew = true;
			}

			Grid newGrid = new Grid {
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto }
				},
				RowSpacing=0,
				ColumnSpacing=0,
				Padding=0,
			};

			grid.RowDefinitions.Add (new RowDefinition{ Height = 160 });

			for (int i = 0; i < count; i++) {
				if (column == 3) {
					column = 0;
					row++;
					grid.RowDefinitions.Add (new RowDefinition{ Height = 160 });
				}
				Grid cell = new Grid ();

				if (addNew) {
					if (profiles != null) {
						cell = AddNewGrid (App.userProfile.RecievedFriendRequests.Count);
					} else if (groups != null) {
						cell = AddNewGrid (App.userProfile.GroupsInviteTo.Count);
					}
					grid.Children.Add (cell, column, row);
					grid.Children.Add (addNewButton, column, row);
					addNew = false;
				} else {
					if (profiles != null) {
						if (gridStyle == "invite") {
							cell.Children.Add (new ProfileDesignView (profiles [subjectNr], null, eventInvitingTo, 100, ProfileDesignView.ProfileDesign.Invite), 0, 0);
						} else {
							cell.Children.Add (new ProfileDesignView (profiles [subjectNr], null, null, 100, ProfileDesignView.ProfileDesign.WithButtons), 0, 0);
						}
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
					new ColumnDefinition{ Width = 50},
					new ColumnDefinition{ Width = 25},
				},
				RowDefinitions = {
					new RowDefinition{ Height = 25 },
					new RowDefinition{ Height = 45 },
				},
			};

			Button groupButton = new Button {
				HeightRequest = 75,
				WidthRequest = 75,
				BorderRadius = 37,
				BackgroundColor = Color.FromHex ("00e1c4"),
				HorizontalOptions = LayoutOptions.Center,
				Text = "+",
				TextColor = Color.White
			};
			cellGrid.Children.Add (groupButton, 0,2,0,2);

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
				cellGrid.Children.Add (notificationButton, 1, 2, 0, 1);
			}

			return cellGrid;
		}
	}
}