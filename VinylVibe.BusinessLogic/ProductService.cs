using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinylVibe.BusinessLogic.Interfaces;
using VinylVibe.DataAccess.Repository.IRepository;
using VinylVibe.Models.ViewModel;
using VinylVibe.Models;

namespace VinylVibe.BusinessLogic
{
	public class ProductService :IProductService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProductService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IEnumerable<Product> GetAllProducts()
		{
			return _unitOfWork.Product.GetAll(includeProperties: "Genre");
		}

		public ProductViewModel GetProductViewModel(int? id)
		{
			IEnumerable<SelectListItem> genreList = _unitOfWork.Genre.GetAll().Select(x => new SelectListItem
			{
				Text = x.Name,
				Value = x.Id.ToString()
			});

			ProductViewModel productViewModel = new()
			{
				GenreList = genreList,
				Product = new Product()
			};

			if (id != null && id > 0)
			{
				productViewModel.Product = _unitOfWork.Product.Get(x => x.Id == id);
			}

			return productViewModel;
		}

		public bool UpsertProduct(ProductViewModel productVM, IFormFile? file, string wwwRootPath)
		{
			if (productVM == null)
				return false;

			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				string productPath = Path.Combine(wwwRootPath, @"images\product");

				if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
				{
					var oldImage = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImage))
					{
						System.IO.File.Delete(oldImage);
					}
				}

				using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
				{
					file.CopyTo(fileStream);
				}

				productVM.Product.ImageUrl = @"\images\product\" + fileName;
			}
			else if (productVM.Product.Id == 0 && string.IsNullOrEmpty(productVM.Product.ImageUrl))
			{
				return false; 
			}

			if (productVM.Product.Id == 0)
			{
				_unitOfWork.Product.Add(productVM.Product);
			}
			else
			{
				_unitOfWork.Product.Update(productVM.Product);
			}

			_unitOfWork.Save();
			return true;
		}

		public bool DeleteProduct(int id, string wwwRootPath)
		{
			var product = _unitOfWork.Product.Get(x => x.Id == id);
			if (product == null)
				return false;


            var isProductInOrder = _unitOfWork.OrderDetails
                                       .GetAll(x => x.ProductId == id)  
                                       .Any();
            if (isProductInOrder)
                return false; 


            var oldImage = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
			if (System.IO.File.Exists(oldImage))
			{
				System.IO.File.Delete(oldImage);
			}

			_unitOfWork.Product.Delete(product);
			_unitOfWork.Save();
			return true;
		}


	}
}
