using System;
using Xamarin.Forms;
namespace HowlOut
{
	public class HowlOutNavigationPage : NavigationPage
	{
		public HowlOutNavigationPage(Page root) : base(root)
		{
			Init();
		}

		public HowlOutNavigationPage()
		{
			Init();
		}

		void Init()
		{
			BarBackgroundColor = Color.FromHex("#0000000");
			BarTextColor = Color.White;
		}
	}
}
