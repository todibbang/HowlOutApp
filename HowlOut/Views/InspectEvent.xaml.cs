using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using Plugin.Geolocator;

namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
		bool mapInitialized = false;

		public InspectEvent (Event eve, int inspectType)
		{
			InitializeComponent ();
			setInfo (eve);

			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;

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
					setMap (eve);
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

			mapButton.Clicked += (sender, e) => 
			{
				App.coreView.setContentView(new MapView(eve), 0);
			};


		}

		public async void setMap(Event eve)
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;

			var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);

			quickDistance.Text = "" + MapView.distance(eve.Latitude, eve.Longitude, position.Latitude, position.Longitude) + " km away";

			var map = new Map(
				MapSpan.FromCenterAndRadius(
					new Position(eve.Latitude,eve.Longitude), Distance.FromMiles(0.1))) {
				IsShowingUser = true,
				HeightRequest = 200,
				WidthRequest = 320,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var pin = new Pin
			{
				Type = PinType.Place,
				Position = new Position(eve.Latitude,eve.Longitude),
				Label = eve.Title,
				Address = eve.PositionName,
			};

			map.Pins.Add (pin);
			mapLayout.Children.Add(map);
		}

		public void setInfo (Event eve)
		{
			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(eve.OwnerId);
			eventHolderPhoto.Source = ImageSource.FromUri(profilePicUri);

			eventTitle.Text = eve.Title;

			DateTime today = DateTime.Now.ToLocalTime();
			System.Diagnostics.Debug.WriteLine ("- " + (eve.StartDate - today) + " : " + (today - eve.StartDate));

			quickTime.Text = "på " + eve.StartDate.DayOfWeek + " kl. " + eve.StartDate.TimeOfDay.Hours;



			eventDescription.Text = eve.Description;

			eventAttending.Text = (eve.AttendingIDs.Count + 1) + "/" + eve.MaxSize;
			eventHolderLikes.Text = "22";
			eventLoyaltyRaiting.Text = "22";

			StartTime.Text = "Starts " + eve.StartDate.DayOfWeek + ", " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMMM") + " at " + eve.StartDate.TimeOfDay;
			EndTime.Text = "Ends " + eve.EndDate.DayOfWeek + ", " + eve.EndDate.Day + " " + eve.EndDate.ToString("MMMM") + " at " + eve.EndDate.TimeOfDay;
		}
	}
}