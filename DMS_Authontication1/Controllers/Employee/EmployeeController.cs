using CrystalDecisions.CrystalReports.Engine;
using DMS_Authontication1.Data_Function;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using DMS_Authontication1.ViewModel.EmployeeVM;
using DMS_Authontication1.ViewModel.HR;
using DMS_TEST.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.Employee
{
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class EmployeeController : Controller
    {
        #region Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private DBApproval dbAproval;
        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationDbContext applicationDb = new ApplicationDbContext();

        #endregion

        #region Ctor

        public EmployeeController()
        {
            dbAproval = new DBApproval();
        }

        public EmployeeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion


        #region Actions


        [Authorize(Roles = "User")]
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexAdmin()
        {
            return View();
        }
        public JsonResult RequestList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            long lgSearch;
            long.TryParse(sSearch, out lgSearch);
            var result2 = new
            {
                sEcho = sEcho,
                aaData = db.DeliveryRequests.OrderByDescending(m => m.Id).AsEnumerable()
            .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || /*r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) ||*/ r.Id == lgSearch : true)
            .Where(d => d.IsConfirmed == false && d.IsDeleted == false)
            .Select(l => new DeliveryRequest
            {
                Id = l.Id,
                CardId = l.CardId,
                Phone1 = l.Phone1,
                Time = l.Time,
                CreatedDate = l.CreatedDate,
                //EmployeePersonalData = l.EmployeePersonalData,
            }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.DeliveryRequests.AsEnumerable()
           .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Id == lgSearch : true)
            .Where(d => d.IsConfirmed == false && d.IsDeleted == false).Count(),
                iTotalDisplayRecords = db.DeliveryRequests.AsEnumerable()
           .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Id == lgSearch : true)
            .Where(d => d.IsConfirmed == false && d.IsDeleted == false).Count()
            };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize(Roles = "Admin")]
        public ActionResult OldRequest()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Activation_Employee()
        {
            return View();
        }
        public JsonResult NoActiveEmployeeList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            long lgSearch;
            long.TryParse(sSearch, out lgSearch);
            var result2 = new
            {
                sEcho = sEcho,
                aaData = db.EmployeePersonalDatas.Where(e => e.IsActive == false).OrderByDescending(m => m.Id).AsEnumerable()
            .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.FullName.ToLower().Contains(sSearch.ToLower()) || r.Id == lgSearch : true)

            .Select(l => new EmployeePersonalData
            {
                Id = l.Id,
                FullName = l.FullName,
                NationalId = l.NationalId,
                CardId = l.CardId,
                BirthDate = l.BirthDate,
                IsActive = l.IsActive,
                CreatedDate = l.CreatedDate,
                NationalIdImage = l.NationalIdImage,
            }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.EmployeePersonalDatas.Where(e => e.IsActive == false).OrderByDescending(m => m.Id).AsEnumerable()
            .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.FullName.ToLower().Contains(sSearch.ToLower()) || r.Id == lgSearch : true).Count(),
                iTotalDisplayRecords = db.EmployeePersonalDatas.Where(e => e.IsActive == false).OrderByDescending(m => m.Id).AsEnumerable()
            .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.FullName.ToLower().Contains(sSearch.ToLower()) || r.Id == lgSearch : true).Count()
            };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult RequestListOld(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            long lgSearch;
            long.TryParse(sSearch, out lgSearch);
            var result2 = new
            {
                sEcho = sEcho,
                aaData = db.DeliveryRequests.OrderByDescending(m => m.Id).AsEnumerable()
            .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || /*r.APPROVE_FLAG.ToLower().Contains(sSearch.ToLower()) ||*/ r.Id == lgSearch : true)
            .Where(d => !d.IsConfirmed == false && d.IsDeleted == false)
            .Select(l => new DeliveryRequest
            {
                Id = l.Id,
                CardId = l.CardId,
                Phone1 = l.Phone1,
                Time = l.Time,
                CreatedDate = l.CreatedDate,
                //EmployeePersonalData = l.EmployeePersonalData,
            }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.DeliveryRequests.AsEnumerable()
           .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Id == lgSearch : true)
            .Where(d => !d.IsConfirmed == false && d.IsDeleted == false).Count(),
                iTotalDisplayRecords = db.DeliveryRequests.AsEnumerable()
           .Where(r => sSearch != "" ? r.CardId.Contains(sSearch) || r.Id == lgSearch : true)
            .Where(d => !d.IsConfirmed == false && d.IsDeleted == false).Count()
            };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DelieveryDetails(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryRequest model = db.DeliveryRequests.Where(i => i.Id == id).
                Include(x => x.DeliveryRequestDetails)
                .Include(x => x.Country).Include(x => x.Region).Include(x => x.Serv_Providers1)
                .Include(x => x.SERV_PROVIDERS_NEW).Include(x => x.EmployeePersonalData).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmOrder(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryRequest model = db.DeliveryRequests.Where(i => i.Id == id).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            model.IsConfirmed = true;
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("IndexAdmin");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult AddEmployee(string cardId = null)
        {
            var usr = User.Identity.GetUserId();
            var Country = db.Countries.ToList();

            var subCards = CardList(usr);

            var cards = subCards.Select(c => new
            {
                CardIDValue = c.CARD_ID,
                CardIdString = c.CARD_ID
            }).ToList();
            SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
            ViewBag.Cardslist = Cardlist;

            SelectList Countrylist = new SelectList(Country, "Id", "ArName");
            ViewBag.Country = Countrylist;
            if (cardId == null)
            {
                var CardID = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                ViewBag.haveChronic = db.Med_Card.Where(c => c.CARD_NO == CardID && c.LOOK_01 == 0).FirstOrDefault() == null ? false : true;
                var exist = db.EmployeeDatas.Where(e => e.CARD_ID == CardID).FirstOrDefault();
                ViewBag.CardId = CardID;
                if (exist != null)
                {
                    ViewBag.Status = "تم تسجل بيانات هذا الكارت من قبل يمكنك التعديل علي هذه البيانات او الخروج";
                    return View(exist);
                }
                return View(exist);
            }
            else
            {
                ViewBag.haveChronic = db.Med_Card.Where(c => c.CARD_NO == cardId && c.LOOK_01 == 0).FirstOrDefault() == null ? false : true;
                var exist = db.EmployeeDatas.Where(e => e.CARD_ID == cardId).FirstOrDefault();
                ViewBag.CardId = cardId;
                if (exist != null)
                {
                    ViewBag.Status = "تم تسجل بيانات هذا الكارت من قبل يمكنك التعديل علي هذه البيانات او الخروج";
                    return View(exist);
                }
                exist = new EmployeeData();
                exist.CARD_ID = cardId;
                return View(exist);
            }
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult AddEmployee(EmployeeData employeeData)
        {

            if (!ModelState.IsValid)
            {
                var usr = User.Identity.GetUserId();

                var subCards = CardList(usr);

                var cards = subCards.Select(c => new
                {
                    CardIDValue = c.CARD_ID,
                    CardIdString = c.CARD_ID
                }).ToList();
                SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
                ViewBag.Cardslist = Cardlist;
                ViewBag.haveChronic = db.Med_Card.Where(c => c.CARD_NO == employeeData.CARD_ID && c.LOOK_01 == 0).FirstOrDefault() == null ? false : true;
                var Country = db.Countries.ToList();
                SelectList Countrylist = new SelectList(Country, "Id", "ArName");
                ViewBag.Country = Countrylist;
                return View(employeeData);
            }
            //employeeData.CARD_ID = db.EmployeePersonalDatas.Where(e => e.UserId ==usr).FirstOrDefault().CardId;
            EmployeeData exitEmployee = db.EmployeeDatas.AsNoTracking().Where(e => e.CARD_ID == employeeData.CARD_ID).FirstOrDefault();
            if (exitEmployee != null)
            {
                exitEmployee = employeeData;
                db.Entry(employeeData).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("~/Employee/Success");
            }
            db.EmployeeDatas.Add(employeeData);
            db.SaveChanges();
            return Redirect("~/Employee/Success");
        }


        [Authorize(Roles = "User")]
        public ActionResult AddEmployeePartial(string cardId = null)
        {
            var usr = User.Identity.GetUserId();
            var Country = db.Countries.ToList();

            var subCards = CardList(usr);

            var cards = subCards.Select(c => new
            {
                CardIDValue = c.CARD_ID,
                CardIdString = c.CARD_ID
            }).ToList();
            SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
            ViewBag.Cardslist = Cardlist;

            SelectList Countrylist = new SelectList(Country, "Id", "ArName");
            ViewBag.Country = Countrylist;
            if (cardId == null)
            {
                var CardID = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                ViewBag.haveChronic = db.Med_Card.Where(c => c.CARD_NO == CardID && c.LOOK_01 == 0).FirstOrDefault() == null ? false : true;
                var exist = db.EmployeeDatas.Where(e => e.CARD_ID == CardID).FirstOrDefault();
                ViewBag.CardId = CardID;
                if (exist != null)
                {
                    ViewBag.Status = "تم تسجل بيانات هذا الكارت من قبل يمكنك التعديل علي هذه البيانات او الخروج";
                    return PartialView("_AddEmployee", exist);
                }
                return PartialView("_AddEmployee", exist);
            }
            else
            {
                ViewBag.haveChronic = db.Med_Card.Where(c => c.CARD_NO == cardId && c.LOOK_01 == 0).FirstOrDefault() == null ? false : true;
                var exist = db.EmployeeDatas.Where(e => e.CARD_ID == cardId).FirstOrDefault();
                ViewBag.CardId = cardId;
                if (exist != null)
                {
                    ViewBag.Status = "تم تسجل بيانات هذا الكارت من قبل يمكنك التعديل علي هذه البيانات او الخروج";
                    return PartialView("_AddEmployee", exist);
                }
                exist = new EmployeeData();
                exist.CARD_ID = cardId;
                return PartialView("_AddEmployee", exist);
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Success()
        {

            return View();
        }


        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult MedicalNetwork()
        {

            var usr = User.Identity.GetUserId();
            var subCards = CardList(usr);


            var compclasses = (from comcont in subCards
                               join insclass in db.Insurance_Class
                              on comcont.CLASS_CODE equals insclass.CLASS_CODE
                               select new
                               {
                                   ClassCode = comcont.CLASS_CODE,
                                   ClassString = comcont.CLASS_CODE + " | " + insclass.CLASS_ANAME
                               }).ToList().Distinct();

            SelectList compclasseslist = new SelectList(compclasses, "ClassCode", "ClassString");
            ViewBag.compclasseslist = compclasseslist;
            var cards = subCards.Select(c => new
            {
                CardIDValue = c.CARD_ID,
                CardIdString = c.CARD_ID
            }).ToList();
            SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
            ViewBag.Cardslist = Cardlist;

            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;

            var address = db.Basic_Data.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_CODE", "BS_ANAME");
            ViewBag.address = addresslist;

            //var subStr= SUBSTR(CardId, INSTR(CardId, '-', 1, 3) + 1, 2));
            return View();
        }



        //public ActionResult DeliveryService2()
        //{
        //    var userId = User.Identity.GetUserId();
        //    var CardId = db.EmployeePersonalDatas.Where(e => e.UserId == userId).FirstOrDefault().CardId;
        //    var medCard = db.Med_Card.AsNoTracking().Where(m => m.CARD_NO == CardId).OrderByDescending(x => x.CREATED_DATE).FirstOrDefault();

        //    if (medCard == null)
        //    {
        //        ViewBag.hasMed = false;
        //        return View();

        //    }
        //    ViewBag.hasMed = true;
        //    var Country = db.Countries.ToList();
        //    SelectList Countrylist = new SelectList(Country, "Id", "ArName");
        //    ViewBag.Country = Countrylist;
        //    var Providers = db.Serv_Providers1.Where(m => m.PRV_TYPE == 2).ToList();
        //    SelectList ProviderListlist = new SelectList(Providers, "Id", "PR_ANAME");
        //    ViewBag.Provider = ProviderListlist;
        //    return View();
        //}
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult DeliveryService2()
        {
            DeliveryRequestVm model = new DeliveryRequestVm();
            var userId = User.Identity.GetUserId();
            var CardId = db.EmployeePersonalDatas.Where(e => e.UserId == userId).FirstOrDefault().CardId;
            var medCard2 = db.Med_Card.AsNoTracking().Where(m => m.CARD_NO == CardId).OrderByDescending(x => x.CREATED_DATE).FirstOrDefault();

            if (medCard2 == null)
            {
                ViewBag.hasMed = false;
                return View();

            }
            model.CardId = CardId;
            ViewBag.hasMed = true;
            var Country = db.Countries.ToList();
            SelectList Countrylist = new SelectList(Country, "Id", "ArName");
            ViewBag.Country = Countrylist;
            var Providers = db.Serv_Providers1.Where(m => m.PRV_TYPE == 2).ToList();
            SelectList ProviderListlist = new SelectList(Providers, "Id", "PR_ANAME");
            ViewBag.Provider = ProviderListlist;

            List<ChronicViewModel> data = new List<ChronicViewModel>();
            model.Medicines = new List<ChronicViewModel>();
            var Rosita = db.Roshitas.Where(r => r.CardId == medCard2.CARD_NO && r.Manager == "Doctor_Chronic").Where(x => x.RoshetaType == "11603" || x.RoshetaType == "11602").OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            model.Medicines = db.RoshitaDetails.Where(x => x.RoshitaID == Rosita.Id && x.IsDealed == false && x.TotalUnits != 0)
                        .Join(db.Med_Medicine, d => d.MedicienCode, m => m.MED_CODE, (d, m) => new { d, m })
                        //.Join(db.MedicineDatas, d => d.MedicienCode, m => m.M_CODE, (d, m) => new { d, m })
                        .Where(l => l.m.CARD_NO == CardId)
                        .Select(l => new ChronicViewModel
                        {
                            Id = l.d.Id,
                            MED_CODE = l.d.MedicienCode,
                            MED_NAME = l.d.MedicienName,
                            DOSE = l.d.Dose,
                            MED_DURATION = l.d.Duration,
                            NO_OF_UINT = l.d.TotalUnits,
                            TOTAL_AMT = l.d.Amount,
                            DOSAGE_FORM = l.m.DOSAGE_FORM,
                            UNIT_NO = l.m.UNIT_NO,
                            PACK_SIZE = l.m.PACK_SIZE,
                            UNIT_PRICE = l.m.UNIT_PRICE
                        }).ToList();

            return View(model);
        }

        public JsonResult SaveDeleiveryRequest(DeliveryRequestVm data)
        {
            DeliveryRequest model = new DeliveryRequest();
            model.CreatedBy = User.Identity.Name;
            model.CreatedDate = DateTime.Now;
            model.IsConfirmed = false;
            model.IsDeleted = false;
            model.Phone1 = data.Phone1;
            model.Phone2 = data.Phone2;
            model.NeighborHood = data.NeighborHood;
            model.StreetName = data.StreetName;
            model.FlatNo = data.FlatNo;
            model.BuldingNo = data.BuldingNo;
            model.CountryId = data.CountryId;
            model.RegionId = data.RegionId;
            model.Time = data.Time;
            model.ProviderId = data.ProviderId;
            model.BranchId = data.BranchId;
            model.CardId = data.CardId;
            var usrId = User.Identity.GetUserId();
            var employeeDataId = db.EmployeePersonalDatas.Where(e => e.UserId == usrId).FirstOrDefault().Id;
            model.EmployeeDataId = employeeDataId;
            foreach (var item in data.Medicines)
            {
                model.DeliveryRequestDetails.Add(new DeliveryRequestDetail
                {
                    Amount = item.TOTAL_AMT,
                    MedicineCode = item.MED_CODE,
                    MedicineName = item.MED_NAME,
                    DosageForm = item.DOSAGE_FORM

                });
            }
            db.DeliveryRequests.Add(model);
            db.SaveChanges();
            return Json("OK");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult DeliveryService()
        {
            var userId = User.Identity.GetUserId();
            var CardId = db.EmployeePersonalDatas.Where(e => e.UserId == userId).FirstOrDefault().CardId;
            var medCard = db.Med_Card.AsNoTracking().Where(m => m.CARD_NO == CardId).OrderByDescending(x => x.CREATED_DATE).FirstOrDefault();

            if (medCard == null)
            {
                ViewBag.hasMed = false;
                return View();

            }
            ViewBag.hasMed = true;
            var Country = db.Countries.ToList();
            SelectList Countrylist = new SelectList(Country, "Id", "ArName");
            ViewBag.Country = Countrylist;
            var Providers = db.Serv_Providers1.Where(m => m.PRV_TYPE == 2).ToList();
            SelectList ProviderListlist = new SelectList(Providers, "Id", "PR_ANAME");
            ViewBag.Provider = ProviderListlist;
            return View();
        }


        //[Authorize(Roles = "User")]
        //[HttpPost]
        //public ActionResult DeliveryService(DeliveryRequestVm deliveryRequestVm)
        //{



        //    var Country = db.Countries.ToList();
        //    SelectList Countrylist = new SelectList(Country, "Id", "ArName");
        //    ViewBag.Country = Countrylist;
        //    var Providers = db.Serv_Providers1.Where(m => m.PRV_TYPE == 2).ToList();
        //    SelectList ProviderListlist = new SelectList(Providers, "Id", "PR_ANAME");
        //    ViewBag.Provider = ProviderListlist;
        //    ViewBag.hasMed = true;
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.hasMed = true;


        //        return View(deliveryRequestVm);
        //    }
        //    DeliveryRequest deliveryRequest = new DeliveryRequest();
        //    if (deliveryRequestVm.ImageFile != null)
        //    {


        //        string fileName = Path.GetFileNameWithoutExtension(deliveryRequestVm.ImageFile.FileName);
        //        string extension = Path.GetExtension(deliveryRequestVm.ImageFile.FileName);
        //        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
        //        deliveryRequest.Roshita = "~/Content/EmployeesRoshita/" + fileName;
        //        fileName = Path.Combine(Server.MapPath("~/Content/EmployeesRoshita/"), fileName);
        //    }

        //    var usrId = User.Identity.GetUserId();
        //    var employeeDataId = db.EmployeePersonalDatas.Where(e => e.UserId == usrId).FirstOrDefault().Id;

        //    deliveryRequest.EmployeeDataId = employeeDataId;
        //    deliveryRequest.CreatedBy = User.Identity.GetUserName();
        //    deliveryRequest.CreatedDate = DateTime.Now;
        //    deliveryRequest.IsDeleted = false;
        //    deliveryRequest.CountryId = deliveryRequestVm.CountryId;
        //    deliveryRequest.RegionId = deliveryRequestVm.RegionId;
        //    deliveryRequest.NeighborHood = deliveryRequestVm.NeighborHood;
        //    deliveryRequest.StreetName = deliveryRequestVm.StreetName;
        //    deliveryRequest.BuldingNo = deliveryRequestVm.BuldingNo;
        //    deliveryRequest.FlatNo = deliveryRequestVm.FlatNo;
        //    deliveryRequest.Phone1 = deliveryRequestVm.Phone1;
        //    deliveryRequest.Phone2 = deliveryRequestVm.Phone2;
        //    deliveryRequest.ProviderId = deliveryRequestVm.ProviderId;
        //    deliveryRequest.BranchId = deliveryRequestVm.BranchId;
        //    deliveryRequest.Time = deliveryRequestVm.Time;

        //    db.DeliveryRequests.Add(deliveryRequest);
        //    db.SaveChanges();
        //    ViewBag.Status = "تم تسجل البيانات بنجاح";

        //    return View();
        //}

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Approvales(string card = null)
        {
            var usrId = User.Identity.GetUserId();

            var subCards = CardList(usrId);

            var cards = subCards.Select(c => new
            {
                CardIDValue = c.CARD_ID,
                CardIdString = c.CARD_ID
            }).ToList();
            SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
            ViewBag.Cardslist = Cardlist;

            string CardId = card;
            if (card == null)
            {
                CardId = db.EmployeePersonalDatas.Where(e => e.UserId == usrId).FirstOrDefault().CardId;
            }
            var employe = db.Comp_Employees.Where(e => e.CARD_ID == CardId &&
                            DateTime.Now >= e.INS_START_DATE && DateTime.Now <= e.INS_END_DATE)
                           .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();
            if (employe != null)
            {

                DataTable dt = dbAproval.RunReader("SELECT CODE,APROVAL_TYP,MEDICAL_REPLAY,VALUE_AFTER,RECIV_DATE,CREATED_DATE,END_DATE,EXPAIRE_DATE,CREATED_BY FROM MEDICAL_APPROVALS WHERE CARD_NO = '"
                                           + CardId + "' AND CLASS_CODE = '" + employe.CLASS_CODE + "'" +
                                           " AND COMP_CONTRACT_NO = '" + employe.CONTRACT_NO + "' AND EXPAIRE_DATE >= sysdate-14 ORDER BY CREATED_DATE DESC");
                List<Approval> approval = new List<Approval>();
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        approval.Add(new Approval
                        {
                            Code = row["CODE"].ToString(),
                            Approval_Type = row["APROVAL_TYP"].ToString(),
                            Medical_Replay = row["MEDICAL_REPLAY"].ToString(),
                            Value_After = float.Parse(row["VALUE_AFTER"].ToString()),
                            Recieve_Date = row["RECIV_DATE"].ToString(),
                            Created_Date = row["CREATED_DATE"].ToString(),
                            End_Date = row["END_DATE"].ToString(),
                            Expaire_Date = row["EXPAIRE_DATE"].ToString(),
                            CreatedBy = row["CREATED_BY"].ToString()

                        });
                    }
                    //return new JsonResult { Data = approval, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
                ViewBag.CardId = CardId;
                return View(approval);
            }
            else
            {
                List<Approval> approval = new List<Approval>();
                return View(approval);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterEmployeeVM model)
        {
            if (ModelState.IsValid)
            {
                var existEmployeeeData = db.EmployeePersonalDatas.FirstOrDefault(u => u.CardId == model.CardId);
                if (existEmployeeeData != null)
                {
                    ViewBag.Error = "This Card had been Registered Before";
                    return View(model);
                }
                var usernamechick = applicationDb.Users.Where(u => u.UserName == model.UserName).FirstOrDefault();
                if (usernamechick != null)
                {
                    ViewBag.Error = "This User Name had been Registered Before";
                    return View(model);
                }
                var emailchick = applicationDb.Users.Where(u => u.Email == model.Email).FirstOrDefault();
                if (emailchick != null)
                {
                    ViewBag.Error = "This Email had been Registered Before";
                    return View(model);
                }
                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //var compId = model.CardId.Split('-');



                var fullName = model.FullName.Split(' ');

                var status = ChicActiveCard(model.CardId.Split('-')[0], model.CardId);
                if (status.data != "Y" && status.message != "ok")
                {
                    ViewBag.Error = "You Can't Register With This Card As " + status.message;
                    return View(model);
                }
                var employee = db.Comp_Employees.Where(x => x.CARD_ID == model.CardId && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                if ((fullName[0] != employee.EMP_ANAME_ST && fullName[1] != employee.EMP_ANAME_SC && fullName[2] != employee.EMP_ANAME_TH)
                    && (fullName[0] != employee.EMP_ENAME_ST && fullName[1] != employee.EMP_ENAME_SC && fullName[2] != employee.EMP_ENAME_TH))
                {
                    ViewBag.Error = "You Shoud Enter Name That Belongs To This Card ";
                    return View(model);
                }
                if (model.ImageFile != null)
                {

                    string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.NationalIdImage = "~/Content/EmployeeRegister/" + fileName;
                    //fileName = Path.Combine(Server.MapPath("~/Content/EmployeeRegister/"), fileName);
                    model.ImageFile.SaveAs(Server.MapPath("~/Content/EmployeeRegister/" + fileName));
                }
                var user = new ApplicationUser
                {
                    TypeId = "0",
                    UserName = model.UserName,
                    Email = model.Email,
                    FName = fullName[0],
                    LName = fullName[2],
                    Type = "User",
                    Provider = "0",
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumder1 = model.PhoneNumber
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, "User");
                    foreach (ERPRolesModulesPage page in db.ERPRolesModulesPages.Where(x => x.RoleId == roleId))
                    {
                        ERPUsersModulesPage Newmodel = new ERPUsersModulesPage();
                        Newmodel.PageId = page.PageId;
                        Newmodel.UserId = user.Id;
                        Newmodel.CreatedBy = User.Identity.Name;
                        Newmodel.CreatedDate = DateTime.Now;
                        Newmodel.FullControl = page.FullControl;
                        Newmodel.EditPermission = page.EditPermission;
                        Newmodel.AddPermission = page.AddPermission;
                        Newmodel.ActivationControl = page.ActivationControl;
                        Newmodel.Preview = page.Preview;
                        db.ERPUsersModulesPages.Add(Newmodel);
                    }
                    var employeeData = new EmployeePersonalData
                    {
                        FullName = model.FullName,
                        NationalId = model.NationalId,
                        BirthDate = model.BirthDate,
                        CardId = model.CardId,
                        UserId = user.Id,
                        CreatedDate = DateTime.Now,
                        IsActive = false,
                        NationalIdImage = model.NationalIdImage
                    };
                    db.EmployeePersonalDatas.Add(employeeData);
                    db.SaveChanges();
                    // await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    //Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    var employeeActive = db.Comp_Employees.Where(e => e.CARD_ID == model.CardId && e.INS_START_DATE <= DateTime.Now && e.INS_END_DATE >= DateTime.Now)
                        .OrderByDescending(o => o.CONTRACT_NO).FirstOrDefault();
                    if (employeeActive != null)
                    {
                        employeeActive.DEPT_ID = 1;
                        db.Entry(employeeActive).State = EntityState.Modified;
                        int resultSave = db.SaveChanges();
                        if (resultSave > 0)
                        {
                            var x = dbAproval.RunNonQuery(@" update DMS_TEST.COMP_EMPLOYEES set DEPT_ID ='1' where  CARD_ID='" + model.CardId.Trim() + "' and CONTRACT_NO=(select max(CONTRACT_NO) from COMP_EMPLOYEES e where C_COMP_ID='" + model.CardId.Split('-')[0] + "')");
                        }
                    }


                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult ConfirmRegister(ConfirmRegisterEmployeeVM model)
        {
            return View(model);
        }

        // POST: /Account/ConfirmRegister
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmRegisterPost(ConfirmRegisterEmployeeVM model)
        {
            if (ModelState.IsValid)
            {
                var existEmployeeeData = db.EmployeePersonalDatas.FirstOrDefault(u => u.CardId == model.CardId);
                if (existEmployeeeData != null)
                {
                    ViewBag.Error = "This Card had been Registered Before";
                    return View(model);
                }
                var usernamechick = applicationDb.Users.Where(u => u.UserName == model.UserName).FirstOrDefault();
                if (usernamechick != null)
                {
                    ViewBag.Error = "This User Name had been Registered Before";
                    return View(model);
                }
                var emailchick = applicationDb.Users.Where(u => u.Email == model.Email).FirstOrDefault();
                if (emailchick != null)
                {
                    ViewBag.Error = "This Email had been Registered Before";
                    return View(model);
                }
                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //var compId = model.CardId.Split('-');



                var fullName = model.FullName.Split(' ');

                var status = ChicActiveCard(model.CardId.Split('-')[0], model.CardId);
                if (status.data != "Y" && status.message != "ok")
                {
                    ViewBag.Error = "You Can't Register With This Card As " + status.message;
                    return View(model);
                }
                var user = new ApplicationUser
                {
                    TypeId = "0",
                    UserName = model.UserName,
                    Email = model.Email,
                    FName = fullName[0],
                    LName = fullName[2],
                    Type = "User",
                    Provider = "0",
                    EmailConfirmed = true,

                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, "User");
                    var employeeData = new EmployeePersonalData
                    {
                        FullName = model.FullName,
                        CardId = model.CardId,
                        UserId = user.Id,
                        BirthDate = DateTime.Now,
                        NationalId = "Empty",
                        CreatedDate = DateTime.Now,
                        IsActive = false,
                    };
                    db.EmployeePersonalDatas.Add(employeeData);
                    db.SaveChanges();
                    var _ERPRolesUsersPages = db.ERPUsersModulesPages.Where(x => x.UserId == user.Id)
                         .Join(db.ERPModulesPages, rmp => rmp.PageId, mp => mp.Id, (rmp, mp) => new { rmp, mp })
                         .Select(l => new ModulesPagesViewModel
                         {
                             ModuleName = l.mp.ERPModule.Name,
                             PageName = l.mp.Name,
                             FullControl = l.rmp.FullControl,
                             Preview = l.rmp.Preview,
                             AddPermission = l.rmp.AddPermission,
                             EditPermission = l.rmp.EditPermission,
                             ActivationControl = l.rmp.ActivationControl,
                             PageId = l.rmp.PageId
                         }).OrderBy(x => x.ModuleName).ToList();
                    string seralize = JsonConvert.SerializeObject(_ERPRolesUsersPages);

                    await UserManager.AddClaimAsync(user.Id, new Claim("SomeClaimType", seralize));
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    bool foundUser = false;
                    bool foundNetworkUser = false;
                    var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == user.Id).FirstOrDefault().CardId;
                    var providerUser = int.Parse(cardId.Split('-')[0]);
                    if (providerUser == 10362)
                    {
                        //Session["IsIndemnity"] = foundUser;
                        Session["IsNetwork"] = foundNetworkUser;
                    }
                    else
                    {
                        foundNetworkUser = true;
                        var CurrentDateUser = DateTime.Now.Date;
                        var companyUser = db.Contract_Data.Where(c => c.C_COMP_ID == providerUser && c.DATE_FROM <= CurrentDateUser
                       && c.DATE_TO >= CurrentDateUser).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                        if (companyUser != null)
                        {
                            var GetServActive = db.COMP_CUSTOMIZED_D.Where(p => p.C_COMP_ID == providerUser && p.CONTRACT_NO == companyUser.CONTRACT_NO
                              && p.SERV_CODE == "12").FirstOrDefault();
                            if (GetServActive != null)
                                foundUser = true;
                        }
                    }

                    Session["IsIndemnity"] = foundUser;
                    Session["IsNetwork"] = foundNetworkUser;
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    //Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    var employeeActive = db.Comp_Employees.Where(e => e.CARD_ID == model.CardId && e.INS_START_DATE <= DateTime.Now && e.INS_END_DATE >= DateTime.Now)
                        .OrderByDescending(o => o.CONTRACT_NO).FirstOrDefault();
                    if (employeeActive != null)
                    {
                        employeeActive.DEPT_ID = 1;
                        db.Entry(employeeActive).State = EntityState.Modified;
                        int resultSave = db.SaveChanges();
                        if (resultSave > 0)
                        {
                            var x = dbAproval.RunNonQuery(@" update DMS_TEST.COMP_EMPLOYEES set DEPT_ID ='1' where  CARD_ID='" + model.CardId.Trim() + "' and CONTRACT_NO=(select max(CONTRACT_NO) from COMP_EMPLOYEES e where C_COMP_ID='" + model.CardId.Split('-')[0] + "')");
                        }
                    }

                    return RedirectToAction("Index", "Employee");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [Authorize(Roles = "User")]
        public ActionResult IndexIndemnity()
        {

            return View(db.IndemnityMasters.Where(x => x.CreatedBy == User.Identity.Name && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList());
        }

        [Authorize(Roles = "User")]
        // GET: Indemnities/Create
        public ActionResult CreateAdmin()
        {
            var usr = User.Identity.GetUserId();

            var subCards = CardList(usr);

            var cards = subCards.Select(c => new
            {
                CardIDValue = c.CARD_ID,
                CardIdString = c.CARD_ID
            }).ToList();
            SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
            ViewBag.Cardslist = Cardlist;
            var CardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
            var CompId = CardId.Split('-')[0];
            ViewBag.UserCompId = CompId;

            ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
            ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");

            IndemnityMasterVm indemnityVMs = new IndemnityMasterVm
            {
                IndemnityCardsImages = new IndemnityVM[20],
                ServiceTest = new IndemnityServiceVm[200],
            };
            indemnityVMs.RelatedCardId = CardId;
            ViewBag.error = "0";
            return View(indemnityVMs);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(IndemnityMasterVm IndemnityMaster)
        {

            // General Object
            IndemnityMaster model = new IndemnityMaster();
            model.NationalID = IndemnityMaster.NationalId;
            model.Name = IndemnityMaster.Name;
            model.RelatedCardId = IndemnityMaster.RelatedCardId;
            model.Type = 1;
            model.BankName = IndemnityMaster.BankName;
            model.BankBranch = IndemnityMaster.BankBranch;
            model.BankAccount = IndemnityMaster.BankAccount;
            model.CompanyName = IndemnityMaster.CompanyName;
            model.CardId = IndemnityMaster.RelatedCardId;
            model.CreatedBy = User.Identity.Name;
            model.CreatedDate = DateTime.Now;
            model.Status = "Pending";
            model.IsDeleted = false;


            List<string> paths = new List<string>();
            List<string> exten = new List<string>();
            HttpPostedFileBase[] ImageFiles = new HttpPostedFileBase[100];

            // loop for services and list of each service file 
            // item is service object that contain IndemnityServiceMaster object 'viewModel'

            foreach (var item in IndemnityMaster.ServiceTest)
            {

                IndemnityServiceMaster viewmodel = new IndemnityServiceMaster();
                if (item.AttachPDF[0] != null)
                {
                    for (int i = 0; i < item.AttachPDF.Length; i++)
                    {
                        IndemnityServiceFile serviceFile = new IndemnityServiceFile();
                        string extension = Path.GetExtension(item.AttachPDF[i].FileName);
                        exten.Add(extension);
                        string fileName = item.ServiceCardId + "_" + item.SpecialistId + "_" + item.ServiceId + "_" + i + extension;
                        paths.Add("~/Content/IndemnitiesAttachments/" + fileName);
                        item.AttachPDF[i].SaveAs(Server.MapPath("/Content/IndemnitiesAttachments/" + fileName /*ImageFile.FileName*/));
                        serviceFile.ServicePhotoCard = item.ServiceCardId;
                        serviceFile.FilePath = "~/Content/IndemnitiesAttachments/" + fileName;
                        viewmodel.IndemnityServiceFiles.Add(serviceFile);
                        //item.AttachPDF[i].FileName.Replace(item.AttachPDF[i].FileName,fileName);

                    }
                }
                else
                {
                    var usr1 = User.Identity.GetUserId();

                    var subCards1 = CardList(usr1);

                    var cards1 = subCards1.Select(c => new
                    {
                        CardIDValue = c.CARD_ID,
                        CardIdString = c.CARD_ID
                    }).ToList();
                    SelectList Cardlist1 = new SelectList(cards1, "CardIDValue", "CardIdString");
                    ViewBag.Cardslist = Cardlist1;
                    var CardId1 = db.EmployeePersonalDatas.Where(e => e.UserId == usr1).FirstOrDefault().CardId;
                    var CompId1 = CardId1.Split('-')[0];
                    ViewBag.UserCompId = CompId1;

                    ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
                    ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");

                    IndemnityMaster.RelatedCardId = CardId1;
                    ViewBag.error = "0";
                    return View(IndemnityMaster);
                }
                viewmodel.ServiceCard = item.ServiceCardId;
                viewmodel.SpecialistId = item.SpecialistId;
                viewmodel.ServiceId = item.ServiceId;
                viewmodel.Value = item.Value;
                viewmodel.ServiceDate = item.ServiceDate;

                //item.AttachPDF.CopyTo(ImageFiles, ImageFiles.Length);

                model.IndemnityServiceMasters.Add(viewmodel);

            }

            foreach (var item in IndemnityMaster.IndemnityCardsImages)
            {
                int j = 0;
                if (item.NationalIdPDF[0] != null)
                {
                    for (int i = 0; i < item.NationalIdPDF.Length; i++)
                    {
                        IndemnityNationalFile nationFile = new IndemnityNationalFile();
                        string extension = Path.GetExtension(item.NationalIdPDF[i].FileName);
                        exten.Add(extension);
                        string fileName = item.PhotoCardId + "_" + j++ + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        paths.Add("~/Content/IndemnitiesAttachments/" + fileName);
                        item.NationalIdPDF[i].SaveAs(Server.MapPath("/Content/IndemnitiesAttachments/" + fileName /*ImageFile.FileName*/));
                        nationFile.PhotoCardId = item.PhotoCardId;
                        nationFile.ImgUrl = "~/Content/IndemnitiesAttachments/" + fileName;
                        model.IndemnityNationalFiles.Add(nationFile);
                        item.NationalIdPDF[i].FileName.Replace(item.NationalIdPDF[i].FileName, item.PhotoCardId);

                    }
                }
                else
                {
                    var usr1 = User.Identity.GetUserId();

                    var subCards1 = CardList(usr1);

                    var cards1 = subCards1.Select(c => new
                    {
                        CardIDValue = c.CARD_ID,
                        CardIdString = c.CARD_ID
                    }).ToList();
                    SelectList Cardlist1 = new SelectList(cards1, "CardIDValue", "CardIdString");
                    ViewBag.Cardslist = Cardlist1;
                    var CardId1 = db.EmployeePersonalDatas.Where(e => e.UserId == usr1).FirstOrDefault().CardId;
                    var CompId1 = CardId1.Split('-')[0];
                    ViewBag.UserCompId = CompId1;

                    ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
                    ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");

                    IndemnityMaster.RelatedCardId = CardId1;
                    ViewBag.error = "0";
                    return View(IndemnityMaster);
                }

            }


            db.IndemnityMasters.Add(model);
            db.SaveChanges();

            var userid = User.Identity.GetUserId();
            var Hospitalprovider = UserManager.FindById(userid);
            string sub = @"Request Indemnity From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
            string msg = @"<h3> Request Number  : </h3>" + model.Id + "<br/>";
            if (model.Type == 1)
            {
                msg += "<h3> Employee Name: </h3> " + model.Name + " <br/> " +
                "<h3> NationalId : </h3 > " + model.NationalID + " <br/> ";

            }
            else
            {
                msg += "<h3> Company Name : </h3> " + model.CompanyName + " <br/> ";
            }
            msg += "<h3> Bank Name: </h3> " + model.BankName + " <h3> Bank Branch: </h3>  " + model.BankBranch +
                " <h3> Bank Account: </h3> " + model.BankAccount + " <br/> " +
                "<h3>Take the following Services : </ h3 > <br/> ";
            foreach (var item in model.IndemnityServiceMasters)
            {
                msg += "<h3> CardId : </h3> " + item.ServiceCard + " <h3> SpecialistId : </h3>  " + item.SpecialistId +
                     "<h3> ServiceId : </h3> " + item.ServiceId + " <h3> Service Value: </h3>  " + item.Value +
                      "<h3> Service Date : </h3> " + String.Format("{0: d MMMM  yyyy}", item.ServiceDate) + " <br/> ";
            }
            msg += "<h3> Replay Email : </h3> " + IndemnityMaster.Email + " <br/> ";

            AlternateView altView = AlternateView.CreateAlternateViewFromString(msg, null, MediaTypeNames.Text.Html);

            // For Dowload Services images and send it 
            foreach (var item in model.IndemnityServiceMasters)
            {
                if (item.IndemnityServiceFiles.Count > 0)
                {
                    int i = 0;
                    foreach (var img in item.IndemnityServiceFiles)
                    {
                        string NameFile = item.ServiceCard + "_" + item.SpecialistId + "_" + item.ServiceId + "_" + (i++);
                        string extension = Path.GetExtension(img.FilePath);
                        var extenti = MediaTypeNames.Application.Pdf;
                        //string path= Server.MapPath()
                        if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                        {
                            extenti = MediaTypeNames.Image.Jpeg;
                        }
                        LinkedResource Img = new LinkedResource(Server.MapPath(img.FilePath), extenti);
                        Img.ContentId = "MyImage" + 0;
                        Img.ContentType.Name = NameFile + extension;
                        altView.LinkedResources.Add(Img);
                        var image = DownloadAttachment(img.FilePath);
                        msg = msg + image + "<br/>";
                    }
                }
            }

            // For Dowload National Id images and send it 
            if (model.IndemnityNationalFiles.Count > 0)
            {
                foreach (var item in model.IndemnityNationalFiles)
                {

                    string NameFile = item.PhotoCardId + "_" + DateTime.Now.ToString("yymmss");
                    string extension = Path.GetExtension(item.ImgUrl);
                    var extenti = MediaTypeNames.Application.Pdf;
                    //string path= Server.MapPath()
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        extenti = MediaTypeNames.Image.Jpeg;
                    }
                    LinkedResource Img = new LinkedResource(Server.MapPath(item.ImgUrl), extenti);
                    Img.ContentId = "MyImage" + 0;
                    Img.ContentType.Name = NameFile + extension;
                    altView.LinkedResources.Add(Img);
                    var image = DownloadAttachment(item.ImgUrl);
                    msg = msg + image + "<br/>";
                }
            }

            SendMail("indhrrequest@gmail.com", sub, msg, altView/*, Hospitalprovider*/);
            //SendMail("dms.medical2@gmail.com", sub, msg, altView/*, Hospitalprovider*/);

            if (model.Id > 0)
            {
                return RedirectToAction("IndexIndemnity");
            }
            var usr = User.Identity.GetUserId();

            var subCards = CardList(usr);

            var cards = subCards.Select(c => new
            {
                CardIDValue = c.CARD_ID,
                CardIdString = c.CARD_ID
            }).ToList();
            SelectList Cardlist = new SelectList(cards, "CardIDValue", "CardIdString");
            ViewBag.Cardslist = Cardlist;
            var CardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
            var CompId = CardId.Split('-')[0];
            ViewBag.UserCompId = CompId;

            ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
            ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");

            IndemnityMasterVm indemnityVMs = new IndemnityMasterVm
            {
                IndemnityCardsImages = new IndemnityVM[20],
                ServiceTest = new IndemnityServiceVm[200],
            };
            indemnityVMs.RelatedCardId = CardId;
            ViewBag.error = "0";
            return View(indemnityVMs);
        }

        [Authorize(Roles = "User")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndemnityMaster indemnity = db.IndemnityMasters.Where(i => i.Id == id).
                Include(x => x.IndemnityServiceMasters).FirstOrDefault();
            if (indemnity == null)
            {
                return HttpNotFound();
            }
            return View(indemnity);
        }



        // GET: Indemnities/Delete/5
        [Authorize(Roles = "User")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndemnityMaster indemnity = db.IndemnityMasters.Find(id);
            if (indemnity == null)
            {
                return HttpNotFound();
            }
            return View(indemnity);
        }

        // POST: Indemnities/Delete/5
        [Authorize(Roles = "User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndemnityMaster indemnity = db.IndemnityMasters.Find(id);
            if (indemnity == null)
            {
                return HttpNotFound();
            }
            indemnity.IsDeleted = true;
            db.Entry(indemnity).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("IndexIndemnity");
        }

        #endregion

        #region Json

        public ActionResult Download(long id)
        {
            var files = db.IndemnityServiceFiles.Where(x => x.ServiceId == id).Select(x => x.FilePath).ToList();
            var masterid = db.IndemnityServiceMasters.Where(s => s.Id == id).FirstOrDefault().MasterIdFK;
            var nationalFiles = db.IndemnityNationalFiles.Where(n => n.MasterIdFK == masterid).Select(na => na.ImgUrl).ToList();

            List<string> Files = new List<string>();

            foreach (var item in files)
            {
                Files.Add((Server.MapPath(item)));
            }
            foreach (var item in nationalFiles)
            {
                Files.Add((Server.MapPath(item)));
            }
            var archive = Server.MapPath("~/Content/IndemnitiesAttachments/archive.zip");
            var temp = Server.MapPath("~/Content/IndemnitiesAttachments/temp");

            // clear any existing archive
            if (System.IO.File.Exists(archive))
            {
                System.IO.File.Delete(archive);
            }
            // empty the temp folder
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

            // copy the selected files to the temp folder
            Files.ForEach(f => System.IO.File.Copy(f, Path.Combine(temp, Path.GetFileName(f))));

            // create a new archive
            ZipFile.CreateFromDirectory(temp, archive);

            return File(archive, "application/zip", "archive.zip");
        }

        public JsonResult ChickServiceDate(string id, string servdate)
        {
            DateTime date = DateTime.Parse(servdate);
            var employe = db.Comp_Employees.Where(e => e.CARD_ID == id &&
                            date >= e.INS_START_DATE && date <= e.INS_END_DATE)
                           .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();

            if (employe != null)
            {
                var result = new { Success = "Yes" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = new { Success = "No" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetStateList(int? id)
        {

            db.Configuration.ProxyCreationEnabled = false;
            var Region = db.Regions.Where(r => r.GovernmentId == id).ToList();
            SelectList Regionlist = new SelectList(Region, "Id", "ArName");

            return Json(Regionlist, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApprovalCode"></param>
        /// <returns> PDF file of the approval </returns>

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult PrintApproval(string id)
        {

            DataTable dts = dbAproval.RunReader("SELECT CARD_NO,DIAG_NAME,SERVECE_TYP,CREATED_BY FROM MEDICAL_APPROVALS WHERE CODE='" + id + "'");
            string cardNum = dts.Rows[0][0].ToString();
            DataTable dtsrev = dbAproval.RunReader(@"select APPROVAL_SUB_SERV.S_SERV_NAME, CASE WHEN (S_SERV_NAME = 'الاقامة'OR S_SERV_CODE LIKE '114%') 
                                                                        THEN CONCAT(APPROVAL_SUB_SERV.DETAILS, APPROVAL_SUB_SERV.DISCRIPTION) ELSE APPROVAL_SUB_SERV.DETAILS END
                                                             FROM  APPROVAL_SUB_SERV WHERE APPROVAL_SUB_SERV.CODE = '" + id + "'");
            DataTable dtdiag = dbAproval.RunReader(@"select MEDICAL_APPROVALS.APROVAL_IMAG, APPROVAL_DIAG.DIAG_NAME, MEDICAL_APPROVALS.PATH FROM      MEDICAL_APPROVALS, APPROVAL_DIAG
                                                              WHERE     MEDICAL_APPROVALS.CODE = APPROVAL_DIAG.CODE  AND MEDICAL_APPROVALS.CODE = '" + id + "' ");
            string DServ_Details = "";
            string DDiag_Details = "";

            if (dtsrev.Rows.Count != 0)
            {
                for (int i = 0; i < dtsrev.Rows.Count; i++)
                    DServ_Details = DServ_Details + dtsrev.Rows[i][0].ToString() + " : " + dtsrev.Rows[i][1].ToString() + "\n";
            }
            try
            {
                if (dtdiag.Rows.Count != 0)
                {
                    for (int i = 0; i < dtdiag.Rows.Count; i++)
                    {
                        DDiag_Details = DDiag_Details + dtdiag.Rows[i][1].ToString() + "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Hospital"), "ReportApproval.rpt"));

            rd.SetDatabaseLogon("APP", "12369");

            rd.SetParameterValue("crd", cardNum);
            rd.SetParameterValue("cod", id);
            rd.SetParameterValue("diag", DDiag_Details);
            rd.SetParameterValue("service", DServ_Details);
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
                return File(stream, "application/pdf", DateTime.Now.ToString("ddMMyyyy") + "MedicalApproval.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Help Class

        public ReturnResult ChicActiveCard(string compid, string CardId)
        {
            int CompId = Convert.ToInt32(compid);
            try
            {
                var emp = db.Contract_Comp.Where(x => x.C_COMP_ID == CompId).FirstOrDefault().ACTIVE;

                var empCardTerminationFlag = db.Comp_Employees.Where(x => x.CARD_ID == CardId && x.INS_START_DATE <= DateTime.Now && x.INS_END_DATE >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                if (empCardTerminationFlag != null)
                {
                    if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "N")
                    {
                        return (new ReturnResult { data = "Y", message = "ok" });
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE > DateTime.Now)
                    {
                        return (new ReturnResult { data = "Y", message = "ok" });
                    }
                    else if (emp == "Y" && empCardTerminationFlag.TERMINATE_FLAG == "Y" && empCardTerminationFlag.TERMINATE_DATE < DateTime.Now)
                    {
                        return (new ReturnResult { data = "N", message = "Expired Card" });
                    }
                }
                else
                {
                    var CompTerminationFlag = db.Contract_Data.Where(x => x.C_COMP_ID == CompId && x.DATE_FROM <= DateTime.Now && x.DATE_TO >= DateTime.Now).OrderByDescending(x => x.CONTRACT_NO).FirstOrDefault();
                    if (CompTerminationFlag != null)
                    {
                        return (new ReturnResult { data = "N", message = "Card is not existed" });

                    }
                }
                return (new ReturnResult { data = "N", message = "Expired Company" });
            }
            catch (Exception ex)
            {
                return (new ReturnResult { data = "EX", message = ex.Message });
            }
        }

        public class ReturnResult
        {
            public string data { get; set; }
            public string message { get; set; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion

        #region Help Functions

        public JsonResult ChangeStatus(int Id, string status)
        {
            var employeedata = db.EmployeePersonalDatas.Where(x => x.Id == Id).FirstOrDefault();
            int result = -2;
            int flag = 0;
            if (status == "true")
            {
                employeedata.IsActive = true;
                flag = 1;
            }
            else
            {
                employeedata.IsActive = false;
                flag = 0;
            }
            db.Entry(employeedata).State = EntityState.Modified;
            result = db.SaveChanges();
            if (result > 0)
            {
                var x = dbAproval.RunNonQuery(@" update DMS_TEST.COMP_EMPLOYEES set DEPT_ID ='" + flag + "' where  CARD_ID='" + employeedata.CardId.Trim() + "' and CONTRACT_NO=(select max(CONTRACT_NO) from COMP_EMPLOYEES e where C_COMP_ID='" + employeedata.CardId.Split('-')[0] + "')");
            }
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public List<Comp_Employees> CardList(string userId)
        {
            var CardId = db.EmployeePersonalDatas.Where(e => e.UserId == userId).FirstOrDefault().CardId;
            var EmpCode = long.Parse(CardId.Split('-')[2]);
            var CompId = int.Parse(CardId.Split('-')[0]);
            var Employee = db.Comp_Employees.Where(e => e.CARD_ID == CardId &&
                             DateTime.Now >= e.INS_START_DATE && DateTime.Now <= e.INS_END_DATE)
                            .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();
            if (Employee != null)
            {
                var maxContract = Employee.CONTRACT_NO;
                var subCards = db.Comp_Employees.Where(e => e.EMP_CODE == EmpCode &&
                               (e.TERMINATE_FLAG == "N" || e.TERMINATE_FLAG == null) && e.C_COMP_ID == CompId
                               && e.CONTRACT_NO == maxContract).ToList();
                return subCards;
            }
            else
            {
                return new List<Comp_Employees>();
            }
        }

        public FileResult DownloadAttachment(string FileName)
        {
            var path = System.IO.Path.Combine(Server.MapPath(FileName));
            if (System.IO.File.Exists(path))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            return null;
        }

        public void SendMail(string to, string subject, string Message, AlternateView altView/*, ApplicationUser applicationUser*/)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("hrindemnity@gmail.com" /*EmailAndPassword.Email*/, to, subject, Message);
            mail.AlternateViews.Add(altView);
            System.Net.NetworkCredential mailAuthenticaion = new System.Net.NetworkCredential("hrindemnity@gmail.com", "dms123456"/*EmailAndPassword.Email, EmailAndPassword.Password*/);

            System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            mailclient.EnableSsl = true;
            mailclient.UseDefaultCredentials = false;
            mailclient.Credentials = mailAuthenticaion;
            mail.IsBodyHtml = true;
            mailclient.Send(mail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult DownloadNationaId(long id)
        {
            var files = db.EmployeePersonalDatas.Where(x => x.Id == id).Select(x => x.NationalIdImage).FirstOrDefault();

            string Files = Server.MapPath(files);

            var archive = Server.MapPath("~/Content/EmployeeRegister/archive.zip");
            var temp = Server.MapPath("~/Content/EmployeeRegister/temp");

            // clear any existing archive
            if (System.IO.File.Exists(archive))
            {
                System.IO.File.Delete(archive);
            }
            // empty the temp folder
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));
            System.IO.File.Copy(Files, Path.Combine(temp, Path.GetFileName(Files)));

            // copy the selected files to the temp folder
            //Files.ForEach(f => System.IO.File.Copy(f, Path.Combine(temp, Path.GetFileName(f))));

            // create a new archive
            ZipFile.CreateFromDirectory(temp, archive);

            return File(archive, "application/zip", "archive.zip");
        }
        #endregion

    }
}