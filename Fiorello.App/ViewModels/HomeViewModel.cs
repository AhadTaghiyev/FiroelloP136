using System;
using Fiorello.Core.Entities;

namespace Fiorello.App.ViewModels
{
	public class HomeViewModel
	{
		public IEnumerable<Category> Categories { get; set; }
		public IEnumerable<Blog> Blogs { get; set; }
		public IEnumerable<Employee> Employees { get; set; }
		public IEnumerable<Product> Products { get; set; }
    }
}

