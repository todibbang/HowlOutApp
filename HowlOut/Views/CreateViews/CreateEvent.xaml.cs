using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;
using System.Net.Http;
using ModernHttpClient;
using System.Threading.Tasks;
using System.IO;

namespace HowlOut
{
	public partial class CreateEvent : ContentView
	{
		public ContentView createContent { 
			get { return this; }
			set { this.createContent = value; }
		}

		MapsView mapView;
		DataManager _dataManager;

		public Event newEvent;
		//Dictionary<string, int> agePicker = new Dictionary<string, int> { };
		//Dictionary<string, int> sizePicker = new Dictionary<string, int> { };

		List<Button> typeButtons = new List<Button>();

		private bool Launching = false;

		Plugin.Media.Abstractions.MediaFile mediaFile;
		//Image banner = new Image();

		public CreateEvent(Event givenEvent, bool isCreate)
		{
			_dataManager = new DataManager();
			newEvent = givenEvent;
			InitializeComponent();

			mapView = new MapsView(App.lastKnownPosition);

			/*
			for (int i = 18; i < 100; i++) agePicker.Add("" + i, i);
			foreach (string age in agePicker.Keys) { minAge.Items.Add(age); maxAge.Items.Add(age); }

			int sizeNumber = 5;
			for (int i = 0; i < 20; i++) { sizePicker.Add("" + sizeNumber, sizeNumber); sizeNumber += 5; }
			foreach (string size in sizePicker.Keys) { minSize.Items.Add(size); maxSize.Items.Add(size); }
			*/
			/// set title and description
			title.TextChanged += (sender, e) => { newEvent.Title = title.Text; };
			description.TextChanged += (sender, e) => { newEvent.Description = description.Text; };


			int row = 0;
			int column = 1;
			eventTypeGrid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto });
			foreach (EventType en in Enum.GetValues(typeof(EventType)))
			{
				if (column == 9)
				{
					eventTypeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					column = 1;
					row++;
				}

				Button b = new Button()
				{
					TextColor = App.HowlOut,
					BorderRadius = 25,
					BorderWidth = 1,
					HeightRequest = 50,
					WidthRequest = 50,
					BorderColor = App.HowlOutFade,
					BackgroundColor = Color.White,
					FontSize = 12,
					Text = en.ToString(),
				};
				typeButtons.Add(b);
				eventTypeGrid.Children.Add(b,column,row);
				b.Clicked += (sender, e) =>
				{
					typeButtonPressed(b, en);
				};

				if (newEvent.EventTypes.Contains(en))
				{
					b.BackgroundColor = App.HowlOut;
					b.TextColor = Color.White;
				}
				column += 2;
			}

