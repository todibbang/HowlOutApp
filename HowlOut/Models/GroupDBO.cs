using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class GroupDBO
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageSource { get; set; }

		public Visibility Visibility { get; set; }

		public Profile Owner { get; set; }
		public Organization OrganizationOwner { get; set; }

		public GroupDBO ()
		{
		}
	}
}

