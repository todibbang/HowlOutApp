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
			createList(thisGrid, profiles, groups,  orgs, eventInvitingTo,  groupInvitingTo, organizationInvitingTo, false);
		}

		public ListsAndButtons(List<Profile> profiles, List<Group> groups, List<Organization> orgs, bool preview)
		{
			InitializeComponent();
			createList(thisGrid, profiles, groups, orgs, null, null, null, preview);
		}


		public void createList(Grid grid, List<Profile> profiles, List<Group> groups, List<Organization> orgs, 
		                       Event eventInvitingTo, Group groupInvitingTo, Organization organizationInvitingTo, bool preview)
		{


			int height = 60;
			float rowHeight = height * 1.6f;
			int column = 0;
			int row = 0;
			int count = 0;
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
				if (column == 5)
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
						cell.Children.Add(new ProfileDesignView(profiles[i], height, true, GenericDesignView.Design.Name ), 0, 0);
					}
				}
				else if (groups != null)
				{
					cell.Children.Add(new GroupDesignView(groups[i], height, GenericDesignView.Design.Name), 0, 0);
				}
				else if (orgs != null)
				{
					cell.Children.Add(new OrganizationDesignView(orgs[i], height, GenericDesignView.Design.Name), 0, 0);
				}

				grid.Children.Add(cell, column, row);
				column ++;

				if (preview && count > 5 && i == 3)
				{
					Button b = new Button() { BackgroundColor = App.HowlOut, HeightRequest = height, WidthRequest = height, Text="more", TextColor=Color.White, BorderRadius=height / 2};
					Grid g = new Grid() { };
					g.RowDefinitions.Add(new RowDefinition { Height = 4 });
					g.RowDefinitions.Add(new RowDefinition { Height = height });
					g.ColumnDefinitions.Add(new ColumnDefinition { Width = height });
					g.Children.Add(b,0,1);
					grid.Children.Add(g, column, row);
					b.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new ListsAndButtons(profiles, groups, orgs, false), "", null);
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