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
		UtilityManager utilityManager = new UtilityManager();

		public InspectEvent (Event eve, int inspectType)
		{
			InitializeComponent ();
			setInfo (eve);

			ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };

			CommentList.ItemsSource = comments;
			comments.Add (new Comment {
				Title = "Rob Finnerty",
				Content = "Test1 asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb"
			});

			if (inspectType == 1) {
				searchSpecific.IsVisible = true;
				manageSpecific.IsVisible = false;
			} else if (inspectType == 2) {
				searchSpecific.IsVisible = false;
				manageSpecific.IsVisible = true;
			}
			detailsButton.Clicked += (sender, e) =>
			{
				if(detailedInfo.IsVisible == false) {
					detailedInfo.IsVisible = true;
					quickInfo.IsVisible = false;
				} else {
					detailedInfo.IsVisible = false;
					quickInfo.IsVisible = true;
				}

				if(mapInitialized != true) {
					mapInitialized = true;
					utilityManager.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
					utilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.PositionName);
				}
			};


			eventHolderButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectProfile(eve.OwnerId), 0);
			};

			eventGroupButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectGroup(), 0);
			};

			mapButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new MapView(eve), 0);
			};
		}

		public async void setInfo (Event eve)
		{
			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;

			DataManager dataManager = new DataManager();
			UtilityManager utilityManager = new UtilityManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(eve.OwnerId);
			eventHolderPhoto.Source = ImageSource.FromUri(profilePicUri);

			eventTitle.Text = eve.Title;

			DateTime today = DateTime.Now.ToLocalTime();
			System.Diagnostics.Debug.WriteLine ("- " + (eve.StartDate - today) + " : " + (today - eve.StartDate));

			quickTime.Text = "på " + eve.StartDate.DayOfWeek + " kl. " + eve.StartDate.TimeOfDay.Hours;

			addressLine.Text = eve.PositionName;


			eventDescription.Text = eve.Description;

			eventAttending.Text = (eve.AttendingIDs.Count + 1) + "/" + eve.MaxSize;
			eventHolderLikes.Text = "22";
			eventLoyaltyRaiting.Text = "22";

			StartTime.Text = "Starts " + eve.StartDate.DayOfWeek + ", " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMM") + " at " + eve.StartDate.TimeOfDay;
			EndTime.Text = "Ends " + eve.EndDate.DayOfWeek + ", " + eve.EndDate.Day + " " + eve.EndDate.ToString("MMM") + " at " + eve.EndDate.TimeOfDay;

			Position position = utilityManager.getCurrentUserPosition();

			System.Diagnostics.Debug.WriteLine ("Position received: " + position.Latitude + ", " + position.Longitude + ", " + eve.Latitude + ", " + eve.Longitude);

			quickDistance.Text = "" + utilityManager.distance(eve.Latitude, eve.Longitude, position.Latitude, position.Longitude) + " km away";
		}
	}
}