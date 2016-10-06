﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
		Button selectButton = new Button() { TextColor= App.HowlOut, BorderColor=App.HowlOutFade, BorderWidth=1, BorderRadius=10, HeightRequest=40 };

		bool searching = false;
		bool searchingQQ = false;

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
				//App.coreView.setContentView (createEventView, "CreateEvent");
				App.coreView.returnToPreviousView();
			};

			searchBar.TextChanged += (sender, e) => {
				if(mapTapped || searchBar.Text == "" || searchBar.Text == null) { 
					mapTapped = false; 
					searchList.HeightRequest=0;
					searchList.IsVisible = false;
				} else {
					System.Diagnostics.Debug.WriteLine("Before1");
					updateAutocompleteList();
					System.Diagnostics.Debug.WriteLine("After1");
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
			utilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.AddressName);
			searchList.IsVisible = false;
			searchList.HeightRequest=0;
			selectButton.IsVisible = false;
			searchBar.IsVisible = false;
		}

		public async void getResults(Position position, ExtMap map)
		{
			tappedAddress="";
			var possibleAddresses = await getAddressFromPosition.GetAddressesForPositionAsync (position);
			foreach (var address in possibleAddresses) { tappedAddress += address; }
			tappedAddress = Regex.Replace(tappedAddress, @"\r\n?|\n", ",");
			utilityManager.setPin (map.tapPosition, map, tappedAddress, tappedAddress);
			setTheNewAddressToTheEvent (map.tapPosition, tappedAddress);
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
			else if (!searchingQQ){
				searchingQQ = true;
				await Task.Delay(100); 
				updateAutocompleteList();
				searchingQQ = false;
			}
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
			setSelectedAddress (selectedAddress);
		}

		public async void setSelectedAddress(Address newAddress)
		{
			newAddress.data.position = await dataManager.GetCoordinates(newAddress.data.href);
			utilityManager.setMapForEvent(newAddress.data.position, map, mapLayout);
			utilityManager.setPin (newAddress.data.position, map, newAddress.forslagstekst, newAddress.forslagstekst);
			setTheNewAddressToTheEvent (newAddress.data.position, newAddress.forslagstekst);
		}

		public void setTheNewAddressToTheEvent(Position position, string address)
		{
			if (createEventView != null) {
				createEventView.newEvent.Latitude = position.Latitude;
				createEventView.newEvent.Longitude = position.Longitude;
				createEventView.newEvent.AddressName = address;
				createEventView.setLocationButton(address);
			}
		}
	}
}

