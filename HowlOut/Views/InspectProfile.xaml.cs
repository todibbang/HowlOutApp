using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class InspectProfile : ContentView
	{
		public InspectProfile ()
		{
			InitializeComponent ();

			Likes.Text = "24";
			Loyalty.Text = "82%";
			NameAndAge.Text = "Tobias Bjerge Bang, 23";
		}
	}
}

