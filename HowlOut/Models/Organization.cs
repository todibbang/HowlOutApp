using System;
using System.Collections.Generic;
namespace HowlOut
{
	public class Organization
	{
		public string OrganizationId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageSource { get; set; }

		public List <Profile> Members { get; set; }
		public List <Comment> Comments { get; set; }

		public Organization()
		{
			
		}
	}
}
