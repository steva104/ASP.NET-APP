using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinylVibe.Models;

namespace VinylVibe.BusinessLogic.Interfaces
{
	public interface ICompanyService
	{

		IEnumerable<Company> GetAllCompanies();
		Company GetCompanyById(int id);
		void AddCompany(Company company);
		void UpdateCompany(Company company);
		void DeleteCompany(int id);
		

	}
}
