using System;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Core.Entities
{
	public class Category:BaseModel
	{
		[Required]
		public string Name { get; set; }
		public	List<ProductCategory> ProductCategories { get; set; }
	}
}

