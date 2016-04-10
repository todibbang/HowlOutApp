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
			Button addNewButton, ListType listType, Event eventInvitingTo, Group groupInvitingTo)
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
			/*

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
			*/

			/*
			grid.ColumnDefinitions.Add (new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) });
			grid.ColumnDefinitions.Add (new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) });
			grid.ColumnDefinitions.Add (new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) });
			*/
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
						if (listType.Equals(ListType.InviteToEvent)) {
							cell.Children.Add (new ProfileDesignView (profiles [subjectNr], null, eventInvitingTo, 100, ProfileDesignView.Design.InviteProfileToEvent, true), 0, 0);
						} else if (listType.Equals(ListType.InviteToGroup)) {
							cell.Children.Add (new ProfileDesignView (profiles [subjectNr], groupInvitingTo, null, 100, ProfileDesignView.Design.InviteProfileToGroup, true), 0, 0);
						} else if (listType.Equals(ListType.FriendAndGroupRequests)) {
							cell.Children.Add (new ProfileDesignView (profiles [subjectNr], groupInvitingTo, null, 100, ProfileDesignView.Design.WithOptions, true), 0, 0);
						} else {
							cell.Children.Add (new ProfileDesignView (profiles [subjectNr], null, null, 100, ProfileDesignView.Design.WithDescription, true), 0, 0);
						}
					} else if (groups != null) {
						if (listType.Equals (ListType.InviteToEvent)) {
							cell.Children.Add (new GroupDesignView (groups [subjectNr], eventInvitingTo, 100, GroupDesignView.Design.InviteGroupToEvent), 0, 0); 
						} else if (listType.Equals(ListType.FriendAndGroupRequests)) {
							cell.Children.Add (new GroupDesignView (groups [subjectNr], eventInvitingTo, 100, GroupDesignView.Design.WithOptions), 0, 0);
						} else {
							cell.Children.Add (new GroupDesignView (groups [subjectNr], eventInvitingTo, 100, GroupDesignView.Design.WithDescription), 0, 0); 
						}//cell = groupCellCreator (groups [subjectNr]);
					}
					grid.Children.Add (cell, column, row);
					subjectNr++;
				}
				column ++;
			}
			grid.RowDefinitions.Add (new RowDefinition{ Height = 100 });
			//grid.Children.Add (new StackLayout(){HeightRequest=100},0,row);

			//grid = newGrid;
		}

		private Grid AddNewGrid(int i)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,

				ColumnDefinitions = {
					new ColumnDefinition{ Width = 45},
					new ColumnDefinition{ Width = 25},
				},
				RowDefinitions = {
					new RowDefinition{ Height = 25 },
					new RowDefinition{ Height = 45 },
					new RowDefinition{ Height = 55 },
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

		public enum ListType {
			Normal,
			InviteToEvent,
			InviteToGroup,
			FriendAndGroupRequests
		}
	}
}