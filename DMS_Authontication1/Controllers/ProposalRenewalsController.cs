using DMS_Authontication1.Helper;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace DMS_Authontication1.Controllers
{
    [Authorize]
    public class ProposalRenewalsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private DMS_TESTEntities db = new DMS_TESTEntities();



        public ProposalRenewalsController()
        {
            _dbContext = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_dbContext));

        
    }
    #region MainPage
    // GET: ProposalMain
    public ActionResult Index()
        {
            var companyname = db.Contract_Comp.Select(c => new
            {
                COMP_ID = c.C_COMP_ID,
                Name = c.C_ANAME + " || " + c.C_COMP_ID

            }).ToList();
            SelectList companylist = new SelectList(companyname, "COMP_ID", "Name");
            ViewBag.company = companylist;

            return View();
        }

        public JsonResult getInformation(Int32 CompId)
        {
            int maxContract = db.Contract_Data
                    .Where(c => c.C_COMP_ID == CompId) 
                    .Max(c => c.CONTRACT_NO);

            int countClass = db.CompContractClasses
                    .Where(c => c.C_COMP_ID == CompId)
                     .Select(c => c.CLASS_CODE).Distinct().Count();

            int countEmp = db.Comp_Employees
                    .Where(c => c.C_COMP_ID == CompId && c.CONTRACT_NO == maxContract && (c.TERMINATE_FLAG ?? "N") != "Y")
                     .Select(c => c.CARD_ID).Distinct().Count();

            return new JsonResult { Data = new { maxContract, countClass, countEmp }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion




        #region ProposalStepTwo
        public ActionResult ProposalStepTwoCreate(int countCat)
        {
            // Retrieve data from temporary storage or session
            // ...
            // Retrieve the list of Areas and CompanyActivities from the database
            var colors = _dbContext.CardColors.ToList();
            var residenceDegree = _dbContext.ResidenceDegrees.ToList();
            var medicalNetworks = _dbContext.MedicalNetworks.ToList();


            // Convert the lists to SelectList items for use in dropdown lists
            ViewBag.ColorList = new SelectList(colors, "Id", "Name");
            ViewBag.ResidenceList = new SelectList(residenceDegree, "Id", "Name");
            ViewBag.MedicalNetworkList = new SelectList(medicalNetworks, "Id", "Name");


            ////////////////Test

            int id = 10;
            ViewBag.MainId = id;
            var mainProposal = _dbContext.ProposalMains.Include("ProposalStepTwos").FirstOrDefault(p => p.Id == id);
            //var categoriesCount = mainProposal.CategoriesCount;
            var categoriesCount = 4;
            ViewBag.CatCount = countCat;
            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
            var model = new List<ProposalStepTwoViewModel>(categoriesCount);
            if (mainProposal.ProposalStepTwos.Count() > 0)
            {

                foreach (var stepTwo in mainProposal.ProposalStepTwos)
                {
                    var item = MapEntityToViewModelStepTwo(stepTwo);
                    item.Id = stepTwo.Id;
                    model.Add(item);

                }
            }
            else
            {

                for (int i = 0; i < categoriesCount; i++)
                {
                    // Create an instance of ProposalStepTwoViewModel and add it to the list
                    model.Add(new ProposalStepTwoViewModel());
                }
            }
            // Render the view for the second step
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProposalStepTwoCreate(List<ProposalStepTwoViewModel> Model, int? MainProposalId
            ,HttpPostedFileBase EmployeesDataFileBase, string BackOrForward = "")
        {
            try
            {
                if (Model.Any())
                {
                    if(Model.First().Id != 0)
                    {
                        // Check if ConsumptionFile is not passed in the Model, but ModelState has an error for it
                        if (Model.First().EmployeesDataFile == null && ModelState.ContainsKey("EmployeesDataFile"))
                        {
                            // Remove the model error for the File property if it exists
                            ModelState["EmployeesDataFile"].Errors.Clear();
                        }
                    }
                    
                }
                var fileName = "";
                if (EmployeesDataFileBase != null)
                {

                    string FolderPath = Path.Combine(Server.MapPath("~/ProposalAssets/Files/"), "EmpolyeeFiles");

                    fileName = DocumentSetting.UploadFile(EmployeesDataFileBase, FolderPath);
                }
                var index = 0;
                foreach(var model in Model)
                {
                    if (MainProposalId != null)
                    {
                        model.ProposalMainId = (int)MainProposalId;

                    }
                    if (EmployeesDataFileBase != null)
                    {
                        model.EmployeesDataFileName = fileName;
                        model.EmployeesDataFile = EmployeesDataFileBase;

                    }
                    if (ModelState["Id"] != null)
                        ModelState["Id"].Errors.Clear();
                    if (ModelState.IsValid)
                    {
                        var existingProposal = _dbContext.ProposalStepTwos.Find(model.Id);

                        if (existingProposal != null)
                        {
                            // Update the properties of the existing entity
                            MapViewModelToEntityModifyStepTwo(model, existingProposal);

                            // Mark the entity as modified
                            _dbContext.Entry(existingProposal).State = System.Data.Entity.EntityState.Modified;
                            // Save changes
                            await _dbContext.SaveChangesAsync();


                        }
                        else
                        {
                            //var CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                            // Map view model to entity and save to database
                            var proposalSecond = MapViewModelToEntityStepTwo(model);
                            proposalSecond.ClassCode = index + 1;
                            index = index + 1;
                            // Save the entity to the database
                            _dbContext.ProposalStepTwos.Add(proposalSecond);
                            await _dbContext.SaveChangesAsync();
                        }
                       
                    }
                    
                       
                    
                    else
                    {

                        List<string> errors = new List<string>();

                        foreach (var value in ModelState.Values)
                        {
                            foreach (var error in value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        ViewBag.Errors = errors;
                        // Retrieve the list of Areas and CompanyActivities from the database
                        var colors = _dbContext.CardColors.ToList();
                        var residenceDegree = _dbContext.ResidenceDegrees.ToList();
                        var medicalNetworks = _dbContext.MedicalNetworks.ToList();


                        // Convert the lists to SelectList items for use in dropdown lists
                        ViewBag.ColorList = new SelectList(colors, "Id", "Name");
                        ViewBag.ResidenceList = new SelectList(residenceDegree, "Id", "Name");
                        ViewBag.MedicalNetworkList = new SelectList(medicalNetworks, "Id", "Name");
                        ViewBag.MainId = MainProposalId;

                        ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                        ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
                        // If the model state is not valid, redisplay the form with errors
                        // Populate dropdowns or other data needed for the view
                        return View(Model);
                    }
                }
                if (!string.IsNullOrEmpty(BackOrForward))
                {
                    if (BackOrForward == "back")
                        return RedirectToAction(nameof(Index), new { id = MainProposalId });
                    else
                        return RedirectToAction(nameof(ProposalStepThreeCreate), new { id = MainProposalId });

                }
                else
                {
                    // Redirect to a third page of the form
                    return RedirectToAction(nameof(ProposalStepThreeCreate), new { id = MainProposalId });
                }
          
            }
            catch(Exception e)
            {
                var firstEntity = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId);
                if(firstEntity != null)
                    _dbContext.ProposalMains.Remove(firstEntity);
                return RedirectToAction(nameof(Index));
            }

        }
        // GET: ProposalMain/ProposalStepTwoEdit/1
        public ActionResult ProposalStepTwoEdit()
        {
            int newId = Convert.ToInt32(Request.QueryString["newId"]);
            int prevId = Convert.ToInt32(Request.QueryString["prevId"]);
            // Retrieve data from temporary storage or session
            // ...
            // Retrieve the list of Areas and CompanyActivities from the database
            var colors = _dbContext.CardColors.ToList();
            var residenceDegree = _dbContext.ResidenceDegrees.ToList();
            var medicalNetworks = _dbContext.MedicalNetworks.ToList();


            ViewBag.MainId = newId;
            ViewBag.PrevId = prevId;
            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
            var proposalMain = _dbContext.ProposalMains.Include("ProposalStepTwos").FirstOrDefault(p => p.Id == prevId);
            ViewBag.CatCount = proposalMain.CategoriesCount;
            var models = proposalMain.ProposalStepTwos.ToList();
            // Convert the lists to SelectList items for use in dropdown lists
            ViewBag.ColorList = new SelectList(colors, "Id", "Name");
            ViewBag.ResidenceList = new SelectList(residenceDegree, "Id", "Name");
            ViewBag.MedicalNetworkList = new SelectList(medicalNetworks, "Id", "Name");
            if(models == null || models.Count == 0)
            {
                var emptyModels = new List<ProposalStepTwoViewModel>(proposalMain.CategoriesCount);
                for (int i = 0; i < proposalMain.CategoriesCount; i++)
                {
                    // Create an instance of ProposalStepTwoViewModel and add it to the list
                    emptyModels.Add(new ProposalStepTwoViewModel());
                }
                return View(emptyModels);
            }
            //call the function to map model to viewModel
            var mappedModels = new List<ProposalStepTwoViewModel>();
            foreach(var entity in models)
            {
                mappedModels.Add(MapEntityToViewModelStepTwo(entity));
            }
            return View(mappedModels);
        }
        // POST: ProposalMain/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProposalStepTwoEdit(List<ProposalStepTwoViewModel> Model, int id, int PrevId,
            HttpPostedFileBase EmployeesDataFileBase)
        {
            try
            {

                var fileName = "";
                if (EmployeesDataFileBase != null)
                {

                    string FolderPath = Path.Combine(Server.MapPath("~/ProposalAssets/Files/"), "EmpolyeeFiles");

                    fileName = DocumentSetting.UploadFile(EmployeesDataFileBase, FolderPath);
                }
                else
                {
                    fileName = Model[0].EmployeesDataFileName;
                }
                var index = 0;
                foreach (var model in Model)
                {

                    model.ProposalMainId = (int)id;
                    model.EmployeesDataFileName = fileName;
                    model.EmployeesDataFile = EmployeesDataFileBase;
                    if (ModelState.IsValid)
                    {
                        //var CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                        // Map view model to entity and save to database
                        var proposalSecond = MapViewModelToEntityStepTwo(model);
                        proposalSecond.ClassCode = index + 1;
                        index = index + 1;

                        // Save the entity to the database
                        _dbContext.ProposalStepTwos.Add(proposalSecond);
                        await _dbContext.SaveChangesAsync();

                    }
                    else
                    {

                        List<string> errors = new List<string>();

                        foreach (var value in ModelState.Values)
                        {
                            foreach (var error in value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        ViewBag.Errors = errors;
                        // Retrieve the list of Areas and CompanyActivities from the database
                        var colors = _dbContext.CardColors.ToList();
                        var residenceDegree = _dbContext.ResidenceDegrees.ToList();
                        var medicalNetworks = _dbContext.MedicalNetworks.ToList();


                        // Convert the lists to SelectList items for use in dropdown lists
                        ViewBag.ColorList = new SelectList(colors, "Id", "Name");
                        ViewBag.ResidenceList = new SelectList(residenceDegree, "Id", "Name");
                        ViewBag.MedicalNetworkList = new SelectList(medicalNetworks, "Id", "Name");
                        ViewBag.MainId = id;

                        ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id).CategoriesCount;
                        ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
                        // If the model state is not valid, redisplay the form with errors
                        // Populate dropdowns or other data needed for the view
                        return View(Model);
                    }
                }
                // Redirect to a third page of the form
                return RedirectToAction(nameof(ProposalStepThreeEdit), new { newId = id , prevId = PrevId });
            }
            catch (Exception e)
            {
                var firstEntity = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id);
                if (firstEntity != null)
                    _dbContext.ProposalMains.Remove(firstEntity);
                return RedirectToAction(nameof(Index));
            }
        }
        private ProposalStepTwo MapViewModelToEntityStepTwo(ProposalStepTwoViewModel vM)
        {
            // Use a mapping library or manually map properties as needed
            // This is a simplified example, adjust as needed
            var proposaltwo = new ProposalStepTwo
            {
                AnnualCoverageCeiling = vM.AnnualCoverageCeiling,
                ParticipantsCount = vM.ParticipantsCount,
                Price = vM.Price,
                MinAge= vM.MinAge,
                MaxAge = vM.MaxAge,
                AvgAge = vM.AvgAge,
                ChecksInsideHospital = vM.ChecksInsideHospital,
                PhysicalTherapyInsideHospital = vM.PhysicalTherapyInsideHospital,
                OutsideClincInsideHospital = vM.OutsideClinicInsideHospital,
                DentalServicesInsideHospital = vM.DentalServicesInsideHospital,
                EmployeesDataFileName = vM.EmployeesDataFileName,
                Accidents = vM.Accidents,
                Death = vM.Death,
                //relations
                CardColorId = vM.CardColorId,
                ResidenceDegreeId = vM.ResidenceDegreeId,
                MedicalNetworkId = vM.MedicalNetworkId,
                ProposalMainId = vM.ProposalMainId
                

                // Map other properties as needed
            };

            return proposaltwo;
        }
        private ProposalStepTwoViewModel MapEntityToViewModelStepTwo(ProposalStepTwo proposalEntity)
        {
            var proposalMain = new ProposalStepTwoViewModel
            {
                AnnualCoverageCeiling = proposalEntity.AnnualCoverageCeiling,
                ParticipantsCount = proposalEntity.ParticipantsCount,
                Price = proposalEntity.Price,
                MinAge = proposalEntity.MinAge,
                MaxAge = proposalEntity.MaxAge,
                AvgAge = proposalEntity.AvgAge,
                ChecksInsideHospital = proposalEntity.ChecksInsideHospital,
                PhysicalTherapyInsideHospital = proposalEntity.PhysicalTherapyInsideHospital,
                OutsideClinicInsideHospital = proposalEntity.OutsideClincInsideHospital,
                DentalServicesInsideHospital = proposalEntity.DentalServicesInsideHospital,
                EmployeesDataFileName = proposalEntity.EmployeesDataFileName,
                Death = proposalEntity.Death,
                Accidents = proposalEntity.Accidents,
                //relations
                CardColorId = proposalEntity.CardColorId,
                ResidenceDegreeId = proposalEntity.ResidenceDegreeId,
                MedicalNetworkId = proposalEntity.MedicalNetworkId,
                ProposalMainId = proposalEntity.ProposalMainId


                // Map other properties as needed
            };
            return proposalMain;
        }
        // Helper method to map view model to entity
        private void MapViewModelToEntityModifyStepTwo(ProposalStepTwoViewModel proposalViewModel, ProposalStepTwo existingProposal)
        {
            existingProposal.AnnualCoverageCeiling = proposalViewModel.AnnualCoverageCeiling;
            existingProposal.ParticipantsCount = proposalViewModel.ParticipantsCount;
            existingProposal.Price = proposalViewModel.Price;
            existingProposal.MinAge = proposalViewModel.MinAge;
            existingProposal.MaxAge = proposalViewModel.MaxAge;
            existingProposal.AvgAge = proposalViewModel.AvgAge;
            existingProposal.ChecksInsideHospital = proposalViewModel.ChecksInsideHospital;
            existingProposal.PhysicalTherapyInsideHospital = proposalViewModel.PhysicalTherapyInsideHospital;
            existingProposal.OutsideClincInsideHospital = proposalViewModel.OutsideClinicInsideHospital;
            existingProposal.DentalServicesInsideHospital = proposalViewModel.DentalServicesInsideHospital;
            existingProposal.EmployeesDataFileName = proposalViewModel.EmployeesDataFileName;
            existingProposal.Accidents = proposalViewModel.Accidents;
            existingProposal.Death = proposalViewModel.Death;
            existingProposal.CardColorId = proposalViewModel.CardColorId;
            existingProposal.ResidenceDegreeId = proposalViewModel.ResidenceDegreeId;
            existingProposal.MedicalNetworkId = proposalViewModel.MedicalNetworkId;
            existingProposal.ProposalMainId = proposalViewModel.ProposalMainId;
          
        }
        #endregion







        #region ProposalStepThree
        public ActionResult ProposalStepThreeCreate(int id)
        {
            // Retrieve data from temporary storage or session
            // ...
            // Retrieve the list of Areas and CompanyActivities from the database
          


            // Convert the lists to SelectList items for use in dropdown lists
          
            ViewBag.MainId = id;
            var mainProposal = _dbContext.ProposalMains.Include("ProposalInsiceMedicalAuthorities").FirstOrDefault(p => p.Id == id);

            var categoriesCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id).CategoriesCount;
            ViewBag.CatCount = categoriesCount;
            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
            var model = new List<ProposalInsideMedicalAuthorityViewModel>(categoriesCount);
            if (mainProposal.ProposalInsiceMedicalAuthorities.Count() > 0)
            {

                foreach (var stepThree in mainProposal.ProposalInsiceMedicalAuthorities)
                {
                    var item = MapEntityToViewModelStepThree(stepThree);
                    item.Id = stepThree.Id;
                    model.Add(item);

                }
            }
            else
            {

                for (int i = 0; i < categoriesCount; i++)
                {
                    // Create an instance of ProposalStepTwoViewModel and add it to the list
                    model.Add(new ProposalInsideMedicalAuthorityViewModel());
                }
            }
           
            // Render the view for the second step
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProposalStepThreeCreate(List<ProposalInsideMedicalAuthorityViewModel> Model, int? MainProposalId
            ,string BackOrForwardOrMain="")
        {
            try
            {
                var secondproposal = _dbContext.ProposalStepTwos.Where(sp => sp.ProposalMainId == MainProposalId).ToList();
                var secodProposalIndex = 0;
                foreach (var model1 in Model)
                {
                    int index = Model.IndexOf(model1);
                    #region binding checkBoxes
                    #region bind on to true
                        //model1.HospitalsResidenceServiceLimitOption = Request.Form["Model["+index+"].HospitalsResidenceServiceLimitOption"] == "on";
                        //model1.HospitalsResidenceServicePercentageoption = Request.Form["Model["+index+"].HospitalsResidenceServicePercentageoption"] == "on";

                        //model1.OutsideClinicsLimitOption = Request.Form["Model["+index+"].OutsideClinicsLimitOption"] == "on";
                        //model1.OutsideClinicsPercentageoption = Request.Form["Model["+index+"].OutsideClinicsPercentageoption"] == "on";

                        //model1.ExaminationAndAnalysisLimitOption = Request.Form["Model["+index+"].ExaminationAndAnalysisLimitOption"] == "on";
                        //model1.ExaminationAndAnalysisPercentageoption = Request.Form["Model["+index+"].ExaminationAndAnalysisPercentageoption"] == "on";

                        //model1.PhysicalTherapyLimitOption = Request.Form["Model["+index+"].PhysicalTherapyLimitOption"] == "on";
                        //model1.PhysicalTherapyPercentageoption = Request.Form["Model["+index+"].PhysicalTherapyPercentageoption"] == "on";

                        //model1.DailyTherapyLimitOption = Request.Form["Model["+index+"].DailyTherapyLimitOption"] == "on";
                        //model1.DailyTherapyPercentageoption = Request.Form["Model["+index+"].DailyTherapyPercentageoption"] == "on";

                        //model1.ChronicTherapyLimitOption = Request.Form["Model["+index+"].ChronicTherapyLimitOption"] == "on";
                        //model1.ChronicTherapyPercentageoption = Request.Form["Model["+index+"].ChronicTherapyPercentageoption"] == "on";

                        //model1.NatChildBirthLimitOption = Request.Form["Model["+index+"].NatChildBirthLimitOption"] == "on";
                        //model1.NatChildBirthPercentageoption = Request.Form["Model["+index+"].NatChildBirthPercentageoption"] == "on";

                        //model1.CaesChildBirthLimitOption = Request.Form["Model["+index+"].CaesChildBirthLimitOption"] == "on";
                        //model1.CaesChildBirthPercentageoption = Request.Form["Model["+index+"].CaesChildBirthPercentageoption"] == "on";

                        //model1.LegalAbortionLimitOption = Request.Form["Model["+index+"].LegalAbortionLimitOption"] == "on";
                        //model1.LegalAbortionPercentageoption = Request.Form["Model["+index+"].LegalAbortionPercentageoption"] == "on";

                        //model1.PregFollowUpLimitOption = Request.Form["Model["+index+"].PregFollowUpLimitOption"] == "on";
                        //model1.PregFollowUpPercentageoption = Request.Form["Model["+index+"].PregFollowUpPercentageoption"] == "on";

                        //model1.AdvancedDentalServiceLimitOption = Request.Form["Model["+index+"].AdvancedDentalServiceLimitOption"] == "on";
                        //model1.AdvancedDentalServicePercentageoption = Request.Form["Model["+index+"].AdvancedDentalServicePercentageoption"] == "on";

                        //model1.BasicDentalServiceLimitOption = Request.Form["Model["+index+"].BasicDentalServiceLimitOption"] == "on";
                        //model1.BasicDentalServicePercentageoption = Request.Form["Model["+index+"].BasicDentalServicePercentageoption"] == "on";

                        //model1.OpticsLimitOption = Request.Form["Model["+index+"].OpticsLimitOption"] == "on";
                        //model1.OpticsPercentageoption = Request.Form["Model["+index+"].OpticsPercentageoption"] == "on";
                        #endregion

                    var maxVal = secondproposal[secodProposalIndex].AnnualCoverageCeiling;
                    secodProposalIndex++;
                    #region setting limit to max val
                    if (!model1.HospitalsResidenceServiceLimitOption)
                    {
                        model1.HospitalsResidenceServiceLimit = maxVal;
                    }
                    if (!model1.OutsideClinicsLimitOption)
                    {
                        model1.OutsideClinicsLimit = maxVal;
                    }
                    if (!model1.ExaminationAndAnalysisLimitOption)
                    {
                        model1.ExaminationAndAnalysisLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.DailyTherapyLimitOption)
                    {
                        model1.DailyTherapyLimit = maxVal;
                    }
                    if (!model1.ChronicTherapyLimitOption)
                    {
                        model1.ChronicTherapyLimit = maxVal;
                    }
                    if (!model1.NatChildBirthLimitOption)
                    {
                        model1.NatChildBirthLimit = maxVal;
                    }
                    if (!model1.CaesChildBirthLimitOption)
                    {
                        model1.CaesChildBirthLimit = maxVal;
                    }
                    if (!model1.LegalAbortionLimitOption)
                    {
                        model1.LegalAbortionLimit = maxVal;
                    }
                    if (!model1.PregFollowUpLimitOption)
                    {
                        model1.PregFollowUpLimit = maxVal;
                    }
                    if (!model1.AdvancedDentalServiceLimitOption)
                    {
                        model1.AdvancedDentalServiceLimit = maxVal;
                    }
                    if (!model1.BasicDentalServiceLimitOption)
                    {
                        model1.BasicDentalServiceLimit = maxVal;
                    }
                    if (!model1.OpticsLimitOption)
                    {
                        model1.OpticsLimit = maxVal;
                    }
                    #endregion
                    #region CheckMax
                    // Use reflection to iterate through properties
                    var properties = model1.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        // Check if the property ends with "Limit"
                        if (property.Name.EndsWith("Limit"))
                        {
                            // Get the value of the property
                            var propertyValue = (decimal)property.GetValue(model1);

                            // Check if the value exceeds maxVal
                            if (propertyValue > maxVal)
                            {
                                ModelState.AddModelError($"Model[{Model.IndexOf(model1)}].{property.Name}", $"The value cannot exceed the maximum value of {maxVal}.");
                            }
                        }
                    }
                    #endregion
                 
                    var maxPer = 100;
                    ///////percentage max //
                    ///
                    #region percentage limit
                    if (!model1.HospitalsResidenceServicePercentageoption)
                    {
                        model1.HospitalsResidenceServicePercentage = maxPer;
                    }
                    if (!model1.OutsideClinicsPercentageoption)
                    {
                        model1.OutsideClinicsPercentage = maxPer;
                    }
                    if (!model1.ExaminationAndAnalysisPercentageoption)
                    {
                        model1.ExaminationAndAnalysisPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.DailyTherapyPercentageoption)
                    {
                        model1.DailyTherapyPercentage = maxPer;
                    }
                    if (!model1.ChronicTherapyPercentageoption)
                    {
                        model1.ChronicTherapyPercentage = maxPer;
                    }
                    if (!model1.NatChildBirthPercentageoption)
                    {
                        model1.NatChildBirthPercentage = maxPer;
                    }
                    if (!model1.CaesChildBirthPercentageoption)
                    {
                        model1.CaesChildBirthPercentage = maxPer;
                    }
                    if (!model1.LegalAbortionPercentageoption)
                    {
                        model1.LegalAbortionPercentage = maxPer;
                    }
                    if (!model1.PregFollowUpPercentageoption)
                    {
                        model1.PregFollowUpPercentage = maxPer;
                    }
                    if (!model1.AdvancedDentalServicePercentageoption)
                    {
                        model1.AdvancedDentalServicePercentage = maxPer;
                    }
                    if (!model1.BasicDentalServicePercentageoption)
                    {
                        model1.BasicDentalServicePercentage = maxPer;
                    }
                    if (!model1.OpticsPercentageoption)
                    {
                        model1.OpticsPercentage = maxPer;
                    }
                    #endregion
                    #endregion

                    model1.ProposalMainId = (int)MainProposalId;
                }
                var index1 = 0;
                foreach (var model in Model)
                {
      

                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var existingProposal = _dbContext.ProposalInsideMedicalAuthorities.Find(model.Id);

                            if (existingProposal != null)
                            {
                                // Update the properties of the existing entity
                                MapViewModelToEntityModifyStepThree(model, existingProposal);

                                // Mark the entity as modified
                                _dbContext.Entry(existingProposal).State = System.Data.Entity.EntityState.Modified;
                                // Save changes
                                await _dbContext.SaveChangesAsync();

                            }
                            else
                            {
                                //var CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                                // Map view model to entity and save to database
                                var proposalThird = MapViewModelToEntityStepThree(model);
                                proposalThird.ClassCode = index1 + 1;
                                index1 = index1 + 1;

                                // Save the entity to the database
                                _dbContext.ProposalInsideMedicalAuthorities.Add(proposalThird);
                                await _dbContext.SaveChangesAsync();

                                

                            }
                        }
                        else
                        {
                            List<string> errors = new List<string>();

                            foreach (var value in ModelState.Values)
                            {
                                foreach (var error in value.Errors)
                                {
                                    errors.Add(error.ErrorMessage);
                                }
                            }
                            ViewBag.MainId = MainProposalId;
                            ViewBag.Errors = errors;
                            ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
                            // If the model state is not valid, redisplay the form with errors
                            // Populate dropdowns or other data needed for the view
                            return View(Model);
                        }


                    }
                    catch(Exception ex)
                    {

                        //List<string> errors = new List<string>();

                        //foreach (var value in ModelState.Values)
                        //{
                        //    foreach (var error in value.Errors)
                        //    {
                        //        errors.Add(error.ErrorMessage);
                        //    }
                        //}
                        ViewBag.Errors = ex.Message;
                 
                        ViewBag.MainId = MainProposalId;

                        ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                        ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
                        // If the model state is not valid, redisplay the form with errors
                        // Populate dropdowns or other data needed for the view
                        return View(Model);
                    }
                }
                // Redirect to a third page of the form
                switch (BackOrForwardOrMain)
                {
                    case "back":
                        return RedirectToAction(nameof(ProposalStepTwoCreate), new { id = MainProposalId });

                    case "forward":
                        return RedirectToAction(nameof(ProposalStepFourCreate), new { id = MainProposalId });
                    case "main":
                        return RedirectToAction(nameof(Index), new { id = MainProposalId });
                    default:
                        break;
                }
                return RedirectToAction(nameof(ProposalStepFourCreate), new { id = MainProposalId });
            }
            catch (Exception e)
            {
                var firstEntity = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId);
                if (firstEntity != null)
                    _dbContext.ProposalMains.Remove(firstEntity);
                return RedirectToAction(nameof(Index));
            }

        }
        // GET: ProposalMain/ProposalStepTwoEdit/1
        public ActionResult ProposalStepTHreeEdit()
        {
            int newId = Convert.ToInt32(Request.QueryString["newId"]);
            int prevId = Convert.ToInt32(Request.QueryString["prevId"]);
            // Retrieve data from temporary storage or session
            // ...



            ViewBag.MainId = newId;
            ViewBag.PrevId = prevId;
            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
            var proposalMain = _dbContext.ProposalMains.Include("ProposalInsiceMedicalAuthorities").FirstOrDefault(p => p.Id == prevId);
            ViewBag.CatCount = proposalMain.CategoriesCount;
            var models = proposalMain.ProposalInsiceMedicalAuthorities.ToList();
            if (models == null)
            {
                var emptyModels = new List<ProposalStepTwoViewModel>(proposalMain.CategoriesCount);
                for (int i = 0; i < proposalMain.CategoriesCount; i++)
                {
                    // Create an instance of ProposalStepTwoViewModel and add it to the list
                    emptyModels.Add(new ProposalStepTwoViewModel());
                }
                return View(emptyModels);
            }
         
            //call the function to map model to viewModel
            var mappedModels = new List<ProposalInsideMedicalAuthorityViewModel>();
            foreach (var entity in models)
            {
                mappedModels.Add(MapEntityToViewModelStepThree(entity));
            }
            return View(mappedModels);
        }
        // POST: ProposalMain/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProposalStepThreeEdit(List<ProposalInsideMedicalAuthorityViewModel> Model, int id, int PrevId)
        {
            try
            {
                var secondproposal = _dbContext.ProposalStepTwos.Where(sp => sp.ProposalMainId == id).ToList();
                var secodProposalIndex = 0;
                foreach (var model1 in Model)
                {
                    int index = Model.IndexOf(model1);
                    #region binding checkBoxes
                    #region bind on to true
                    //model1.HospitalsResidenceServiceLimitOption = Request.Form["Model["+index+"].HospitalsResidenceServiceLimitOption"] == "on";
                    //model1.HospitalsResidenceServicePercentageoption = Request.Form["Model["+index+"].HospitalsResidenceServicePercentageoption"] == "on";

                    //model1.OutsideClinicsLimitOption = Request.Form["Model["+index+"].OutsideClinicsLimitOption"] == "on";
                    //model1.OutsideClinicsPercentageoption = Request.Form["Model["+index+"].OutsideClinicsPercentageoption"] == "on";

                    //model1.ExaminationAndAnalysisLimitOption = Request.Form["Model["+index+"].ExaminationAndAnalysisLimitOption"] == "on";
                    //model1.ExaminationAndAnalysisPercentageoption = Request.Form["Model["+index+"].ExaminationAndAnalysisPercentageoption"] == "on";

                    //model1.PhysicalTherapyLimitOption = Request.Form["Model["+index+"].PhysicalTherapyLimitOption"] == "on";
                    //model1.PhysicalTherapyPercentageoption = Request.Form["Model["+index+"].PhysicalTherapyPercentageoption"] == "on";

                    //model1.DailyTherapyLimitOption = Request.Form["Model["+index+"].DailyTherapyLimitOption"] == "on";
                    //model1.DailyTherapyPercentageoption = Request.Form["Model["+index+"].DailyTherapyPercentageoption"] == "on";

                    //model1.ChronicTherapyLimitOption = Request.Form["Model["+index+"].ChronicTherapyLimitOption"] == "on";
                    //model1.ChronicTherapyPercentageoption = Request.Form["Model["+index+"].ChronicTherapyPercentageoption"] == "on";

                    //model1.NatChildBirthLimitOption = Request.Form["Model["+index+"].NatChildBirthLimitOption"] == "on";
                    //model1.NatChildBirthPercentageoption = Request.Form["Model["+index+"].NatChildBirthPercentageoption"] == "on";

                    //model1.CaesChildBirthLimitOption = Request.Form["Model["+index+"].CaesChildBirthLimitOption"] == "on";
                    //model1.CaesChildBirthPercentageoption = Request.Form["Model["+index+"].CaesChildBirthPercentageoption"] == "on";

                    //model1.LegalAbortionLimitOption = Request.Form["Model["+index+"].LegalAbortionLimitOption"] == "on";
                    //model1.LegalAbortionPercentageoption = Request.Form["Model["+index+"].LegalAbortionPercentageoption"] == "on";

                    //model1.PregFollowUpLimitOption = Request.Form["Model["+index+"].PregFollowUpLimitOption"] == "on";
                    //model1.PregFollowUpPercentageoption = Request.Form["Model["+index+"].PregFollowUpPercentageoption"] == "on";

                    //model1.AdvancedDentalServiceLimitOption = Request.Form["Model["+index+"].AdvancedDentalServiceLimitOption"] == "on";
                    //model1.AdvancedDentalServicePercentageoption = Request.Form["Model["+index+"].AdvancedDentalServicePercentageoption"] == "on";

                    //model1.BasicDentalServiceLimitOption = Request.Form["Model["+index+"].BasicDentalServiceLimitOption"] == "on";
                    //model1.BasicDentalServicePercentageoption = Request.Form["Model["+index+"].BasicDentalServicePercentageoption"] == "on";

                    //model1.OpticsLimitOption = Request.Form["Model["+index+"].OpticsLimitOption"] == "on";
                    //model1.OpticsPercentageoption = Request.Form["Model["+index+"].OpticsPercentageoption"] == "on";
                    #endregion

                    var maxVal = secondproposal[secodProposalIndex].AnnualCoverageCeiling;
                    secodProposalIndex++;
                    #region setting limit to max val
                    if (!model1.HospitalsResidenceServiceLimitOption)
                    {
                        model1.HospitalsResidenceServiceLimit = maxVal;
                    }
                    if (!model1.OutsideClinicsLimitOption)
                    {
                        model1.OutsideClinicsLimit = maxVal;
                    }
                    if (!model1.ExaminationAndAnalysisLimitOption)
                    {
                        model1.ExaminationAndAnalysisLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.DailyTherapyLimitOption)
                    {
                        model1.DailyTherapyLimit = maxVal;
                    }
                    if (!model1.ChronicTherapyLimitOption)
                    {
                        model1.ChronicTherapyLimit = maxVal;
                    }
                    if (!model1.NatChildBirthLimitOption)
                    {
                        model1.NatChildBirthLimit = maxVal;
                    }
                    if (!model1.CaesChildBirthLimitOption)
                    {
                        model1.CaesChildBirthLimit = maxVal;
                    }
                    if (!model1.LegalAbortionLimitOption)
                    {
                        model1.LegalAbortionLimit = maxVal;
                    }
                    if (!model1.PregFollowUpLimitOption)
                    {
                        model1.PregFollowUpLimit = maxVal;
                    }
                    if (!model1.AdvancedDentalServiceLimitOption)
                    {
                        model1.AdvancedDentalServiceLimit = maxVal;
                    }
                    if (!model1.BasicDentalServiceLimitOption)
                    {
                        model1.BasicDentalServiceLimit = maxVal;
                    }
                    if (!model1.OpticsLimitOption)
                    {
                        model1.OpticsLimit = maxVal;
                    }
                    #endregion
                    #region CheckMax
                    // Use reflection to iterate through properties
                    var properties = model1.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        // Check if the property ends with "Limit"
                        if (property.Name.EndsWith("Limit"))
                        {
                            // Get the value of the property
                            var propertyValue = (decimal)property.GetValue(model1);

                            // Check if the value exceeds maxVal
                            if (propertyValue > maxVal)
                            {
                                ModelState.AddModelError($"Model[{Model.IndexOf(model1)}].{property.Name}", $"The value cannot exceed the maximum value of {maxVal}.");
                            }
                        }
                    }
                    #endregion
                    var maxPer = 100;
                    ///////percentage max //
                    ///
                    #region percentage limit
                    if (!model1.HospitalsResidenceServicePercentageoption)
                    {
                        model1.HospitalsResidenceServicePercentage = maxPer;
                    }
                    if (!model1.OutsideClinicsPercentageoption)
                    {
                        model1.OutsideClinicsPercentage = maxPer;
                    }
                    if (!model1.ExaminationAndAnalysisPercentageoption)
                    {
                        model1.ExaminationAndAnalysisPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.DailyTherapyPercentageoption)
                    {
                        model1.DailyTherapyPercentage = maxPer;
                    }
                    if (!model1.ChronicTherapyPercentageoption)
                    {
                        model1.ChronicTherapyPercentage = maxPer;
                    }
                    if (!model1.NatChildBirthPercentageoption)
                    {
                        model1.NatChildBirthPercentage = maxPer;
                    }
                    if (!model1.CaesChildBirthPercentageoption)
                    {
                        model1.CaesChildBirthPercentage = maxPer;
                    }
                    if (!model1.LegalAbortionPercentageoption)
                    {
                        model1.LegalAbortionPercentage = maxPer;
                    }
                    if (!model1.PregFollowUpPercentageoption)
                    {
                        model1.PregFollowUpPercentage = maxPer;
                    }
                    if (!model1.AdvancedDentalServicePercentageoption)
                    {
                        model1.AdvancedDentalServicePercentage = maxPer;
                    }
                    if (!model1.BasicDentalServicePercentageoption)
                    {
                        model1.BasicDentalServicePercentage = maxPer;
                    }
                    if (!model1.OpticsPercentageoption)
                    {
                        model1.OpticsPercentage = maxPer;
                    }
                    #endregion
                    #endregion
                    model1.ProposalMainId = (int)id;
                }
                var index1 = 0;
                foreach (var model in Model)
                {
                  
                    model.ProposalMainId = (int)id;
                    if (ModelState.IsValid)
                    { 

                
                        //var CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                        // Map view model to entity and save to database
                        var proposalThird = MapViewModelToEntityStepThree(model);
                        proposalThird.ClassCode = index1 + 1;
                        index1 = index1 + 1;

                        // Save the entity to the database
                        _dbContext.ProposalInsideMedicalAuthorities.Add(proposalThird);
                        await _dbContext.SaveChangesAsync();

                    }
                    else
                    {

                        List<string> errors = new List<string>();

                        foreach (var value in ModelState.Values)
                        {
                            foreach (var error in value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        ViewBag.Errors = errors;
                   
                        ViewBag.MainId = id; 

                        ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id).CategoriesCount;
                        ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
                        // If the model state is not valid, redisplay the form with errors
                        // Populate dropdowns or other data needed for the view
                        return View(Model);
                    }
                }
                // Redirect to a third page of the form
                return RedirectToAction(nameof(ProposalStepFourEdit), new { newId = id, prevId = PrevId });
            }
            catch (Exception e)
            {
                var firstEntity = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id);
                if (firstEntity != null)
                    _dbContext.ProposalMains.Remove(firstEntity);
                return RedirectToAction(nameof(Index));
            }
        }
        private ProposalInsideMedicalAuthority MapViewModelToEntityStepThree(ProposalInsideMedicalAuthorityViewModel vM)
        {
            // Use a mapping library or manually map properties as needed
            // This is a simplified example, adjust as needed
            var proposalthree = new ProposalInsideMedicalAuthority
            {
                HospitalsResidenceServiceLimitOption = vM.HospitalsResidenceServiceLimitOption,
                HospitalsResidenceServicePercentageoption = vM.HospitalsResidenceServicePercentageoption,
                HospitalsResidenceServiceLimit = vM.HospitalsResidenceServiceLimit,
                HospitalsResidenceServicePercentage = vM.HospitalsResidenceServicePercentage,
                OutsideClinicsLimitOption = vM.OutsideClinicsLimitOption,
                OutsideClinicsPercentageoption = vM.OutsideClinicsPercentageoption,
                OutsideClinicsLimit = vM.OutsideClinicsLimit,
                OutsideClinicsPercentage = vM.OutsideClinicsPercentage,
                ExaminationAndAnalysisLimitOption = vM.ExaminationAndAnalysisLimitOption,
                ExaminationAndAnalysisPercentageoption = vM.ExaminationAndAnalysisPercentageoption,
                ExaminationAndAnalysisLimit = vM.ExaminationAndAnalysisLimit,
                ExaminationAndAnalysisPercentage = vM.ExaminationAndAnalysisPercentage,
                PhysicalTherapyLimitOption = vM.PhysicalTherapyLimitOption,
                PhysicalTherapyPercentageoption = vM.PhysicalTherapyPercentageoption,
                PhysicalTherapyLimit = vM.PhysicalTherapyLimit,
                PhysicalTherapyPercentage = vM.PhysicalTherapyPercentage,
                DailyTherapyLimitOption = vM.DailyTherapyLimitOption,
                DailyTherapyPercentageoption = vM.DailyTherapyPercentageoption,
                DailyTherapyLimit = vM.DailyTherapyLimit,
                DailyTherapyPercentage = vM.DailyTherapyPercentage,
                ChronicTherapyLimitOption = vM.ChronicTherapyLimitOption,
                ChronicTherapyPercentageoption = vM.ChronicTherapyPercentageoption,
                ChronicTherapyLimit = vM.ChronicTherapyLimit,
                ChronicTherapyPercentage = vM.ChronicTherapyPercentage,
                NatChildBirthLimitOption = vM.NatChildBirthLimitOption,
                NatChildBirthPercentageoption = vM.NatChildBirthPercentageoption,
                NatChildBirthLimit = vM.NatChildBirthLimit,
                NatChildBirthPercentage = vM.NatChildBirthPercentage,
                CaesChildBirthLimitOption = vM.CaesChildBirthLimitOption,
                CaesChildBirthPercentageoption = vM.CaesChildBirthPercentageoption,
                CaesChildBirthLimit = vM.CaesChildBirthLimit,
                CaesChildBirthPercentage = vM.CaesChildBirthPercentage,
                LegalAbortionLimitOption = vM.LegalAbortionLimitOption,
                LegalAbortionPercentageoption = vM.LegalAbortionPercentageoption,
                LegalAbortionLimit = vM.LegalAbortionLimit,
                LegalAbortionPercentage = vM.LegalAbortionPercentage,
                PregFollowUpLimitOption = vM.PregFollowUpLimitOption,
                PregFollowUpPercentageoption = vM.PregFollowUpPercentageoption,
                PregFollowUpLimit = vM.PregFollowUpLimit,
                PregFollowUpPercentage = vM.PregFollowUpPercentage,
                AdvancedDentalServiceLimitOption = vM.AdvancedDentalServiceLimitOption,
                AdvancedDentalServicePercentageoption = vM.AdvancedDentalServicePercentageoption,
                AdvancedDentalServiceLimit = vM.AdvancedDentalServiceLimit,
                AdvancedDentalServicePercentage = vM.AdvancedDentalServicePercentage,
                BasicDentalServiceLimitOption = vM.BasicDentalServiceLimitOption,
                BasicDentalServicePercentageoption = vM.BasicDentalServicePercentageoption,
                BasicDentalServiceLimit = vM.BasicDentalServiceLimit,
                BasicDentalServicePercentage = vM.BasicDentalServicePercentage,
                OpticsLimitOption = vM.OpticsLimitOption,
                OpticsPercentageoption = vM.OpticsPercentageoption,
                OpticsLimit = vM.OpticsLimit,
                OpticsPercentage = vM.OpticsPercentage,
                IntensiveCareDaysCount = vM.IntensiveCareDaysCount,
                DailyRoshitasCountPerMonth = vM.DailyRoshitasCountPerMonth,
                CoronaVaccineCoverage = vM.CoronaVaccineCoverage,
           
                //relations

                ProposalMainId = vM.ProposalMainId


                // Map other properties as needed
            };

            return proposalthree;
        }
        private ProposalInsideMedicalAuthorityViewModel MapEntityToViewModelStepThree(ProposalInsideMedicalAuthority vM)
        {
            var proposalMain = new ProposalInsideMedicalAuthorityViewModel
            {
                Id = vM.Id,
                HospitalsResidenceServiceLimitOption = vM.HospitalsResidenceServiceLimitOption,
                HospitalsResidenceServicePercentageoption = vM.HospitalsResidenceServicePercentageoption,
                HospitalsResidenceServiceLimit = vM.HospitalsResidenceServiceLimit,
                HospitalsResidenceServicePercentage = vM.HospitalsResidenceServicePercentage,
                OutsideClinicsLimitOption = vM.OutsideClinicsLimitOption,
                OutsideClinicsPercentageoption = vM.OutsideClinicsPercentageoption,
                OutsideClinicsLimit = vM.OutsideClinicsLimit,
                OutsideClinicsPercentage = vM.OutsideClinicsPercentage,
                ExaminationAndAnalysisLimitOption = vM.ExaminationAndAnalysisLimitOption,
                ExaminationAndAnalysisPercentageoption = vM.ExaminationAndAnalysisPercentageoption,
                ExaminationAndAnalysisLimit = vM.ExaminationAndAnalysisLimit,
                ExaminationAndAnalysisPercentage = vM.ExaminationAndAnalysisPercentage,
                PhysicalTherapyLimitOption = vM.PhysicalTherapyLimitOption,
                PhysicalTherapyPercentageoption = vM.PhysicalTherapyPercentageoption,
                PhysicalTherapyLimit = vM.PhysicalTherapyLimit,
                PhysicalTherapyPercentage = vM.PhysicalTherapyPercentage,
                DailyTherapyLimitOption = vM.DailyTherapyLimitOption,
                DailyTherapyPercentageoption = vM.DailyTherapyPercentageoption,
                DailyTherapyLimit = vM.DailyTherapyLimit,
                DailyTherapyPercentage = vM.DailyTherapyPercentage,
                ChronicTherapyLimitOption = vM.ChronicTherapyLimitOption,
                ChronicTherapyPercentageoption = vM.ChronicTherapyPercentageoption,
                ChronicTherapyLimit = vM.ChronicTherapyLimit,
                ChronicTherapyPercentage = vM.ChronicTherapyPercentage,
                NatChildBirthLimitOption = vM.NatChildBirthLimitOption,
                NatChildBirthPercentageoption = vM.NatChildBirthPercentageoption,
                NatChildBirthLimit = vM.NatChildBirthLimit,
                NatChildBirthPercentage = vM.NatChildBirthPercentage,
                CaesChildBirthLimitOption = vM.CaesChildBirthLimitOption,
                CaesChildBirthPercentageoption = vM.CaesChildBirthPercentageoption,
                CaesChildBirthLimit = vM.CaesChildBirthLimit,
                CaesChildBirthPercentage = vM.CaesChildBirthPercentage,
                LegalAbortionLimitOption = vM.LegalAbortionLimitOption,
                LegalAbortionPercentageoption = vM.LegalAbortionPercentageoption,
                LegalAbortionLimit = vM.LegalAbortionLimit,
                LegalAbortionPercentage = vM.LegalAbortionPercentage,
                PregFollowUpLimitOption = vM.PregFollowUpLimitOption,
                PregFollowUpPercentageoption = vM.PregFollowUpPercentageoption,
                PregFollowUpLimit = vM.PregFollowUpLimit,
                PregFollowUpPercentage = vM.PregFollowUpPercentage,
                AdvancedDentalServiceLimitOption = vM.AdvancedDentalServiceLimitOption,
                AdvancedDentalServicePercentageoption = vM.AdvancedDentalServicePercentageoption,
                AdvancedDentalServiceLimit = vM.AdvancedDentalServiceLimit,
                AdvancedDentalServicePercentage = vM.AdvancedDentalServicePercentage,
                BasicDentalServiceLimitOption = vM.BasicDentalServiceLimitOption,
                BasicDentalServicePercentageoption = vM.BasicDentalServicePercentageoption,
                BasicDentalServiceLimit = vM.BasicDentalServiceLimit,
                BasicDentalServicePercentage = vM.BasicDentalServicePercentage,
                OpticsLimitOption = vM.OpticsLimitOption,
                OpticsPercentageoption = vM.OpticsPercentageoption,
                OpticsLimit = vM.OpticsLimit,
                OpticsPercentage = vM.OpticsPercentage,
                IntensiveCareDaysCount = vM.IntensiveCareDaysCount,
                DailyRoshitasCountPerMonth = vM.DailyRoshitasCountPerMonth,
                CoronaVaccineCoverage = vM.CoronaVaccineCoverage,
              
                //relations

                ProposalMainId = vM.ProposalMainId


                // Map other properties as needed
            };
            return proposalMain;
        }
        private void MapViewModelToEntityModifyStepThree(ProposalInsideMedicalAuthorityViewModel vM, ProposalInsideMedicalAuthority existingProposal)
        {
            existingProposal.HospitalsResidenceServiceLimitOption = vM.HospitalsResidenceServiceLimitOption;
            existingProposal.HospitalsResidenceServicePercentageoption = vM.HospitalsResidenceServicePercentageoption;
            existingProposal.HospitalsResidenceServiceLimit = vM.HospitalsResidenceServiceLimit;
            existingProposal.HospitalsResidenceServicePercentage = vM.HospitalsResidenceServicePercentage;
            existingProposal.OutsideClinicsLimitOption = vM.OutsideClinicsLimitOption;
            existingProposal.OutsideClinicsPercentageoption = vM.OutsideClinicsPercentageoption;
            existingProposal.OutsideClinicsLimit = vM.OutsideClinicsLimit;
            existingProposal.OutsideClinicsPercentage = vM.OutsideClinicsPercentage;
            existingProposal.ExaminationAndAnalysisLimitOption = vM.ExaminationAndAnalysisLimitOption;
            existingProposal.ExaminationAndAnalysisPercentageoption = vM.ExaminationAndAnalysisPercentageoption;
            existingProposal.ExaminationAndAnalysisLimit = vM.ExaminationAndAnalysisLimit;
            existingProposal.ExaminationAndAnalysisPercentage = vM.ExaminationAndAnalysisPercentage;
            existingProposal.PhysicalTherapyLimitOption = vM.PhysicalTherapyLimitOption;
            existingProposal.PhysicalTherapyPercentageoption = vM.PhysicalTherapyPercentageoption;
            existingProposal.PhysicalTherapyLimit = vM.PhysicalTherapyLimit;
            existingProposal.PhysicalTherapyPercentage = vM.PhysicalTherapyPercentage;
            existingProposal.DailyTherapyLimitOption = vM.DailyTherapyLimitOption;
            existingProposal.DailyTherapyPercentageoption = vM.DailyTherapyPercentageoption;
            existingProposal.DailyTherapyLimit = vM.DailyTherapyLimit;
            existingProposal.DailyTherapyPercentage = vM.DailyTherapyPercentage;
            existingProposal.ChronicTherapyLimitOption = vM.ChronicTherapyLimitOption;
            existingProposal.ChronicTherapyPercentageoption = vM.ChronicTherapyPercentageoption;
            existingProposal.ChronicTherapyLimit = vM.ChronicTherapyLimit;
            existingProposal.ChronicTherapyPercentage = vM.ChronicTherapyPercentage;
            existingProposal.NatChildBirthLimitOption = vM.NatChildBirthLimitOption;
            existingProposal.NatChildBirthPercentageoption = vM.NatChildBirthPercentageoption;
            existingProposal.NatChildBirthLimit = vM.NatChildBirthLimit;
            existingProposal.NatChildBirthPercentage = vM.NatChildBirthPercentage;
            existingProposal.CaesChildBirthLimitOption = vM.CaesChildBirthLimitOption;
            existingProposal.CaesChildBirthPercentageoption = vM.CaesChildBirthPercentageoption;
            existingProposal.CaesChildBirthLimit = vM.CaesChildBirthLimit;
            existingProposal.CaesChildBirthPercentage = vM.CaesChildBirthPercentage;
            existingProposal.LegalAbortionLimitOption = vM.LegalAbortionLimitOption;
            existingProposal.LegalAbortionPercentageoption = vM.LegalAbortionPercentageoption;
            existingProposal.LegalAbortionLimit = vM.LegalAbortionLimit;
            existingProposal.LegalAbortionPercentage = vM.LegalAbortionPercentage;
            existingProposal.PregFollowUpLimitOption = vM.PregFollowUpLimitOption;
            existingProposal.PregFollowUpPercentageoption = vM.PregFollowUpPercentageoption;
            existingProposal.PregFollowUpLimit = vM.PregFollowUpLimit;
            existingProposal.PregFollowUpPercentage = vM.PregFollowUpPercentage;
            existingProposal.AdvancedDentalServiceLimitOption = vM.AdvancedDentalServiceLimitOption;
            existingProposal.AdvancedDentalServicePercentageoption = vM.AdvancedDentalServicePercentageoption;
            existingProposal.AdvancedDentalServiceLimit = vM.AdvancedDentalServiceLimit;
            existingProposal.AdvancedDentalServicePercentage = vM.AdvancedDentalServicePercentage;
            existingProposal.BasicDentalServiceLimitOption = vM.BasicDentalServiceLimitOption;
            existingProposal.BasicDentalServicePercentageoption = vM.BasicDentalServicePercentageoption;
            existingProposal.BasicDentalServiceLimit = vM.BasicDentalServiceLimit;
            existingProposal.BasicDentalServicePercentage = vM.BasicDentalServicePercentage;
            existingProposal.OpticsLimitOption = vM.OpticsLimitOption;
            existingProposal.OpticsPercentageoption = vM.OpticsPercentageoption;
            existingProposal.OpticsLimit = vM.OpticsLimit;
            existingProposal.OpticsPercentage = vM.OpticsPercentage;
            existingProposal.IntensiveCareDaysCount = vM.IntensiveCareDaysCount;
            existingProposal.DailyRoshitasCountPerMonth = vM.DailyRoshitasCountPerMonth;
            existingProposal.CoronaVaccineCoverage = vM.CoronaVaccineCoverage;

            //relations

            existingProposal.ProposalMainId = vM.ProposalMainId;

        }
        #endregion

        #region ProposalStepFour
        public ActionResult ProposalStepFourCreate(int id)
        {
            // Retrieve data from temporary storage or session
            // ...
            // Retrieve the list of Areas and CompanyActivities from the database



            // Convert the lists to SelectList items for use in dropdown lists

            ViewBag.MainId = id;
            var mainProposal = _dbContext.ProposalMains.Include("ProposalOutsideMedicalAuthorities").FirstOrDefault(p => p.Id == id);

            var categoriesCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id).CategoriesCount;
            ViewBag.CatCount = categoriesCount;
            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
            var model = new List<ProposalOutsideMedicalAuthorityViewModel>(categoriesCount);
            if (mainProposal.ProposalOutsideMedicalAuthorities.Count() > 0)
            {
                var proposalOutsideMedicalAuthorities = _dbContext.ProposalOutsideMedicalAuthorities
                    .Include("PricesOutsideMedicalAuthority").Where(p => p.ProposalMainId == id);
                foreach (var stepFour in proposalOutsideMedicalAuthorities)
                {
                    var item = MapEntityToViewModelStepFour(stepFour);
                    item.Id = stepFour.Id;
                    model.Add(item);

                }
            }
            else
            {

                for (int i = 0; i < categoriesCount; i++)
                {
                    // Create an instance of ProposalStepTwoViewModel and add it to the list
                    model.Add(new ProposalOutsideMedicalAuthorityViewModel());
                }
            }
            var priceList = _dbContext.PricesOutsideMedicalAuthorities.ToList();
            // Convert the lists to SelectList items for use in dropdown lists
            ViewBag.PriceList = new SelectList(priceList, "Id", "Price");
            // Render the view for the second step
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProposalStepFourCreate(List<ProposalOutsideMedicalAuthorityViewModel> Model, int? MainProposalId,
            string BackOrForwardOrMain="")
        {
            var mainId = 0;
            try
            {

                var secondproposal = _dbContext.ProposalStepTwos.Where(sp => sp.ProposalMainId == MainProposalId).ToList();
                var secodProposalIndex = 0;
                foreach (var model1 in Model)
                {
                    int index = Model.IndexOf(model1);
                    #region binding checkBoxes
                    #region bind on to true
                    //model1.HospitalsResidenceServiceLimitOption = Request.Form["Model["+index+"].HospitalsResidenceServiceLimitOption"] == "on";
                    //model1.HospitalsResidenceServicePercentageoption = Request.Form["Model["+index+"].HospitalsResidenceServicePercentageoption"] == "on";

                    //model1.OutsideClinicsLimitOption = Request.Form["Model["+index+"].OutsideClinicsLimitOption"] == "on";
                    //model1.OutsideClinicsPercentageoption = Request.Form["Model["+index+"].OutsideClinicsPercentageoption"] == "on";

                    //model1.ExaminationAndAnalysisLimitOption = Request.Form["Model["+index+"].ExaminationAndAnalysisLimitOption"] == "on";
                    //model1.ExaminationAndAnalysisPercentageoption = Request.Form["Model["+index+"].ExaminationAndAnalysisPercentageoption"] == "on";

                    //model1.PhysicalTherapyLimitOption = Request.Form["Model["+index+"].PhysicalTherapyLimitOption"] == "on";
                    //model1.PhysicalTherapyPercentageoption = Request.Form["Model["+index+"].PhysicalTherapyPercentageoption"] == "on";

                    //model1.DailyTherapyLimitOption = Request.Form["Model["+index+"].DailyTherapyLimitOption"] == "on";
                    //model1.DailyTherapyPercentageoption = Request.Form["Model["+index+"].DailyTherapyPercentageoption"] == "on";

                    //model1.ChronicTherapyLimitOption = Request.Form["Model["+index+"].ChronicTherapyLimitOption"] == "on";
                    //model1.ChronicTherapyPercentageoption = Request.Form["Model["+index+"].ChronicTherapyPercentageoption"] == "on";

                    //model1.NatChildBirthLimitOption = Request.Form["Model["+index+"].NatChildBirthLimitOption"] == "on";
                    //model1.NatChildBirthPercentageoption = Request.Form["Model["+index+"].NatChildBirthPercentageoption"] == "on";

                    //model1.CaesChildBirthLimitOption = Request.Form["Model["+index+"].CaesChildBirthLimitOption"] == "on";
                    //model1.CaesChildBirthPercentageoption = Request.Form["Model["+index+"].CaesChildBirthPercentageoption"] == "on";

                    //model1.LegalAbortionLimitOption = Request.Form["Model["+index+"].LegalAbortionLimitOption"] == "on";
                    //model1.LegalAbortionPercentageoption = Request.Form["Model["+index+"].LegalAbortionPercentageoption"] == "on";

                    //model1.PregFollowUpLimitOption = Request.Form["Model["+index+"].PregFollowUpLimitOption"] == "on";
                    //model1.PregFollowUpPercentageoption = Request.Form["Model["+index+"].PregFollowUpPercentageoption"] == "on";

                    //model1.AdvancedDentalServiceLimitOption = Request.Form["Model["+index+"].AdvancedDentalServiceLimitOption"] == "on";
                    //model1.AdvancedDentalServicePercentageoption = Request.Form["Model["+index+"].AdvancedDentalServicePercentageoption"] == "on";

                    //model1.BasicDentalServiceLimitOption = Request.Form["Model["+index+"].BasicDentalServiceLimitOption"] == "on";
                    //model1.BasicDentalServicePercentageoption = Request.Form["Model["+index+"].BasicDentalServicePercentageoption"] == "on";

                    //model1.OpticsLimitOption = Request.Form["Model["+index+"].OpticsLimitOption"] == "on";
                    //model1.OpticsPercentageoption = Request.Form["Model["+index+"].OpticsPercentageoption"] == "on";
                    #endregion

                    var maxVal = secondproposal[secodProposalIndex].AnnualCoverageCeiling;
                    secodProposalIndex++;
                    #region setting limit to max val
                    if (!model1.HospitalsResidenceServiceLimitOption)
                    {
                        model1.HospitalsResidenceServiceLimit = maxVal;
                    }
                    if (!model1.OutsideClinicsLimitOption)
                    {
                        model1.OutsideClinicsLimit = maxVal;
                    }
                    if (!model1.ExaminationAndAnalysisLimitOption)
                    {
                        model1.ExaminationAndAnalysisLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.DailyTherapyLimitOption)
                    {
                        model1.DailyTherapyLimit = maxVal;
                    }
                    if (!model1.ChronicTherapyLimitOption)
                    {
                        model1.ChronicTherapyLimit = maxVal;
                    }
                    if (!model1.NatChildBirthLimitOption)
                    {
                        model1.NatChildBirthLimit = maxVal;
                    }
                    if (!model1.CaesChildBirthLimitOption)
                    {
                        model1.CaesChildBirthLimit = maxVal;
                    }
                    if (!model1.LegalAbortionLimitOption)
                    {
                        model1.LegalAbortionLimit = maxVal;
                    }
                    if (!model1.PregFollowUpLimitOption)
                    {
                        model1.PregFollowUpLimit = maxVal;
                    }
                    if (!model1.AdvancedDentalServiceLimitOption)
                    {
                        model1.AdvancedDentalServiceLimit = maxVal;
                    }
                    if (!model1.BasicDentalServiceLimitOption)
                    {
                        model1.BasicDentalServiceLimit = maxVal;
                    }
                    if (!model1.OpticsLimitOption)
                    {
                        model1.OpticsLimit = maxVal;
                    }
                    #endregion
                    #region CheckMax
                    // Use reflection to iterate through properties
                    var properties = model1.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        // Check if the property ends with "Limit"
                        if (property.Name.EndsWith("Limit"))
                        {
                            // Get the value of the property
                            var propertyValue = (decimal)property.GetValue(model1);

                            // Check if the value exceeds maxVal
                            if (propertyValue > maxVal)
                            {
                                ModelState.AddModelError($"Model[{Model.IndexOf(model1)}].{property.Name}", $"The value cannot exceed the maximum value of {maxVal}.");
                            }
                        }
                    }
                    #endregion
                    var maxPer = 100;
                    ///////percentage max //
                    ///
                    #region percentage limit
                    if (!model1.HospitalsResidenceServicePercentageoption)
                    {
                        model1.HospitalsResidenceServicePercentage = maxPer;
                    }
                    if (!model1.OutsideClinicsPercentageoption)
                    {
                        model1.OutsideClinicsPercentage = maxPer;
                    }
                    if (!model1.ExaminationAndAnalysisPercentageoption)
                    {
                        model1.ExaminationAndAnalysisPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.DailyTherapyPercentageoption)
                    {
                        model1.DailyTherapyPercentage = maxPer;
                    }
                    if (!model1.ChronicTherapyPercentageoption)
                    {
                        model1.ChronicTherapyPercentage = maxPer;
                    }
                    if (!model1.NatChildBirthPercentageoption)
                    {
                        model1.NatChildBirthPercentage = maxPer;
                    }
                    if (!model1.CaesChildBirthPercentageoption)
                    {
                        model1.CaesChildBirthPercentage = maxPer;
                    }
                    if (!model1.LegalAbortionPercentageoption)
                    {
                        model1.LegalAbortionPercentage = maxPer;
                    }
                    if (!model1.PregFollowUpPercentageoption)
                    {
                        model1.PregFollowUpPercentage = maxPer;
                    }
                    if (!model1.AdvancedDentalServicePercentageoption)
                    {
                        model1.AdvancedDentalServicePercentage = maxPer;
                    }
                    if (!model1.BasicDentalServicePercentageoption)
                    {
                        model1.BasicDentalServicePercentage = maxPer;
                    }
                    if (!model1.OpticsPercentageoption)
                    {
                        model1.OpticsPercentage = maxPer;
                    }
                    #endregion
                    #endregion

                    model1.ProposalMainId = (int)MainProposalId;
                }
                var index1 = 0;
                foreach (var model in Model)
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var existingProposal = _dbContext.ProposalOutsideMedicalAuthorities
                                .Include("PricesOutsideMedicalAuthority").FirstOrDefault(p => p.Id == model.Id);

                            if (existingProposal != null)
                            {
                                // Update the properties of the existing entity
                                MapViewModelToEntityModifyStepFour(model, existingProposal);

                                // Mark the entity as modified
                                _dbContext.Entry(existingProposal).State = System.Data.Entity.EntityState.Modified;
                                // Save changes
                                await _dbContext.SaveChangesAsync();
                                mainId = existingProposal.ProposalMainId;

                            }
                            else
                            {

                                //var CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                                // Map view model to entity and save to database
                                var proposalThird = MapViewModelToEntityStepFour(model);
                                proposalThird.ClassCode = index1 + 1;
                                index1 = index1 + 1;

                                // Save the entity to the database
                                _dbContext.ProposalOutsideMedicalAuthorities.Add(proposalThird);
                                await _dbContext.SaveChangesAsync();
                                mainId = proposalThird.ProposalMainId;

                            }

                        }
                        else
                        {
                            List<string> errors = new List<string>();

                            foreach (var value in ModelState.Values)
                            {
                                foreach (var error in value.Errors)
                                {
                                    errors.Add(error.ErrorMessage);
                                }
                            }
                            var priceList = _dbContext.PricesOutsideMedicalAuthorities.ToList();
                            // Convert the lists to SelectList items for use in dropdown lists
                            ViewBag.PriceList = new SelectList(priceList, "Id", "Price");
                            ViewBag.Errors = errors;

                            ViewBag.MainId = MainProposalId;

                            ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();

                            // If the model state is not valid, redisplay the form with errors
                            // Populate dropdowns or other data needed for the view
                            return View(Model);
                        }
                    }
                    catch (Exception ex)
                    {

                        List<string> errors = new List<string>();

                        foreach (var value in ModelState.Values)
                        {
                            foreach (var error in value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        var priceList = _dbContext.PricesOutsideMedicalAuthorities.ToList();
                        // Convert the lists to SelectList items for use in dropdown lists
                        ViewBag.PriceList = new SelectList(priceList, "Id", "Price");
                        ViewBag.Errors = ex.Message;

                        ViewBag.MainId = MainProposalId;

                        ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                        ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();

                        // If the model state is not valid, redisplay the form with errors
                        // Populate dropdowns or other data needed for the view
                        return View(Model);
                    }
                }

                // Redirect to a third page of the form
                ProposalNotificationHub objNotifHub = new ProposalNotificationHub();
                ProposalNotification notification = new ProposalNotification();

                //notification.Details = "proposal";
                ////notification.RoshitaId = "";
                //notification.DetailsURL = "/ProposalMain/index";
                notification.ProposalMainId = mainId;
                notification.SentTo = "Proposal_Admin";
                notification.CreatedBy = User.Identity.Name;
                notification.CreatedDate = DateTime.Now;
                
                //notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                notification.Title = "An offer has been created";
                _dbContext.ProposalNotifications.Add(notification);
                await _dbContext.SaveChangesAsync();
                var notificationId = notification.Id;
    
                List<string> userIds = new List<string>();
                //var usersInProposalAdminRole = await _userManager.GetUsersInRoleAsync("Proposal_Admin");

                //foreach (var user in usersInProposalAdminRole)
                //{
                //    // Process each user as needed
                //}
                var role = await _roleManager.FindByNameAsync("Proposal_Admin");
               
                var usersInProposalAdminRole = new List<ApplicationUser>();
                if (role != null)
                {
                    var userIdsInRole = _dbContext.Set<IdentityUserRole>()
                           .Where(ur => ur.RoleId == role.Id )
                           .Select(ur => ur.UserId)
                           .ToList();

                    usersInProposalAdminRole = _userManager.Users
                        .Where(user => userIdsInRole.Contains(user.Id))
                        .ToList();

                    // Now 'usersInProposalAdminRole' contains users with the "Proposal_Admin" role.
                }
                foreach (var user in usersInProposalAdminRole)
                {
                    if (user.Id == User.Identity.GetUserId())
                        continue;
                    userIds.Add(user.Id);
                    
                }
                var proposalNotificationApplicationUserList = new List<ProposalNotificationApplicationUser>();
                foreach (var userId in userIds)
                {
                    var proposalNotificationApplicationUser = new ProposalNotificationApplicationUser
                    {
                        ProposalNotificationId = notificationId,
                        ApplicationUserId = userId
                    };
                    proposalNotificationApplicationUserList.Add(proposalNotificationApplicationUser);
                }
                _dbContext.ProposalNotificationApplicationUsers.AddRange(proposalNotificationApplicationUserList);

                await _dbContext.SaveChangesAsync();

                objNotifHub.SendProposalMessages();
                // Store a message in TempData
                TempData["SuccessMessage"] = "Proposal has been created successfully.";
                switch (BackOrForwardOrMain)
                {
                    case "back":
                        return RedirectToAction(nameof(ProposalStepThreeCreate), new { id = MainProposalId });

                    case "forward":
                        return RedirectToAction(nameof(Index));
                    case "main":
                        return RedirectToAction(nameof(Index), new { id = MainProposalId });
                    default:
                        break;
                }
                return RedirectToAction(nameof(Index));

                //return RedirectToAction(nameof(ProposalStepFourCreate), new { id = MainProposalId });

            }
            catch (Exception e)
            {
                var firstEntity = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId);
                if (firstEntity != null)
                    _dbContext.ProposalMains.Remove(firstEntity);
                return RedirectToAction(nameof(Index));
            }

        }


        // GET: ProposalMain/ProposalStepTwoEdit/1
        public ActionResult ProposalStepFourEdit()
        {
            int newId = Convert.ToInt32(Request.QueryString["newId"]);
            int prevId = Convert.ToInt32(Request.QueryString["prevId"]);
            // Retrieve data from temporary storage or session
            // ...



            ViewBag.MainId = newId;
            ViewBag.PrevId = prevId;
            ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
            var proposalMain = _dbContext.ProposalMains.Include("ProposalOutsideMedicalAuthorities").FirstOrDefault(p => p.Id == prevId);
            ViewBag.CatCount = proposalMain.CategoriesCount;
            var models = _dbContext.ProposalOutsideMedicalAuthorities.Include("PricesOutsideMedicalAuthority")
                .Where(p => p.ProposalMainId == proposalMain.Id).ToList();
            if (models == null)
            {

                var emptyModels = new List<ProposalStepTwoViewModel>(proposalMain.CategoriesCount);
                for (int i = 0; i < proposalMain.CategoriesCount; i++)
                {
                    // Create an instance of ProposalStepTwoViewModel and add it to the list
                    emptyModels.Add(new ProposalStepTwoViewModel());
                }
                return View(emptyModels);
            }

            //call the function to map model to viewModel
            var mappedModels = new List<ProposalOutsideMedicalAuthorityViewModel>();
            foreach (var entity in models)
            {
                mappedModels.Add(MapEntityToViewModelStepFour(entity));
            }
            var priceList = _dbContext.PricesOutsideMedicalAuthorities.ToList();
            // Convert the lists to SelectList items for use in dropdown lists
            ViewBag.PriceList = new SelectList(priceList, "Id", "Price");
            return View(mappedModels);
        }
        // POST: ProposalMain/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProposalStepFourEdit(List<ProposalOutsideMedicalAuthorityViewModel> Model, int id, int PrevId)
        {
            try
            {

                var secondproposal = _dbContext.ProposalStepTwos.Where(sp => sp.ProposalMainId == id).ToList();
                var secodProposalIndex = 0;
                foreach (var model1 in Model)
                {
                    int index = Model.IndexOf(model1);
                    #region binding checkBoxes
                    #region bind on to true
                    //model1.HospitalsResidenceServiceLimitOption = Request.Form["Model["+index+"].HospitalsResidenceServiceLimitOption"] == "on";
                    //model1.HospitalsResidenceServicePercentageoption = Request.Form["Model["+index+"].HospitalsResidenceServicePercentageoption"] == "on";

                    //model1.OutsideClinicsLimitOption = Request.Form["Model["+index+"].OutsideClinicsLimitOption"] == "on";
                    //model1.OutsideClinicsPercentageoption = Request.Form["Model["+index+"].OutsideClinicsPercentageoption"] == "on";

                    //model1.ExaminationAndAnalysisLimitOption = Request.Form["Model["+index+"].ExaminationAndAnalysisLimitOption"] == "on";
                    //model1.ExaminationAndAnalysisPercentageoption = Request.Form["Model["+index+"].ExaminationAndAnalysisPercentageoption"] == "on";

                    //model1.PhysicalTherapyLimitOption = Request.Form["Model["+index+"].PhysicalTherapyLimitOption"] == "on";
                    //model1.PhysicalTherapyPercentageoption = Request.Form["Model["+index+"].PhysicalTherapyPercentageoption"] == "on";

                    //model1.DailyTherapyLimitOption = Request.Form["Model["+index+"].DailyTherapyLimitOption"] == "on";
                    //model1.DailyTherapyPercentageoption = Request.Form["Model["+index+"].DailyTherapyPercentageoption"] == "on";

                    //model1.ChronicTherapyLimitOption = Request.Form["Model["+index+"].ChronicTherapyLimitOption"] == "on";
                    //model1.ChronicTherapyPercentageoption = Request.Form["Model["+index+"].ChronicTherapyPercentageoption"] == "on";

                    //model1.NatChildBirthLimitOption = Request.Form["Model["+index+"].NatChildBirthLimitOption"] == "on";
                    //model1.NatChildBirthPercentageoption = Request.Form["Model["+index+"].NatChildBirthPercentageoption"] == "on";

                    //model1.CaesChildBirthLimitOption = Request.Form["Model["+index+"].CaesChildBirthLimitOption"] == "on";
                    //model1.CaesChildBirthPercentageoption = Request.Form["Model["+index+"].CaesChildBirthPercentageoption"] == "on";

                    //model1.LegalAbortionLimitOption = Request.Form["Model["+index+"].LegalAbortionLimitOption"] == "on";
                    //model1.LegalAbortionPercentageoption = Request.Form["Model["+index+"].LegalAbortionPercentageoption"] == "on";

                    //model1.PregFollowUpLimitOption = Request.Form["Model["+index+"].PregFollowUpLimitOption"] == "on";
                    //model1.PregFollowUpPercentageoption = Request.Form["Model["+index+"].PregFollowUpPercentageoption"] == "on";

                    //model1.AdvancedDentalServiceLimitOption = Request.Form["Model["+index+"].AdvancedDentalServiceLimitOption"] == "on";
                    //model1.AdvancedDentalServicePercentageoption = Request.Form["Model["+index+"].AdvancedDentalServicePercentageoption"] == "on";

                    //model1.BasicDentalServiceLimitOption = Request.Form["Model["+index+"].BasicDentalServiceLimitOption"] == "on";
                    //model1.BasicDentalServicePercentageoption = Request.Form["Model["+index+"].BasicDentalServicePercentageoption"] == "on";

                    //model1.OpticsLimitOption = Request.Form["Model["+index+"].OpticsLimitOption"] == "on";
                    //model1.OpticsPercentageoption = Request.Form["Model["+index+"].OpticsPercentageoption"] == "on";
                    #endregion

                    var maxVal = secondproposal[secodProposalIndex].AnnualCoverageCeiling;
                    secodProposalIndex++;
                    #region setting limit to max val
                    if (!model1.HospitalsResidenceServiceLimitOption)
                    {
                        model1.HospitalsResidenceServiceLimit = maxVal;
                    }
                    if (!model1.OutsideClinicsLimitOption)
                    {
                        model1.OutsideClinicsLimit = maxVal;
                    }
                    if (!model1.ExaminationAndAnalysisLimitOption)
                    {
                        model1.ExaminationAndAnalysisLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.PhysicalTherapyLimitOption)
                    {
                        model1.PhysicalTherapyLimit = maxVal;
                    }
                    if (!model1.DailyTherapyLimitOption)
                    {
                        model1.DailyTherapyLimit = maxVal;
                    }
                    if (!model1.ChronicTherapyLimitOption)
                    {
                        model1.ChronicTherapyLimit = maxVal;
                    }
                    if (!model1.NatChildBirthLimitOption)
                    {
                        model1.NatChildBirthLimit = maxVal;
                    }
                    if (!model1.CaesChildBirthLimitOption)
                    {
                        model1.CaesChildBirthLimit = maxVal;
                    }
                    if (!model1.LegalAbortionLimitOption)
                    {
                        model1.LegalAbortionLimit = maxVal;
                    }
                    if (!model1.PregFollowUpLimitOption)
                    {
                        model1.PregFollowUpLimit = maxVal;
                    }
                    if (!model1.AdvancedDentalServiceLimitOption)
                    {
                        model1.AdvancedDentalServiceLimit = maxVal;
                    }
                    if (!model1.BasicDentalServiceLimitOption)
                    {
                        model1.BasicDentalServiceLimit = maxVal;
                    }
                    if (!model1.OpticsLimitOption)
                    {
                        model1.OpticsLimit = maxVal;
                    }
                    #endregion
                    #region CheckMax
                    // Use reflection to iterate through properties
                    var properties = model1.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        // Check if the property ends with "Limit"
                        if (property.Name.EndsWith("Limit"))
                        {
                            // Get the value of the property
                            var propertyValue = (decimal)property.GetValue(model1);

                            // Check if the value exceeds maxVal
                            if (propertyValue > maxVal)
                            {
                                ModelState.AddModelError($"Model[{Model.IndexOf(model1)}].{property.Name}", $"The value cannot exceed the maximum value of {maxVal}.");
                            }
                        }
                    }
                    #endregion
                    var maxPer = 100;
                    ///////percentage max //
                    ///
                    #region percentage limit
                    if (!model1.HospitalsResidenceServicePercentageoption)
                    {
                        model1.HospitalsResidenceServicePercentage = maxPer;
                    }
                    if (!model1.OutsideClinicsPercentageoption)
                    {
                        model1.OutsideClinicsPercentage = maxPer;
                    }
                    if (!model1.ExaminationAndAnalysisPercentageoption)
                    {
                        model1.ExaminationAndAnalysisPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.PhysicalTherapyPercentageoption)
                    {
                        model1.PhysicalTherapyPercentage = maxPer;
                    }
                    if (!model1.DailyTherapyPercentageoption)
                    {
                        model1.DailyTherapyPercentage = maxPer;
                    }
                    if (!model1.ChronicTherapyPercentageoption)
                    {
                        model1.ChronicTherapyPercentage = maxPer;
                    }
                    if (!model1.NatChildBirthPercentageoption)
                    {
                        model1.NatChildBirthPercentage = maxPer;
                    }
                    if (!model1.CaesChildBirthPercentageoption)
                    {
                        model1.CaesChildBirthPercentage = maxPer;
                    }
                    if (!model1.LegalAbortionPercentageoption)
                    {
                        model1.LegalAbortionPercentage = maxPer;
                    }
                    if (!model1.PregFollowUpPercentageoption)
                    {
                        model1.PregFollowUpPercentage = maxPer;
                    }
                    if (!model1.AdvancedDentalServicePercentageoption)
                    {
                        model1.AdvancedDentalServicePercentage = maxPer;
                    }
                    if (!model1.BasicDentalServicePercentageoption)
                    {
                        model1.BasicDentalServicePercentage = maxPer;
                    }
                    if (!model1.OpticsPercentageoption)
                    {
                        model1.OpticsPercentage = maxPer;
                    }
                    #endregion
                    #endregion
                    model1.ProposalMainId = (int)id;
                }
                var index1 = 0;
                foreach (var model in Model)
                {
                 
                    model.ProposalMainId = (int)id;
                    if (ModelState.IsValid)
                    {

                    
                        //var CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == MainProposalId).CategoriesCount;
                        // Map view model to entity and save to database
                        var proposalThird = MapViewModelToEntityStepFour(model);
                        proposalThird.ClassCode = index1 + 1;
                        index1 = index1 + 1;

                        // Save the entity to the database
                        _dbContext.ProposalOutsideMedicalAuthorities.Add(proposalThird);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {

                    

                        List<string> errors = new List<string>();

                        foreach (var value in ModelState.Values)
                        {
                            foreach (var error in value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                        var priceList = _dbContext.PricesOutsideMedicalAuthorities.ToList();
                        // Convert the lists to SelectList items for use in dropdown lists
                        ViewBag.PriceList = new SelectList(priceList, "Id", "Price");
                        ViewBag.Errors = errors;

                        ViewBag.MainId = id;

                        ViewBag.CatCount = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id).CategoriesCount;
                        ViewBag.previousUrl = System.Web.HttpContext.Current.Request.UrlReferrer?.ToString();
                        // If the model state is not valid, redisplay the form with errors
                        // Populate dropdowns or other data needed for the view
                        return View(Model);
                    }
                }
                // Redirect to a third page of the form
                // Redirect to a third page of the form
                ProposalNotificationHub objNotifHub = new ProposalNotificationHub();
                ProposalNotification notification = new ProposalNotification();

                //notification.Details = "proposal";
                ////notification.RoshitaId = "";
                //notification.DetailsURL = "/ProposalMain/index";
                notification.ProposalMainId = id;
                notification.SentTo = "Proposal_Admin";
                notification.CreatedBy = User.Identity.Name;
                notification.CreatedDate = DateTime.Now;

                //notification.DetailsURL = "/DoctorMedicinesLabsRaysApproval/index";
                notification.Title = "An offer has been created";
                _dbContext.ProposalNotifications.Add(notification);
                await _dbContext.SaveChangesAsync();
                objNotifHub.SendProposalMessages();
                TempData["SuccessMessage"] = "Proposal has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                var firstEntity = _dbContext.ProposalMains.FirstOrDefault(p => p.Id == id);
                if (firstEntity != null)
                    _dbContext.ProposalMains.Remove(firstEntity);
                return RedirectToAction(nameof(Index));
            }
        }
        private ProposalOutsideMedicalAuthority MapViewModelToEntityStepFour(ProposalOutsideMedicalAuthorityViewModel vM)
        {
            // Use a mapping library or manually map properties as needed
            // This is a simplified example, adjust as needed
            var proposalthree = new ProposalOutsideMedicalAuthority
            {
                HospitalsResidenceServiceLimitOption = vM.HospitalsResidenceServiceLimitOption,
                HospitalsResidenceServicePercentageoption = vM.HospitalsResidenceServicePercentageoption,
                HospitalsResidenceServiceLimit = vM.HospitalsResidenceServiceLimit,
                HospitalsResidenceServicePercentage = vM.HospitalsResidenceServicePercentage,
                OutsideClinicsLimitOption = vM.OutsideClinicsLimitOption,
                OutsideClinicsPercentageoption = vM.OutsideClinicsPercentageoption,
                OutsideClinicsLimit = vM.OutsideClinicsLimit,
                OutsideClinicsPercentage = vM.OutsideClinicsPercentage,
                ExaminationAndAnalysisLimitOption = vM.ExaminationAndAnalysisLimitOption,
                ExaminationAndAnalysisPercentageoption = vM.ExaminationAndAnalysisPercentageoption,
                ExaminationAndAnalysisLimit = vM.ExaminationAndAnalysisLimit,
                ExaminationAndAnalysisPercentage = vM.ExaminationAndAnalysisPercentage,
                PhysicalTherapyLimitOption = vM.PhysicalTherapyLimitOption,
                PhysicalTherapyPercentageoption = vM.PhysicalTherapyPercentageoption,
                PhysicalTherapyLimit = vM.PhysicalTherapyLimit,
                PhysicalTherapyPercentage = vM.PhysicalTherapyPercentage,
                DailyTherapyLimitOption = vM.DailyTherapyLimitOption,
                DailyTherapyPercentageoption = vM.DailyTherapyPercentageoption,
                DailyTherapyLimit = vM.DailyTherapyLimit,
                DailyTherapyPercentage = vM.DailyTherapyPercentage,
                ChronicTherapyLimitOption = vM.ChronicTherapyLimitOption,
                ChronicTherapyPercentageoption = vM.ChronicTherapyPercentageoption,
                ChronicTherapyLimit = vM.ChronicTherapyLimit,
                ChronicTherapyPercentage = vM.ChronicTherapyPercentage,
                NatChildBirthLimitOption = vM.NatChildBirthLimitOption,
                NatChildBirthPercentageoption = vM.NatChildBirthPercentageoption,
                NatChildBirthLimit = vM.NatChildBirthLimit,
                NatChildBirthPercentage = vM.NatChildBirthPercentage,
                CaesChildBirthLimitOption = vM.CaesChildBirthLimitOption,
                CaesChildBirthPercentageoption = vM.CaesChildBirthPercentageoption,
                CaesChildBirthLimit = vM.CaesChildBirthLimit,
                CaesChildBirthPercentage = vM.CaesChildBirthPercentage,
                LegalAbortionLimitOption = vM.LegalAbortionLimitOption,
                LegalAbortionPercentageoption = vM.LegalAbortionPercentageoption,
                LegalAbortionLimit = vM.LegalAbortionLimit,
                LegalAbortionPercentage = vM.LegalAbortionPercentage,
                PregFollowUpLimitOption = vM.PregFollowUpLimitOption,
                PregFollowUpPercentageoption = vM.PregFollowUpPercentageoption,
                PregFollowUpLimit = vM.PregFollowUpLimit,
                PregFollowUpPercentage = vM.PregFollowUpPercentage,
                AdvancedDentalServiceLimitOption = vM.AdvancedDentalServiceLimitOption,
                AdvancedDentalServicePercentageoption = vM.AdvancedDentalServicePercentageoption,
                AdvancedDentalServiceLimit = vM.AdvancedDentalServiceLimit,
                AdvancedDentalServicePercentage = vM.AdvancedDentalServicePercentage,
                BasicDentalServiceLimitOption = vM.BasicDentalServiceLimitOption,
                BasicDentalServicePercentageoption = vM.BasicDentalServicePercentageoption,
                BasicDentalServiceLimit = vM.BasicDentalServiceLimit,
                BasicDentalServicePercentage = vM.BasicDentalServicePercentage,
                OpticsLimitOption = vM.OpticsLimitOption,
                OpticsPercentageoption = vM.OpticsPercentageoption,
                OpticsLimit = vM.OpticsLimit,
                OpticsPercentage = vM.OpticsPercentage,
                IntensiveCareDaysCount = vM.IntensiveCareDaysCount,
                DailyRoshitasCountPerMonth = vM.DailyRoshitasCountPerMonth,
                CoronaVaccineCoverage = vM.CoronaVaccineCoverage,
                DoctorsExaminationValue = vM.DoctorsExaminationValue,
                //relations
                PricesOutsideMedicalAuthorityId = vM.PricesOutsideMedicalAuthorityId,

                ProposalMainId = vM.ProposalMainId


                // Map other properties as needed
            };

            return proposalthree;
        }
        private ProposalOutsideMedicalAuthorityViewModel MapEntityToViewModelStepFour(ProposalOutsideMedicalAuthority vM)
        {
            var proposalMain = new ProposalOutsideMedicalAuthorityViewModel
            {
                Id = vM.Id,
                HospitalsResidenceServiceLimitOption = vM.HospitalsResidenceServiceLimitOption,
                HospitalsResidenceServicePercentageoption = vM.HospitalsResidenceServicePercentageoption,
                HospitalsResidenceServiceLimit = vM.HospitalsResidenceServiceLimit,
                HospitalsResidenceServicePercentage = vM.HospitalsResidenceServicePercentage,
                OutsideClinicsLimitOption = vM.OutsideClinicsLimitOption,
                OutsideClinicsPercentageoption = vM.OutsideClinicsPercentageoption,
                OutsideClinicsLimit = vM.OutsideClinicsLimit,
                OutsideClinicsPercentage = vM.OutsideClinicsPercentage,
                ExaminationAndAnalysisLimitOption = vM.ExaminationAndAnalysisLimitOption,
                ExaminationAndAnalysisPercentageoption = vM.ExaminationAndAnalysisPercentageoption,
                ExaminationAndAnalysisLimit = vM.ExaminationAndAnalysisLimit,
                ExaminationAndAnalysisPercentage = vM.ExaminationAndAnalysisPercentage,
                PhysicalTherapyLimitOption = vM.PhysicalTherapyLimitOption,
                PhysicalTherapyPercentageoption = vM.PhysicalTherapyPercentageoption,
                PhysicalTherapyLimit = vM.PhysicalTherapyLimit,
                PhysicalTherapyPercentage = vM.PhysicalTherapyPercentage,
                DailyTherapyLimitOption = vM.DailyTherapyLimitOption,
                DailyTherapyPercentageoption = vM.DailyTherapyPercentageoption,
                DailyTherapyLimit = vM.DailyTherapyLimit,
                DailyTherapyPercentage = vM.DailyTherapyPercentage,
                ChronicTherapyLimitOption = vM.ChronicTherapyLimitOption,
                ChronicTherapyPercentageoption = vM.ChronicTherapyPercentageoption,
                ChronicTherapyLimit = vM.ChronicTherapyLimit,
                ChronicTherapyPercentage = vM.ChronicTherapyPercentage,
                NatChildBirthLimitOption = vM.NatChildBirthLimitOption,
                NatChildBirthPercentageoption = vM.NatChildBirthPercentageoption,
                NatChildBirthLimit = vM.NatChildBirthLimit,
                NatChildBirthPercentage = vM.NatChildBirthPercentage,
                CaesChildBirthLimitOption = vM.CaesChildBirthLimitOption,
                CaesChildBirthPercentageoption = vM.CaesChildBirthPercentageoption,
                CaesChildBirthLimit = vM.CaesChildBirthLimit,
                CaesChildBirthPercentage = vM.CaesChildBirthPercentage,
                LegalAbortionLimitOption = vM.LegalAbortionLimitOption,
                LegalAbortionPercentageoption = vM.LegalAbortionPercentageoption,
                LegalAbortionLimit = vM.LegalAbortionLimit,
                LegalAbortionPercentage = vM.LegalAbortionPercentage,
                PregFollowUpLimitOption = vM.PregFollowUpLimitOption,
                PregFollowUpPercentageoption = vM.PregFollowUpPercentageoption,
                PregFollowUpLimit = vM.PregFollowUpLimit,
                PregFollowUpPercentage = vM.PregFollowUpPercentage,
                AdvancedDentalServiceLimitOption = vM.AdvancedDentalServiceLimitOption,
                AdvancedDentalServicePercentageoption = vM.AdvancedDentalServicePercentageoption,
                AdvancedDentalServiceLimit = vM.AdvancedDentalServiceLimit,
                AdvancedDentalServicePercentage = vM.AdvancedDentalServicePercentage,
                BasicDentalServiceLimitOption = vM.BasicDentalServiceLimitOption,
                BasicDentalServicePercentageoption = vM.BasicDentalServicePercentageoption,
                BasicDentalServiceLimit = vM.BasicDentalServiceLimit,
                BasicDentalServicePercentage = vM.BasicDentalServicePercentage,
                OpticsLimitOption = vM.OpticsLimitOption,
                OpticsPercentageoption = vM.OpticsPercentageoption,
                OpticsLimit = vM.OpticsLimit,
                OpticsPercentage = vM.OpticsPercentage,
                IntensiveCareDaysCount = vM.IntensiveCareDaysCount,
                DailyRoshitasCountPerMonth = vM.DailyRoshitasCountPerMonth,
                CoronaVaccineCoverage = vM.CoronaVaccineCoverage,
                DoctorsExaminationValue = vM.DoctorsExaminationValue,
                //relations

                ProposalMainId = vM.ProposalMainId,
                PricesOutsideMedicalAuthorityId = vM.PricesOutsideMedicalAuthorityId,
                PricesOutsideMedicalAuthority = vM.PricesOutsideMedicalAuthority.Price


                // Map other properties as needed
            };
            return proposalMain;
        }
        private void MapViewModelToEntityModifyStepFour(ProposalOutsideMedicalAuthorityViewModel vM, ProposalOutsideMedicalAuthority existingProposal)
        {
            existingProposal.HospitalsResidenceServiceLimitOption = vM.HospitalsResidenceServiceLimitOption;
            existingProposal.HospitalsResidenceServicePercentageoption = vM.HospitalsResidenceServicePercentageoption;
            existingProposal.HospitalsResidenceServiceLimit = vM.HospitalsResidenceServiceLimit;
            existingProposal.HospitalsResidenceServicePercentage = vM.HospitalsResidenceServicePercentage;
            existingProposal.OutsideClinicsLimitOption = vM.OutsideClinicsLimitOption;
            existingProposal.OutsideClinicsPercentageoption = vM.OutsideClinicsPercentageoption;
            existingProposal.OutsideClinicsLimit = vM.OutsideClinicsLimit;
            existingProposal.OutsideClinicsPercentage = vM.OutsideClinicsPercentage;
            existingProposal.ExaminationAndAnalysisLimitOption = vM.ExaminationAndAnalysisLimitOption;
            existingProposal.ExaminationAndAnalysisPercentageoption = vM.ExaminationAndAnalysisPercentageoption;
            existingProposal.ExaminationAndAnalysisLimit = vM.ExaminationAndAnalysisLimit;
            existingProposal.ExaminationAndAnalysisPercentage = vM.ExaminationAndAnalysisPercentage;
            existingProposal.PhysicalTherapyLimitOption = vM.PhysicalTherapyLimitOption;
            existingProposal.PhysicalTherapyPercentageoption = vM.PhysicalTherapyPercentageoption;
            existingProposal.PhysicalTherapyLimit = vM.PhysicalTherapyLimit;
            existingProposal.PhysicalTherapyPercentage = vM.PhysicalTherapyPercentage;
            existingProposal.DailyTherapyLimitOption = vM.DailyTherapyLimitOption;
            existingProposal.DailyTherapyPercentageoption = vM.DailyTherapyPercentageoption;
            existingProposal.DailyTherapyLimit = vM.DailyTherapyLimit;
            existingProposal.DailyTherapyPercentage = vM.DailyTherapyPercentage;
            existingProposal.ChronicTherapyLimitOption = vM.ChronicTherapyLimitOption;
            existingProposal.ChronicTherapyPercentageoption = vM.ChronicTherapyPercentageoption;
            existingProposal.ChronicTherapyLimit = vM.ChronicTherapyLimit;
            existingProposal.ChronicTherapyPercentage = vM.ChronicTherapyPercentage;
            existingProposal.NatChildBirthLimitOption = vM.NatChildBirthLimitOption;
            existingProposal.NatChildBirthPercentageoption = vM.NatChildBirthPercentageoption;
            existingProposal.NatChildBirthLimit = vM.NatChildBirthLimit;
            existingProposal.NatChildBirthPercentage = vM.NatChildBirthPercentage;
            existingProposal.CaesChildBirthLimitOption = vM.CaesChildBirthLimitOption;
            existingProposal.CaesChildBirthPercentageoption = vM.CaesChildBirthPercentageoption;
            existingProposal.CaesChildBirthLimit = vM.CaesChildBirthLimit;
            existingProposal.CaesChildBirthPercentage = vM.CaesChildBirthPercentage;
            existingProposal.LegalAbortionLimitOption = vM.LegalAbortionLimitOption;
            existingProposal.LegalAbortionPercentageoption = vM.LegalAbortionPercentageoption;
            existingProposal.LegalAbortionLimit = vM.LegalAbortionLimit;
            existingProposal.LegalAbortionPercentage = vM.LegalAbortionPercentage;
            existingProposal.PregFollowUpLimitOption = vM.PregFollowUpLimitOption;
            existingProposal.PregFollowUpPercentageoption = vM.PregFollowUpPercentageoption;
            existingProposal.PregFollowUpLimit = vM.PregFollowUpLimit;
            existingProposal.PregFollowUpPercentage = vM.PregFollowUpPercentage;
            existingProposal.AdvancedDentalServiceLimitOption = vM.AdvancedDentalServiceLimitOption;
            existingProposal.AdvancedDentalServicePercentageoption = vM.AdvancedDentalServicePercentageoption;
            existingProposal.AdvancedDentalServiceLimit = vM.AdvancedDentalServiceLimit;
            existingProposal.AdvancedDentalServicePercentage = vM.AdvancedDentalServicePercentage;
            existingProposal.BasicDentalServiceLimitOption = vM.BasicDentalServiceLimitOption;
            existingProposal.BasicDentalServicePercentageoption = vM.BasicDentalServicePercentageoption;
            existingProposal.BasicDentalServiceLimit = vM.BasicDentalServiceLimit;
            existingProposal.BasicDentalServicePercentage = vM.BasicDentalServicePercentage;
            existingProposal.OpticsLimitOption = vM.OpticsLimitOption;
            existingProposal.OpticsPercentageoption = vM.OpticsPercentageoption;
            existingProposal.OpticsLimit = vM.OpticsLimit;
            existingProposal.OpticsPercentage = vM.OpticsPercentage;
            existingProposal.IntensiveCareDaysCount = vM.IntensiveCareDaysCount;
            existingProposal.DailyRoshitasCountPerMonth = vM.DailyRoshitasCountPerMonth;
            existingProposal.CoronaVaccineCoverage = vM.CoronaVaccineCoverage;
            existingProposal.DoctorsExaminationValue = vM.DoctorsExaminationValue;
            //relations
            existingProposal.PricesOutsideMedicalAuthorityId = vM.PricesOutsideMedicalAuthorityId;
            existingProposal.ProposalMainId = vM.ProposalMainId;

        }
        #endregion

        //#region Pricing
        //public async Task<ActionResult> PricingIndex(string searchValue = "", DateTime? startDate = null, DateTime? endDate = null)
        //{
        //    //startDate ??= new DateTime(2020, 1, 1);
        //    //endDate ??= new DateTime(2100, 12, 31);
        //    if (!startDate.HasValue)
        //    {
        //        // Set a default value for startDate (e.g., January 1, 2020)
        //        startDate = new DateTime(2020, 1, 1);
        //    }

        //    if (!endDate.HasValue)
        //    {
        //        // Set a default value for endDate (e.g., December 31, 2100)
        //        endDate = new DateTime(2100, 12, 31);
        //    }
        //    await DeleteProposalMainsWithEmptyEntities();
        //    if (string.IsNullOrEmpty(searchValue))
        //    {
        //        var proposals = _dbContext.ProposalMains
        //       .Include("Area")
        //       .Include("CompanyActivity")
        //       .Include("Broker")
        //       .Include("User")
        //       .Where(p => p.User.UserName != User.Identity.Name)
        //       .OrderBy(pr => pr.Code)
        //       .ToList();
        //        ViewBag.SearchValue = searchValue;
        //        var mappedProposals = MapToViewModels(proposals);
        //        return View(mappedProposals);
        //    }
        //    else
        //    {
        //        var allProposalMains = _dbContext.ProposalMains
        //        .Include("Broker")
        //        .Include("Area")
        //        .Include("CompanyActivity")
        //        .Include("User")
        //        .Where(b => (b.User.UserName != User.Identity.Name) &&
        //            (!startDate.HasValue || b.ContractDate >= startDate) &&
        //            (!endDate.HasValue || b.ContractDate <= endDate) &&
        //            (b.CompanyArabicName.ToLower().Contains(searchValue) ||
        //             b.CompanyEnglishName.ToLower().Contains(searchValue) ||
        //             b.Id.ToString().Contains(searchValue))
        //        )
        //       .OrderBy(pr => pr.Code)

        //        .ToList();
        //        var mappedAll = MapToViewModels(allProposalMains);
        //        ViewBag.SearchValue = searchValue;
        //        return View(mappedAll);

        //    }


        //}
        //public ActionResult PricingPage1(int proposalMainId)
        //{
        //    var proposalMain = _dbContext.ProposalMains.Include("Area").Include("Broker").Include("CompanyActivity").FirstOrDefault(p => p.Id == proposalMainId);
        //    var viewModel = new PricingAggregateViewModel { ProposalMain = proposalMain };

        //    return View(viewModel);
        //}
        //public ActionResult PricingPage2(int proposalMainId)
        //{
        //    string userId = User.Identity.GetUserId();
        //    var pricingRecord = _dbContext.Pricings.FirstOrDefault(p => (p.UserId == userId && p.ProposalMainId == proposalMainId));
        //    var proposalMain = _dbContext.ProposalMains.Find(proposalMainId);
        //    var newPrices = pricingRecord?.Prices
        //                   ?.Split(',')
        //                   .Select(Convert.ToDouble)
        //                   .ToList();

        //    if (newPrices == null || newPrices.Count == 0)
        //    {
        //        newPrices = Enumerable.Repeat(0.0, proposalMain.CategoriesCount).ToList();
        //    }
        //    var proposalStepTwo = _dbContext.ProposalStepTwos.Include("CardColor").Include("MedicalNetwork").Include("ResidenceDegree").Where(p => p.ProposalMainId == proposalMainId).ToList();


        //    var viewModel = new PricingAggregateViewModelPage2
        //    {
        //        ProposalMainId = proposalStepTwo.FirstOrDefault().ProposalMainId,
        //        ProposalStepTwoes = proposalStepTwo,
        //        Catcount = proposalMain.CategoriesCount,
        //        NewPrices = newPrices
        //    };

        //    return View(viewModel);
        //}
        //[HttpPost]
        //public ActionResult SaveNewPrices(PricingAggregateViewModelPage2 viewModel)
        //{

        //    // Access viewModel.NewPrices to get the entered new prices
        //    var prices = viewModel.NewPrices;
        //    if (prices.Count() != viewModel.Catcount)
        //        return RedirectToAction(nameof(PricingIndex));
        //    if(!ModelState.IsValid)
        //        return RedirectToAction("PricingPage2" , viewModel.ProposalMainId);

        //    string userId = User.Identity.GetUserId();
        //    var pricingRecord = _dbContext.Pricings.FirstOrDefault(
        //        p => (p.UserId == userId && p.ProposalMainId == viewModel.ProposalMainId));
        //    if (pricingRecord != null)
        //    {
        //        pricingRecord.Prices = string.Join(",", prices.Select(d => d.ToString()));
                
        //        _dbContext.SaveChanges();
        //        var userName = User.Identity.Name;
        //        var currentUserId = User.Identity.GetUserId();
        //        var proposalMainId = viewModel.ProposalMainId;
        //        var notificationEntry = _dbContext.ProposalNotifications.Where(pn => pn.ProposalMainId == proposalMainId).FirstOrDefault();
        //        var notificationRelationEntry = _dbContext.ProposalNotificationApplicationUsers
        //            .Where(n => n.ApplicationUserId == currentUserId
        //        && n.ProposalNotificationId == notificationEntry.Id).FirstOrDefault();
        //        _dbContext.ProposalNotificationApplicationUsers.Remove(notificationRelationEntry);
        //        _dbContext.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    // Save the new prices to your new table with userId and ProposalMainId
        //    var model = new Pricing
        //    {
        //        Prices = string.Join(",", prices.Select(d => d.ToString())),
        //        CategoriesCount = viewModel.Catcount,
        //        UserId = userId,
        //        ProposalMainId = viewModel.ProposalMainId
        //    };
        //    _dbContext.Pricings.Add(model);
        //    _dbContext.SaveChanges();
        //    var userName1 = User.Identity.Name;
        //    var currentUserId1 = User.Identity.GetUserId();
        //    var proposalMainId1 = viewModel.ProposalMainId;
        //    var notificationEntry1 = _dbContext.ProposalNotifications.Where(pn => pn.ProposalMainId == proposalMainId1).FirstOrDefault();
        //    var notificationRelationEntry1 = _dbContext.ProposalNotificationApplicationUsers
        //        .Where(n => n.ApplicationUserId == currentUserId1
        //    && n.ProposalNotificationId == notificationEntry1.Id).FirstOrDefault();
        //    _dbContext.ProposalNotificationApplicationUsers.Remove(notificationRelationEntry1);
        //    _dbContext.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //    // Redirect to the next page or return a view based on your logic
        //}
        //public ActionResult PricingPage3(int proposalMainId)
        //{
        //    var proposal3 = _dbContext.ProposalInsideMedicalAuthorities.Where(p => p.ProposalMainId == proposalMainId).ToList();
        //    var viewModel = new PricingAggregateViewModel { ProposalInsideMedicalAuthorities = proposal3 };
        //    return View(viewModel);
        //}
        //public ActionResult PricingPage4(int proposalMainId)
        //{
        //    var proposal4 = _dbContext.ProposalOutsideMedicalAuthorities.Include("PricesOutsideMedicalAuthority").Where(p => p.ProposalMainId == proposalMainId).ToList();
        //    var viewModel = new PricingAggregateViewModel { ProposalOutsideMedicalAuthorities = proposal4 };

        //    return View(viewModel);
        //}
        //#endregion

    }
}