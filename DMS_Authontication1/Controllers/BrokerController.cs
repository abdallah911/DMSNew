using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using DMS_Authontication1.Helper;

namespace DMS_Authontication1.Controllers
{
    public class BrokerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public BrokerController()
        {
            _dbContext = new ApplicationDbContext();
        }
        #region Index
        public ActionResult Index(string SearchValue = "")
        {
            if (!string.IsNullOrEmpty(SearchValue))
            {
                var resultSearch = PerformSearch(SearchValue);
                ViewBag.SearchValue = SearchValue;
                return View(resultSearch);
            }
            else
            {
                List<BrokerViewModel> brokers = GetBrokers();
                ViewBag.SearchValue = null;
                return View(brokers);
            }
        }
        #endregion
        // GET: Broker
        //public ActionResult Index()
        //{
        //    List<BrokerViewModel> brokers = GetBrokers();
        //    return View(brokers);
        //}
        // GET: Broker/Details/1
        public ActionResult Details(int id)
        {
            BrokerViewModel broker = GetBrokerById(id);

            if (broker == null)
            {
                return HttpNotFound();
            }

            return View(broker);
        }
        // GET: Broker/Create
        public ActionResult Create()
        {
            // Fetch a list of companies with no relation to any brokers

            return View();
        }
        // POST: Broker/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BrokerViewModel broker)
        {
            if (ModelState.IsValid)
            {
                #region Upload TaxCard
                string FolderPath = Path.Combine(Server.MapPath("~/ProposalAssets/Files/"), "TaxCardFiles");

                broker.TaxCardName = DocumentSetting.UploadFile(broker.TaxCard, FolderPath);
                #endregion
                #region Upload PracticeCard
                string FolderPath2 = Path.Combine(Server.MapPath("~/ProposalAssets/Files/"), "PracticeCardFiles");

                broker.PracticeCardName = DocumentSetting.UploadFile(broker.PracticeCard, FolderPath2);
                #endregion
                // Save the broker to the database, including the selected company
                var brokerViewModel = SaveBroker(broker,true,0);

                return RedirectToAction(nameof(Index));
            }
            List<string> errors = new List<string>();

            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            ViewBag.Errors = errors;
            return View(broker);
        }

