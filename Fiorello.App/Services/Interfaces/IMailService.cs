using System;
namespace Fiorello.App.Services.Interfaces
{
	public interface IMailService
	{
		public Task Send(string from, string to, string link, string text,string subject);
	}
}

