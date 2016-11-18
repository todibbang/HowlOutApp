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

		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		public ListsAndButtons ()
		{
			InitializeComponent ();
		}

		public ListsAndButtons(List<Profile> profiles, List<Group> groups, List<Organization> orgs,
							   Event eventInvitingTo, Group groupInvitingTo, Organization organizationInvitingTo)
		{
			InitializeComponent();
			createList(profiles, groups, orgs, eventInvitingTo, groupInvitingTo, organizationInvitingTo, true, true);
		}

		public ListsAndButtons(List<Profile> profiles, List<Group> groups, List<Organization> orgs, bool preview, bool buttons)
		{
			InitializeComponent();
			createList(profiles, groups, orgs, null, null, null, preview, buttons);
		}


		public void createList(List<Profile> profiles, List<Group> groups, List<Organization> orgs, 
		                       Event eventInvitingTo, Group groupInvitingTo, Organization organizationInvitingTo, bool preview, bool buttons)
		{
			thisGrid.Children.Clear();
			thisGrid.RowDefinitions.Clear();
			thisGrid.ColumnDefinitions.Clear();
			Grid grid = thisGrid;

			GenericDesignView.Design design = GenericDesignView.Design.Name;

			int height = 0;
			int newRowValue = 3;
			if (buttons)
			{
				design = GenericDesignView.Design.NameAndButtons;
				newRowValue = 3;
				height = 100;
			}
			else {
				newRowValue = 4;
				height = 75;
			}
			float rowHeight = height * 1.8f;
			int column = 0;
			int row = 0;
			int count = 0;



			if (!preview && !buttons)
			{
				findMoreFriends.IsVisible = true;
				findFriendsBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new FindNewFriendsView(0), "", null);
				};
			}


			/*
			if (!preview && buttons)
			{
				design = GenericDesignView.Design.NameAndButtons;
				newRowValue = 4;
				height = 80;
				rowHeight = height * 2f;
			}
			*/

			grid.Children.Clear();

			if (profiles != null)
			{
				count = profiles.Count;
			}
			else if (groups != null)
			{
				count = groups.Count;
			}
			else if (orgs != null)
			{
				count = orgs.Count;
			}

			grid.RowDefinitions.Add (new RowDefinition{ Height = rowHeight });

			for (int i = 0; i < count; i++)
			{
				if (column == newRowValue)
				{
					column = 0;
					row++;
					grid.RowDefinitions.Add(new RowDefinition { Height = rowHeight });
				}
				Grid cell = new Grid();

				if (profiles != null)
				{
					if (eventInvitingTo != null)
					{
						cell.Children.Add(new ProfileDesignView(profiles[i], eventInvitingTo, height), 0, 0);
					}
					else if (groupInvitingTo != null)
					{
						cell.Children.Add(new ProfileDesignView(profiles[i], groupInvitingTo, height), 0, 0);
					}
					else if (organizationInvitingTo != null)
					{
						cell.Children.Add(new ProfileDesignView(profiles[i], organizationInvitingTo, height), 0, 0);
					}
					else {
						cell.Children.Add(new ProfileDesignView(profiles[i], height, true, design ), 0, 0);
					}
				}
				else if (groups != null)
				{
					cell.Children.Add(new GroupDesignView(groups[i], height, design), 0, 0);
				}
				else if (orgs != null)
				{
					cell.Children.Add(new OrganizationDesignView(orgs[i], height, design), 0, 0);
				}

				grid.Children.Add(cell, column, row);
				column ++;

				if (preview && count > newRowValue && i == (newRowValue-2))
				{
					Button b = new Button() { BackgroundColor = App.HowlOut, HeightRequest = height, WidthRequest = height, Text="more", TextColor=Color.White, BorderRadius=height / 2};
					Grid g = new Grid() { };
					g.RowDefinitions.Add(new RowDefinition { Height = 12});
					g.RowDefinitions.Add(new RowDefinition { Height = height });
					g.ColumnDefinitions.Add(new ColumnDefinition { Width = height });
					g.Children.Add(b,0,1);
					grid.Children.Add(g, column, row);
					b.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new ListsAndButtons(profiles, groups, orgs, false, buttons), "", null);
					};

					break;
				}
			}
		}
		public enum ListType {
			Normal,
			Invite,
			FriendAndGroupRequests,
			EventAttendeesSeenAsOwner
		}
	}
}