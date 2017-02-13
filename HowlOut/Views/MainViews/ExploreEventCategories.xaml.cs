using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class ExploreEventCategories : ContentView, ViewModelInterface
	{
		public async Task<UpperBar> getUpperBar()
		{
			return null;
		}
		public void reloadView() { }
		public void viewInFocus(UpperBar bar) { }
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }

		public EventListView searchEventList = new EventListView(0);

		List<string> ab = new List<string> { "img1.jpg", "img2.jpeg", "img3.jpeg", "img4.jpeg", "img5.jpeg", "img6.jpeg", "img7.jpg", "img8.jpg", "img9.jpg", "img10.jpeg", "img11.jpeg", "img12.jpeg", "img13.jpg", "img14.jpg", "img15.jpg", "img16.jpg" };
		ObservableCollection<Button> bannerButtons = new ObservableCollection<Button>();
		//EventListView eventListView;

		public ExploreEventCategories()
		{
			InitializeComponent();
			lateSetup();

			List<string> avaliableBanners = new List<string>();
			avaliableBanners.Add("friends_and_followed.jpg");
			avaliableBanners.Add("banner_hjort.png");
			avaliableBanners.AddRange(ab);

			var categoryList = new StackLayout() { Spacing = 0 };
			//categoryList.Children.Add(new StackLayout() { HeightRequest = 60 });
			//categoryList.HeightRequest += 60;

			var layoutForHeaderCategories = new Grid() { HeightRequest = App.coreView.Width * 0.20, ColumnSpacing = 0 };

			for (int i = 0; i < 2; i++)
			{
				Grid newGrid = new Grid()
				{
					RowSpacing = 0,
					ColumnSpacing = 0,
					Padding = 0,
					//WidthRequest = App.coreView.Width,
					HeightRequest = App.coreView.Width * 0.20
				};


				newGrid.RowDefinitions.Add(new RowDefinition { Height = App.coreView.Width * 0.20 });
				newGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

				Button newBannerButton = new Button()
				{
					TextColor = Color.White,
					HeightRequest = App.coreView.Width * 0.20,
					BackgroundColor = Color.Transparent,
					BorderColor = App.HowlOutBackground,
					BorderWidth = 8,
					BorderRadius = 15,
				};
				string cat = "";
				if (i != 0 && i != 1)
				{
					cat = ((EventCategory.Category)(i - 2)).ToString();
				}
				else if (i == 0) { cat = "Friends & Followed"; }
				else if (i == 1) { cat = "Here & Now"; }

				newBannerButton.Clicked += (sender, e) =>
				{
					if (cat == "Friends & Followed")
					{
						App.coreView.setContentViewWithQueue(new EventListView(1));
					}
					else if (cat == "Here & Now")
					{
						App.coreView.setContentViewWithQueue(new EventMapView());
					}
					else {
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
					Text = cat,
					TextColor = Color.White,
					FontSize = 24,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalTextAlignment = TextAlignment.Center,
				};

				newImage.Source = avaliableBanners[i];

				var ImageContainer = new Grid() { Padding = new Thickness(5, 5, 5, 5) };
				ImageContainer.Children.Add(newImage);
				newGrid.Children.Add(ImageContainer, 0, 0);
				newGrid.Children.Add(labelBg, 0, 0);
				newGrid.Children.Add(label, 0, 0);
				newGrid.Children.Add(newBannerButton, 0, 0);


				if (i != 0 && i != 1)
				{
					layoutForHeaderCategories.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
					layoutForHeaderCategories.Children.Add(newGrid, layoutForHeaderCategories.Children.Count, 0);
				}
				else {
					categoryList.HeightRequest += App.coreView.Width * 0.20;
					categoryList.Children.Add(newGrid);
				}
				bannerButtons.Add(newBannerButton);
				//bannerList.Children.Add (new StackLayout(){Padding=5});
			}
			categoryList.Children.Add(layoutForHeaderCategories);
			//categoryList.HeightRequest += App.coreView.Width * 0.20;

			//eventListView = new EventListView(6);
			eventList.Children.Add(searchEventList);
			//searchEventList.HeaderLayout.Children.Add(categoryList);
			//searchEventList.HeaderLayout.HeightRequest = categoryList.HeightRequest;
			//searchEventList.HeaderLayout.Padding = new Thickness(0,60,0,0);





			searchBar.TextChanged += async (sender, e) =>
			{
				if (string.IsNullOrWhiteSpace(searchBar.Text))
				{
					await Task.Delay(10);
					DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
				}
				searchEventList.UpdateList(true, searchBar.Text);
			};

			searchBarDelete.Clicked += (sender, e) =>
			{
				searchBar.Text = "";
			};
			TapGestureRecognizer settingsGesture = new TapGestureRecognizer();
			settingsGesture.Tapped += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchPreference));
			};
			settingsButton.GestureRecognizers.Add(settingsGesture);

			notiButton.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(App.coreView.notifications);
			};

			categoryList.Focused += async (sender, e) =>
			{
				await Task.Delay(10);
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			};
		}

		public void update()
		{
			searchEventList.UpdateList(true, searchBar.Text);
		}

		async void lateSetup()
		{
			await Task.Delay(100);
			//notiBadg.Children.Add(App.coreView.notiButton);
		}
	}
}
