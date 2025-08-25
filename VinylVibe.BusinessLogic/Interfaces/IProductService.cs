using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinylVibe.Models.ViewModel;
using VinylVibe.Models;

namespace VinylVibe.BusinessLogic.Interfaces
{
	public interface IProductService
	{
		IEnumerable<Product> GetAllProducts();
		ProductViewModel GetProductViewModel(int? id);
		bool UpsertProduct(ProductViewModel productVM, IFormFile? file, string wwwRootPath);
		bool DeleteProduct(int id, string wwwRootPath);


	}
}
