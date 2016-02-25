using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
		bool mapInitialized = false;
		ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };

		UtilityManager util = new UtilityManager();
		DataManager dataManager = new DataManager();

		public InspectEvent (Event eve, int inspectType)
		{
			InitializeComponent ();
			setInfo (eve);

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
					util.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
					util.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.PositionName);
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
			var profilePicUri = dataManager.GetFacebookProfileImageUri(eve.OwnerId);
			eventHolderPhoto.Source = ImageSource.FromUri(profilePicUri);
			Position position = util.getCurrentUserPosition();

			// Time
			DateTime today = DateTime.Now.ToLocalTime();
			System.Diagnostics.Debug.WriteLine ("- " + (eve.StartDate - today) + " : " + (today - eve.StartDate));
			quickTime.Text = "" + eve.StartDate.DayOfWeek + " at " + util.getTime(eve.StartDate);
			StartTime.Text = "" + eve.StartDate.DayOfWeek + " the " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMMM").ToLower();
			EndTime.Text = "From " + util.getTime(eve.StartDate) + " till " + util.getTime(eve.EndDate);

			//Place
			string [] addressList = new string [3];
			addressList = Regex.Split(eve.PositionName, ",");
			for (int i = 0; i < addressList.Length; i++) { 
				Label label = new Label (); 
				label.Text = addressList [i];
				addressLayout.Children.Add(label);
			}
			quickDistance.Text = util.distance(eve.Latitude, eve.Longitude, position.Latitude, position.Longitude);
			Label labelD = new Label ();
			labelD.Text = quickDistance.Text;
			addressLayout.Children.Add(labelD);

			//Other
			eventTitle.Text = eve.Title;
			eventDescription.Text = eve.Description;
			eventAttending.Text = (eve.Attendees.Count + 1) + "/" + eve.MaxSize;

			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;
		}
	}
}