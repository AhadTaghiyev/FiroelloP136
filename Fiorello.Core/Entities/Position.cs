using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Core.Entities
{
	public class Position:BaseModel
	{
		[Required(ErrorMessage ="The field name can not be empty")]
		[StringLength(30)]
		[DisplayName("Position Name")]
	    public string Name { get; set; }
		public List<Employee>? Employees { get; set; }

	
	}
}

