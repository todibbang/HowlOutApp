using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class FilterSearch : ContentView
	{	
		int MinAgeSearched = 0;

		public FilterSearch ()
		{
			InitializeComponent ();

			Dictionary<string, int> agePicker = new Dictionary<string, int> {
			};
			for (int i = 18; i < 100; i++)
				agePicker.Add ("" + i, i);

			foreach (string number in agePicker.Keys)
			{
				minAge.Items.Add(number);
				maxAge.Items.Add(number);
			}

			minAge.SelectedIndexChanged += (sender, args) => {
				if (minAge.SelectedIndex != -1) { string number = minAge.Items[minAge.SelectedIndex]; MinAgeSearched = agePicker[number]; } };

		}
	}
}

