﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HowlOut
{
	public interface SocialController
	{
		Task<bool> addEventToCalendar(Event eve);
		Task<List<string>> getFacebookFriends();
		void setNotificationBadge(int nr);
		//Task<bool> openEmail(string subject, string body);
	}
}
