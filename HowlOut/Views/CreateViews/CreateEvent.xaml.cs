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
		public ContentView content { 
			get { return this; }
			set { this.content = value; }
		}

		MapsView mapView;
		DataManager _dataManager;

		public Event newEvent;
		//Dictionary<string, int> agePicker = new Dictionary<string, int> { };
		//Dictionary<string, int> sizePicker = new Dictionary<string, int> { };

		List<Button> typeButtons = new List<Button>();

		private bool Launching = false;
		bool validAttendingAmount = false;
		bool isCreate = false;

		Plugin.Media.Abstractions.MediaFile mediaFile;
		//Image banner = new Image();

		public CreateEvent(Event givenEvent, bool isCreate)
		{
			_dataManager = new DataManager();
			InitializeComponent();
			setCreateView(givenEvent, isCreate);
		}

		private void setCreateView(Event givenEvent, bool isCreate)
		{
			newEvent = givenEvent;
			this.isCreate = isCreate;

			if (isCreate)
			{
				setNewEvent();
			}
			else {
				setEditEvent();
			}
			mapView = new MapsView(App.lastKnownPosition);

			title.TextChanged += (sender, e) => { newEvent.Title = title.Text; };
			description.TextChanged += (sender, e) => { newEvent.Description = description.Text; };

			EventCategory.ManageCategories(eventTypeGrid, newEvent.EventTypes, true);

			/// set time and date
			startDate.MinimumDate = DateTime.Now;
			endDate.MinimumDate = DateTime.Now;
			startDate.PropertyChanged += (sender, e) => { checkValidDate(); };
			startTime.PropertyChanged += (sender, e) => { checkValidDate(); };
			endDate.PropertyChanged += (sender, e) => { checkValidDate(); };
			endTime.PropertyChanged += (sender, e) => { checkValidDate(); };

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
					bool success = await App.coreView.otherFunctions.GroupEventIsFor(SelectSenderLayout, newEvent);
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
				visibilityString.Text = visibilityPicker.Items[visibilityPicker.SelectedIndex];
			};
			visibilityString.Text = visibilityPicker.Title;

			NumberAttendendeesEntry.TextChanged += (sender, e) =>
			{
				string t = NumberAttendendeesEntry.Text;
				if (t.Contains(","))
				{
					t = t.Remove(t.Length - 1);

				}

				if (!string.IsNullOrWhiteSpace(t))
				{
					try
					{
						newEvent.MaxSize = int.Parse(t);
						validAttendingAmount = true;
					}
					catch (Exception ex)
					{
						validAttendingAmount = false;
					}
				}
				NumberAttendendeesEntry.Text = t;
			};

			var pictureImage = new TapGestureRecognizer();
			pictureImage.Tapped += async (sender, e) =>
			{
				try
				{
					mediaFile = await _dataManager.UtilityManager.TakePicture();
					if (mediaFile != null)
					{
						SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					}
				}
				catch (Exception ex) { }
			};
			takePictureButton.GestureRecognizers.Add(pictureImage);

			var albumImage = new TapGestureRecognizer();
			albumImage.Tapped += async (SenderOfEvent, e) =>
			{
				try
				{
					mediaFile = await _dataManager.UtilityManager.PictureFromAlbum();
					if (mediaFile != null)
					{
						SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					}
				}
				catch (Exception ex) { }
			};
			albumPictureButton.GestureRecognizers.Add(albumImage);

			selectBannerButton.Clicked += (sender, e) =>
			{
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createEventView = this;
				App.coreView.setContentViewWithQueue(selectBannerView, "", null);
			};

			launchButton.Clicked += async (sender, e) =>
			{
				if (isCreate && !Launching)
				{
					bool continueCreating = await App.coreView.otherFunctions.SenderOfEvent(SelectSenderLayout, newEvent, null);
					if (continueCreating)
					{
						LaunchEvent(newEvent);
						Launching = true;
					}
				}
				else if (!isCreate && !Launching)
				{
					LaunchEvent(newEvent);
					Launching = true;
				}
			};

			cancelButton.Clicked += (sender, e) => { CancelTheEvent(); };
		}

		void checkValidDate()
		{
			if (endDate.Date.Add(endTime.Time).Ticks < startDate.Date.Add(startTime.Time).Ticks)
			{
				endDate.Date = startDate.Date;
				endTime.Time.Add(new TimeSpan((startTime.Time.Hours + 3) % 24, 0, 0));
				endTime.Time = startDate.Date.Add(startTime.Time.Add(new TimeSpan(3,0,0))).TimeOfDay;
				if ((startTime.Time.Hours + 3) % 24 < startTime.Time.Hours)
				{
					endDate.Date = startDate.Date.AddDays(1);
				}
			}

			newEvent.StartDate = startDate.Date.Add(startTime.Time);
			newEvent.EndDate = endDate.Date.Add(endTime.Time);

			System.Diagnostics.Debug.WriteLine(newEvent.StartDate.ToString("g") + ", " + newEvent.EndDate.ToString("g"));

			StartDateString.Text = startDate.Date.ToString("ddd") + " " + startDate.Date.ToString("dd/MM/yyyy");
			EndDateString.Text = endDate.Date.ToString("ddd") + " " +  endDate.Date.ToString("dd/MM/yyyy");

			StartTimeString.Text = " -   " + new DateTime(startTime.Time.Ticks).ToString("HH:mm");
			EndTimeString.Text = " -   " + new DateTime(endTime.Time.Ticks).ToString("HH:mm");
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
			newEvent.EventId = "0";
			newEvent.EndDate = endDate.Date.Add(endTime.Time);
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

			locationEntry.Text = newEvent.AddressName;
			NumberAttendendeesEntry.Text = newEvent.MaxSize+"";
			validAttendingAmount = true;
			if (newEvent.GroupSpecific != null)
			{
				visibilityPicker.Title = "Group: " + newEvent.GroupSpecific.Name;
			}
			else {
				visibilityPicker.Title = newEvent.Visibility.ToString();
			}

			visibilityPicker.IsEnabled = false;
			SelectedBannerImage.Source = newEvent.ImageSource;

		}
			
		private async void LaunchEvent(Event eventToCreate)
		{
			App.coreView.IsLoading(true);
			if (mediaFile != null)
			{
				eventToCreate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
			}
			if (eventToCreate.OrganizationOwner == null)
			{
				eventToCreate.ProfileOwner = App.userProfile;
			}

			if (String.IsNullOrWhiteSpace (eventToCreate.Title)) {
				await App.coreView.displayAlertMessage ("Title Missing", "Title is missing", "Ok");
			} else if (String.IsNullOrWhiteSpace (eventToCreate.Description)) {
				await App.coreView.displayAlertMessage ("Description Missing", "Description is missing", "Ok");
			} else if ((eventToCreate.MaxSize == -1) || !validAttendingAmount) {
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
					if (isCreate)
					{
						App.coreView.setContentViewWithQueue(inspect, "Event", inspect.getScrollView());
						App.coreView.updateHomeView();
					}
					else {
						App.coreView.setContentViewReplaceCurrent(inspect, "", null, 2);
					}
					//setCreateView(new Event(), true);
					App.coreView.createEvent = new CreateEvent(new Event(), true);
					App.coreView.updateCreateViews();

				} else {
					await App.coreView.displayAlertMessage ("Error", "Event not created, try again", "Ok");
				}
			}
			Launching = false;
			App.coreView.IsLoading(false);
		}

		public async void CancelTheEvent()
		{
			bool confirmDelete = await App.coreView.displayConfirmMessage ("Warning", "You are about to delete this event permanently, would you like to continue", "Yes", "No");

			if (confirmDelete) {
				bool wasEventDeleted = await _dataManager.EventApiManager.DeleteEvent (newEvent.EventId);
				if (wasEventDeleted) {
					await App.coreView.displayAlertMessage ("Event Deleted", "The event was successfully cancelled", "Ok");
					//App.coreView.exploreEvents = new EventListView(0);
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

