using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class MapsView : ContentView, ViewModelInterface
	{
		public Geocoder getAddressFromPosition = new Geocoder();
		public String tappedAddress = "";
		public CreateEvent createEventView;
		UtilityManager utilityManager = new UtilityManager();
		DataManager dataManager = new DataManager();
		Event eve;

		ExtMap map = new ExtMap() { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };
		Button selectButton = new Button() { BackgroundColor = App.HowlOut, TextColor = Color.White, BorderRadius = 10, HeightRequest = 40 };

		bool create = false;

		bool searching = false;
		bool searchingQQ = false;

		bool searchingTap = false;
		public bool mapTapped = false;

		public MapsView(Position position)
		{
			InitializeComponent();
			create = true;

			mapLayout.VerticalOptions = LayoutOptions.FillAndExpand;
			utilityManager.setMapForEvent(position, map, mapLayout, 1.2);
			selectButton.Text = "Current Location";
			//selectButton.BackgroundColor = Color.White;
			selectLayout.Children.Add(selectButton);
			searchList.IsVisible = false;
			searchList.HeightRequest = 0;

			map.Tapped += (sender, e) =>
			{
				if (!searchingTap)
				{
					selectButton.Text = "Select This Position";
					searchingTap = true;
					mapTapped = true;
					getResults(map.tapPosition, map);
					searchList.IsVisible = false;
					searchList.HeightRequest = 0;
				}
			};

			selectButton.Clicked += async (sender, e) =>
			{
				if (!mapTapped)
				{
					await getResults(position, map);
				}

				createEventView.otherViews.IsVisible = false;
				App.coreView.createView.displayBackButton(false, 1);
				//App.coreView.setContentView (createEventView, "CreateEvent");
				//App.coreView.returnToPreviousView();
			};

			searchBar.TextChanged += (sender, e) =>
			{
				if (searchingTap || searchBar.Text == "" || searchBar.Text == null)
				{
					searchingTap = false;
					searchList.HeightRequest = 0;
					searchList.IsVisible = false;
				}
				else {
					System.Diagnostics.Debug.WriteLine("Before1");
					updateAutocompleteList();
					System.Diagnostics.Debug.WriteLine("After1");
					searchList.HeightRequest = 300;
					searchList.IsVisible = true;
					mapTapped = false;
				}

				if (searchBar.Text == "" || searchBar.Text == null)
				{
					selectButton.Text = "Current Location";
					mapTapped = false;
				}
			};
			if (searchBar.Text == "" || searchBar.Text == null)
			{
				searchList.HeightRequest = 0;
				searchList.IsVisible = false;
			}

			searchList.ItemSelected += OnItemSelected;
		}

		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			if (create) ub.setNavigationlabel("Select location for event");
			return ub;
		}

		public void viewInFocus(UpperBar bar)
		{
			//if (create) App.coreView.topBar.setNavigationlabel("Select location for event");
		}

		public void viewExitFocus() { }
		public void reloadView() { }

		public ContentView getContentView() { return this; }

		public MapsView(Event eve)
		{
			InitializeComponent();
			utilityManager.setMapForEvent(new Position(eve.Latitude, eve.Longitude), map, mapLayout, 1.2);
			utilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.AddressName);
			this.eve = eve;
			searchList.IsVisible = false;
			searchList.HeightRequest = 0;
			selectButton.IsVisible = false;
			searchBar.IsVisible = false;
		}

		public MapsView(Position position, bool setOwn)
		{
			InitializeComponent();
			searchLayout.IsVisible = false;
			searchList.IsVisible = false;
			selectLayout.IsVisible = false;



			if (setOwn)
			{
				utilityManager.setMapForEvent(position, map, mapLayout, 30);
				map.Tapped += async (sender, e) =>
				{
					await App.storeManualPosition(map.tapPosition.Latitude.ToString(), map.tapPosition.Longitude.ToString());
					await Task.Delay(10);
					App.coreView.returnToPreviousView();
					System.Diagnostics.Debug.WriteLine(map.tapPosition.Latitude + ", " + map.tapPosition.Longitude);

					//App.coreView.exploreEventCategories.update();
					App.coreView.exploreEvents.UpdateList(true, "");
					//App.coreView.joinedEvents.UpdateList(true, "");
				};
			}
			else {
				setMap(position, 30, true);
				//utilityManager.setMapForEvent(position, map, mapLayout, 10);
			}
		}

		public async void setMap(Position position, double dis, bool pin)
		{
			await Task.Delay(100);

			utilityManager.setMapForEvent(position, map, mapLayout, dis);
			if(pin)
				utilityManager.setPin(position, map, "Your Location", "");
		}

		public async Task getResults(Position position, ExtMap map)
		{
			tappedAddress = "";
			var possibleAddresses = await getAddressFromPosition.GetAddressesForPositionAsync(position);
			foreach (var address in possibleAddresses) { tappedAddress += address; }
			tappedAddress = Regex.Replace(tappedAddress, @"\r\n?|\n", ",");
			utilityManager.setPin(position, map, tappedAddress, tappedAddress);
			setTheNewAddressToTheEvent(position, tappedAddress);
			searchBar.Text = tappedAddress;

		}

		public async void updateAutocompleteList()
		{
			if (!searching)
			{
				searching = true;
				System.Diagnostics.Debug.WriteLine("Before2");
				var addresses = await dataManager.AutoCompletionPlace(searchBar.Text);
				searchList.ItemsSource = addresses;
				System.Diagnostics.Debug.WriteLine("After2");
				searching = false;
			}
			else if (!searchingQQ)
			{
				searchingQQ = true;
				await Task.Delay(100);
				updateAutocompleteList();
				searchingQQ = false;
			}
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (searchList.SelectedItem == null)
				return;
			var selectedAddress = searchList.SelectedItem as Address;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			searchBar.Text = selectedAddress.forslagstekst;
			searchList.SelectedItem = null;
			searchList.IsVisible = false;
			searchList.HeightRequest = 0;
			mapTapped = true;
			selectButton.Text = "Select This Position";
			setSelectedAddress(selectedAddress);
		}

		public async Task setSelectedAddress(Address newAddress)
		{
			newAddress.data.position = await dataManager.GetCoordinates(newAddress.data.href);
			utilityManager.setMapForEvent(newAddress.data.position, map, mapLayout, 1.2);
			utilityManager.setPin(newAddress.data.position, map, newAddress.forslagstekst, newAddress.forslagstekst);
			setTheNewAddressToTheEvent(newAddress.data.position, newAddress.forslagstekst);
		}

		public void setTheNewAddressToTheEvent(Position position, string address)
		{
			if (createEventView != null)
			{
				createEventView.newEvent.Latitude = position.Latitude;
				createEventView.newEvent.Longitude = position.Longitude;
				createEventView.newEvent.AddressName = address;
				createEventView.setLocationButton(address);
			}
		}
	}
}

