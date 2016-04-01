using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class InspectManageComment : ContentView
	{
		public InspectManageComment ()
		{
			InitializeComponent ();
		}
		public InspectManageComment (Comment comment)
		{
			InitializeComponent ();
			BindingContext = comment;
		}
	}
}

