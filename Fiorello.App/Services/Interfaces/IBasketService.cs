using System;
using Fiorello.App.ViewModels;

namespace Fiorello.App.Services.Interfaces
{
	public interface IBasketService
	{
		public  Task  AddBasket(int id, int? count);
		public  Task<List<BasketItemViewModel>> GetAllBaskets();
		public  Task Remove(int id);
    }
}

