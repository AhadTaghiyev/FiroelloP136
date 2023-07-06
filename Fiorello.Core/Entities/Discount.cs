using System;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Core.Entities
{
	public class Discount:BaseModel
	{
		[Required]
		public double Percent { get; set; }
		public List<Product>? Products { get; set; }

	}
}

