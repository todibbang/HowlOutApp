using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public partial class MapView : ContentView
	{
		public MapView (Event eve)
		{
			InitializeComponent ();
			ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };

			UtilityManager utilityManager = new UtilityManager ();
			utilityManager.setMapForEvent (eve, map, mapLayout);

			System.Diagnostics.Debug.WriteLine ("crazy0: ");

			map.Tapped += (sender, e) => 
			{
				

				System.Diagnostics.Debug.WriteLine ("crazy1: ");


				var pin = new Pin
				{
					Type = PinType.Place,
					Position = new Position(map.tapPosition.Latitude, map.tapPosition.Longitude),
					Label = "",
					Address = "",
				};

				map.Pins.Add (pin);
			};

		}
	}
}

