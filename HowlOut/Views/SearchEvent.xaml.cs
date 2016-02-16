using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class SearchEvent : ContentView
	{
		//ObservableCollection<Event> events = new ObservableCollection<Event>();

		public SearchEvent ()
		{
			InitializeComponent ();

			GetAllEvents (SearchEventList);

			//SearchEventList.ItemsSource = events;
			//events.Add(new Event{ Title="Rob Finnerty", Time="today", Position="2 km away", Description="zxcv asdf qwer zxv asdf qwer asdf qewrt asfdg qwre asfg qt asfdg adfg ewrt sdfg qert dsfg wert cb dfh utryu gh fghn dfn df dfn vb d df hr yr ety gh  bnc cvbn df gh ert hy"});

			SearchEventList.ItemSelected += OnItemSelected;
		}

		private async void GetAllEvents(ListView listView)
		{
			DataManager dataManager = new DataManager();
			listView.ItemsSource = await dataManager.GetAllEvents();
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(SearchEventList.SelectedItem == null)
				return;

			var ev = SearchEventList.SelectedItem as Event;

			App.coreView.setContentView(new InspectEvent(ev, 1), 0);

		}
	}
}

