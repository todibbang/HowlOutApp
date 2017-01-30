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
	public partial class CreateEvent : ContentView, ViewModelInterface
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}
		public ContentView otherViews { get { return OtherViews; } set { } }

		MapsView mapView;
		DataManager _dataManager;

		public Event newEvent;
		public bool IsCreate = false;
		//Dictionary<string, int> agePicker = new Dictionary<string, int> { };
		//Dictionary<string, int> sizePicker = new Dictionary<string, int> { };

		List<Button> typeButtons = new List<Button>();

		private bool Launching = false;
		bool validAttendingAmount = false;
		bool isCreate = false;

		List<byte[]> imageStreams;
		//Image banner = new Image();

		public CreateEvent(Event givenEvent, bool isCreate)
		{
			_dataManager = new DataManager();
			IsCreate = isCreate;
			InitializeComponent();
			setCreateView(givenEvent, isCreate);
			if (!isCreate)
			{
				mainGrid.Padding = new Thickness(0, 55, 0, 250);
			}
		}

		public void viewInFocus(UpperBar bar)
		{

		}

		public void viewExitFocus() { }
		public void reloadView() { }
		public ContentView getContentView() { return this; }

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
			locationButton.Clicked += async (sender, e) =>
			{
				if (newEvent.Latitude == 0 && newEvent.Longitude == 0)
				{
					mapView = new MapsView(App.lastKnownPosition);
				}
				else {
					mapView = new MapsView(new Position(newEvent.Latitude, newEvent.Longitude));
				}
				mapView.createEventView = this;

				otherViews.Content = mapView;
				otherViews.IsVisible = true;

				App.coreView.IsLoading(true);
				await Task.Delay(100);
				if (isCreate)
				{
					App.coreView.createView.displayBackButton(true, 1);
					App.coreView.setContentView(0);
				}
				await Task.Delay(500);
				App.coreView.IsLoading(false); 
			};

			visibilityPicker.SelectedIndexChanged += (sender, e) =>
			{
				//visibilityPicker.Items[1] = "Group";
				//newEvent.GroupSpecific = null;
				System.Diagnostics.Debug.WriteLine(visibilityPicker.SelectedIndex);
				if (visibilityPicker.SelectedIndex == 0) { newEvent.Visibility = EventVisibility.Public; }
				if (visibilityPicker.SelectedIndex == 1) { newEvent.Visibility = EventVisibility.Private; }
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
					imageStreams = await _dataManager.UtilityManager.TakePicture(SelectedBannerImage);
				}
				catch (Exception ex) { }
			};
			takePictureButton.GestureRecognizers.Add(pictureImage);

			var albumImage = new TapGestureRecognizer();
			albumImage.Tapped += async (SenderOfEvent, e) =>
			{
				try
				{
					imageStreams = await _dataManager.UtilityManager.PictureFromAlbum(SelectedBannerImage);
				}
				catch (Exception ex) { }
			};
			albumPictureButton.GestureRecognizers.Add(albumImage);

			selectBannerButton.Clicked += async (sender, e) =>
			{
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createEventView = this;

				otherViews.Content = selectBannerView;
				otherViews.IsVisible = true;

				App.coreView.IsLoading(true);
				await Task.Delay(100);
				if (isCreate)
				{
					App.coreView.createView.displayBackButton(true, 1);
					App.coreView.setContentView(0);
				}
				await Task.Delay(500);
				App.coreView.IsLoading(false);
			};

			launchButton.Clicked += (sender, e) =>
			{
				if (!Launching)
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
				endTime.Time = startDate.Date.Add(startTime.Time.Add(new TimeSpan(3, 0, 0))).TimeOfDay;
				if ((startTime.Time.Hours + 3) % 24 < startTime.Time.Hours)
				{
					endDate.Date = startDate.Date.AddDays(1);
				}
			}

			newEvent.StartDate = startDate.Date.Add(startTime.Time);
			newEvent.EndDate = endDate.Date.Add(endTime.Time);

			System.Diagnostics.Debug.WriteLine(newEvent.StartDate.ToString("g") + ", " + newEvent.EndDate.ToString("g"));

			StartDateString.Text = startDate.Date.ToString("ddd") + " " + startDate.Date.ToString("dd/MM/yyyy");
			EndDateString.Text = endDate.Date.ToString("ddd") + " " + endDate.Date.ToString("dd/MM/yyyy");

			StartTimeString.Text = " -   " + new DateTime(startTime.Time.Ticks).ToString("HH:mm");
			EndTimeString.Text = " -   " + new DateTime(endTime.Time.Ticks).ToString("HH:mm");
		}


		public void setBanner(string banner)
		{
			SelectedBannerImage.Source = banner;
			newEvent.ImageSource = banner;
			imageStreams = null;
		}

		private void setNewEvent()
		{
			cancelButton.IsVisible = false;
			//newEvent.ProfileOwner = new Profile(){ProfileId = App.StoredUserFacebookId};

			System.Diagnostics.Debug.WriteLine(startTime.Time);
			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay);
			System.Diagnostics.Debug.WriteLine(new TimeSpan(1, 10, 0));

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
			if (!didTrySucceed)
			{
				startTime.Time.Add(new TimeSpan(DateTime.Now.TimeOfDay.Hours + 3, 0, 0));
				endTime.Time.Add(new TimeSpan(DateTime.Now.TimeOfDay.Hours + 5, 0, 0));
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
			NumberAttendendeesEntry.Text = newEvent.MaxSize + "";
			validAttendingAmount = true;
			visibilityPicker.Title = newEvent.Visibility.ToString();

			//visibilityPicker.IsEnabled = false;
			SelectedBannerImage.Source = newEvent.ImageSource;

		}

		private async void LaunchEvent(Event eventToCreate)
		{
			if (String.IsNullOrWhiteSpace(eventToCreate.Title))
			{
				await App.coreView.displayAlertMessage("Title Missing", "Title is missing", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(eventToCreate.Description))
			{
				await App.coreView.displayAlertMessage("Description Missing", "Description is missing", "Ok");
			}
			else if ((eventToCreate.MaxSize == -1) || !validAttendingAmount)
			{
				await App.coreView.displayAlertMessage("Attendendees Needed Missing", "", "Ok");
			}
			else if (eventToCreate.EventTypes.Count == 0)
			{
				await App.coreView.displayAlertMessage("EventTypes Missing", "No Event Type has been selected", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(eventToCreate.AddressName) || eventToCreate.Latitude == 0)
			{
				await App.coreView.displayAlertMessage("Address Missing", "No valid address has been selected", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(eventToCreate.ImageSource) && imageStreams == null)
			{
				await App.coreView.displayAlertMessage("Banner Missing", "No banner has been selected", "Ok");
			}
			else {
				bool continueCreating = true;
				if (isCreate)
				{
					continueCreating = await App.coreView.otherFunctions.SenderOfEvent(SelectSenderLayout, eventToCreate);
				}
				else {
					continueCreating = true;
				}
				App.coreView.IsLoading(true);
				if (continueCreating)
				{

					if (imageStreams != null)
					{
						eventToCreate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[2]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
					}
					if (isCreate)
					{
						if (eventToCreate.GroupOwner == null)
						{
							eventToCreate.ProfileOwners = new List<Profile> { App.userProfile };
						}
						eventToCreate.MinAge = 0;
						eventToCreate.MaxAge = 100;
						eventToCreate.MinSize = 1;
					}

					Event eventCreated = await _dataManager.EventApiManager.CreateEditEvent(eventToCreate);

					if (eventCreated != null)
					{
						App.coreView.joinedEvents.UpdateList(true, "");
						eventCreated.Attendees = new List<Profile>();
						eventCreated.Followers = new List<Profile>();
						InspectController inspect = new InspectController(eventCreated);
						if (isCreate)
						{
							App.coreView.updateMainViews(4);
							App.coreView.setContentView(1);
							App.coreView.setContentViewWithQueue(inspect);
							App.coreView.updateMainViews(0);
						}
						else {
							App.coreView.setContentViewReplaceCurrent(inspect, 1);
						}
						App.coreView.updateMainViews(0);
					}
					else {
						await App.coreView.displayAlertMessage("Error", "Event not created, try again", "Ok");
					}
				}
			}
			Launching = false;
			App.coreView.IsLoading(false);
		}

		public async void CancelTheEvent()
		{
			bool confirmDelete = await App.coreView.displayConfirmMessage("Warning", "You are about to delete this event permanently, would you like to continue", "Yes", "No");
			App.coreView.IsLoading(true);
			if (confirmDelete)
			{
				bool wasEventDeleted = await _dataManager.EventApiManager.DeleteEvent(newEvent.EventId);
				if (wasEventDeleted)
				{
					await App.coreView.displayAlertMessage("Event Deleted", "The event was successfully cancelled", "Ok");
					App.coreView.joinedEvents.UpdateList(true, "");
					App.coreView.updateMainViews(4);
					App.coreView.setContentView(1);
				}
				else {
					App.coreView.displayAlertMessage("Event Not Deleted", "The event was not cancelled, try again", "Ok");
				}
			}
			App.coreView.IsLoading(false);
		}

		public void setLocationButton(string name)
		{
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

