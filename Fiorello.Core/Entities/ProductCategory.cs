using System;
namespace Fiorello.Core.Entities
{
	public class ProductCategory:BaseModel
	{
		public int ProductId { get; set; }
		public int CategoryId { get; set; }
		public Product Product { get; set; }
		public Category Category { get; set; }
    }
}

