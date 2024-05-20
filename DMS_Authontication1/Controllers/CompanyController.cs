using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CompanyController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: Company
        public ActionResult Index()
        {
            //get all companies
            List<CompanyViewModel> companies = GetCompanies();
            return View(companies);
        }
        // GET: Company/Details/1
        public ActionResult Details(int id)
        {
            // Fetch a company by ID from your data source
            CompanyViewModel company = GetCompanyById(id);

            if (company == null)
            {
                return HttpNotFound();
            }

            return View(company);
        }
        // GET: Company/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompanyViewModel company)
        {
            if (ModelState.IsValid)
            {
                // Save the company to your data source
                SaveCompany(company);

                return RedirectToAction("Index");
            }

            // If the model is not valid, return to the create view with the current model
            return View(company);
        }
        //method for getting all companies
        private List<CompanyViewModel> GetCompanies()
        {
            return _dbContext.Comapnies.Select(c => new CompanyViewModel
            {
                Id = c.Id,
                Name = c.Name,
                CreatedOn = c.CreatedOn
            }).ToList();
        }
        //method for getting company by id
        private CompanyViewModel GetCompanyById(int id)
        {
            // Fetch and return a company by ID from the database
            var companyEntity = _dbContext.Comapnies.Find(id);

            if (companyEntity == null)
            {
                return null;
            }

            return new CompanyViewModel
            {
                Id = companyEntity.Id,
                Name = companyEntity.Name,
                CreatedOn = companyEntity.CreatedOn
            };
        }
        //method for saving my entity in database
        private void SaveCompany(CompanyViewModel company)
        {
            // Save the company to the database
            var companyEntity = new Company
            {
                Name = company.Name,
                CreatedOn = DateTime.Now
            };

            _dbContext.Comapnies.Add(companyEntity);
            _dbContext.SaveChanges();
        }
    }
}