using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ManageEvent : ContentView
	{
		//ObservableCollection<Event> events = new ObservableCollection<Event>();

		public ManageEvent ()
		{
			InitializeComponent ();

			//ManageEventList.ItemsSource = events;

			//events.Add(new Event{ Title="Rob Finnerty", Time="today", Position="2 km away", Description="zxcv asdf qwer zxv asdf qwer asdf qewrt asfdg qwre asfg qt asfdg adfg ewrt sdfg qert dsfg wert cb dfh utryu gh fghn dfn df dfn vb d df hr yr ety gh  bnc cvbn df gh ert hy"});

			GetEventsMatchingOwner (ManageEventList);

			ManageEventList.ItemSelected += OnItemSelected;
		}

		private async void GetEventsMatchingOwner(ListView listView)
		{
			DataManager dataManager = new DataManager();
			listView.ItemsSource = await dataManager.GetEventsWithOwnerId();
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(ManageEventList.SelectedItem == null)
				return;

			var eve = ManageEventList.SelectedItem as Event;

			App.coreView.setContentView(new InspectEvent(eve, 2), 0);

		}
	}
}

