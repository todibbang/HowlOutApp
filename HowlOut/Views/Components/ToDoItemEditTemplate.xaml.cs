using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ToDoItemEditTemplate : ContentView
	{
		public TapGestureRecognizer deleteTapped = new TapGestureRecognizer();

		public ToDoItemEditTemplate()
		{
			InitializeComponent();

			removeIcon.GestureRecognizers.Add(deleteTapped);

			lateSetup();
		}

		async void lateSetup()
		{
			await Task.Delay(100);
			var thisItem = (ToDoItem)this.BindingContext;

			profilesNeededPicker.SelectedIndex = (thisItem.ProfilesNeeded - 1);
			profilesNeededPicker.Title = (thisItem.ProfilesNeeded) + "";
		}
	}
}
