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

		//public Image organizationImage { get { return organizationOwnerImage; } }
		public Button organizationBtn{ get { return organizationOwnerBtn; } }
		//public Label subName { get { return subNameLabel; } }

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
			buttonLayout.HeightRequest = 12 + dimentions * 0.1;
			editProfileButtons.HeightRequest = 10 + dimentions * 0.1;
			//buttonLayoutEdit.HeightRequest = dimentions * 0.16;

			nameLabel.FontSize = 4 + (dimentions * 0.06);
			//subNameLabel.FontSize = 3 + (dimentions * 0.05);
			descriptionLabel.FontSize = dimentions * 0.055;
			//nameLabelEdit.FontSize = dimentions * 0.115;
			descriptionLabelEdit.FontSize = dimentions * 0.055;

			MainButton.BorderRadius = (int)(0.5 * dimentions);
			MainButton.BorderWidth = (int)(0.06 * dimentions);

			organizationOwnerBtn.BorderRadius = (int)(0.18 * dimentions);
			organizationOwnerBtn.BorderWidth = (int)(0.02 * dimentions);

			setButtonDimentions(addButton, dimentions);
			setButtonDimentions(editButton, dimentions);
			setButtonDimentions(removeButton, dimentions);
			setButtonDimentions(updateProfileButton, dimentions);
			setButtonDimentions(logOutButton, dimentions);

			/*
			if (dimentions < 100)
			{
				buttonLayout.Children.Add(setPillButtonLayout(new List<Button>() {addBtn, editButton, removeBtn }));
				buttonLayout.IsVisible = true;
			}
			*/
		}

		void setButtonDimentions(Button b, int dimentions)
		{
			b.BorderRadius = (int) (buttonLayout.HeightRequest / 5.0);
			b.WidthRequest = (dimentions * 0.5);
			b.FontSize = 3 + (int)(0.05 * dimentions);
			b.Clicked += async (sender, e) =>
			{
				//await b.ScaleTo(0.7, 50, Easing.Linear);
				//await b.ScaleTo(1, 50, Easing.Linear);
			};
		}

		public void setPillButtonLayout(List<Button> buttons)
		{
			Grid buttonGrid = new Grid();
			int bNumber = 0;

			buttonGrid.HorizontalOptions = LayoutOptions.FillAndExpand;

			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 3 });
			for (int i = 0; i < (buttons.Count * 2 - 1); i++)
			{
				if (i % 2 == 0)
				{
					buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				}
				else {
					buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 1 });
				}

				if (i == (buttons.Count * 2 - 1) - 1)
				{
					buttonGrid.Children.Add(new Button() { BorderColor = App.HowlOut, BorderWidth = 0.5, BorderRadius = buttons[0].BorderRadius, BackgroundColor = App.HowlOut }, 0, i + 3, 0, 1);
				}
			}
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 3 });

			for (int i = 0; i < (buttons.Count * 2 - 1); i++)
			{
				if (i % 2 == 0)
				{
					buttons[bNumber].WidthRequest = buttons[bNumber].WidthRequest * 0.7;
					buttonGrid.Children.Add(buttons[bNumber], i+1, 0);
					bNumber++;
				}
				else {
					buttonGrid.Children.Add(new StackLayout() { WidthRequest = 1, BackgroundColor = App.HowlOutBackground }, i+1, 0);
				}
			}
			buttonLayout.Children.Add(buttonGrid);
		}

		public void SetInfo(string source, string name, string description, Design design, ModelType modelType)
		{
			//Image img = new Image() { Source = source };


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
				descriptionLayout.IsVisible = true;
				if (!string.IsNullOrWhiteSpace(description))
				{
					descriptionLineOne.IsVisible = true;
					descriptionLineTwo.IsVisible = true;
				}
			}

			if (name == "") { nameLayout.IsVisible = false;}

			if (modelType == ModelType.Group)
			{
				/*
				modelTypeIcon.IsVisible = true;
				modelTypeIcon.Source = "ic_group.png";
				modelTypeIcon2.IsVisible = true;
				modelTypeIcon2.Source = "ic_group.png";
				*/
				//MainButton.BorderColor = Color.FromHex("#ff66aacc");
			}
			else {
				MainButton.IsVisible = false;
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

		public void HandleButtonRequests(Func<Task<bool>> a, Button b, string text)
		{
			b.IsVisible = true;
			b.Text = text;
			b.Clicked += async (sender, e) =>
			{
				await b.ScaleTo(0.7, 50, Easing.Linear);
				await b.ScaleTo(1, 50, Easing.Linear);
				await a.Invoke();
				await Task.Delay(10);
				App.coreView.reloadCurrentView();
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

