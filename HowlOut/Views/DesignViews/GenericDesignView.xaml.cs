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
		//public StackLayout editLayout { get { return editProfile; } }

		public Button updateProfileBtn { get { return updateProfileButton; } }
		public Button profileLogOutBtn { get { return logOutButton; } }

		public IconView pictureButton  { get { return takePictureButton; } }
		public IconView albumButton { get { return albumPictureButton; } }
		public Button fbImageButton { get { return selectBannerButton; } }
		public Image profileImage { get { return ProfileImage; } }
		public CustomEditor descriptionEdit { get { return descriptionLabelEdit; } }

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
			nameLayout.HeightRequest = dimentions * 0.2;
			//descriptionLayout.HeightRequest = dimentions * 0.2;
			buttonLayout.HeightRequest = dimentions * 0.16;
			editProfileButtons.HeightRequest = dimentions * 0.16;
			//buttonLayoutEdit.HeightRequest = dimentions * 0.16;

			nameLabel.FontSize = dimentions * 0.115;
			descriptionLabel.FontSize = dimentions * 0.055;
			//nameLabelEdit.FontSize = dimentions * 0.115;
			descriptionLabelEdit.FontSize = dimentions * 0.055;

			MainButton.BorderRadius = (int)(0.440 * dimentions);
			MainButton.BorderWidth = (int)(0.04 * dimentions);
			MainButton.FontSize = (int)(0.2 * dimentions);


			setButtonDimentions(addButton, dimentions);
			setButtonDimentions(editButton, dimentions);
			setButtonDimentions(removeButton, dimentions);
			setButtonDimentions(updateProfileButton, dimentions);
			setButtonDimentions(logOutButton, dimentions);
		}

		void setButtonDimentions(Button b, int dimentions)
		{
			b.BorderRadius = (int)(0.08 * dimentions);
			b.BorderWidth = (0.003 * dimentions);
			b.WidthRequest = (dimentions * 0.6);
			b.FontSize = 3 + (int)(0.05 * dimentions);
			b.Clicked += async (sender, e) =>
			{
				//await b.ScaleTo(0.7, 50, Easing.Linear);
				//await b.ScaleTo(1, 50, Easing.Linear);
			};
		}

		public void SetInfo(string source, string name, string description, Design design)
		{
			ProfileImage.Source = source;
			nameLabel.Text = name;
			//nameLabelEdit.Text = name;
			descriptionLabel.Text = description;
			descriptionLabelEdit.Text = description;
			if (design == Design.Name)
			{
				nameLayout.IsVisible = true;
			}
			else if (design == Design.NameAndButtons)
			{
				nameLayout.IsVisible = true;
				buttonLayout.IsVisible = true;
			}
			else if (design == Design.ShowAll)
			{
				nameLayout.IsVisible = true;
				buttonLayout.IsVisible = true;
				descriptionLabel.IsVisible = true;
			}
		}

		public void ShowHideEditLayout(bool show)
		{
			descriptionLabel.IsVisible = !show;
			descriptionLabelEdit.IsVisible = show;
			nameLayout.IsVisible = !show;
			editProfileButtons.IsVisible = show;
			selectFromBannerList.IsVisible = show;
			takePictureButton.IsVisible = show;
			albumPictureButton.IsVisible = show;

			if (show)
			{
				editButton.Text = "Cancel";
			}
			else {
				editButton.Text = "Edit";
			}
		}

		public void HandleButtonRequests(Func<Task<bool>> a, Button b, string text, string clickedText)
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

		public enum Design {
			OnlyImage,
			Name,
			NameAndButtons,
			ShowAll
		}
	}
}

