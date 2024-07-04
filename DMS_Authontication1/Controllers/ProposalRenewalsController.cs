using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Helper;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;

namespace DMS_Authontication1.Controllers
{
    [Authorize]
    public class ProposalRenewalsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private DMS_TESTEntities db = new DMS_TESTEntities();
        DBApproval dbOra = new DBApproval();


        public ProposalRenewalsController()
        {
            _dbContext = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_dbContext));


        }
        #region MainPage        
        public async Task<ActionResult> Show()
        {           
            //if (!startDate.HasValue)
            //    startDate = new DateTime(2020, 1, 1);
            //if (!endDate.HasValue)
            //    endDate = new DateTime(2040, 12, 31);
            //var renewals = db.RenewalMains.ToList();
            var renewals = db.RenewalMains.OrderByDescending(r => r.Id).ToList();


            return View(renewals);
        }

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
            int maxContract = int.Parse(dbOra.RunReader(@"SELECT NVL(MAX(CONTRACT_NO), 0) FROM DMS_TEST.CONTRACT_DATA WHERE C_COMP_ID = '" + CompId + "'").Rows[0][0].ToString());

            int countClass = int.Parse(dbOra.RunReader(@"SELECT NVL(COUNT (DISTINCT CLASS_CODE), 0) FROM DMS_TEST.COMP_CONTRACT_CLASS WHERE C_COMP_ID = '" + CompId +"' AND CONTRACT_NO = '" + maxContract +"'").Rows[0][0].ToString());

            int countEmp = int.Parse(dbOra.RunReader(@"SELECT NVL(COUNT (DISTINCT CARD_ID),0) FROM DMS_TEST.COMP_EMPLOYEES WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + maxContract + "' AND NVL(TERMINATE_FLAG, 'N') != 'Y'").Rows[0][0].ToString());
            
            //int maxContract = db.Contract_Data
            //        .Where(c => c.C_COMP_ID == CompId)
            //        .Max(c => c.CONTRACT_NO);

            //int countClass = db.CompContractClasses
            //        .Where(c => c.C_COMP_ID == CompId)
            //         .Select(c => c.CLASS_CODE).Distinct().Count();

            //int countEmp = db.Comp_Employees
            //        .Where(c => c.C_COMP_ID == CompId && c.CONTRACT_NO == maxContract && (c.TERMINATE_FLAG ?? "N") != "Y")
            //         .Select(c => c.CARD_ID).Distinct().Count();

            return new JsonResult { Data = new { maxContract, countClass, countEmp }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveData(RenewalMain model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = User.Identity.GetUserId();

                    var renewalMain = new RenewalMain
                    {
                        CompId = model.CompId,
                        ContractNo = model.ContractNo,
                        ClassCount = model.ClassCount,
                        EmpCount = model.EmpCount,
                        UserId = currentUserId,
                        CreatedDate = DateTime.Now.Date
                    };

                    db.RenewalMains.Add(renewalMain);
                    db.SaveChanges();

                    int id = renewalMain.Id;

                    renewalMain.Code = id.ToString();

                    db.SaveChanges();

                    //return RedirectToAction("RenewalStepTwoShow", new { mainId = id, model.CompId, ContractNo = model.ContractNo, countCat = model.ClassCount, typeAction = 1, mainIdOld = id });
                    return RedirectToAction("RenewalBasicData", new { mainId = id, model.CompId, ContractNo = model.ContractNo, countCat = model.ClassCount, typeAction = 1, mainIdOld = id });

                    
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");                
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveData2(RenewalMain model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = User.Identity.GetUserId();

                    var renewalMain = new RenewalMain
                    {
                        CompId = model.CompId,
                        ContractNo = model.ContractNo,
                        ClassCount = model.ClassCount,
                        EmpCount = model.EmpCount,
                        UserId = currentUserId,
                        CreatedDate = DateTime.Now.Date
                    };

                    db.RenewalMains.Add(renewalMain);
                    //db.RenewalMains.Add(renewalMain);
                    db.SaveChanges();

                    string cod = model.Code;

                    //string cod = db.RenewalStepTwoes.Where(r => r.Code.StartsWith("10_")).ToList();

                    int ind = cod.IndexOf('_');
                    string subCod;
                    string OldCod;
                    //if (ind == -1)
                    //    cod = cod + "_" + "1";
                    //else
                    //{
                    //    string subCod = cod.Substring(0, ind + 1);
                    //    string maxNumber = db.RenewalMains
                    //         .Where(r => r.Code.StartsWith(subCod))
                    //    .Select(r => r.Code)
                    //    .ToList()
                    //    .Select(code => int.Parse(code.Split('_').LastOrDefault()))
                    //    .Max()
                    //    .ToString();

                    //    cod = subCod + (int.Parse(maxNumber.ToString()) + 1).ToString();
                    //}

                    if (ind == -1)
                    {
                        subCod = cod + "_";
                        OldCod = cod;
                    }
                    else
                    {
                        subCod = cod.Substring(0, ind + 1);
                        OldCod = cod.Substring(0, ind);
                    }
                        

                    string maxNumber = db.RenewalMains
                    .Where(r => r.Code.StartsWith(subCod))
                    .Select(r => r.Code)
                    .ToList()
                    .Select(code => code.Split('_').LastOrDefault())
                    .Where(code => code != null)  // Filter out null values
                    .Select(code => int.Parse(code))
                    .DefaultIfEmpty(0)  // Default to 1 if there are no valid integers
                    .Max()
                    .ToString();

                    cod = subCod + (int.Parse(maxNumber.ToString()) + 1).ToString();


                    int id = renewalMain.Id;



                    renewalMain.Code = cod;

                    db.SaveChanges();

                  //  return RedirectToAction("RenewalStepTwoShow", new { mainId = id, model.CompId, ContractNo = model.ContractNo, countCat = model.ClassCount, typeAction = 2, mainIdOld = model.Id });
                    return RedirectToAction("RenewalBasicData", new { mainId = id, model.CompId, ContractNo = model.ContractNo, countCat = model.ClassCount, typeAction = 2, mainIdOld = OldCod });


                    //return View("Index");
                    //return RedirectToAction(nameof(ProposalStepFourCreate));
                }
                else
                    return RedirectToAction("Show");
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
                //return View("Index");
                //return RedirectToAction("Index");
            }

        }
        #endregion
        #region RenewalBasicData
        public ActionResult RenewalBasicData(int mainId, int CompId, int ContractNo, int countCat, int typeAction, int mainIdOld)
        {

            //ViewBag.CompId = CompId;
            //ViewBag.ContractNo = ContractNo;
            //ViewBag.CountClass = countCat;
            //ViewBag.typAction = typeAction;
            //ViewBag.MainId = mainIdOld;


            var polService = db.PoolServices.ToList();
            var brokerr = _dbContext.Brokers.ToList();           

            ViewBag.PolServ = new SelectList(polService, "Id", "ServiceName");
            ViewBag.Brokerss = new SelectList(brokerr, "Id", "Name");

            var model2 = TempData["RenewalBasicDataModell"] as RenwalBasicDataViewModel;

            if (model2 != null)
            {
                ViewBag.Errors = TempData["ErrorMessage"];
                return View(model2);
            }

            var polMed = dbOra.RunReader(@"SELECT NVL(P.AMOUNT, 0), P.AMOUNT_TYPE
                                           FROM APP.POLL_DATA P, APP.POLL_DATA_SERVICE S
                                           WHERE P.POLL_CODE = S.POLL_CODE AND P.COMP_ID = '" + CompId + "' AND P.CONTRACT_NO = '" + ContractNo + "' AND S.SERV_CODE IN('11601', '11602', '11603')");

            var polAll = dbOra.RunReader(@"SELECT NVL(P.AMOUNT, 0), P.AMOUNT_TYPE
                                           FROM APP.POLL_DATA P, APP.POLL_DATA_SERVICE S
                                           WHERE P.POLL_CODE = S.POLL_CODE AND P.COMP_ID = '" + CompId + "' AND P.CONTRACT_NO = '" + ContractNo + "' AND S.SERV_CODE NOT IN ('11601', '11602', '11603')");


            var lossR = dbOra.RunReader(@"SELECT GROSS, EXPECTED_OF_CONTRACT, NET_PREMIUM, (OVER_HEAD / 100)  OVER_HEAD 
                                           FROM APP.LR_CONSUM_FINAL 
                                           WHERE COMP_ID = '" + CompId + "' ORDER BY RN");
            
            var stopLos = dbOra.RunReader(@"SELECT NVL(STOP_LOSS, 0) STOP_LOSS
                                            FROM DMS_TEST.STOP_SEQ_DATA_D
                                            WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo  + "'");

            var model = new RenwalBasicDataViewModel();

            model.CompId = CompId;
            model.ContractNo = ContractNo;
            model.CountClass = countCat;
            model.typAction = typeAction;
            model.MainId = mainIdOld;

            if (polMed != null && polMed.Rows.Count > 0)
                model.ValueMedicationOld = Convert.ToDouble(polMed.Rows[0][0].ToString());

            if (polAll != null && polAll.Rows.Count > 0)
                model.ValueAllOld = Convert.ToDouble(polAll.Rows[0][0].ToString());


            if (lossR != null && lossR.Rows.Count > 0)
            {
                if (lossR.Rows.Count == 1)
                {
                    model.ExpectedLossRatioOld = Convert.ToDouble((Convert.ToDouble(lossR.Rows[0]["EXPECTED_OF_CONTRACT"].ToString()) / Convert.ToDouble(lossR.Rows[0]["NET_PREMIUM"].ToString()) + Convert.ToDouble(lossR.Rows[0]["OVER_HEAD"].ToString())) * 100);
                    model.LossRatioOld = 0;
                }
                else
                {
                    model.ExpectedLossRatioOld = Convert.ToDouble((Convert.ToDouble(lossR.Rows[0]["EXPECTED_OF_CONTRACT"].ToString()) / Convert.ToDouble(lossR.Rows[0]["NET_PREMIUM"].ToString()) + Convert.ToDouble(lossR.Rows[0]["OVER_HEAD"].ToString())) * 100);
                    model.LossRatioOld = Convert.ToDouble((Convert.ToDouble(lossR.Rows[1]["GROSS"].ToString()) / Convert.ToDouble(lossR.Rows[1]["NET_PREMIUM"].ToString()) + Convert.ToDouble(lossR.Rows[1]["OVER_HEAD"].ToString())) * 100); 
                }
            }
            if (stopLos != null && stopLos.Rows.Count > 0)
                model.StopLossOld = Convert.ToDouble(stopLos.Rows[0][0].ToString());

            //model.poolService = polService.Select(s => s.Id).ToList();

            RenewalBasicData oldNew = db.RenewalBasicDatas.FirstOrDefault(r => r.MainId == mainIdOld);

            if (typeAction == 2 && oldNew != null)
            {
                model.ValuePool = oldNew.ValuePool.Value;
                model.PercentPool = oldNew.PercentPool.Value;
                model.TypeCovarge = oldNew.TypeCovarge.Value;
                model.StopLoss = oldNew.StopLoss.Value;
                model.VisitorValue = oldNew.VisitorValue.Value;
                model.VisitorNumber = oldNew.VisitorNumber.Value;
                model.VisitorType = oldNew.VisitorType.Value;
                model.IsBroker = oldNew.IsBroker.Value;
                if (model.IsBroker == true)
                {
                    model.BrokerId = oldNew.BrokerId.Value;
                    model.BrokerPercentage = oldNew.BrokerPercentage.Value;
                }
                model.IssuanceExpenses = oldNew.IssuanceExpenses.Value;
                model.AdminExpenses = oldNew.AdminExpenses.Value;
                model.MainId = oldNew.MainId.Value;
                model.IsMedication = oldNew.IsMedication.Value;
                model.IsInpatient = oldNew.IsInpatient.Value;
                model.IsLab = oldNew.IsLab.Value;
                model.Notes = oldNew.Notes;

                model.poolService = db.RenewalServicePools
                                   .Where(x => x.MainId == oldNew.Id)
                                   .Select(x => x.ServiceId ?? 0)
                                   .ToList();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveDataBasicData(RenwalBasicDataViewModel model)
        {
            try
            {
                if (model.IsBroker == false)
                {
                    model.BrokerId = null;
                    model.BrokerPercentage = null;
                }
                if (ModelState.IsValid)
                {
                    var renewalBasicData = new RenewalBasicData
                    {
                        ValuePool = model.ValuePool,
                        PercentPool = model.PercentPool,
                        TypeCovarge = model.TypeCovarge,
                        StopLoss = model.StopLoss,
                        VisitorValue = model.VisitorValue,
                        VisitorNumber = model.VisitorNumber,
                        VisitorType = model.VisitorType,
                        IsBroker = model.IsBroker,
                        BrokerId = model.BrokerId,
                        BrokerPercentage = model.BrokerPercentage,
                        IssuanceExpenses = model.IssuanceExpenses,
                        AdminExpenses = model.AdminExpenses,
                        MainId = model.MainId,
                        IsMedication = model.IsMedication,
                        IsInpatient = model.IsInpatient,
                        IsLab = model.IsLab,
                        Notes = model.Notes

                    };


                    if (model.poolService != null)
                    {
                        renewalBasicData.RenewalServicePools = new List<RenewalServicePool>();
                        foreach (var serviceId in model.poolService)
                        {
                            renewalBasicData.RenewalServicePools.Add(new RenewalServicePool
                            {
                                ServiceId = serviceId
                            });
                        }
                    }

                    db.RenewalBasicDatas.Add(renewalBasicData);
                    db.SaveChanges();

                    //if (model.poolService != null)
                    //{
                    //    foreach (var serviceId in model.poolService)
                    //    {
                    //        var renewalServicePool = new RenewalServicePool
                    //        {
                    //            MainId = renewalBasicData.Id,
                    //            ServiceId = serviceId
                    //        };
                    //        db.RenewalServicePools.Add(renewalServicePool);
                    //    }
                    //    db.SaveChanges();
                    //}



                        return RedirectToAction("RenewalStepTwoShow", new { mainId = model.MainId, model.CompId, ContractNo = model.ContractNo, countCat = model.CountClass, typeAction = model.typAction, mainIdOld = model.MainIdOld });

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

                    TempData["RenewalBasicDataModell"] = model;
                    TempData["ErrorMessage"] = errors;

                    return RedirectToAction("RenewalBasicData", new { mainId = model.MainId, model.CompId, ContractNo = model.ContractNo, countCat = model.CountClass, typeAction = model.typAction, mainIdOld = model.MainIdOld });

                    //return RedirectToAction("RenewalStepTwoShow", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
                }
            }
            catch (Exception e)
            {
                TempData["RenewalBasicDataModell"] = model;
                TempData["ErrorMessage"] = e.Message;

                return RedirectToAction("RenewalBasicData", new { mainId = model.MainId, model.CompId, ContractNo = model.ContractNo, countCat = model.CountClass, typeAction = model.typAction, mainIdOld = model.MainIdOld });

            }
        }

        #endregion

        #region ProposalStepTwo
        public ActionResult RenewalStepTwoShow(int mainId, int CompId, int ContractNo, int countCat, int typeAction, int mainIdOld)
        {
            var colors = _dbContext.CardColors.ToList();
            var residenceDegree = _dbContext.ResidenceDegrees.ToList();
            var medicalNetworks = _dbContext.MedicalNetworks.ToList();

            ViewBag.ColorList = new SelectList(colors, "Id", "Name");
            ViewBag.ResidenceList = new SelectList(residenceDegree, "Id", "Name");
            ViewBag.MedicalNetworkList = new SelectList(medicalNetworks, "Id", "Name");

            ViewBag.MainId = mainId;
            ViewBag.CatCount = countCat;

            var model2 = TempData["RenewalStepTwoModel"] as List<RenwalStepTwoViewModel>;

            if (model2 != null)
            {
                ViewBag.Errors = TempData["ErrorMessage"];
                return View(model2);
            }

            var model = new List<RenwalStepTwoViewModel>(countCat);

            var compclassold = dbOra.RunReader(@"SELECT C_COMP_ID, CONTRACT_NO, CLASS_CODE, MAX_AMOUNT, NVL(ANNUAL_PREM, 0) ANNUAL_PREM, HOSPITAL_DEGREE, COVER_RELATION,
                                                        DECODE(CLASS_CODE, '1', 1, '2', 2, '11', 3, '12', 4, '33', 5, '44', 6, '6', 7, '99', 8, '2F', 9, '3F', 10, '4F', 11, '5F', 12, CLASS_CODE)
                                                 FROM DMS_TEST.COMP_CONTRACT_CLASS
                                                 WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' ORDER BY DECODE(CLASS_CODE, '1', 1, '2', 2, '11', 3, '12', 4, '33', 5, '44', 6, '6', 7, '99', 8, '2F', 9, '3F', 10, '4F', 11, '5F', 12, CLASS_CODE)");

            foreach (System.Data.DataRow row in compclassold.Rows)
            {
                var classCode = row["CLASS_CODE"].ToString();

                int countEmpClass = db.Comp_Employees
                    .Where(c => c.C_COMP_ID == CompId && c.CONTRACT_NO == ContractNo && c.CLASS_CODE == classCode && (c.TERMINATE_FLAG ?? "N") != "Y")
                     .Select(c => c.CARD_ID).Distinct().Count();


                var colrCardTable = dbOra.RunReader(@"SELECT DECODE(CARD_COLOR, 7402, 1, 7403, 2, 7405, 3, 7407, 4) CARD_COLOR
                                                 FROM APP.PRINT_CARD
                                                 WHERE COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' AND CLASS_CODE = '" + classCode + "' ORDER BY CLASS_CODE");
                int colrCrd = 1;
                if (colrCardTable.Rows.Count > 0)
                    colrCrd = int.Parse(colrCardTable.Rows[0][0].ToString());


                decimal mxAmount = Convert.ToDecimal(row["MAX_AMOUNT"].ToString());
                decimal pric = Convert.ToDecimal(row["ANNUAL_PREM"].ToString());
                int resDegree = int.Parse(row["HOSPITAL_DEGREE"].ToString());
                int medl = int.Parse(row["COVER_RELATION"].ToString());

                RenewalStepTwo oldNew = db.RenewalStepTwoes.FirstOrDefault(r => r.MainId == mainIdOld && r.ClassCode == classCode);

                if (typeAction == 2 && oldNew != null)
                {

                    var renwalTwoMain = new RenwalStepTwoViewModel
                    {
                        AnnualCoverageCeiling = oldNew.AnnualCoverageCeiling,
                        ParticipantsCount = oldNew.ParticipantsCount,
                        Price = oldNew.Price,

                        ChecksInsideHospital = MapApprovalStepTwoReves(oldNew.ChecksInsideHospital),
                        PhysicalTherapyInsideHospital = MapApprovalStepTwoReves(oldNew.PhysicalTherapyInsideHospital),
                        OutsideClinicInsideHospital = MapApprovalStepTwoReves(oldNew.OutsideClincInsideHospital),
                        DentalServicesInsideHospital = MapApprovalStepTwoReves(oldNew.DentalServicesInsideHospital),

                        Death = oldNew.Death,
                        Accidents = oldNew.Accidents,
                        //relations
                        CardColorId = oldNew.CardColorId,
                        ResidenceDegreeId = oldNew.ResidenceDegreeId,
                        MedicalNetworkId = oldNew.MedicalNetworkId,

                        MainId = mainId,

                        ////////////////////
                        ///
                        
                        
                        AnnualCoverageCeilingOld = mxAmount,
                        ParticipantsCountOld = countEmpClass,
                        PriceOld = pric,
                        DeathOld = mxAmount,
                        AccidentsOld = mxAmount,
                        ChecksInsideHospitalOld = ApprovalStepTwo.PriorApproval,
                        PhysicalTherapyInsideHospitalOld = ApprovalStepTwo.PriorApproval,
                        OutsideClinicInsideHospitalOld = ApprovalStepTwo.PriorApproval,
                        DentalServicesInsideHospitalOld = ApprovalStepTwo.PriorApproval,
                        CardColorIdOld = colrCrd,
                        ResidenceDegreeIdOld = resDegree,
                        MedicalNetworkIdOld = medl,

                        ClassCode = classCode,
                        CompId = CompId,
                        ContractNo = ContractNo,
                        CountClass = countCat,
                        typAction = typeAction,
                        MainIdOld = mainIdOld, 
                        Notes = oldNew.Notes,
                        PricePercent = (int)oldNew.PricePercent,
                        BirthNumber = (int)oldNew.BirthNumber,
                        BirthPercent = (int)oldNew.BirthPercent,
                        OpticalNumber = (int)oldNew.OpticalNumber,
                        OpticalPercent = (int)oldNew.OpticalPercent,
                        DentalNumber = (int)oldNew.DentalNumber,
                        DentalPercent = (int)oldNew.DentalPercent
                    };

                    model.Add(renwalTwoMain);
                }
                else
                {
                    var renwalTwoMain = new RenwalStepTwoViewModel
                    {
                        AnnualCoverageCeiling = mxAmount,
                        ParticipantsCount = countEmpClass,
                        Price = pric,

                        ChecksInsideHospital = ApprovalStepTwo.Yes,
                        PhysicalTherapyInsideHospital = ApprovalStepTwo.Yes,
                        OutsideClinicInsideHospital = ApprovalStepTwo.Yes,
                        DentalServicesInsideHospital = ApprovalStepTwo.Yes,

                        Death = mxAmount,
                        Accidents = mxAmount,
                        //relations
                        CardColorId = colrCrd,
                        ResidenceDegreeId = resDegree,
                        MedicalNetworkId = medl,

                        MainId = mainId,

                        AnnualCoverageCeilingOld = mxAmount,
                        ParticipantsCountOld = countEmpClass,
                        PriceOld = pric,
                        DeathOld = mxAmount,
                        AccidentsOld = mxAmount,
                        ChecksInsideHospitalOld = ApprovalStepTwo.Yes,
                        PhysicalTherapyInsideHospitalOld = ApprovalStepTwo.Yes,
                        OutsideClinicInsideHospitalOld = ApprovalStepTwo.Yes,
                        DentalServicesInsideHospitalOld = ApprovalStepTwo.Yes,
                        CardColorIdOld = colrCrd,
                        ResidenceDegreeIdOld = resDegree,
                        MedicalNetworkIdOld = medl,

                        ClassCode = classCode,
                        CompId = CompId,
                        ContractNo = ContractNo,
                        CountClass = countCat,
                        typAction = typeAction,
                        MainIdOld = mainIdOld
                    };

                    model.Add(renwalTwoMain);
                }
            }
            return View(model);
        }
        private ApprovalStepTwo MapApprovalStepTwoReves(int stat)
        {
            switch (stat)
            {
                case 0:
                    return ApprovalStepTwo.PriorApproval;
                case 1:
                    return ApprovalStepTwo.Yes;
                case 2:
                    return ApprovalStepTwo.No;
                default:
                    return ApprovalStepTwo.PriorApproval;
            }
        }

        private int MapApprovalStepTwo(ApprovalStepTwo stat)
        {
            switch (stat)
            {
                case ApprovalStepTwo.PriorApproval:
                    return 0;
                case ApprovalStepTwo.Yes:
                    return 1;
                case ApprovalStepTwo.No:
                    return 2;
                default:
                    return 0;
            }
        }
        private RenewalStepTwo MapViewModelToEntityStepTwo(RenwalStepTwoViewModel vM)
        {
            // Use a mapping library or manually map properties as needed
            // This is a simplified example, adjust as needed
            var renewaltwo = new RenewalStepTwo
            {
                AnnualCoverageCeiling = vM.AnnualCoverageCeiling,
                ParticipantsCount = vM.ParticipantsCount,
                Price = vM.Price,
                MinAge = 0,
                MaxAge = 0,
                AvgAge = 0,
                ChecksInsideHospital = MapApprovalStepTwo(vM.ChecksInsideHospital),
                PhysicalTherapyInsideHospital = MapApprovalStepTwo(vM.PhysicalTherapyInsideHospital),
                OutsideClincInsideHospital = MapApprovalStepTwo(vM.OutsideClinicInsideHospital),
                DentalServicesInsideHospital = MapApprovalStepTwo(vM.DentalServicesInsideHospital),
                //EmployeesDataFileName = vM.EmployeesDataFileName,
                Accidents = vM.Accidents,
                Death = vM.Death,
                //relations
                CardColorId = vM.CardColorId,
                ResidenceDegreeId = vM.ResidenceDegreeId,
                MedicalNetworkId = vM.MedicalNetworkId,
                MainId = vM.MainId,
                ClassCode = vM.ClassCode,
                PricePercent = vM.PricePercent,
                BirthNumber = vM.BirthNumber,
                BirthPercent = vM.BirthPercent,
                OpticalNumber = vM.OpticalNumber,
                OpticalPercent = vM.OpticalPercent,
                DentalNumber = vM.DentalNumber,
                DentalPercent = vM.DentalPercent,
                Notes = vM.Notes
                // Map other properties as needed
            };

            return renewaltwo;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveDataStepTwo(List<RenwalStepTwoViewModel> model)
        {
            try
            {
                foreach (var mod in model)
                {
                    if (ModelState.IsValid)
                    {
                        var renewalSecond = MapViewModelToEntityStepTwo(mod);

                        db.RenewalStepTwoes.Add(renewalSecond);
                        db.SaveChanges();
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

                        ViewBag.ColorList = new SelectList(colors, "Id", "Name");
                        ViewBag.ResidenceList = new SelectList(residenceDegree, "Id", "Name");
                        ViewBag.MedicalNetworkList = new SelectList(medicalNetworks, "Id", "Name");

                        ViewBag.MainId = mod.MainId;
                        ViewBag.CatCount = mod.CountClass;
                      
                        TempData["ErrorMessage"] = errors;
                        TempData["RenewalStepTwoModel"] = model;
                       
                        return RedirectToAction("RenewalStepTwoShow", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
                        //return View(model);
                    }
                }
                return RedirectToAction("RenewalStepThree", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
            }
            catch (Exception e)
            {
                TempData["RenewalStepTwoModel"] = model;
                TempData["ErrorMessage"] = e.Message;
                //ViewBag.ErrorMessage = e.Message;
                return RedirectToAction("RenewalStepTwoShow", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
            }
        }
        #endregion
        #region ProposalStepThree
        public ActionResult RenewalStepThree(int mainId, int CompId, int ContractNo, int countCat, int typeAction, int mainIdOld)
        {
            ViewBag.MainId = mainId;
            ViewBag.CatCount = countCat;
            ViewBag.basicService = db.BasicDentalServices.ToList();
            ViewBag.advancService = db.DentalServices.ToList();

            var model2 = TempData["RenewalStepThreeModel"] as List<RenewalInsideMedicalAuthorityViewModel>;

            if (model2 != null)
            {
                ViewBag.Errors = TempData["ErrorMessage"];
                return View(model2);
            }

            var model = new List<RenewalInsideMedicalAuthorityViewModel>(countCat);

            var compclassold = dbOra.RunReader(@"SELECT C_COMP_ID, CONTRACT_NO, CLASS_CODE, MAX_AMOUNT, NVL(ANNUAL_PREM, 0) ANNUAL_PREM, HOSPITAL_DEGREE, COVER_RELATION,
                                                        DECODE(CLASS_CODE, '1', 1, '2', 2, '11', 3, '12', 4, '33', 5, '44', 6, '6', 7, '99', 8, '2F', 9, '3F', 10, '4F', 11, '5F', 12, CLASS_CODE)
                                                 FROM DMS_TEST.COMP_CONTRACT_CLASS
                                                 WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' ORDER BY DECODE(CLASS_CODE, '1', 1, '2', 2, '11', 3, '12', 4, '33', 5, '44', 6, '6', 7, '99', 8, '2F', 9, '3F', 10, '4F', 11, '5F', 12, CLASS_CODE)");

            foreach (System.Data.DataRow row in compclassold.Rows)
            {
                var classCode = row["CLASS_CODE"].ToString();
                decimal maxAmount = decimal.Parse(row["MAX_AMOUNT"].ToString());
                
                #region Get old data
                var servCode = dbOra.RunReader(@"SELECT DISTINCT D_SERV_CODE, D_SERV_CODE SER_SERV, CEILING_AMT, CEILING_PERT, NVL(CORONA, 'N') CORONA
                                                 FROM DMS_TEST.COMP_CUSTOMIZED_D                                                                  
                                                 WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' AND CLASS_CODE = '" + classCode + "'  "
                                     + "           UNION ALL "
                                     + "             SELECT DISTINCT D_SERV_CODE, SER_SERV, CEILING_AMT, CEILING_PERT, NVL(CORONA, 'N') CORONA "
                                     + "             FROM DMS_TEST.COMP_CUSTOMIZED_D_D "
                                     + "           WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' AND CLASS_CODE = '" + classCode + "' "
                                     + "         ORDER BY D_SERV_CODE");


                decimal inpAm = servCode.AsEnumerable()
                              .Any(r => r.Field<string>("SER_SERV") == "111")
                              ? servCode.AsEnumerable()
                                       .First(r => r.Field<string>("SER_SERV") == "111")
                                       .Field<decimal?>("CEILING_AMT") ?? maxAmount
                              : 0;
                decimal inpPer = servCode.AsEnumerable()
                              .Any(r => r.Field<string>("SER_SERV") == "111")
                              ? servCode.AsEnumerable()
                                       .First(r => r.Field<string>("SER_SERV") == "111")
                                       .Field<decimal?>("CEILING_PERT") ?? 100
                              : 0;
                bool Isinp = inpPer == 0 ? false : true;
                ///outp
                decimal oputAm = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "112")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "112")
                                      .Field<decimal?>("CEILING_AMT") ?? maxAmount
                             : 0;
                decimal oputPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "112")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "112")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool IsOut = oputPer == 0 ? false : true;
                ///lab ray
                decimal labRayAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11206")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11206")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal labRayPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11206")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11206")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool IslabRay = labRayPer == 0 ? false : true;
                ///Phys
                decimal phyAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11204")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11204")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal phyPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11204")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11204")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isphy = phyPer == 0 ? false : true;
                ///daily
                decimal dailyAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11601")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11601")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal dailyPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11601")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11601")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isdaily = dailyPer == 0 ? false : true;

                ///chron
                decimal chronAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11602")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11602")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal chronPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11602")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11602")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Ischron = chronPer == 0 ? false : true;
                ///norma
                decimal normaAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11502")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11502")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal normaPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11502")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11502")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isnorma = normaPer == 0 ? false : true;
                ///caesar
                decimal caesarAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11503")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11503")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal caesarPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11503")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11503")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Iscaesar = caesarPer == 0 ? false : true;
                ///misca
                decimal miscaAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11504")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11504")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal miscaPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11504")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11504")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Ismisca = miscaPer == 0 ? false : true;

                ///follo
                decimal folloAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11501")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11501")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal folloPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "11501")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "11501")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isfollo = folloPer == 0 ? false : true;
                ///dental
                decimal denAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "114")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "114")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal denPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "114")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "114")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isden = denPer == 0 ? false : true;
                ///optical
                decimal optAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "113")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "113")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal optPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "113")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "113")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isopt = optPer == 0 ? false : true;
                ///icu
                decimal icuAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "11106")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "11106")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;

                //decimal icuPer = servCode.AsEnumerable()
                //             .Any(r => r.Field<string>("SER_SERV") == "11106")
                //             ? servCode.AsEnumerable()
                //                      .First(r => r.Field<string>("SER_SERV") == "11106")
                //                      .Field<decimal?>("CEILING_PERT") ?? 100
                //             : 0;
                //bool Isicu = icuPer == 0 ? false : true;


                string tstcoronaa = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "112")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "112")
                                      .Field<string>("CORONA") ?? "N"
                             : "N";

                bool coronaa = tstcoronaa == "Y" ? true : false;
                ///no roshita                
                var dtNoRoshita = dbOra.RunReader(@"SELECT NVL(DAY_NO_ROSHTA_MON, 0) DAY_NO_ROSHTA_MON
                                                 FROM APP.COMP_CUSTOMIZED_D_D_MED                                                                  
                                                 WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' AND CLASS_CODE = '" + classCode + "' AND SER_SERV = '11601'");
                int noRoshita = 0;
                if (dtNoRoshita.Rows.Count > 0)
                    noRoshita = int.Parse(dtNoRoshita.Rows[0][0].ToString());

                #endregion

                //var rows = servCode.AsEnumerable().Where(r => r.Field<int>("SER_SERV") == 111);

                //DataRow rowss = servCode.AsEnumerable().SingleOrDefault(r => r.Field<int>("SER_SERV") == 111);

                //DataRow[] rows = servCode.Select("SER_SERV = 111");

                RenewalInsideMedicalAuthority oldNew = db.RenewalInsideMedicalAuthorities.FirstOrDefault(r => r.MainId == mainIdOld && r.ClassCode == classCode);

                if (typeAction == 2 && oldNew != null)
                {
                    var renwalThreeMain = new RenewalInsideMedicalAuthorityViewModel
                    {
                        HospitalsResidenceServiceLimit = oldNew.HospitalsResidenceServiceLimit,
                        HospitalsResidenceServiceLimitOld = inpAm,
                        HospitalsResidenceServicePercentage = oldNew.HospitalsResidenceServicePercentage,
                        HospitalsResidenceServicePercentageOld = inpPer,
                        HospitalsResidenceServicePercentageoption = oldNew.HospitalsResidenceServicePercentageoption,
                        HospitalsResidenceServiceLimitOption = oldNew.HospitalsResidenceServiceLimitOption,
                        OutsideClinicsLimit = oldNew.OutsideClinicsLimit,
                        OutsideClinicsLimitOld = oputAm,
                        OutsideClinicsPercentage = oldNew.OutsideClinicsPercentage,
                        OutsideClinicsPercentageOld = oputPer,
                        OutsideClinicsLimitOption = oldNew.OutsideClinicsLimitOption,
                        OutsideClinicsPercentageoption = oldNew.OutsideClinicsPercentageoption,
                        ExaminationAndAnalysisLimit = oldNew.ExaminationAndAnalysisLimit,
                        ExaminationAndAnalysisLimitOld = labRayAm,
                        ExaminationAndAnalysisPercentage = oldNew.ExaminationAndAnalysisPercentage,
                        ExaminationAndAnalysisPercentageOld = labRayPer,
                        ExaminationAndAnalysisLimitOption = oldNew.ExaminationAndAnalysisLimitOption,
                        ExaminationAndAnalysisPercentageoption = oldNew.ExaminationAndAnalysisPercentageoption,
                        PhysicalTherapyLimit = oldNew.PhysicalTherapyLimit,
                        PhysicalTherapyLimitOld = phyAm,
                        PhysicalTherapyPercentage = oldNew.PhysicalTherapyPercentage,
                        PhysicalTherapyPercentageOld = phyPer,
                        PhysicalTherapyLimitOption = oldNew.PhysicalTherapyLimitOption,
                        PhysicalTherapyPercentageoption = oldNew.PhysicalTherapyPercentageoption,
                        DailyTherapyLimit = oldNew.DailyTherapyLimit,
                        DailyTherapyLimitOld = dailyAm,
                        DailyTherapyPercentage = oldNew.DailyTherapyPercentage,
                        DailyTherapyPercentageOld = dailyPer,
                        DailyTherapyLimitOption = oldNew.DailyTherapyLimitOption,
                        DailyTherapyPercentageoption = Isdaily,
                        ChronicTherapyLimit = oldNew.ChronicTherapyLimit,
                        ChronicTherapyLimitOld = chronAm,
                        ChronicTherapyPercentage = oldNew.ChronicTherapyPercentage,
                        ChronicTherapyPercentageOld = chronPer,
                        ChronicTherapyLimitOption = oldNew.ChronicTherapyLimitOption,
                        ChronicTherapyPercentageoption = oldNew.ChronicTherapyPercentageoption,
                        NatChildBirthLimit = oldNew.NatChildBirthLimit,
                        NatChildBirthLimitOld = normaAm,
                        NatChildBirthPercentage = oldNew.NatChildBirthPercentage,
                        NatChildBirthPercentageOld = normaPer,
                        NatChildBirthLimitOption = oldNew.NatChildBirthLimitOption,
                        NatChildBirthPercentageoption = oldNew.NatChildBirthPercentageoption,
                        CaesChildBirthLimit = oldNew.CaesChildBirthLimit,
                        CaesChildBirthLimitOld = caesarAm,
                        CaesChildBirthPercentage = oldNew.CaesChildBirthPercentage,
                        CaesChildBirthPercentageOld = caesarPer,
                        CaesChildBirthLimitOption = oldNew.CaesChildBirthLimitOption,
                        CaesChildBirthPercentageoption = oldNew.CaesChildBirthPercentageoption,
                        LegalAbortionLimit = oldNew.LegalAbortionLimit,
                        LegalAbortionLimitOld = miscaAm,
                        LegalAbortionPercentage = oldNew.LegalAbortionPercentage,
                        LegalAbortionPercentageOld = miscaPer,
                        LegalAbortionLimitOption = oldNew.LegalAbortionLimitOption,
                        LegalAbortionPercentageoption = oldNew.LegalAbortionPercentageoption,
                        PregFollowUpLimit = oldNew.PregFollowUpLimit,
                        PregFollowUpLimitOld = folloAm,
                        PregFollowUpPercentage = oldNew.PregFollowUpPercentage,
                        PregFollowUpPercentageOld = folloPer,
                        PregFollowUpLimitOption = oldNew.PregFollowUpLimitOption,
                        PregFollowUpPercentageoption = oldNew.PregFollowUpPercentageoption,

                        AdvancedDentalServiceLimit = oldNew.AdvancedDentalServiceLimit,
                        AdvancedDentalServiceLimitOld = denAm,
                        BasicDentalServiceLimit = oldNew.BasicDentalServiceLimit,
                        BasicDentalServiceLimitOld = denAm,
                        AdvancedDentalServicePercentage = oldNew.AdvancedDentalServicePercentage,
                        AdvancedDentalServicePercentageOld = denPer,
                        BasicDentalServicePercentage = oldNew.BasicDentalServicePercentage,
                        BasicDentalServicePercentageOld = denPer,
                        AdvancedDentalServicePercentageoption = oldNew.AdvancedDentalServicePercentageoption,
                        AdvancedDentalServiceLimitOption = oldNew.AdvancedDentalServiceLimitOption,
                        BasicDentalServiceLimitOption = oldNew.BasicDentalServiceLimitOption,
                        BasicDentalServicePercentageoption = oldNew.BasicDentalServicePercentageoption,
                        OpticsLimit = oldNew.OpticsLimit,
                        OpticsLimitOld = optAm,
                        OpticsPercentage = oldNew.OpticsPercentage,
                        OpticsPercentageOld = optPer,
                        OpticsLimitOption = oldNew.OpticsLimitOption,
                        OpticsPercentageoption = oldNew.OpticsPercentageoption,
                        IntensiveCareDaysCount = int.Parse(oldNew.IntensiveCareDaysCount.ToString()),
                        IntensiveCareDaysCountOld = int.Parse(icuAm.ToString()),
                        DailyRoshitasCountPerMonth = oldNew.DailyRoshitasCountPerMonth,
                        DailyRoshitasCountPerMonthOld = noRoshita,
                        CoronaVaccineCoverage = oldNew.CoronaVaccineCoverage,
                        CoronaVaccineCoverageOld = coronaa,

                        MainId = mainId,
                        ClassCode = classCode,
                        CompId = CompId,
                        ContractNo = ContractNo,
                        CountClass = countCat,
                        typAction = typeAction,
                        MainIdOld = mainIdOld,

                        NaturalBirthType = oldNew.NaturalBirthType,
                        NaturalBirthCovaregType = oldNew.NaturalBirthCovaregType,
                        CaesarBirthType = oldNew.CaesarBirthType,
                        CaesarBirthCovaregType = oldNew.CaesarBirthCovaregType,
                        FollowUpPregType = oldNew.FollowUpPregType,
                        FollowUpPregCovaregType = oldNew.FollowUpPregCovaregType,
                        AdvancedDentalType = oldNew.AdvancedDentalType,
                        AdvancedDentalCovaregType = oldNew.AdvancedDentalCovaregType,
                        BasicDentalType = oldNew.BasicDentalType,
                        BasicDentalCovaregType = oldNew.BasicDentalCovaregType,
                        LegalAbortionType = oldNew.LegalAbortionType,
                        LegalAbortionCovaregType = oldNew.LegalAbortionCovaregType,
                        DentalVisit = oldNew.DentalVisit,
                        OpticalVisit = oldNew.OpticalVisit,
                        BirthVisit = oldNew.BirthVisit,
                        TransportAmbulancePercent = (int)oldNew.TransportAmbulancePercent,
                        Notes = oldNew.Notes,
                        //,



                        AdvancDentalService = db.RenewalInsideAdvanceDentals
                                   .Where(x => x.MainId == oldNew.Id)
                                   .Select(x => x.ServiceId ?? 0)
                                   .ToList(),
                        BascDentalService = db.RenewalInsideBasicDentals
                                   .Where(x => x.MainId == oldNew.Id)
                                   .Select(x => x.ServiceId ?? 0)
                                   .ToList()                      
                    };

                    model.Add(renwalThreeMain);
                }
                else
                {
                    var renwalThreeMain = new RenewalInsideMedicalAuthorityViewModel
                    {
                        HospitalsResidenceServiceLimit = inpAm,
                        HospitalsResidenceServiceLimitOld = inpAm,
                        HospitalsResidenceServicePercentage = inpPer,
                        HospitalsResidenceServicePercentageOld = inpPer,
                        HospitalsResidenceServicePercentageoption = Isinp,
                        HospitalsResidenceServiceLimitOption = Isinp,
                        OutsideClinicsLimit = oputAm,
                        OutsideClinicsLimitOld = oputAm,
                        OutsideClinicsPercentage = oputPer,
                        OutsideClinicsPercentageOld = oputPer,
                        OutsideClinicsLimitOption = IsOut,
                        OutsideClinicsPercentageoption = IsOut,
                        ExaminationAndAnalysisLimit = labRayAm,
                        ExaminationAndAnalysisLimitOld = labRayAm,
                        ExaminationAndAnalysisPercentage = labRayPer,
                        ExaminationAndAnalysisPercentageOld = labRayPer,
                        ExaminationAndAnalysisLimitOption = IslabRay,
                        ExaminationAndAnalysisPercentageoption = IslabRay,
                        PhysicalTherapyLimit = phyAm,
                        PhysicalTherapyLimitOld = phyAm,
                        PhysicalTherapyPercentage = phyPer,
                        PhysicalTherapyPercentageOld = phyPer,
                        PhysicalTherapyLimitOption = Isphy,
                        PhysicalTherapyPercentageoption = Isphy,
                        DailyTherapyLimit = dailyAm,
                        DailyTherapyLimitOld = dailyAm,
                        DailyTherapyPercentage = dailyPer,
                        DailyTherapyPercentageOld = dailyPer,
                        DailyTherapyLimitOption = Isdaily,
                        DailyTherapyPercentageoption = Isdaily,
                        ChronicTherapyLimit = chronAm,
                        ChronicTherapyLimitOld = chronAm,
                        ChronicTherapyPercentage = chronPer,
                        ChronicTherapyPercentageOld = chronPer,
                        ChronicTherapyLimitOption = Ischron,
                        ChronicTherapyPercentageoption = Ischron,
                        NatChildBirthLimit = normaAm,
                        NatChildBirthLimitOld = normaAm,
                        NatChildBirthPercentage = normaPer,
                        NatChildBirthPercentageOld = normaPer,
                        NatChildBirthLimitOption = Isnorma,
                        NatChildBirthPercentageoption = Isnorma,
                        CaesChildBirthLimit = caesarAm,
                        CaesChildBirthLimitOld = caesarAm,
                        CaesChildBirthPercentage = caesarPer,
                        CaesChildBirthPercentageOld = caesarPer,
                        CaesChildBirthLimitOption = Iscaesar,
                        CaesChildBirthPercentageoption = Iscaesar,
                        LegalAbortionLimit = miscaAm,
                        LegalAbortionLimitOld = miscaAm,
                        LegalAbortionPercentage = miscaPer,
                        LegalAbortionPercentageOld = miscaPer,
                        LegalAbortionLimitOption = Ismisca,
                        LegalAbortionPercentageoption = Ismisca,
                        PregFollowUpLimit = folloAm,
                        PregFollowUpLimitOld = folloAm,
                        PregFollowUpPercentage = folloPer,
                        PregFollowUpPercentageOld = folloPer,
                        PregFollowUpLimitOption = Isfollo,
                        PregFollowUpPercentageoption = Isfollo,

                        AdvancedDentalServiceLimit = denAm,
                        AdvancedDentalServiceLimitOld = denAm,
                        BasicDentalServiceLimit = denAm,
                        BasicDentalServiceLimitOld = denAm,
                        AdvancedDentalServicePercentage = denPer,
                        AdvancedDentalServicePercentageOld = denPer,
                        BasicDentalServicePercentage = denPer,
                        BasicDentalServicePercentageOld = denPer,
                        AdvancedDentalServicePercentageoption = Isden,
                        AdvancedDentalServiceLimitOption = Isden,
                        BasicDentalServiceLimitOption = Isden,
                        BasicDentalServicePercentageoption = Isden,
                        OpticsLimit = optAm,
                        OpticsLimitOld = optAm,
                        OpticsPercentage = optPer,
                        OpticsPercentageOld = optPer,
                        OpticsLimitOption = Isopt,
                        OpticsPercentageoption = Isopt,
                        IntensiveCareDaysCount = int.Parse(icuAm.ToString()),
                        IntensiveCareDaysCountOld = int.Parse(icuAm.ToString()),
                        DailyRoshitasCountPerMonth = noRoshita,
                        DailyRoshitasCountPerMonthOld = noRoshita,
                        CoronaVaccineCoverage = coronaa,
                        CoronaVaccineCoverageOld = coronaa,

                        MainId = mainId,
                        ClassCode = classCode,
                        CompId = CompId,
                        ContractNo = ContractNo,
                        CountClass = countCat,
                        typAction = typeAction,
                        MainIdOld = mainIdOld


                    };

                    model.Add(renwalThreeMain);
                }
            }

            return View(model);
        }
        private RenewalInsideMedicalAuthority MapViewModelToEntityStepThree(RenewalInsideMedicalAuthorityViewModel vM)
        {
            // Use a mapping library or manually map properties as needed
            // This is a simplified example, adjust as needed
            var renewalthree = new RenewalInsideMedicalAuthority
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
                MainId = vM.MainId, 
                ClassCode = vM.ClassCode,

                NaturalBirthType = vM.NaturalBirthType,
                NaturalBirthCovaregType = vM.NaturalBirthCovaregType,
                CaesarBirthType = vM.CaesarBirthType,
                CaesarBirthCovaregType = vM.CaesarBirthCovaregType,
                FollowUpPregType = vM.FollowUpPregType,
                FollowUpPregCovaregType = vM.FollowUpPregCovaregType,
                AdvancedDentalType = vM.AdvancedDentalType,
                AdvancedDentalCovaregType = vM.AdvancedDentalCovaregType,
                BasicDentalType = vM.BasicDentalType,
                BasicDentalCovaregType = vM.BasicDentalCovaregType,
                LegalAbortionType = vM.LegalAbortionType,
                LegalAbortionCovaregType = vM.LegalAbortionCovaregType,
                DentalVisit = vM.DentalVisit,
                OpticalVisit = vM.OpticalVisit,
                BirthVisit = vM.BirthVisit,
                TransportAmbulancePercent = vM.TransportAmbulancePercent,
                Notes = vM.Notes,
                 
            };
            //if (vM.AdvancDentalService != null)
            //{
            //    vM.AdvancDentalService = new List<RenewalInsideAdvanceDental>();

            //    foreach (var serviceId in vM.AdvancDentalService)
            //    {
            //        vM.AdvancDentalService.Add(new RenewalInsideAdvanceDental
            //        {
            //            ServiceId = serviceId
            //        });
            //    }
            //}

            //if (model.poolService != null)
            //{
            //    renewalBasicData.RenewalServicePools = new List<RenewalServicePool>();
            //    foreach (var serviceId in model.poolService)
            //    {
            //        renewalBasicData.RenewalServicePools.Add(new RenewalServicePool
            //        {
            //            ServiceId = serviceId
            //        });
            //    }
            //}
            return renewalthree;
        }

        public async Task<ActionResult> SaveDataStepThree(List<RenewalInsideMedicalAuthorityViewModel> model)
        {
            try
            {
                foreach (var mod in model)
                {
                    if (ModelState.IsValid)
                    {
                        var renewalThree = MapViewModelToEntityStepThree(mod);

                        db.RenewalInsideMedicalAuthorities.Add(renewalThree);

                        db.SaveChanges();

                        if (mod.AdvancDentalService != null)
                        {
                            var mainId = renewalThree.Id; // Assuming MainId is the key property

                            foreach (var dentalService in mod.AdvancDentalService)
                            {
                                var advancDentalService = new RenewalInsideAdvanceDental
                                {
                                    MainId = mainId,
                                    ServiceId = dentalService
                                };
                                db.RenewalInsideAdvanceDentals.Add(advancDentalService);
                            }

                            db.SaveChanges();
                        }

                        if (mod.BascDentalService != null)
                        {
                            var mainId = renewalThree.Id;

                            foreach (var dentalService in mod.BascDentalService)
                            {
                                var basicDentalService = new RenewalInsideBasicDental
                                {
                                    MainId = mainId,
                                    ServiceId = dentalService
                                };
                                db.RenewalInsideBasicDentals.Add(basicDentalService);
                            }

                            db.SaveChanges();
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

                        TempData["ErrorMessage"] = errors;
                        TempData["RenewalStepThreeModel"] = model;

                        return RedirectToAction("RenewalStepThree", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
                    }
                }

                return RedirectToAction("RenewalStepFour", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                TempData["RenewalStepThreeModel"] = model;

                return RedirectToAction("RenewalStepThree", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
            }
        }
        #endregion
        #region ProposalStepFour
        public ActionResult RenewalStepFour(int mainId, int CompId, int ContractNo, int countCat, int typeAction, int mainIdOld)
        {
            ViewBag.MainId = mainId;
            ViewBag.CatCount = countCat;
            ViewBag.basicService = db.BasicDentalServices.ToList();
            ViewBag.advancService = db.DentalServices.ToList();

            if (TempData["Save"] != null && !string.IsNullOrEmpty(TempData["Save"].ToString()))
                ViewBag.Save = "YES";
            else
                ViewBag.Save = "NO";

            var model2 = TempData["RenewalStepFourModel"] as List<RenewalOutsideMedicalAuthorityViewModel>;

            if (model2 != null)
            {
                ViewBag.Errors = TempData["ErrorMessage"];
                return View(model2);
            }

            var model = new List<RenewalOutsideMedicalAuthorityViewModel>(countCat);

            var compclassold = dbOra.RunReader(@"SELECT C_COMP_ID, CONTRACT_NO, CLASS_CODE, MAX_AMOUNT, NVL(ANNUAL_PREM, 0) ANNUAL_PREM, HOSPITAL_DEGREE, COVER_RELATION,
                                                        DECODE(CLASS_CODE, '1', 1, '2', 2, '11', 3, '12', 4, '33', 5, '44', 6, '6', 7, '99', 8, '2F', 9, '3F', 10, '4F', 11, '5F', 12, CLASS_CODE)
                                                 FROM DMS_TEST.COMP_CONTRACT_CLASS
                                                 WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' ORDER BY DECODE(CLASS_CODE, '1', 1, '2', 2, '11', 3, '12', 4, '33', 5, '44', 6, '6', 7, '99', 8, '2F', 9, '3F', 10, '4F', 11, '5F', 12, CLASS_CODE)");

            foreach (System.Data.DataRow row in compclassold.Rows)
            {
                var classCode = row["CLASS_CODE"].ToString();
                decimal maxAmount = decimal.Parse(row["MAX_AMOUNT"].ToString());

                var servCode = dbOra.RunReader(@"SELECT DISTINCT D_SERV_CODE, D_SERV_CODE SER_SERV, CEILING_AMT, CEILING_PERT, NVL(CORONA, 'N') CORONA
                                                 FROM DMS_TEST.COMP_CUSTOMIZED_D                                                                  
                                                 WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' AND CLASS_CODE = '" + classCode + "'  "
                                     + "           UNION ALL "
                                     + "             SELECT DISTINCT D_SERV_CODE, SER_SERV, CEILING_AMT, CEILING_PERT, NVL(CORONA, 'N') CORONA "
                                     + "             FROM DMS_TEST.COMP_CUSTOMIZED_D_D "
                                     + "           WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' AND CLASS_CODE = '" + classCode + "' "
                                     + "         ORDER BY D_SERV_CODE");

                #region Get Old Data
                decimal inpAm = servCode.AsEnumerable()
                              .Any(r => r.Field<string>("SER_SERV") == "121")
                              ? servCode.AsEnumerable()
                                       .First(r => r.Field<string>("SER_SERV") == "121")
                                       .Field<decimal?>("CEILING_AMT") ?? maxAmount
                              : 0;
                decimal inpPer = servCode.AsEnumerable()
                              .Any(r => r.Field<string>("SER_SERV") == "121")
                              ? servCode.AsEnumerable()
                                       .First(r => r.Field<string>("SER_SERV") == "121")
                                       .Field<decimal?>("CEILING_PERT") ?? 100
                              : 0;
                bool Isinp = inpPer == 0 ? false : true;
                ///outp
                decimal oputAm = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "122")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "122")
                                      .Field<decimal?>("CEILING_AMT") ?? maxAmount
                             : 0;
                decimal oputPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "122")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "122")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool IsOut = oputPer == 0 ? false : true;
                ///lab ray
                decimal labRayAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12206")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12206")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal labRayPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12206")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12206")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool IslabRay = labRayPer == 0 ? false : true;
                ///Phys
                decimal phyAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12204")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12204")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal phyPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12204")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12204")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isphy = phyPer == 0 ? false : true;
                ///daily
                decimal dailyAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12601")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12601")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal dailyPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12601")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12601")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isdaily = dailyPer == 0 ? false : true;

                ///chron
                decimal chronAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12602")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12602")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal chronPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12602")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12602")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Ischron = chronPer == 0 ? false : true;
                ///norma
                decimal normaAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12502")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12502")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal normaPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12502")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12502")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isnorma = normaPer == 0 ? false : true;
                ///caesar
                decimal caesarAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12503")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12503")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal caesarPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12503")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12503")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Iscaesar = caesarPer == 0 ? false : true;
                ///misca
                decimal miscaAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12504")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12504")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal miscaPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12504")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12504")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Ismisca = miscaPer == 0 ? false : true;

                ///follo
                decimal folloAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12501")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12501")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal folloPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "12501")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "12501")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isfollo = folloPer == 0 ? false : true;
                ///dental
                decimal denAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "124")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "124")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal denPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "124")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "124")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isden = denPer == 0 ? false : true;
                ///optical
                decimal optAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "123")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "123")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;
                decimal optPer = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "123")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "123")
                                      .Field<decimal?>("CEILING_PERT") ?? 100
                             : 0;
                bool Isopt = optPer == 0 ? false : true;
                ///icu
                decimal icuAm = servCode.AsEnumerable()
                            .Any(r => r.Field<string>("SER_SERV") == "12106")
                            ? servCode.AsEnumerable()
                                     .First(r => r.Field<string>("SER_SERV") == "12106")
                                     .Field<decimal?>("CEILING_AMT") ?? maxAmount
                            : 0;

                //decimal icuPer = servCode.AsEnumerable()
                //             .Any(r => r.Field<string>("SER_SERV") == "11106")
                //             ? servCode.AsEnumerable()
                //                      .First(r => r.Field<string>("SER_SERV") == "11106")
                //                      .Field<decimal?>("CEILING_PERT") ?? 100
                //             : 0;
                //bool Isicu = icuPer == 0 ? false : true;


                string tstcoronaa = servCode.AsEnumerable()
                             .Any(r => r.Field<string>("SER_SERV") == "122")
                             ? servCode.AsEnumerable()
                                      .First(r => r.Field<string>("SER_SERV") == "122")
                                      .Field<string>("CORONA") ?? "N"
                             : "N";

                bool coronaa = tstcoronaa == "Y" ? true : false;
                ///no roshita                
                var dtNoRoshita = dbOra.RunReader(@"SELECT NVL(DAY_NO_ROSHTA_MON, 0) DAY_NO_ROSHTA_MON
                                                 FROM APP.COMP_CUSTOMIZED_D_D_MED                                                                  
                                                 WHERE C_COMP_ID = '" + CompId + "' AND CONTRACT_NO = '" + ContractNo + "' AND CLASS_CODE = '" + classCode + "' AND SER_SERV = '12601'");
                int noRoshita = 0;
                if (dtNoRoshita.Rows.Count > 0)
                    noRoshita = int.Parse(dtNoRoshita.Rows[0][0].ToString());

                #endregion

                RenewalOutsideMedicalAuthority oldNew = db.RenewalOutsideMedicalAuthorities.FirstOrDefault(r => r.MainId == mainIdOld && r.ClassCode == classCode);

                if (typeAction == 2 && oldNew != null)
                {
                    var renwalFourMain = new RenewalOutsideMedicalAuthorityViewModel
                    {
                        HospitalsResidenceServiceLimit = oldNew.HospitalsResidenceServiceLimit,
                        HospitalsResidenceServiceLimitOld = inpAm,
                        HospitalsResidenceServicePercentage = oldNew.HospitalsResidenceServicePercentage,
                        HospitalsResidenceServicePercentageOld = inpPer,
                        HospitalsResidenceServicePercentageoption = oldNew.HospitalsResidenceServicePercentageoption,
                        HospitalsResidenceServiceLimitOption = oldNew.HospitalsResidenceServiceLimitOption,
                        OutsideClinicsLimit = oldNew.OutsideClinicsLimit,
                        OutsideClinicsLimitOld = oputAm,
                        OutsideClinicsPercentage = oldNew.OutsideClinicsPercentage,
                        OutsideClinicsPercentageOld = oputPer,
                        OutsideClinicsLimitOption = oldNew.OutsideClinicsLimitOption,
                        OutsideClinicsPercentageoption = oldNew.OutsideClinicsPercentageoption,
                        ExaminationAndAnalysisLimit = oldNew.ExaminationAndAnalysisLimit,
                        ExaminationAndAnalysisLimitOld = labRayAm,
                        ExaminationAndAnalysisPercentage = oldNew.ExaminationAndAnalysisPercentage,
                        ExaminationAndAnalysisPercentageOld = labRayPer,
                        ExaminationAndAnalysisLimitOption = oldNew.ExaminationAndAnalysisLimitOption,
                        ExaminationAndAnalysisPercentageoption = oldNew.ExaminationAndAnalysisPercentageoption,
                        PhysicalTherapyLimit = oldNew.PhysicalTherapyLimit,
                        PhysicalTherapyLimitOld = phyAm,
                        PhysicalTherapyPercentage = oldNew.PhysicalTherapyPercentage,
                        PhysicalTherapyPercentageOld = phyPer,
                        PhysicalTherapyLimitOption = oldNew.PhysicalTherapyLimitOption,
                        PhysicalTherapyPercentageoption = oldNew.PhysicalTherapyPercentageoption,
                        DailyTherapyLimit = oldNew.DailyTherapyLimit,
                        DailyTherapyLimitOld = dailyAm,
                        DailyTherapyPercentage = oldNew.DailyTherapyPercentage,
                        DailyTherapyPercentageOld = dailyPer,
                        DailyTherapyLimitOption = oldNew.DailyTherapyLimitOption,
                        DailyTherapyPercentageoption = Isdaily,
                        ChronicTherapyLimit = oldNew.ChronicTherapyLimit,
                        ChronicTherapyLimitOld = chronAm,
                        ChronicTherapyPercentage = oldNew.ChronicTherapyPercentage,
                        ChronicTherapyPercentageOld = chronPer,
                        ChronicTherapyLimitOption = oldNew.ChronicTherapyLimitOption,
                        ChronicTherapyPercentageoption = oldNew.ChronicTherapyPercentageoption,
                        NatChildBirthLimit = oldNew.NatChildBirthLimit,
                        NatChildBirthLimitOld = normaAm,
                        NatChildBirthPercentage = oldNew.NatChildBirthPercentage,
                        NatChildBirthPercentageOld = normaPer,
                        NatChildBirthLimitOption = oldNew.NatChildBirthLimitOption,
                        NatChildBirthPercentageoption = oldNew.NatChildBirthPercentageoption,
                        CaesChildBirthLimit = oldNew.CaesChildBirthLimit,
                        CaesChildBirthLimitOld = caesarAm,
                        CaesChildBirthPercentage = oldNew.CaesChildBirthPercentage,
                        CaesChildBirthPercentageOld = caesarPer,
                        CaesChildBirthLimitOption = oldNew.CaesChildBirthLimitOption,
                        CaesChildBirthPercentageoption = oldNew.CaesChildBirthPercentageoption,
                        LegalAbortionLimit = oldNew.LegalAbortionLimit,
                        LegalAbortionLimitOld = miscaAm,
                        LegalAbortionPercentage = oldNew.LegalAbortionPercentage,
                        LegalAbortionPercentageOld = miscaPer,
                        LegalAbortionLimitOption = oldNew.LegalAbortionLimitOption,
                        LegalAbortionPercentageoption = oldNew.LegalAbortionPercentageoption,
                        PregFollowUpLimit = oldNew.PregFollowUpLimit,
                        PregFollowUpLimitOld = folloAm,
                        PregFollowUpPercentage = oldNew.PregFollowUpPercentage,
                        PregFollowUpPercentageOld = folloPer,
                        PregFollowUpLimitOption = oldNew.PregFollowUpLimitOption,
                        PregFollowUpPercentageoption = oldNew.PregFollowUpPercentageoption,

                        AdvancedDentalServiceLimit = oldNew.AdvancedDentalServiceLimit,
                        AdvancedDentalServiceLimitOld = denAm,
                        BasicDentalServiceLimit = oldNew.BasicDentalServiceLimit,
                        BasicDentalServiceLimitOld = denAm,
                        AdvancedDentalServicePercentage = oldNew.AdvancedDentalServicePercentage,
                        AdvancedDentalServicePercentageOld = denPer,
                        BasicDentalServicePercentage = oldNew.BasicDentalServicePercentage,
                        BasicDentalServicePercentageOld = denPer,
                        AdvancedDentalServicePercentageoption = oldNew.AdvancedDentalServicePercentageoption,
                        AdvancedDentalServiceLimitOption = oldNew.AdvancedDentalServiceLimitOption,
                        BasicDentalServiceLimitOption = oldNew.BasicDentalServiceLimitOption,
                        BasicDentalServicePercentageoption = oldNew.BasicDentalServicePercentageoption,
                        OpticsLimit = oldNew.OpticsLimit,
                        OpticsLimitOld = optAm,
                        OpticsPercentage = oldNew.OpticsPercentage,
                        OpticsPercentageOld = optPer,
                        OpticsLimitOption = oldNew.OpticsLimitOption,
                        OpticsPercentageoption = oldNew.OpticsPercentageoption,
                        IntensiveCareDaysCount = int.Parse(oldNew.IntensiveCareDaysCount.ToString()),
                        IntensiveCareDaysCountOld = int.Parse(icuAm.ToString()),
                        DailyRoshitasCountPerMonth = oldNew.DailyRoshitasCountPerMonth,
                        DailyRoshitasCountPerMonthOld = noRoshita,
                        CoronaVaccineCoverage = oldNew.CoronaVaccineCoverage,
                        CoronaVaccineCoverageOld = coronaa,

                        MainId = mainId,
                        ClassCode = classCode,
                        CompId = CompId,
                        ContractNo = ContractNo,
                        CountClass = countCat,
                        typAction = typeAction,
                        MainIdOld = mainIdOld
                    };

                    model.Add(renwalFourMain);
                }
                else
                {
                    var renwalFourMain = new RenewalOutsideMedicalAuthorityViewModel
                    {
                        HospitalsResidenceServiceLimit = inpAm,
                        HospitalsResidenceServiceLimitOld = inpAm,
                        HospitalsResidenceServicePercentage = inpPer,
                        HospitalsResidenceServicePercentageOld = inpPer,
                        HospitalsResidenceServicePercentageoption = Isinp,
                        HospitalsResidenceServiceLimitOption = Isinp,
                        OutsideClinicsLimit = oputAm,
                        OutsideClinicsLimitOld = oputAm,
                        OutsideClinicsPercentage = oputPer,
                        OutsideClinicsPercentageOld = oputPer,
                        OutsideClinicsLimitOption = IsOut,
                        OutsideClinicsPercentageoption = IsOut,
                        ExaminationAndAnalysisLimit = labRayAm,
                        ExaminationAndAnalysisLimitOld = labRayAm,
                        ExaminationAndAnalysisPercentage = labRayPer,
                        ExaminationAndAnalysisPercentageOld = labRayPer,
                        ExaminationAndAnalysisLimitOption = IslabRay,
                        ExaminationAndAnalysisPercentageoption = IslabRay,
                        PhysicalTherapyLimit = phyAm,
                        PhysicalTherapyLimitOld = phyAm,
                        PhysicalTherapyPercentage = phyPer,
                        PhysicalTherapyPercentageOld = phyPer,
                        PhysicalTherapyLimitOption = Isphy,
                        PhysicalTherapyPercentageoption = Isphy,
                        DailyTherapyLimit = dailyAm,
                        DailyTherapyLimitOld = dailyAm,
                        DailyTherapyPercentage = dailyPer,
                        DailyTherapyPercentageOld = dailyPer,
                        DailyTherapyLimitOption = Isdaily,
                        DailyTherapyPercentageoption = Isdaily,
                        ChronicTherapyLimit = chronAm,
                        ChronicTherapyLimitOld = chronAm,
                        ChronicTherapyPercentage = chronPer,
                        ChronicTherapyPercentageOld = chronPer,
                        ChronicTherapyLimitOption = Ischron,
                        ChronicTherapyPercentageoption = Ischron,
                        NatChildBirthLimit = normaAm,
                        NatChildBirthLimitOld = normaAm,
                        NatChildBirthPercentage = normaPer,
                        NatChildBirthPercentageOld = normaPer,
                        NatChildBirthLimitOption = Isnorma,
                        NatChildBirthPercentageoption = Isnorma,
                        CaesChildBirthLimit = caesarAm,
                        CaesChildBirthLimitOld = caesarAm,
                        CaesChildBirthPercentage = caesarPer,
                        CaesChildBirthPercentageOld = caesarPer,
                        CaesChildBirthLimitOption = Iscaesar,
                        CaesChildBirthPercentageoption = Iscaesar,
                        LegalAbortionLimit = miscaAm,
                        LegalAbortionLimitOld = miscaAm,
                        LegalAbortionPercentage = miscaPer,
                        LegalAbortionPercentageOld = miscaPer,
                        LegalAbortionLimitOption = Ismisca,
                        LegalAbortionPercentageoption = Ismisca,
                        PregFollowUpLimit = folloAm,
                        PregFollowUpLimitOld = folloAm,
                        PregFollowUpPercentage = folloPer,
                        PregFollowUpPercentageOld = folloPer,
                        PregFollowUpLimitOption = Isfollo,
                        PregFollowUpPercentageoption = Isfollo,

                        AdvancedDentalServiceLimit = denAm,
                        AdvancedDentalServiceLimitOld = denAm,
                        BasicDentalServiceLimit = denAm,
                        BasicDentalServiceLimitOld = denAm,
                        AdvancedDentalServicePercentage = denPer,
                        AdvancedDentalServicePercentageOld = denPer,
                        BasicDentalServicePercentage = denPer,
                        BasicDentalServicePercentageOld = denPer,
                        AdvancedDentalServicePercentageoption = Isden,
                        AdvancedDentalServiceLimitOption = Isden,
                        BasicDentalServiceLimitOption = Isden,
                        BasicDentalServicePercentageoption = Isden,
                        OpticsLimit = optAm,
                        OpticsLimitOld = optAm,
                        OpticsPercentage = optPer,
                        OpticsPercentageOld = optPer,
                        OpticsLimitOption = Isopt,
                        OpticsPercentageoption = Isopt,
                        IntensiveCareDaysCount = int.Parse(icuAm.ToString()),
                        IntensiveCareDaysCountOld = int.Parse(icuAm.ToString()),
                        DailyRoshitasCountPerMonth = noRoshita,
                        DailyRoshitasCountPerMonthOld = noRoshita,
                        CoronaVaccineCoverage = coronaa,
                        CoronaVaccineCoverageOld = coronaa,

                        MainId = mainId,
                        ClassCode = classCode,
                        CompId = CompId,
                        ContractNo = ContractNo,
                        CountClass = countCat,
                        typAction = typeAction,
                        MainIdOld = mainIdOld


                    };

                    model.Add(renwalFourMain);
                }
            }


            //for (int i = 0; i < countCat; i++)
            //{
            //    // Create an instance of ProposalStepTwoViewModel and add it to the list
            //    model.Add(new ProposalInsideMedicalAuthorityViewModel());
            //}

            return View(model);
        }

        private RenewalOutsideMedicalAuthority MapViewModelToEntityStepFour(RenewalOutsideMedicalAuthorityViewModel vM)
        {
            // Use a mapping library or manually map properties as needed
            // This is a simplified example, adjust as needed
            var renewalfour = new RenewalOutsideMedicalAuthority
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
                MainId = vM.MainId, 
                ClassCode = vM.ClassCode

            };

            return renewalfour;
        }

        public async Task<ActionResult> SaveDataStepFour(List<RenewalOutsideMedicalAuthorityViewModel> model)
        {
            try
            {
                foreach (var mod in model)
                {
                    if (ModelState.IsValid)
                    {
                        var renewalFour = MapViewModelToEntityStepFour(mod);

                        db.RenewalOutsideMedicalAuthorities.Add(renewalFour);
                        db.SaveChanges();
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

                        TempData["ErrorMessage"] = errors;
                        TempData["RenewalStepFourModel"] = model;

                        return RedirectToAction("RenewalStepFour", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
                    }
                }

                //PrintRenewalReports(model[0].MainId, model[0].CountClass);
                TempData["Save"] = "YES";
                return RedirectToAction("RenewalStepFour", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });

              //  return RedirectToAction("Show");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                TempData["RenewalStepFourModel"] = model;

                return RedirectToAction("RenewalStepFour", new { mainId = model[0].MainId, CompId = model[0].CompId, ContractNo = model[0].ContractNo, countCat = model[0].CountClass, typeAction = model[0].typAction, mainIdOld = model[0].MainIdOld });
            }
        }
        #endregion
        public ActionResult PrintRenewalReports(int id, int countClass)
        {          
            ReportDocument rd = new ReportDocument();
            if (countClass == 1)    
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "OfferRenewal1Class.rpt"));
            else if(countClass == 2)
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "OfferRenewal2Class.rpt"));
            else if (countClass == 3)
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "OfferRenewal3Class.rpt"));
            else if(countClass == 4)
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "OfferRenewal4Class.rpt"));
            
            
            rd.SetDatabaseLogon("dms_report", "W?8Z?PA-C4dNvNe3");

            rd.SetParameterValue("@idd", id);
           
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", DateTime.Now.Date.ToString("ddMMyyyy") + "Renewal Offer.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}