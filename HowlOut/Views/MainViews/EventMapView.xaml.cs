using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class EventMapView : ContentView, ViewModelInterface
	{
		public void reloadView() { }
		public void viewInFocus(UpperBar bar) {
			bar.setNavigationlabel("Here & Now");
		}
		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			ub.setNavigationlabel("Here & Now");
			return ub;
		}
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }

		public EventMapView()
		{
			InitializeComponent();
			setUpMat();
		}

		async void setUpMat()
		{
			var customMap = new CustomMap
			{
				MapType = MapType.Street,
			};
			customMap.CustomPins = new List<CustomPin> ();

			var evelist = new List<Event>();
			evelist = await App.coreView._dataManager.EventApiManager.SearchEvents("");

			foreach (Event eve in evelist)
			{
				var pin = new CustomPin
				{
					Pin = new Pin
					{
						Type = PinType.Generic,
						Position = new Position(eve.Latitude, eve.Longitude),
						Label = eve.Title,
						Address = eve.AddressName + " - " + eve.StartDate.ToString("M"),
					},
					Id = "Xamarin",
					Url = eve.EventId,
					eve = eve
				};

				customMap.CustomPins.Add(pin);
				customMap.Pins.Add(pin.Pin);
			}




			customMap.MoveToRegion(
				MapSpan.FromCenterAndRadius(
					App.lastKnownPosition, Distance.FromKilometers(5)));
    		Content = customMap;
		}
	}
}
