using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Fiorello.Core.Entities
{
	public class Employee : BaseModel
	{
		[Required(ErrorMessage = "The field name can not be empty")]
		[StringLength(30)]
		[DisplayName("Employee FullName")]
		public string FullName { get; set; }
		public string? Image { get; set; }
		public string Description { get; set; }
		public int PositionId { get; set; }
		public Position? Position { get; set; }
		[NotMapped]
		public IFormFile? FormFile{get;set;}
	}
}

