using System;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Core.Entities
{
	public class Tag:BaseModel
	{
		[Required]
		public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }

    }
}

