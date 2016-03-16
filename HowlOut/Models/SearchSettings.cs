﻿using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class SearchSettings
	{
		public List<EventType> EventTypes { get; set; }
		//public List<int> EventTypesAsNumbers { get; set; }

		public double Distance { get; set; }


		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		/*
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
*/
		public SearchSettings ()
		{
			
		}
	}
}

