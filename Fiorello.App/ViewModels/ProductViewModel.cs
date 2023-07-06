using System;
using Fiorello.Core.Entities;

namespace Fiorello.App.ViewModels
{
	public class ProductViewModel
	{
		
		public IEnumerable<Product> Products { get; set; }
		public Product Product { get; set; }
    }
}