        /*                 */
        // GET: Broker/Edit
        public ActionResult Edit(int id)
        {
            // Fetch a list of companies with no relation to any brokers
            var broker = GetBrokerById(id);
            return View(broker);
        }
        // POST: Broker/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BrokerViewModel broker,int id)
        {
            if (broker.PracticeCard == null && ModelState["PracticeCard"] != null )
            {
                // Remove the model error for the File property if it exists
                ModelState["PracticeCard"].Errors.Clear();
            }
            if (broker.TaxCard == null && ModelState["TaxCard"] != null)
            {
                // Remove the model error for the File property if it exists
                ModelState["TaxCard"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                if (broker.TaxCard != null)
                {
                    #region Upload TaxCard
                    string FolderPath = Path.Combine(Server.MapPath("~/ProposalAssets/Files/"), "TaxCardFiles");

                    broker.TaxCardName = DocumentSetting.UploadFile(broker.TaxCard, FolderPath);
                    #endregion
                }

                if (broker.PracticeCard != null)
                {
                    #region Upload PracticeCard
                    string FolderPath2 = Path.Combine(Server.MapPath("~/ProposalAssets/Files/"), "PracticeCardFiles");

                    broker.PracticeCardName = DocumentSetting.UploadFile(broker.PracticeCard, FolderPath2);
                    #endregion
                }

                // Save the broker to the database, including the selected company
                var brokerViewModel = SaveBroker(broker,false,id);

                return RedirectToAction(nameof(Index));
            }
            List<string> errors = new List<string>();

            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            ViewBag.Errors = errors;
            return View(broker);
        }
        //method for getting the brokers data as a list including the companies associated with them
        private List<BrokerViewModel> GetBrokers()
        {
            try
            {

                var brokers = _dbContext.Brokers.ToList();
                // Eagerly load the related company information

                // Convert to BrokerViewModel
                return brokers.Select(b => new BrokerViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    NationalId = b.NationalId,
                    MembershipNo = b.MembershipNo,
                    TaxCardId = b.TaxCardId,
                    CreatedOn = b.CreatedOn,
                    TaxCardName =b.TaxCardName,
                    PracticeCardName = b.PracticeCardName,
                    CompanyName = b.CompanyName
                }).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<BrokerViewModel>();
            }
        }
        //get broker by its id
        private BrokerViewModel GetBrokerById(int id)
        {
            var brokerEntity = _dbContext.Brokers.FirstOrDefault(b => b.Id == id);

            if (brokerEntity == null)
            {
                return null;
            }

            var result = new BrokerViewModel
            {
                Id = brokerEntity.Id,
                Name = brokerEntity.Name,
                NationalId = brokerEntity.NationalId,
                MembershipNo = brokerEntity.MembershipNo,
                TaxCardId = brokerEntity.TaxCardId,
                CreatedOn = brokerEntity.CreatedOn,
                IsCompany = brokerEntity.IsCompany,
                PracticeCardName = brokerEntity.PracticeCardName,
                TaxCardName = brokerEntity.TaxCardName

            };
            //result.PracticeCard = ;
            //result.TaxCard = ;
            if (result.IsCompany == true)
            {
                result.CompanyName = brokerEntity.CompanyName;

            }
            return result;
        }
        //save the broker into our database
        private BrokerViewModel SaveBroker(BrokerViewModel broker, bool newBroker,int id)
        {
            if (newBroker == true)
            {

                    // Save the broker to the database
                    var brokerEntity = new Broker
                {
                    Name = broker.Name,
                    NationalId = broker.NationalId,
                    MembershipNo = broker.MembershipNo,
                    TaxCardId = broker.TaxCardId,
                    CreatedOn = DateTime.Now,
                    TaxCardName = broker.TaxCardName,
                    PracticeCardName = broker.PracticeCardName


                };


                if (broker.IsCompany == true)
                {
                    brokerEntity.IsCompany = true;

                        // If a new company name is provided, create a new company
                        if (!string.IsNullOrWhiteSpace(broker.CompanyName))
                        {

                            brokerEntity.CompanyName = broker.CompanyName;
                        }
                        // Handle the case where neither an existing company nor a new company is selected
                        else
                        {
                            ModelState.AddModelError("CompanyId", "Please enter a new company name.");
                        }
                }
                else
                {
                    brokerEntity.CompanyName = "فردي";

                }

                _dbContext.Brokers.Add(brokerEntity);
                    _dbContext.SaveChanges();

                    var entry =  _dbContext.Entry(brokerEntity);
                    entry.State = EntityState.Modified;
                    _dbContext.SaveChanges();
                var result = new BrokerViewModel
                {
                    Name = brokerEntity.Name,
                    NationalId = brokerEntity.NationalId,
                    MembershipNo = brokerEntity.MembershipNo,
                    TaxCardId = brokerEntity.TaxCardId,
                    CreatedOn = brokerEntity.CreatedOn,
                    IsCompany = brokerEntity.IsCompany,
                    CompanyName = brokerEntity.CompanyName,
                    TaxCardName = brokerEntity.TaxCardName,
                    PracticeCardName = brokerEntity.PracticeCardName

                };
                if (result.IsCompany == true)
                {
                    result.CompanyName = brokerEntity.CompanyName;

                }
                return result;
            }
            else
            {
                // Save the broker to the database
                var brokerEntity = _dbContext.Brokers.Find(id);
                brokerEntity.Name = broker.Name;
                brokerEntity.NationalId = broker.NationalId;
                brokerEntity.MembershipNo = broker.MembershipNo;
                brokerEntity.TaxCardId = broker.TaxCardId;
                brokerEntity.CreatedOn = DateTime.Now;
                if (!string.IsNullOrEmpty(broker.PracticeCardName))
                {
                    brokerEntity.TaxCardName = broker.TaxCardName;
                    brokerEntity.PracticeCardName = broker.PracticeCardName;
                }
                if (broker.IsCompany == true)
                {

                    // If a new company name is provided, create a new company
                    if (!string.IsNullOrWhiteSpace(broker.CompanyName))
                    {

                        brokerEntity.CompanyName = broker.CompanyName;
                    }
                    // Handle the case where neither an existing company nor a new company is selected
                    else
                    {
                        ModelState.AddModelError("CompanyId", "Please enter a new company name.");
                    }
                }

                
                var entry = _dbContext.Entry(brokerEntity);
                entry.State = EntityState.Modified;
                _dbContext.SaveChanges();
                var result = new BrokerViewModel
                {
                    Name = brokerEntity.Name,
                    NationalId = brokerEntity.NationalId,
                    MembershipNo = brokerEntity.MembershipNo,
                    TaxCardId = brokerEntity.TaxCardId,
                    CreatedOn = brokerEntity.CreatedOn,
                    IsCompany = brokerEntity.IsCompany,
                    CompanyName = brokerEntity.CompanyName,
                    TaxCardName = brokerEntity.TaxCardName,
                    PracticeCardName = brokerEntity.PracticeCardName

                };
                if (result.IsCompany == true)
                {
                    result.CompanyName = brokerEntity.CompanyName;

                }
                return result;
            }
            
        }
        // Helper method to perform the search based on the provided searchModel
        private List<BrokerViewModel> PerformSearch(string searchModel)
        {
            // Get all brokers from the database
            var allBrokers = _dbContext.Brokers.ToList();

            // Convert the search value to lowercase for case-insensitive comparison
            var searchValue = searchModel.ToLower();

            // Perform the search based on the search value
            var searchResults = allBrokers
                .Where(b =>
                    b.Name.ToLower().Contains(searchValue) ||
                    b.NationalId.ToLower().Contains(searchValue) ||
                    b.Id.ToString().ToLower().Contains(searchValue)
                )
                .Select(b => new BrokerViewModel
                {
                    // Map the properties you want to display in the search results
                    Id = b.Id,
                    Name = b.Name,
                    NationalId = b.NationalId,
                    MembershipNo = b.MembershipNo,
                    TaxCardId = b.TaxCardId,
                    CreatedOn = b.CreatedOn,
                    TaxCardName = b.TaxCardName,
                    PracticeCardName = b.PracticeCardName,
                    IsCompany = b.IsCompany,
                    CompanyName = b.CompanyName
                })
                .ToList();

            return searchResults;
        }
        public static IFormFile ConvertToIFormFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return null;
            }

            var stream = file.InputStream;
            var fileName = Path.GetFileName(file.FileName);
            var contentDisposition = $"form-data; filename=\"{fileName}\"";

            var formFile = new Microsoft.AspNetCore.Http.Internal.FormFile(stream, 0, file.ContentLength, file.FileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentDisposition = contentDisposition,
                ContentType = file.ContentType
            };

            return formFile;
        }
        private byte[] ConvertFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        private byte[] ConvertToByteArray(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public byte[] ConvertPostedFileToBytes(HttpPostedFileBase postedFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                postedFile.InputStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
      
        private IFormFile ConvertByteArrayToFormFIle(byte[] input, string fileName)
        {

            using (MemoryStream memoryStream = new MemoryStream(input))
            {

                var formFile = new Microsoft.AspNetCore.Http.Internal.FormFile(memoryStream, 0, memoryStream.Length, null, fileName);

                // Append ContentDisposition header
                var contentDispositionHeader = $"form-data; filename=\"{fileName}\"";
                formFile.Headers["Content-Disposition"] = contentDispositionHeader;

                // Now 'formFile' can be used as an IFormFile in your ASP.NET Core application
                return formFile;
            }
        }
    }
}