			/// set time and date
			startDate.PropertyChanged += (sender, e) =>
			{
				newEvent.StartDate = startDate.Date.Add(startTime.Time);
				var newTimeSpan = newEvent.StartDate + new TimeSpan(1, 0, 0);
				if (newEvent.EndDate.Ticks < newTimeSpan.Ticks)
				{
					newEvent.EndDate = newEvent.StartDate + new TimeSpan(2, 0, 0);
					endTime.Time = newEvent.EndDate.TimeOfDay;
					//endDate.Date = newEvent.EndDate;
				}
				System.Diagnostics.Debug.WriteLine(newEvent.StartDate.ToString("g") + ", " + newEvent.EndDate.ToString("g"));
			};
			startTime.PropertyChanged += (sender, e) =>
			{
				newEvent.StartDate = startDate.Date.Add(startTime.Time);
				var newTimeSpan = newEvent.StartDate + new TimeSpan(1, 0, 0);
				if (newEvent.EndDate.Ticks < newTimeSpan.Ticks)
				{
					newEvent.EndDate = newEvent.StartDate + new TimeSpan(2, 0, 0);
					endTime.Time = newEvent.EndDate.TimeOfDay;
					//endDate.Date = newEvent.EndDate;
				}
				System.Diagnostics.Debug.WriteLine(newEvent.StartDate.ToString("g") + ", " + newEvent.EndDate.ToString("g"));
			};
			endDate.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };
			endTime.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };

			/// set location
			locationButton.Clicked += (sender, e) =>
			{
				if (newEvent.Latitude == 0 && newEvent.Longitude == 0)
				{
					mapView = new MapsView(App.lastKnownPosition);
				}
				else {
					mapView = new MapsView(new Position(newEvent.Latitude, newEvent.Longitude));
				}
				mapView.createEventView = this;
				App.coreView.setContentViewWithQueue(mapView, "MapsView", null);
			};

			visibilityPicker.SelectedIndexChanged += async (sender, e) =>
			{
				visibilityPicker.Items[1] = "Group";
				newEvent.GroupSpecific = null;
				System.Diagnostics.Debug.WriteLine(visibilityPicker.SelectedIndex);
				if (visibilityPicker.SelectedIndex == 0)
				{
					newEvent.Visibility = Visibility.Open;
				}
				if (visibilityPicker.SelectedIndex == 1)
				{
					bool success = await App.GroupEventIsFor(SelectSenderLayout, newEvent);
					if (success)
					{
						newEvent.Visibility = Visibility.Closed;
						await Task.Delay(50);
						visibilityPicker.Items[1] = "Group: " + newEvent.GroupSpecific.Name;
					}
					else {
						visibilityPicker.SelectedIndex = 0;
					}
				}
				if (visibilityPicker.SelectedIndex == 2)
				{
					newEvent.Visibility = Visibility.Secret;
				}
			};

			NumberAttendendeesEntry.TextChanged += (sender, e) => { newEvent.MaxSize = int.Parse(NumberAttendendeesEntry.Text); };

			var pictureImage = new TapGestureRecognizer();
			pictureImage.Tapped += async (sender, e) =>
			{
				mediaFile = await _dataManager.UtilityManager.TakePicture();
				if (mediaFile != null)
				{
					SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
				}
			};
			takePictureButton.GestureRecognizers.Add(pictureImage);

			var albumImage = new TapGestureRecognizer();
			albumImage.Tapped += async (SenderOfEvent, e) =>
			{
				mediaFile = await _dataManager.UtilityManager.PictureFromAlbum();
				if (mediaFile != null)
				{
					SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
				}
			};
			albumPictureButton.GestureRecognizers.Add(albumImage);

			selectBannerButton.Clicked += (sender, e) => {
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createEventView = this;
				App.coreView.setContentViewWithQueue(selectBannerView, "", null);
			};


			/*
			/// set age and size limits
			minAge.SelectedIndexChanged += (sender, args) => {
				if (minAge.SelectedIndex != -1) { string age = minAge.Items[minAge.SelectedIndex]; newEvent.MinAge = agePicker[age]; } };
			maxAge.SelectedIndexChanged += (sender, args) => {
				if (maxAge.SelectedIndex != -1) { string age = maxAge.Items[maxAge.SelectedIndex]; newEvent.MaxAge = agePicker[age]; } };
			minSize.SelectedIndexChanged += (sender, args) => {
				if (minSize.SelectedIndex != -1) { string size = minSize.Items[minSize.SelectedIndex]; newEvent.MinSize = sizePicker[size]; } };
			maxSize.SelectedIndexChanged += (sender, args) => {
				if (maxSize.SelectedIndex != -1) { string size = maxSize.Items[maxSize.SelectedIndex]; newEvent.MaxSize = sizePicker[size]; } };
			*/
			if (isCreate) {
				setNewEvent ();
			} else {
				setEditEvent ();
			}

			launchButton.Clicked += async (sender, e) => {
				if(isCreate && !Launching) {
					bool continueCreating = await App.SenderOfEvent(SelectSenderLayout, newEvent, null);
					if (continueCreating)
					{
						LaunchEvent(newEvent);
						Launching = true;
					}
				} else if(!isCreate && !Launching) {
					UpdateEvent(newEvent);
				}
			};

			cancelButton.Clicked += (sender, e) => { CancelTheEvent(); };
		}

		public void setBanner(string banner) {
			SelectedBannerImage.Source = banner;
			newEvent.ImageSource = banner;
			mediaFile = null;
		}

		private void setNewEvent()
		{
			cancelButton.IsVisible = false;
			//newEvent.ProfileOwner = new Profile(){ProfileId = App.StoredUserFacebookId};

			System.Diagnostics.Debug.WriteLine (startTime.Time);
			System.Diagnostics.Debug.WriteLine (DateTime.Now.TimeOfDay);
			System.Diagnostics.Debug.WriteLine (new TimeSpan (1,10,0));

			bool didTrySucceed = false;
			try 
			{
				startTime.Time = (new TimeSpan(DateTime.Now.Hour + 3, 0, 0));
				endTime.Time = (new TimeSpan(DateTime.Now.Hour + 5, 0, 0));
				didTrySucceed = true;
			} 
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				ERROR {0}", ex.Message);
			}
			if(!didTrySucceed)
			{
				startTime.Time.Add (new TimeSpan (DateTime.Now.TimeOfDay.Hours + 3, 0, 0));
				endTime.Time.Add (new TimeSpan (DateTime.Now.TimeOfDay.Hours + 5, 0, 0));
			}

			newEvent.StartDate = startDate.Date.Add(startTime.Time);
			//newEvent.EndDate = endDate.Date.Add(endTime.Time);
		}

		private void setEditEvent()
		{
			cancelButton.IsVisible = true;
			launchButton.Text = "Update";

			title.Text = newEvent.Title;
			description.Text = newEvent.Description;
			startTime.Time = newEvent.StartDate.TimeOfDay;
			endTime.Time = newEvent.EndDate.TimeOfDay;
			startDate.Date = newEvent.StartDate.Date;
			//endDate.Date = newEvent.EndDate.Date;

			locationEntry.Text = newEvent.AddressName;
			NumberAttendendeesEntry.Text = newEvent.MaxSize+"";
			visibilityPicker.SelectedIndex = Array.IndexOf(Enum.GetValues(newEvent.Visibility.GetType()), newEvent.Visibility);

			/*
			int minvalue = 0;
			int maxvalue = 0;

			for (int i = 0; i < minAge.Items.Count; i++) {
				if (minAge.Items [i] == newEvent.MinAge.ToString ())
					minvalue = i;
				if (maxAge.Items [i] == newEvent.MaxAge.ToString ())
					maxvalue = i;
			}

			minAge.SelectedIndex = minvalue;
			maxAge.SelectedIndex = maxvalue;

			for (int i = 0; i < minSize.Items.Count; i++) {
				if (minSize.Items [i] == newEvent.MinSize.ToString ())
					minvalue = i;
				if (maxSize.Items [i] == newEvent.MaxSize.ToString ())
					maxvalue = i;
			}

			minSize.SelectedIndex = minvalue;
			maxSize.SelectedIndex = maxvalue;
			*/
		}
			
		private async void LaunchEvent(Event eventToCreate)
		{
			if (mediaFile != null)
			{
				eventToCreate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
			}
			if (eventToCreate.OrganizationOwner == null)
			{
				eventToCreate.ProfileOwner = App.userProfile;
			}
			eventToCreate.EventId = "0";
			if (String.IsNullOrWhiteSpace (eventToCreate.Title)) {
				await App.coreView.displayAlertMessage ("Title Missing", "Title is missing", "Ok");
			} else if (String.IsNullOrWhiteSpace (eventToCreate.Description)) {
				await App.coreView.displayAlertMessage ("Description Missing", "Description is missing", "Ok");
			} else if ((eventToCreate.MaxSize == -1)) {
				await App.coreView.displayAlertMessage("Attendendees Needed Missing", "", "Ok");
			} else if (eventToCreate.EventTypes.Count == 0) {
				await App.coreView.displayAlertMessage ("EventTypes Missing", "No Event Type has been selected", "Ok");
			} else if (String.IsNullOrWhiteSpace (eventToCreate.AddressName) || eventToCreate.Latitude == 0) {
				await App.coreView.displayAlertMessage ("Address Missing", "No valid address has been selected", "Ok");
			} else if (String.IsNullOrWhiteSpace (eventToCreate.ImageSource)) {
				await App.coreView.displayAlertMessage ("Banner Missing", "No banner has been selected", "Ok");
			}else {

				eventToCreate.MinAge = 0;
				eventToCreate.MaxAge = 100;
				eventToCreate.MinSize = 1;


				Event eventCreated = await _dataManager.EventApiManager.CreateEditEvent(eventToCreate);

				if (eventCreated != null) {
					eventCreated.Attendees = new List<Profile> ();
					eventCreated.Followers = new List<Profile> ();
					InspectController inspect = new InspectController(eventCreated);
					App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
					App.coreView.createView.createEvent = new CreateEvent(new Event(), true);
				} else {
					await App.coreView.displayAlertMessage ("Error", "Event not created, try again", "Ok");
				}
			}
			Launching = false;
		}

		private async void UpdateEvent(Event eventToUpdate)
		{
			Event eventUpdated = await _dataManager.EventApiManager.CreateEditEvent (eventToUpdate);

			if (eventUpdated != null) {
				 
				InspectController inspect = new InspectController(eventUpdated);
				App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());

			} else {
				await App.coreView.displayAlertMessage ("Error", "Event not updated, try again", "Ok");
			}
		}

		private Button typeButtonPressed(Button typeButton, EventType eventType)
		{
			if (typeButton.BackgroundColor == Color.White) {
				if (newEvent.EventTypes.Count < 3) {
					typeButton.BackgroundColor = App.HowlOut;
					typeButton.TextColor = Color.White;
					newEvent.EventTypes.Add (eventType);
				}
			} else {
				typeButton.BackgroundColor = Color.White;
				typeButton.TextColor = App.HowlOut;
				newEvent.EventTypes.Remove (eventType);
			}
			return typeButton;
		}

		public async void CancelTheEvent()
		{
			bool confirmDelete = await App.coreView.displayConfirmMessage ("Warning", "You are about to delete this event permanently, would you like to continue", "Yes", "No");

			if (confirmDelete) {
				bool wasEventDeleted = await _dataManager.EventApiManager.DeleteEvent (newEvent.EventId);
				if (wasEventDeleted) {
					await App.coreView.displayAlertMessage ("Event Deleted", "The event was successfully cancelled", "Ok");
					App.coreView.setContentView (1);
				} else {
					App.coreView.displayAlertMessage ("Event Not Deleted", "The event was not cancelled, try again", "Ok");
				}
			}
		}

		public void setLocationButton(string name) {
			locationEntry.Text = name;
		}

		private Button PreSetButton(Button typeButton, EventType eventType)
		{
			typeButton.BackgroundColor = App.HowlOut;
			typeButton.TextColor = Color.White;

			return typeButton;
		}
	}
}

