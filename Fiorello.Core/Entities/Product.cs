using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace Fiorello.Core.Entities
{
	public class Product:BaseModel
	{
        [Required]
        public string Name { get; set; }
        [Required]

        public decimal Price { get; set; }
        [Required]

        public string Description { get; set; }
        [Required]

        public string Info { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]

        public string Dimensions { get; set; }
        [Required]

        public double Weight { get; set; }
        public int? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        [NotMapped]
        public List<int> CategoryIds { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }
        [NotMapped]
        public List<IFormFile>? FormFiles { get; set; }


    }
}

