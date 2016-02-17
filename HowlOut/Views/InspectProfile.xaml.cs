using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectProfile : ContentView
	{
		public InspectProfile (string facebookUserId)
		{
			InitializeComponent ();

			Likes.Text = "24";
			Loyalty.Text = "82%";
			NameAndAge.Text = "Tobias Bjerge Bang, 23";

			//var profilePicUri = new Uri("http://xamarin.com/content/images/pages/forms/example-app.png");
			var profilePicUri = new Uri("https://graph.facebook.com/v2.5/"+facebookUserId+"/picture");

			ProfileImage.Source = ImageSource.FromUri(profilePicUri);
		}
	}
}

