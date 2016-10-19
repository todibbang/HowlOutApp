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

		public void createList(Grid grid, List<Profile> profiles, List<Group> groups, List<Organization> orgs, 
		                       Event eventInvitingTo, Group groupInvitingTo, Organization organizationInvitingTo)
		{
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

			grid.RowDefinitions.Add (new RowDefinition{ Height = 160 });

			for (int i = 0; i < count; i++)
			{
				if (column == 3)
				{
					column = 0;
					row++;
					grid.RowDefinitions.Add(new RowDefinition { Height = 160 });
				}
				Grid cell = new Grid();

				if (profiles != null)
				{
					if (eventInvitingTo != null)
					{
						cell.Children.Add(new ProfileDesignView(profiles[i], eventInvitingTo, 100), 0, 0);
					}
					else if (groupInvitingTo != null)
					{
						cell.Children.Add(new ProfileDesignView(profiles[i], groupInvitingTo, 100), 0, 0);
					}
					else if (organizationInvitingTo != null)
					{
						cell.Children.Add(new ProfileDesignView(profiles[i], organizationInvitingTo, 100), 0, 0);
					}
					else {
						cell.Children.Add(new ProfileDesignView(profiles[i], 100, true), 0, 0);
					}
				}
				else if (groups != null)
				{
					cell.Children.Add(new GroupDesignView(groups[i], 100), 0, 0);
				}
				else if (orgs != null)
				{
					cell.Children.Add(new OrganizationDesignView(orgs[i], 100), 0, 0);
				}

				grid.Children.Add(cell, column, row);
				column ++;
			}
			grid.RowDefinitions.Add (new RowDefinition{ Height = 100 });
		}

		public enum ListType {
			Normal,
			Invite,
			FriendAndGroupRequests,
			EventAttendeesSeenAsOwner
		}
	}
}