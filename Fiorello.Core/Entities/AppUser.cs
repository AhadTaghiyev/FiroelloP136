using System;
using Microsoft.AspNetCore.Identity;

namespace Fiorello.Core.Entities
{
	public class AppUser:IdentityUser
    {
		public string Name { get; set; }
		public string Surname { get; set; }
	}
}

