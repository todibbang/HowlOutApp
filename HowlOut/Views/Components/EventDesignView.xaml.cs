using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class EventDesignView : ContentView
	{
		DataManager _dataManager;
		public EventDesignView (Event eve, int dimentions, Design design)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();
			ScaleLayout (eve, dimentions, design);
			setTypeSpecificDesign (eve, dimentions, design);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(null,null,eve),"");
			};

			acceptButton.Clicked += (sender, e) => {
				
			};

			declineButton.Clicked += (sender, e) => {
				
			};
		}

		private void ScaleLayout(Event eve, int dimentions, Design design){

			profileGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			profileLayout.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });

			if (design.Equals (Design.Plain)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			} else if (design.Equals (Design.WithName)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.2 });
			} else {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.5 });
			}

			buttonLayout.IsVisible = false;

			infoLabel.FontSize = (int) (0.1 * dimentions);
			acceptButton.FontSize = (int) (0.1 * dimentions);
			declineButton.FontSize = (int) (0.1 * dimentions);

			Loyalty.BorderRadius = (int)(0.175 * dimentions);
			Loyalty.BorderWidth = (int)(0.025 * dimentions);
			Loyalty.Text = "";

			MainButton.IsVisible = true;
			MainButton.BorderRadius = (int) (0.375 * dimentions);
			MainButton.BorderWidth = (int) (0.04 * dimentions);
			MainButton.Text = eve.NumberOfAttendees + "";
			if(eve.NumberOfAttendees == 0 && eve.Attendees != null) {
				MainButton.Text = eve.Attendees.Count + "";
			}
			infoLabel.Text = eve.Title;
		}

		private void setTypeSpecificDesign(Event eve, int dimentions, Design design) {
			
		}

		public enum Design {
			Plain,
			WithName,
		}
	}
}

