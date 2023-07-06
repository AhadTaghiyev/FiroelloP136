using System;
namespace Fiorello.Core.Entities
{
	public class BaseModel
	{
		public int Id { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
    }
}

