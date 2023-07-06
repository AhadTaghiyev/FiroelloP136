using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
namespace Fiorello.Core.Entities

{
	public class Blog:BaseModel
	{
		public string? Image { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		[NotMapped]
		public IFormFile? FormFile { get; set; }
	}
}

