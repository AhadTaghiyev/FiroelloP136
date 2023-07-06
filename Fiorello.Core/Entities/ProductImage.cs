using System;
namespace Fiorello.Core.Entities
{
	public class ProductImage:BaseModel
	{
		public bool IsMain { get; set; }
		public string Image { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }
	}
}

