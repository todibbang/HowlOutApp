using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class MapsView : ContentView
	{
		public Geocoder getAddressFromPosition = new Geocoder ();
		public String tappedAddress = "";
		public CreateEvent createEventView;
		UtilityManager utilityManager = new UtilityManager ();
		DataManager dataManager = new DataManager ();

		ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };
		Button selectButton = new Button ();

		public bool mapTapped = false;

		public MapsView (Position position)
		{
			InitializeComponent ();
			mapLayout.VerticalOptions = LayoutOptions.FillAndExpand;
			utilityManager.setMapForEvent(position, map, mapLayout);
			selectButton.Text = "selectButton";
			selectButton.BackgroundColor = Color.White;
			selectLayout.Children.Add (selectButton);
			searchList.IsVisible = false;
			searchList.HeightRequest=0;

			map.Tapped += (sender, e) => {
				mapTapped = true;
				getResults(map.tapPosition, map);
				searchList.IsVisible = false;
				searchList.HeightRequest=0;
			};

			selectButton.Clicked += (sender, e) => {
				App.coreView.setContentView (createEventView, "CreateEvent");
			};

			searchBar.TextChanged += (sender, e) => {
				if(mapTapped || searchBar.Text == "" || searchBar.Text == null) { 
					mapTapped = false; 
					searchList.HeightRequest=0;
					searchList.IsVisible = false;
				} else {
					updateAutocompleteList();
					searchList.HeightRequest=300;
					searchList.IsVisible = true;
				}
			};
			if (searchBar.Text == "" || searchBar.Text == null) { 
				searchList.HeightRequest=0;
				searchList.IsVisible = false;
			}

			searchList.ItemSelected += OnItemSelected;
		}

		public MapsView (Event eve)
		{
			InitializeComponent ();
			utilityManager.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
			utilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.PositionName);
		}

		public async void getResults(Position position, ExtMap map)
		{
			tappedAddress="";
			var possibleAddresses = await getAddressFromPosition.GetAddressesForPositionAsync (position);
			foreach (var address in possibleAddresses) { tappedAddress += address; }
			tappedAddress = Regex.Replace(tappedAddress, @"\r\n?|\n", ",");

			map.Pins.Clear();
			utilityManager.setPin (new Position (map.tapPosition.Latitude, map.tapPosition.Longitude), map, tappedAddress, tappedAddress);

			if (createEventView != null) {
				createEventView.newEvent.Latitude = map.tapPosition.Latitude;
				createEventView.newEvent.Longitude = map.tapPosition.Longitude;
				createEventView.newEvent.PositionName = tappedAddress;
				createEventView.locationButton.Text = tappedAddress;
			}
			searchBar.Text = tappedAddress;
		}

		public async void updateAutocompleteList()
		{
			var addresses = await dataManager.AutoCompletionPlace(searchBar.Text);
			searchList.ItemsSource = addresses;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(searchList.SelectedItem == null)
				return;
			var selectedAddress = searchList.SelectedItem as Address;
			searchBar.Text = selectedAddress.forslagstekst;
			searchList.SelectedItem = null;
			searchList.IsVisible = false;
			searchList.HeightRequest=0;
		}
	}
}

