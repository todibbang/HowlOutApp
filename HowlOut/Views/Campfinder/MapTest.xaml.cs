using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class MapTest : ContentView
	{
		DataManager _dataManager = new DataManager ();

		double marketX;
		double marketY;
		double marketScale = 4;

		double coffeeX;
		double coffeeY;
		double coffeeScale = 10;

		double startX;
		double startY;
		double startScale;
		double currentScale = 1;

		double xOffset;
		double yOffset;
		bool zooming = false;

		public MapTest ()
		{
			InitializeComponent ();

			marketX = 540 / 100 * 10;
			marketY = 303 / 100 * 30;

			coffeeX = 540 / 100 * 30;
			coffeeY = 303 / 100 * 10;

			currentScale = Donus.Scale;

			//marketX = 100;
			//marketY = 0;
			startMarket ();
			updateMarket ();

			searchList.IsVisible = false;
			searchList.HeightRequest=0;

			searchBar.TextChanged += (sender, e) => {
				if(searchBar.Text == "" || searchBar.Text == null) { 
					searchList.HeightRequest=0;
					searchList.IsVisible = false;
				} else {
					updateAutocompleteList();
					searchList.HeightRequest=300;
					searchList.IsVisible = true;
				}
			};
			if (searchBar.Text == "" || searchBar.Text == null) { 
				searchList.HeightRequest=0;
				searchList.IsVisible = false;
			}

			searchList.ItemSelected += OnItemSelected;



			var tapGestureRecognizer = new TapGestureRecognizer ();
			tapGestureRecognizer.Tapped += async (object sender, EventArgs e) => {

				var panStartPositionX = ((relativeLayoutTest.TranslationX*(-1) + (App.coreView.Width / 2)) /  currentScale);
				var panStartPositionY = ((relativeLayoutTest.TranslationY*(-1) + (App.coreView.Height / 2)) /  currentScale);

				var panDistanceX = (coffeeX - panStartPositionX);
				var panDistanceY = (coffeeY - panStartPositionY);

				int steps = 200;
				var scaleStep = 4 - (currentScale);
				for (int i = 0; i < steps; i++) {
					if (i == 0) {
						CenterOnCamp (false, 1.0, panStartPositionX + ((panDistanceX / steps) * (i + 1)), panStartPositionY + ((panDistanceY / steps) * (i + 1)));
					} else {
						CenterOnCamp (true, 1 + (scaleStep / steps), panStartPositionX + ((panDistanceX / steps) * (i + 1)), panStartPositionY + ((panDistanceY / steps) * (i + 1)));
					}
					await Task.Delay(4);
				}
			};

			Market.GestureRecognizers.Add (tapGestureRecognizer);
			Coffee.GestureRecognizers.Add (tapGestureRecognizer);

			var pinchGestureRecognizer = new PinchGestureRecognizer();
			pinchGestureRecognizer.PinchUpdated += OnPinchUpdated;
			this.GestureRecognizers.Add(pinchGestureRecognizer);
			var panGestureRecognizer = new PanGestureRecognizer();
			panGestureRecognizer.PanUpdated += OnPanUpdated;
			this.GestureRecognizers.Add(panGestureRecognizer);
		}

		public async void updateAutocompleteList()
		{
			var addresses = await _dataManager.AutoCompletionPlace(searchBar.Text);
			searchList.ItemsSource = addresses;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(searchList.SelectedItem == null)
				return;
			var selectedAddress = searchList.SelectedItem as Address;
			searchBar.Text = selectedAddress.forslagstekst;
			searchList.SelectedItem = null;
			searchList.IsVisible = false;
			searchList.HeightRequest=0;
		}

		void CenterOnCamp (bool Status, double Scale, double X, double Y) {
			if (Status == false) {
				startScale = relativeLayoutTest.Scale;
				currentScale = startScale;
				relativeLayoutTest.AnchorX = 0;
				relativeLayoutTest.AnchorY = 0;
			}
			if (Status == true) {
				currentScale += (Scale - 1);
				currentScale = Math.Max (1, currentScale);
				currentScale = Math.Min (4, currentScale);

				relativeLayoutTest.Scale = currentScale;
				relativeLayoutTest.TranslationX = (X * currentScale * (-1)) + (App.coreView.Width / 2);
				relativeLayoutTest.TranslationY = (Y * currentScale * (-1)) + (App.coreView.Height / 2);

				if (((int) relativeLayoutTest.TranslationX) > 0) {
					relativeLayoutTest.TranslationX = 0;
				} else if ((((int) relativeLayoutTest.TranslationX) + (relativeLayoutTest.Width * relativeLayoutTest.Scale)) < App.coreView.Width) {
					relativeLayoutTest.TranslationX = (App.coreView.Width - (relativeLayoutTest.Width * relativeLayoutTest.Scale));
				}
			}
		}

		void OnPinchUpdated (object sender, PinchGestureUpdatedEventArgs e) {
			if (e.Status == GestureStatus.Started) {
				startScale = relativeLayoutTest.Scale;
				xOffset = relativeLayoutTest.TranslationX;
				yOffset = relativeLayoutTest.TranslationY;
				relativeLayoutTest.AnchorX = 0;
				relativeLayoutTest.AnchorY = 0;
			}
			if (e.Status == GestureStatus.Running) {
				currentScale += (e.Scale - 1) * startScale;
				currentScale = Math.Max (1, currentScale);

				double renderedX = relativeLayoutTest.X + xOffset;
				double deltaX = renderedX / Width;
				double deltaWidth = Width / (relativeLayoutTest.Width * startScale);
				double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

				double renderedY = relativeLayoutTest.Y + yOffset;
				double deltaY = renderedY / Height;
				double deltaHeight = Height / (relativeLayoutTest.Height * startScale);
				double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

				double targetX = xOffset - (originX * relativeLayoutTest.Width) * (currentScale - startScale);
				double targetY = yOffset - (originY * relativeLayoutTest.Height) * (currentScale - startScale);

				relativeLayoutTest.TranslationX = targetX;
				relativeLayoutTest.TranslationY = targetY;
				relativeLayoutTest.Scale = currentScale;

				if (((int) relativeLayoutTest.TranslationX) > 0) {
					relativeLayoutTest.TranslationX = 0;
				} else if ((((int) relativeLayoutTest.TranslationX) + (relativeLayoutTest.Width * relativeLayoutTest.Scale)) < App.coreView.Width) {
					relativeLayoutTest.TranslationX = (App.coreView.Width - (relativeLayoutTest.Width * relativeLayoutTest.Scale));
				}
			}
		}

		void OnPanUpdated (object sender, PanUpdatedEventArgs e)
		{
			if (e.StatusType == GestureStatus.Started) {
				startX = relativeLayoutTest.TranslationX;
				startY = relativeLayoutTest.TranslationY;
			}
			if (e.StatusType == GestureStatus.Running) {
				relativeLayoutTest.TranslationX = e.TotalX + startX;
				relativeLayoutTest.TranslationY = e.TotalY + startY;

				if (((int) relativeLayoutTest.TranslationX) > 0) {
					relativeLayoutTest.TranslationX = 0;
				} else if ((((int) relativeLayoutTest.TranslationX) + (relativeLayoutTest.Width * relativeLayoutTest.Scale)) < App.coreView.Width) {
					relativeLayoutTest.TranslationX = (App.coreView.Width - (relativeLayoutTest.Width * relativeLayoutTest.Scale));
				}
			}
		}

		void startMarket() {
			Market.AnchorX = 0;
			Market.AnchorY = 0;
			Coffee.AnchorX = 0;
			Coffee.AnchorY = 0;
		}

		void updateMarket() {
			Market.Scale = currentScale / marketScale;
			Coffee.Scale = currentScale / coffeeScale;

			Market.TranslationX = Donus.TranslationX + (marketX * currentScale) - (Market.Width * Market.Scale / 2);
			Market.TranslationY = Donus.TranslationY + (marketY * currentScale) - (Market.Height * Market.Scale / 2);

			Coffee.TranslationX = Donus.TranslationX + (coffeeX * currentScale) - (Coffee.Width * Coffee.Scale / 2);
			Coffee.TranslationY = Donus.TranslationY + (coffeeY * currentScale) - (Coffee.Height * Coffee.Scale / 2);
		}
	}
}



