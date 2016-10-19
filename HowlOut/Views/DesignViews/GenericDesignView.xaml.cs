using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class GenericDesignView : ContentView
	{
		public DataManager _dataManager;
		public Button subjBtn { get { return SubjectButton; } }
		public Button editBtn { get { return editButton; } }
		public Button addBtn { get { return addButton; } }
		public Button removeBtn { get { return removeButton; } }
		public StackLayout editLayout { get { return editProfile; } }

		public CustomEditor profileDescription { get { return description; } }
		public Button updateProfileBtn { get { return updateProfileButton; } }
		public Button profileLogOutBtn { get { return logOutButton; } }

		public IconView pictureButton  { get { return takePictureButton; } }
		public IconView albumButton { get { return albumPictureButton; } }
		public Button fbImageButton { get { return selectBannerButton; } }
		public Image editImage { get { return SelectedBannerImage; } }

		public GenericDesignView(int dimentions)
		{
			_dataManager = new DataManager();
			InitializeComponent();
			ScaleLayout(dimentions);
		}

		private void ScaleLayout(int dimentions)
		{

			pictureGrid.HeightRequest = dimentions;
			pictureGrid.WidthRequest = dimentions;
			infoLayout.HeightRequest = dimentions * 0.1;
			buttonLayout.HeightRequest = dimentions * 0.16;
			infoLabel.FontSize = dimentions * 0.115;

			MainButton.BorderRadius = (int)(0.440 * dimentions);
			MainButton.BorderWidth = (int)(0.04 * dimentions);
			MainButton.FontSize = (int)(0.2 * dimentions);


			setButtonDimentions(addButton, dimentions);
			setButtonDimentions(editButton, dimentions);
			setButtonDimentions(removeButton, dimentions);
		}

		void setButtonDimentions(Button b, int dimentions)
		{
			b.BorderRadius = (int)(0.08 * dimentions);
			b.BorderWidth = (0.003 * dimentions);
			b.WidthRequest = (dimentions * 0.6);
			b.FontSize = (int)(0.115 * dimentions);
		}

		public void SetImage(string source, string name)
		{
			ProfileImage.Source = source;
			infoLabel.Text = name;
		}

		public async void HandleButtonRequests(Func<Task<bool>> a, Button b, string text, string clickedText)
		{
			b.IsVisible = true;
			b.Text = text;
			b.Clicked += async (sender, e) =>
			{
				await b.ScaleTo(0.7, 50, Easing.Linear);
				await b.ScaleTo(1, 50, Easing.Linear);
				bool success = await a.Invoke();
				if (success)
				{
					b.IsEnabled = false;
					b.Text = clickedText;
				}
			};
		}
	}
}

