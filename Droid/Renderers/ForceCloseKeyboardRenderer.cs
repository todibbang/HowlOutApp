using System;
using Xamarin.Forms;
using HowlOut.Android;
using HowlOut;

[assembly: Dependency(typeof(ForceCloseKeyboardRenderer))]

namespace HowlOut.Android
{
	public class ForceCloseKeyboardRenderer : ForceCloseKeyboard
	{
		public void CloseKeyboard()
		{
			//bool dismissalResult = UIApplication.SharedApplication.KeyWindow.EndEditing(true);
		}
	}
}