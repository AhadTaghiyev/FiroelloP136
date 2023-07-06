using System;
using Fiorello.App.ViewModels;

namespace Fiorello.App.Services.Interfaces
{
	public interface IBasketService
	{
		public  Task  AddBasket(int id);
		public  Task<List<BasketItemViewModel>> GetAllBaskets();
		public  Task Remove(int id);
    }
}

