using System;
namespace HowlOut
{
	public enum MenuType
	{
		Home,
		ViewProfile,
		ViewFriends,
		Logout
	}
	public class HomeMenuItem
	{
		public string Title { get; set; }
		public string Icon { get; set; }
		public MenuType MenuType { get; set; }

		public HomeMenuItem()
		{
			//MenuType = MenuType.Settings;
		}
	}
}
