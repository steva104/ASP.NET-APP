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
    public class CompanyController : Controller
    {

		private readonly ICompanyService _companyService;

		public CompanyController(ICompanyService companyService)
		{
			_companyService = companyService;
		}

		public IActionResult Index()
		{
			var objCompanyList = _companyService.GetAllCompanies();
			return View(objCompanyList);
		}

		public IActionResult Upsert(int? id)
		{
			if (id == null || id == 0)
			{
				return View(new Company());
			}
			else
			{
				var company = _companyService.GetCompanyById(id.Value);
				return View(company);
			}
		}

		[HttpPost]
		public IActionResult Upsert(Company company)
		{
			if (ModelState.IsValid)
			{
				if (company.Id == 0)
				{
					_companyService.AddCompany(company);
				}
				else
				{
					_companyService.UpdateCompany(company);
				}

				TempData["success"] = "Company saved successfully!";
				return RedirectToAction("Index");
			}

			return View(company);
		}

		#region API Calls
		[HttpGet]
		public IActionResult GetAll()
		{
			var objCompanyList = _companyService.GetAllCompanies();
			return Json(new { data = objCompanyList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			if (id == null)
			{
				return Json(new { success = false, message = "Invalid request" });
			}

			_companyService.DeleteCompany(id.Value);
			return Json(new { success = true, message = "Successfully deleted" });
		}
		#endregion
	}
}
