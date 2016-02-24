using System;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public class ExtMap : Map
	{
		public Position tapPosition;
		public bool pressed;

		/// <summary>
		/// Event thrown when the user taps on the map
		/// </summary>
		public event EventHandler<MapTapEventArgs> Tapped;

		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public ExtMap()
		{

		}

		/// <summary>
		/// Constructor that takes a region
		/// </summary>
		/// <param name="region"></param>
		public ExtMap(MapSpan region)
			: base(region)
		{

		}

		#endregion

		public void OnTap(Position coordinate)
		{
			tapPosition = coordinate;
			OnTap(new MapTapEventArgs { Position = coordinate });
		}

		protected virtual void OnTap(MapTapEventArgs e)
		{
			var handler = Tapped;

			if (handler != null)
				handler(this, e);
		}
	}

	/// <summary>
	/// Event args used with maps, when the user tap on it
	/// </summary>
	public class MapTapEventArgs : EventArgs
	{
		public Position Position { get; set; }

		public MapTapEventArgs()
		{
		}
	}
}

