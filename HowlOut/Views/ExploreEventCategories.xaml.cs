using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class ExploreEventCategories : ContentView, ViewModelInterface
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}
		public void reloadView() { }
		public void viewInFocus(UpperBar bar) { }
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }

		List<string> ab = new List<string> { "img1.jpg", "img2.jpeg", "img3.jpeg", "img4.jpeg", "img5.jpeg", "img6.jpeg", "img7.jpg", "img8.jpg", "img9.jpg", "img10.jpeg", "img11.jpeg", "img12.jpeg", "img13.jpg", "img14.jpg", "img15.jpg", "img16.jpg" };
		ObservableCollection<Button> bannerButtons = new ObservableCollection<Button>();
		EventListView eventListView;

		public ExploreEventCategories()
		{
			InitializeComponent();

			List<string> avaliableBanners = new List<string>();
			avaliableBanners.Add(App.userProfile.LargeImageSource);
			avaliableBanners.Add("friends_and_followed.jpg");
			avaliableBanners.AddRange(ab);

			categoryList.Children.Add(new StackLayout() { HeightRequest = 60 });

			for (int i = 0; i < 0; i++)
				//for (int i = 0; i < avaliableBanners.Count; i++)
			{
				Grid newGrid = new Grid()
				{
					RowSpacing = 0,
					ColumnSpacing = 0,
					Padding = 0,
					WidthRequest = App.coreView.Width,
					HeightRequest = App.coreView.Width * 0.45
				};
				newGrid.RowDefinitions.Add(new RowDefinition { Height = App.coreView.Width * 0.45 });
				newGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = App.coreView.Width });

				Button newBannerButton = new Button()
				{
					TextColor = Color.White,
					HeightRequest = App.coreView.Width * 0.45,
					BackgroundColor = Color.Transparent,
				};
				string cat = "";
				if (i != 0 && i != 1)
				{
					cat = ((EventCategory.Category)(i - 2)).ToString();
				}
				else if (i == 0) { cat = "My Search Preferences"; }
				else if (i == 1) { cat = "Friends & Followed"; }

				newBannerButton.Clicked += (sender, e) =>
				{
					try
					{
						eventList.Children.Remove(eventListView);
					}
					catch (Exception ex) {}

					if (cat == "My Search Preferences")
					{
						App.coreView.setContentViewWithQueue(new EventListView(0));
					}
					else if (cat == "Friends & Followed")
					{
						App.coreView.setContentViewWithQueue(new EventListView(1));
					}
					else {
						HorizontalScrollView.IsVisible = false;
						eventListView = new EventListView(6);
						eventList.Children.Add(eventListView);
						searchBar.Text = "#" + cat;
					}
				};

				Image newImage = new Image()
				{
					Source = avaliableBanners[i],
					Aspect = Aspect.AspectFill,
				};

				Label labelBg = new Label()
				{
					//BackgroundColor = Color.FromHex("#90000000"),
					Text = cat,
					TextColor = Color.FromHex("#70000000"),
					FontSize = 24,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalTextAlignment = TextAlignment.Center,
					TranslationX = 2,
					TranslationY = 1,
				};

				Label label = new Label()
				{
					//BackgroundColor = Color.FromHex("#90000000"),
					Text = cat,
					TextColor = Color.White,
					FontSize = 24,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalTextAlignment = TextAlignment.Center,
				};

				newImage.Source = avaliableBanners[i];

				newGrid.Children.Add(newImage, 0, 0);
				newGrid.Children.Add(labelBg, 0, 0);
				newGrid.Children.Add(label, 0, 0);
				newGrid.Children.Add(newBannerButton, 0, 0);



				categoryList.Children.Add(newGrid);
				bannerButtons.Add(newBannerButton);
				//bannerList.Children.Add (new StackLayout(){Padding=5});
			}
			categoryList.Children.Add(new StackLayout() {HeightRequest= 93 });
			eventListView = new EventListView(6);
			eventList.Children.Add(eventListView);

			searchBar.TextChanged += async (sender, e) =>
			{
				if (string.IsNullOrWhiteSpace(searchBar.Text))
				{
					HorizontalScrollView.IsVisible = true;
					await Task.Delay(10);
					DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
					returnButton.IsVisible = false;
				}
				else {
					HorizontalScrollView.IsVisible = false;
					//scrollBackground.IsVisible = false;
					eventListView.UpdateList(true, searchBar.Text);
					returnButton.IsVisible = true;
				}
			};

			searchBarDelete.Clicked += (sender, e) =>
			{
				searchBar.Text = "";
			};
			TapGestureRecognizer returnGesture = new TapGestureRecognizer();
			returnGesture.Tapped += (sender, e) =>
			{
				searchBar.Text = "";
			};
			returnButton.GestureRecognizers.Add(returnGesture);

			categoryList.Focused += async (sender, e) =>
			{
				await Task.Delay(10);
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			};
		}
	}
}
