using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
		bool mapInitialized = false;

		public InspectEvent (Event eve, int inspectType)
		{
			InitializeComponent ();

			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(eve.OwnerId);
			eventHolderPhoto.Source = ImageSource.FromUri(profilePicUri);

			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;


			eventTitle.Text = eve.Title;

			DateTime today = DateTime.Now.ToLocalTime();
			System.Diagnostics.Debug.WriteLine ("- " + (eve.StartDate - today) + " : " + (today - eve.StartDate));

			quickTime.Text = "på " + eve.StartDate.DayOfWeek + " kl. " + eve.StartDate.TimeOfDay.Hours;
			//quickDistance.Text = eve.PositionCoordinates;
			eventDescription.Text = eve.Description;
			eventAttending.Text = "22";
			eventHolderLikes.Text = "22";
			eventLoyaltyRaiting.Text = "22";

			if (inspectType == 1) {
				searchSpecific.IsVisible = true;
				manageSpecific.IsVisible = false;
			} else if (inspectType == 2) {
				searchSpecific.IsVisible = false;
				manageSpecific.IsVisible = true;

				CommentList.ItemsSource = comments;

				comments.Add (new Comment {
					Title = "Rob Finnerty",
					Content = "Test1 asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb"
				});
			}
			detailsButton.Clicked += (sender, e) =>
			{
				if(detailedInfo.IsVisible == false)
				{
					detailedInfo.IsVisible = true;
					quickInfo.IsVisible = false;
				}
				else 
				{
					detailedInfo.IsVisible = false;
					quickInfo.IsVisible = true;
				}

				if(mapInitialized != true)
				{
					mapInitialized = true;
					addMap ();
				}
			};


			eventHolderButton.Clicked += (sender, e) => 
			{
				App.coreView.setContentView(new InspectProfile(eve.OwnerId), 0);
			};

			eventGroupButton.Clicked += (sender, e) => 
			{
				App.coreView.setContentView(new InspectGroup(), 0);
			};
		}

		public void addMap()
		{
			var map = new Map(
				MapSpan.FromCenterAndRadius(
					new Position(37,-122), Distance.FromMiles(0.3))) {
				IsShowingUser = true,
				HeightRequest = 200,
				WidthRequest = 320,
				VerticalOptions = LayoutOptions.FillAndExpand
			};


			mapLayout.Children.Add(map);

		}
	}
}