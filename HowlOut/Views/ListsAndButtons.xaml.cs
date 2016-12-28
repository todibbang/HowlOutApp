using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class ListsAndButtons : ContentView, ViewModelInterface
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

		public ListsAndButtons(List<Profile> profiles, List<Group> groups,
							   Event eventInvitingTo, Group groupInvitingTo)
		{
			InitializeComponent();
			createList(profiles, groups, eventInvitingTo, groupInvitingTo, true, true, false);
		}

		public ListsAndButtons(List<Profile> profiles, List<Group> groups, bool preview, bool buttons)
		{
			InitializeComponent();
			if (!preview) { Padding = new Thickness(0, 60, 0, 0);}
			createList(profiles, groups, null, null, preview, buttons, false);
		}

		public void viewInFocus(UpperBar bar)
		{

		}

		public void viewExitFocus() { }

		public ContentView getContentView() { return this; }

		public async Task<Group> createList(List<Profile> profiles, List<Group> groups,
		                       Event eventInvitingTo, Group groupInvitingTo, bool preview, bool buttons, bool returnClick)
		{
			thisGrid.Children.Clear();
			thisGrid.RowDefinitions.Clear();
			thisGrid.ColumnDefinitions.Clear();
			Grid grid = thisGrid;

			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Group clickedGroup = null;

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
			float rowHeight = height * 1.3f;
			int column = 0;
			int row = 0;
			int count = 0;

			if (!preview && !buttons)
			{
				findFriendsBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new FindNewFriendsView(0));
				};
			}

			grid.Children.Clear();

			if (profiles != null)
			{
				count = profiles.Count;
			}
			else if (groups != null)
			{
				count = groups.Count;
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
					else {
						cell.Children.Add(new ProfileDesignView(profiles[i], height, true, design), 0, 0);
					}
				}
				else if (groups != null)
				{
					cell.Children.Add(new GroupDesignView(groups[i], height, design), 0, 0);
				}

				grid.Children.Add(cell, column, row);
				if(returnClick) {
					Button clickedGroupBtn = new Button() { };
					Group clickedGrp = groups[i];
					grid.Children.Add(clickedGroupBtn, column, row);

					clickedGroupBtn.Clicked += (sender, e) =>
					{
						clickedGroup = clickedGrp;
						tcs.TrySetResult(true);
					};
				}
				column ++;

				if (preview && count > newRowValue && i == (newRowValue-2))
				{
					Button b = new Button() { BackgroundColor = App.HowlOut, HeightRequest = height, WidthRequest = height, Text="more", TextColor=Color.White, BorderRadius=height / 2};
					Grid g = new Grid() { RowSpacing=0 };
					g.RowDefinitions.Add(new RowDefinition { Height = 1});
					g.RowDefinitions.Add(new RowDefinition { Height = height });
					g.ColumnDefinitions.Add(new ColumnDefinition { Width = height });
					Grid g2 = new Grid();
					g.Children.Add(b,0,1);
					g2.Children.Add(g);
					grid.Children.Add(g2, column, row);
					b.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new ListsAndButtons(profiles, groups, false, buttons));
					};

					break;
				}
			}

			if (returnClick)
			{
				await tcs.Task;
			}
			return clickedGroup;
		}
		public enum ListType {
			Normal,
			Invite,
			FriendAndGroupRequests,
			EventAttendeesSeenAsOwner
		}
	}
}