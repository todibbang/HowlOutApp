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
		MapsView mapView;
		DataManager _dataManager;

		public Event newEvent;
		Dictionary<string, int> agePicker = new Dictionary<string, int> { };
		Dictionary<string, int> sizePicker = new Dictionary<string, int> { };

		List<Button> typeButtons = new List<Button>();

		private bool Launching = false;

		Plugin.Media.Abstractions.MediaFile mediaFile;
		Image banner = new Image();

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
					endDate.Date = newEvent.EndDate;
				}
			};
			startTime.PropertyChanged += (sender, e) =>
			{
				newEvent.StartDate = startDate.Date.Add(startTime.Time);
				var newTimeSpan = newEvent.StartDate + new TimeSpan(1, 0, 0);
				if (newEvent.EndDate.Ticks < newTimeSpan.Ticks)
				{
					newEvent.EndDate = newEvent.StartDate + new TimeSpan(2, 0, 0);
					endTime.Time = newEvent.EndDate.TimeOfDay;
					endDate.Date = newEvent.EndDate;
				}
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
					bool success = await GroupEventIsFor();
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

			takePictureButton.Clicked += async (sender, e) =>
			{
				mediaFile = await _dataManager.UtilityManager.TakePicture();
				if (mediaFile != null)
				{
					SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					selectBannerButton.BackgroundColor = Color.Transparent;
				}
			};

			albumPictureButton.Clicked += async (SenderOfEvent, e) =>
			{
				mediaFile = await _dataManager.UtilityManager.PictureFromAlbum();
				if (mediaFile != null)
				{
					SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					selectBannerButton.BackgroundColor = Color.Transparent;
				}
			};

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
					bool continueCreating = await SenderOfEvent();
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
			selectBannerButton.BackgroundColor = Color.Transparent;
			SelectedBannerImage.Source = banner;
			newEvent.BannerSource = banner;
			mediaFile = null;
		}

		private void setNewEvent()
		{
			cancelButton.IsVisible = false;
			newEvent.Owner = new Profile(){ProfileId = App.StoredUserFacebookId};

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
			newEvent.EndDate = endDate.Date.Add(endTime.Time);

			//minAge.SelectedIndex = 0;
			//maxAge.SelectedIndex = agePicker.Count;

			//newEvent.MinAge = agePicker [minAge.Items[minAge.SelectedIndex]];
			//newEvent.MaxAge = agePicker [maxAge.Items[maxAge.SelectedIndex]];

			//minSize.SelectedIndex = 0;
			//maxSize.SelectedIndex = sizePicker.Count;
			//newEvent.MinSize = sizePicker [minSize.Items[minSize.SelectedIndex]];
			//newEvent.MaxSize = sizePicker [maxSize.Items[maxSize.SelectedIndex]];
			//newEvent.MaxSize = -1;
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
			endDate.Date = newEvent.EndDate.Date;

			locationEntry.Text = newEvent.AddressName;

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


			/*
			int t = 0;

			foreach (EventType en in Enum.GetValues(typeof(EventType)))
			{
				if (newEvent.EventTypes.Contains(en))
				{
					typeButtons[t].BackgroundColor = App.HowlOut;
					typeButtons[t].TextColor = Color.White;
				}
				t++;
			}
			*/

			/*
			if(newEvent.EventTypes.Contains(EventType.Party)) { fest = PreSetButton(fest, EventType.Party); };
			if(newEvent.EventTypes.Contains(EventType.Sport)) { sport = PreSetButton(sport, EventType.Sport); };
			if(newEvent.EventTypes.Contains(EventType.Culture)) { kultur = PreSetButton(kultur, EventType.Culture); };
			if(newEvent.EventTypes.Contains(EventType.Movie)) { film = PreSetButton(film, EventType.Movie); };
			if(newEvent.EventTypes.Contains(EventType.Music)) { musik = PreSetButton(musik, EventType.Music); };
			if(newEvent.EventTypes.Contains(EventType.Cafe)) { cafe = PreSetButton(cafe, EventType.Cafe); };
			if(newEvent.EventTypes.Contains(EventType.Food)) { mad = PreSetButton(mad, EventType.Food); };
			if(newEvent.EventTypes.Contains(EventType.Hobby)) { hobby = PreSetButton(hobby, EventType.Hobby); };
			*/
		}
			
		private async void LaunchEvent(Event eventToCreate)
		{
			if (mediaFile != null)
			{
				eventToCreate.BannerSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
			}
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
			} else if (String.IsNullOrWhiteSpace (eventToCreate.BannerSource)) {
				await App.coreView.displayAlertMessage ("Banner Missing", "No banner has been selected", "Ok");
			}else {
				EventDBO newEventAsDBO = new EventDBO{
					Owner = eventToCreate.Owner, 
					OrganisationOwner = eventToCreate.OrganisationOwner,
					Title = eventToCreate.Title, 
					Description = eventToCreate.Description,
					StartDate = eventToCreate.StartDate, 
					EndDate = eventToCreate.EndDate, 
					MinAge = eventToCreate.MinAge,
					MaxAge = eventToCreate.MaxAge, 
					MinSize = eventToCreate.MinSize, 
					MaxSize = eventToCreate.MaxSize, 
					Visibility = eventToCreate.Visibility, 
					Latitude = eventToCreate.Latitude,
					Longitude = eventToCreate.Longitude, 
					AddressName = eventToCreate.AddressName, 
					EventTypes = eventToCreate.EventTypes,
					BannerName = eventToCreate.BannerSource,
					GroupSpecific = eventToCreate.GroupSpecific
				};

				Event eventCreated = await _dataManager.EventApiManager.CreateEvent (newEventAsDBO);

				if (eventCreated != null) {
					eventCreated.Attendees = new List<Profile> ();
					eventCreated.Followers = new List<Profile> ();
					InspectController inspect = new InspectController(null, null, eventCreated);
					App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
					App.coreView.createEventView = new CreateEvent(new Event(), true);
				} else {
					await App.coreView.displayAlertMessage ("Error", "Event not created, try again", "Ok");
				}
			}
			Launching = false;
		}

		private async void UpdateEvent(Event eventToUpdate)
		{
			bool eventUpdated = await _dataManager.EventApiManager.UpdateEvent (eventToUpdate);

			if (eventUpdated) {

				InspectController inspect = new InspectController(null, null, eventToUpdate);
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

		public async Task<bool> GroupEventIsFor()
		{
			bool continueCreating = false;

			// Dummy Data End

			if (App.userProfile.Groups != null && App.userProfile.Groups.Count > 0)
			{
				SelectEventSenderLayout.Children.Clear();
				SelectEventSenderLayout.Children.Add(new Label()
				{
					Text = "Who is the sender of this event?",
					TextColor = Color.White,
					FontSize = 16,
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				});

				List<Button> buttons = new List<Button>();

				foreach (Group o in App.userProfile.Groups)
				{
					organisationButton(o.Name, buttons);
				}
				organisationButton("Cancel", buttons);

				TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
				SelectEventSenderLayout.IsVisible = true;
				foreach (Button b in buttons)
				{
					b.Clicked += (sender, e) =>
					{
						
						if (b == buttons[buttons.Count - 1])
						{
							System.Diagnostics.Debug.WriteLine("Cancel creating event");
							continueCreating = false;
						}
						else {
							System.Diagnostics.Debug.WriteLine("Sender of event is " + App.userProfile.Groups[buttons.IndexOf(b)].Name);
							newEvent.GroupSpecific = App.userProfile.Groups[buttons.IndexOf(b)];
							continueCreating = true;
						}
						tcs.TrySetResult(true);
					};
				}

				await tcs.Task;
				SelectEventSenderLayout.IsVisible = false;
			}

			return continueCreating;
		}

		public async Task<bool> SenderOfEvent()
		{
			bool continueCreating = false;
			App.userProfile.Organizations = new List<Group>();

			// Dummy Data Start
			App.userProfile.Organizations.Add(
				new Group()
				{
				Visibility = Visibility.Organization,
				Name = "ITU",
				}
			);
			App.userProfile.Organizations.Add(
				new Group()
				{
					Visibility = Visibility.Organization,
					Name = "Københavns Erhvervs Akademi",
				}
			);
			// Dummy Data End

			if (App.userProfile.Organizations != null && App.userProfile.Organizations.Count > 0)
			{
				SelectEventSenderLayout.Children.Clear();
				SelectEventSenderLayout.Children.Add(new Label()
				{
					Text = "Who is the sender of this event?",
					TextColor = Color.White,
					FontSize = 16,
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				});

				List<Button> buttons = new List<Button>();
				organisationButton(App.userProfile.Name, buttons);

				foreach (Group o in App.userProfile.Organizations)
				{
					organisationButton(o.Name, buttons);
				}
				organisationButton("Cancel", buttons);

				TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
				SelectEventSenderLayout.IsVisible = true;
				foreach (Button b in buttons)
				{
					b.Clicked += (sender, e) =>
					{
						

						if (b == buttons[0])
						{
							System.Diagnostics.Debug.WriteLine("You are the sender of the event");
							continueCreating = true;
						}
						else if (b == buttons[buttons.Count - 1])
						{
							System.Diagnostics.Debug.WriteLine("Cancel creating event");
							continueCreating = false;
						}
						else {
							System.Diagnostics.Debug.WriteLine("Sender of event is " + App.userProfile.Organizations[buttons.IndexOf(b)-1].Name);
							continueCreating = true;
						}
						tcs.TrySetResult(true);
					};
				}

				await tcs.Task;
				SelectEventSenderLayout.IsVisible = false;
			}

			return continueCreating;
		}

		void organisationButton(String name, List<Button> buttons)
		{
			Button oB = new Button()
			{
				Text = name,
				HeightRequest = 30,
				WidthRequest = 100,
				FontSize = 14,
				TextColor = App.HowlOut,
				BorderColor = App.HowlOut,
				BorderWidth = 1,
				BorderRadius = 10,
				BackgroundColor = Color.White,
			};
			buttons.Add(oB);
			SelectEventSenderLayout.Children.Add(oB);
		}
	}
}

