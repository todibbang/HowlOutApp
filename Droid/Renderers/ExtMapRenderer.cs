using System;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Maps;
using HowlOut.Android.CustomRenderers;
using HowlOut;

[assembly: ExportRenderer(typeof(ExtMap), typeof(ExtMapRenderer))]
namespace HowlOut.Android.CustomRenderers
{
	/// <summary>
	/// Renderer for the xamarin map.
	/// Enable user to get a position by taping on the map.
	/// </summary>
	public class ExtMapRenderer : MapRenderer, IOnMapReadyCallback
	{
		// We use a native google map for Android
		private GoogleMap _map;

		public void OnMapReady(GoogleMap googleMap)
		{
			_map = googleMap;

			if (_map != null)
				_map.MapClick += googleMap_MapClick;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
		{
			if (_map != null)
				_map.MapClick -= googleMap_MapClick;

			base.OnElementChanged(e);

			if (Control != null)
				((MapView)Control).GetMapAsync(this);
		}

		private void googleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
		{
			((ExtMap)Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
		}
	}
}

