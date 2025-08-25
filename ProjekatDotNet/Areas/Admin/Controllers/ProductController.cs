using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VinylVibe.BusinessLogic.Interfaces;
using VinylVibe.DataAccess.Data;
using VinylVibe.DataAccess.Repository.IRepository;
using VinylVibe.Models;
using VinylVibe.Models.ViewModel;


namespace VinylVibeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = VinylVibe.Utility.Details.Role_Admin)]
    public class ProductController : Controller
    {

		private readonly IProductService _productService;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment)
		{
			_productService = productService;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			var products = _productService.GetAllProducts();
			return View(products);
		}

		public IActionResult Upsert(int? id)
		{
			var productViewModel = _productService.GetProductViewModel(id);
			return View(productViewModel);
		}

		[HttpPost]
		public IActionResult Upsert(ProductViewModel productVM, IFormFile? file)
		{
			bool success = _productService.UpsertProduct(productVM, file, _webHostEnvironment.WebRootPath);

			if (!success)
			{
				ModelState.AddModelError("", "Image is required for new products.");
				return View(productVM);
			}

			TempData["success"] = "Product saved successfully!";
			return RedirectToAction("Index");
		}

		#region API Calls

		[HttpGet]
		public IActionResult GetAll()
		{
			var products = _productService.GetAllProducts();
			return Json(new { data = products });
		}

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			bool success = _productService.DeleteProduct(id, _webHostEnvironment.WebRootPath);

			if (!success)
				return Json(new { success = false, message = "There was an error while deleting the product." });

			return Json(new { success = true, message = "Successfully deleted the product." });
		}

		#endregion
	}
}
