using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class ExploreEventCategories : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		List<string> avaliableBanners = new List<string> { "img1.jpg", "img2.jpeg", "img3.jpeg", "img4.jpeg", "img5.jpeg", "img6.jpeg", "img7.jpg", "img8.jpg", "img9.jpg", "img10.jpeg", "img11.jpeg", "img12.jpeg", "img13.jpg", "img14.jpg", "img15.jpg", "img16.jpg" };
		ObservableCollection<Button> bannerButtons = new ObservableCollection<Button>();

		public ExploreEventCategories()
		{
			InitializeComponent();

			for (int i = 0; i < avaliableBanners.Count; i++)
			{
				Grid newGrid = new Grid()
				{
					RowSpacing = 0,
					ColumnSpacing = 0,
					Padding = 0,
					WidthRequest = App.coreView.Width,
					HeightRequest = App.coreView.Width * 0.56
				};
				newGrid.RowDefinitions.Add(new RowDefinition { Height = App.coreView.Width * 0.563 });
				newGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = App.coreView.Width });

				Button newBannerButton = new Button()
				{
					//Text = ((EventCategory.Category) i).ToString() ,
					TextColor = Color.White,
					HeightRequest = App.coreView.Width * 0.563,
					BackgroundColor = Color.Transparent,
				};

				string cat = ((EventCategory.Category)i).ToString();

				newBannerButton.Clicked += (sender, e) =>
				{
					HorizontalScrollView.IsVisible = false;
					//scrollBackground.IsVisible = false;
					searchBar.Text = "#" + cat;
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
			EventListView eventListView = new EventListView(6);

			eventList.Children.Add(eventListView);

			searchBar.TextChanged += async (sender, e) =>
			{
				if (string.IsNullOrWhiteSpace(searchBar.Text))
				{
					HorizontalScrollView.IsVisible = true;
					await Task.Delay(10);
					DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
				}
				else {
					HorizontalScrollView.IsVisible = false;
					//scrollBackground.IsVisible = false;
					eventListView.UpdateList(true, searchBar.Text);
				}
			};

			categoryList.Focused += async (sender, e) =>
			{
				await Task.Delay(10);
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			};
		}
	}
}
