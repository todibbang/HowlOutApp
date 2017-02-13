using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class VoteItemEditTemplate : ContentView
	{
		public TapGestureRecognizer deleteTapped = new TapGestureRecognizer();

		public VoteItemEditTemplate()
		{
			InitializeComponent();

			removeIcon.GestureRecognizers.Add(deleteTapped);
		}
	}
}
