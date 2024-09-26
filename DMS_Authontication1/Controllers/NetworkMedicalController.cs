using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.HR
{
    [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class NetworkMedicalController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();
        ApplicationDbContext myEntities = new ApplicationDbContext();

        [HttpGet]
        public ActionResult NetworkMedical(string cardId)
        {
            if (!string.IsNullOrEmpty(cardId))
                ViewBag.CardId = cardId; 
            

                var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;

            var address = db.Basic_Data.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_ENAME", "BS_ANAME");
            ViewBag.address = addresslist;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RequestReplay()
        {
            return View();
        }

        [Authorize(Roles = "HR,HR_Admin")]//admin later
        [HttpGet]
        public ActionResult Search()
        {
            if (User.IsInRole("HR_Admin"))
            {
                ViewBag.CompName = null;
                ViewBag.compclasseslist = null;
                var userid = User.Identity.GetUserId();
                var compines = db.HrAdminCompanies.Where(x => x.UserId == userid).Select(c => c.CompId).ToList();
                if (compines[0] == "All")
                {
                    var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
                else
                {
                    var companyname = (from comp in compines
                                       join contCo in db.Contract_Comp
                                       on int.Parse(comp) equals contCo.C_COMP_ID
                                       select new
                                       {
                                           Code = contCo.C_COMP_ID,
                                           Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
                                       }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
            }
            else
            {

                var HrUserNamre = User.Identity.GetUserName();
                var compcode = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
                ViewBag.CompName = compcode;
                int companyCode = Convert.ToInt32(compcode);

                var maxcontract = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == companyCode).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault().CONTRACT_NO;

                var compclasses = (from comcont in db.CompContractClasses
                                   where comcont.CONTRACT_NO == maxcontract && comcont.C_COMP_ID == companyCode
                                   join insclass in db.Insurance_Class
                                   on comcont.CLASS_CODE equals insclass.CLASS_CODE
                                   select new
                                   {
                                       ClassCode = comcont.CLASS_CODE,
                                       ClassString = comcont.CLASS_CODE + " | " + insclass.CLASS_ANAME
                                   }).ToList();

                SelectList compclasseslist = new SelectList(compclasses, "ClassCode", "ClassString");
                ViewBag.compclasseslist = compclasseslist;
                ViewBag.company = null;
            }

            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;

            var address = db.Basic_Data.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_CODE", "BS_ANAME");
            ViewBag.address = addresslist;
            return View();
        }

        [Authorize(Roles = "HR,HR_Admin,User")]
        [HttpGet]
        public ActionResult CreateRequest()
        {
            RequestAddProvidersVM requestAddProvidersVM = new RequestAddProvidersVM();
            if (User.IsInRole("HR_Admin"))
            {
                var userid = User.Identity.GetUserId();
                var compines = db.HrAdminCompanies.Where(x => x.UserId == userid).Select(c => c.CompId).ToList();
                if (compines[0] == "All")
                {
                    var companyname = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
                else
                {
                    //var companyname = db.Contract_Comp
                    //    .Select(l => new
                    //    {
                    //        Code = l.C_COMP_ID,
                    //        Name = l.C_ENAME + " || " + l.C_COMP_ID

                    //    }).ToList();
                    var companyname = (from comp in compines
                                       join contCo in db.Contract_Comp
                                       on int.Parse(comp) equals contCo.C_COMP_ID
                                       select new
                                       {
                                           Code = contCo.C_COMP_ID,
                                           Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
                                       }).ToList();
                    SelectList companylist = new SelectList(companyname, "Code", "Name");
                    ViewBag.company = companylist;
                }
            }
            else if (User.IsInRole("User"))
            {
                var HrUserNamre = User.Identity.GetUserId();
                requestAddProvidersVM.CompName = db.EmployeePersonalDatas.Where(u => u.UserId == HrUserNamre).FirstOrDefault().CardId.Split('-')[0];
            }
            else
            {
                var HrUserNamre = User.Identity.GetUserName();
                requestAddProvidersVM.CompName = myEntities.Users.Where(u => u.UserName == HrUserNamre).FirstOrDefault().Provider;
            }
            var provider = db.ProviderTypeNews.ToList();
            SelectList Providerlist = new SelectList(provider, "PrvType", "PrvAName");
            ViewBag.provider = Providerlist;

            var address = db.Basic_Data.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            //var address = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == null).ToList();
            SelectList addresslist = new SelectList(address, "BS_ENAME", "BS_ANAME");
            ViewBag.address = addresslist;
            return View(requestAddProvidersVM);
        }

        // GET:  MedicalNetwork/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = (from request in db.RequestAddProviders
                         join provider in db.ProviderTypeNews
                         on request.ProviderType equals provider.ID
                         where request.ID == id && request.IsDeleted == false
                         select new RequestAddProvidersVM
                         {
                             ID = request.ID,
                             CompName = request.CompName,
                             CreatedBy = request.CreatedBy,
                             CreatedDate = request.CreatedDate,
                             Status = request.Status,
                             ReasonRefuse = request.ReasonRefuse,
                             RespobeStatus = request.RespobeStatus,
                             ResponsableFor = request.ResponsableFor,
                             Country = request.Country,
                             Region = request.Region,
                             ServAddress = request.ServAddress,
                             ServName = request.ServName,
                             NumberOfPeople = request.NumberOfPeople,
                             PhoneNumber = request.PhoneNumber,
                             ProviderTypeName = provider.PrvAName
                         }).FirstOrDefault();
            var name = myEntities.Users.Where(u => u.UserName == model.CreatedBy).FirstOrDefault();
            model.CreatedBy = name.FName + " " + name.LName;
            int compId = int.Parse(model.CompName);
            model.CompName = db.Contract_Comp.Where(c => c.C_COMP_ID == compId).FirstOrDefault().C_ANAME;
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        #region Helper Method

        public JsonResult NonReplayRequestList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch = "")
        {
            long lgSearch;
            long.TryParse(sSearch, out lgSearch);
            var result2 = new
            {
                sEcho = sEcho,
                aaData = db.RequestAddProviders.Where(e => e.IsDeleted == false && e.Status == "W").OrderByDescending(m => m.ID).AsEnumerable()
            .Where(r => sSearch != "" ? r.CompName.Contains(sSearch) || r.ID == lgSearch : true)

            .Select(l => new RequestAddProvider
            {
                ID = l.ID,
                CompName = l.CompName,
                ProviderType = l.ProviderType,
                ServName = l.ServName,
                PhoneNumber = l.PhoneNumber,
                NumberOfPeople = l.NumberOfPeople,
                CreatedDate = l.CreatedDate,
                CreatedBy = l.CreatedBy,
            }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.RequestAddProviders.Where(e => e.IsDeleted == false && e.Status == "W").OrderByDescending(m => m.ID).AsEnumerable()
            .Where(r => sSearch != "" ? r.CompName.Contains(sSearch) || r.ID == lgSearch : true).Count(),
                iTotalDisplayRecords = db.RequestAddProviders.Where(e => e.IsDeleted == false && e.Status == "W").OrderByDescending(m => m.ID).AsEnumerable()
            .Where(r => sSearch != "" ? r.CompName.Contains(sSearch) || r.ID == lgSearch : true).Count()
            };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ChangeStatus(string Id, string status)
        {
            var ID = long.Parse(Id);
            var request = db.RequestAddProviders.Where(x => x.ID == ID).FirstOrDefault();
            int result = -2;
            if (status == "Accept")
            {
                request.Status = "Accept";
                request.UpdatedBy = User.Identity.Name;
                request.UpdatedDate = DateTime.Now;
                request.RespobeStatus = " تم قبول الطلب";
            }
            else
            {
                request.Status = status;
                request.UpdatedBy = User.Identity.Name;
                request.UpdatedDate = DateTime.Now;
                request.RespobeStatus = " تم رفض الطلب";
            }
            db.Entry(request).State = EntityState.Modified;
            result = db.SaveChanges();
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetProvidersByClass(string classLevel, string compId, string country, int region, int providerId, string specialistid)
        {
            int Comp_ID = Convert.ToInt32(compId);
            var coverdRelation = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == Comp_ID && c.CLASS_CODE ==
                                        classLevel).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault().COVER_RELATION;
            //var bsCode = db.Basic_Data.Where(b => b.BS_ENAME == country && b.SOURCE_MOD == "M").FirstOrDefault().BS_CODE;
            //var arbicReagonName = db.Basic_Data.Where(r => r.SOURCE_MOD == "M" && r.BS_ENAME == region).FirstOrDefault().BS_CODE;

            if (coverdRelation == 1 || coverdRelation == 4)
            {
                var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid))
                                    .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();

                return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            else if (coverdRelation == 2)
            {
                var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && (p.PROV_DEGREE == coverdRelation.ToString() || p.PROV_DEGREE == "3") && p.TERMINATE_FLAG != "Y" && p.AREA_CODE == region && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid) /*&& p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3"*/ /*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                    .Select(
                s => new
                {
                    s.PR_ANAME,
                    s.ADDRESS1,
                    s.ADDRESS2,
                    s.TEL1,
                    s.TEL2,
                    s.PR_DESC

                }).ToList();

                //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3").ToList();
                return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }

            else
            {
                var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.PROV_DEGREE == coverdRelation.ToString() && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid))
                                    .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();

                //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "3").ToList();
                return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            // var providerList = db.Serv_Providers.Where(p => p.PRV_TYPE == providerId && p.PROV_DEGREE== coverdRelation.ToString() && p.AREA_CODE == arbicReagonName /*&& p.ADDRESS1.Contains(arbicReagonName)*/).ToList();


            // return new JsonResult { Data = new { providerslist = providerList, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult GetProviders(string cardId, string compId, string country, int region, int providerId, string specialistid)
        {
            int Comp_ID = Convert.ToInt32(compId);
            var cardExist = db.Comp_Employees.AsNoTracking().Where(c => c.CARD_ID == cardId).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault();
            if (cardExist == null)
            {
                return new JsonResult { Data = new { providerslist = 0, msg = "Card Not Found ..." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                var coverdRelation = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == Comp_ID && c.CLASS_CODE ==
                                         cardExist.CLASS_CODE && c.CONTRACT_NO == cardExist.CONTRACT_NO).FirstOrDefault().COVER_RELATION;

                var status = ChicActiveCard(compId, cardId);
                if (status.data == "Y" && status.message == "ok")
                {
                    //var bsCode = db.Basic_Data.Where(b => b.BS_ENAME == country && b.SOURCE_MOD == "M").FirstOrDefault().BS_CODE;
                    //var arbicReagonName = db.Basic_Data.Where(r => r.SOURCE_MOD == "M" && r.BS_ENAME == region).FirstOrDefault().BS_CODE;

                    if (coverdRelation == 1 || coverdRelation == 4)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == region && p.TERMINATE_FLAG != "Y" && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                            .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();

                        return new JsonResult { Data = new { providerslist = providerList, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else if (coverdRelation == 2)
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" && (p.PROV_DEGREE == coverdRelation.ToString() || p.PROV_DEGREE == "3") && p.AREA_CODE == region && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid)/*&& p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3"*/ /*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                           .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC

                                                  }).ToList();
                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "2" || p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    else
                    {
                        var providerList = db.SERV_PROVIDERS_NEW.Where(p => p.PRV_TYPE == providerId && p.TERMINATE_FLAG != "Y" && p.PROV_DEGREE == coverdRelation.ToString() && p.AREA_CODE == region && (string.IsNullOrEmpty(specialistid) ? true : p.DOC_SPEC == specialistid) /*&& p.ADDRESS1.Contains(arbicReagonName)*/)
                                            .Select(
                                                  s => new
                                                  {
                                                      s.PR_ANAME,
                                                      s.ADDRESS1,
                                                      s.ADDRESS2,
                                                      s.TEL1,
                                                      s.TEL2,
                                                      s.PR_DESC
                                                  }).ToList();

                        //var providerList = db.Serv_Providers1.Where(p => p.PRV_TYPE == providerId && p.AREA_CODE == bsCode && p.ADDRESS1.Contains(arbicReagonName) && p.PROV_DEGREE == "3").ToList();
                        return new JsonResult { Data = new { providerslist = providerList, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }
                else
                {
                    return new JsonResult { Data = new { providerslist = 0, msg = status.message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }


            }

        }

        [HttpPost]

        public JsonResult SaveRequest(string CompName, string ServName, string ServAddress, string PhoneNumber,
            int NumberOfPeople, int? ProviderType, string Country = null, string Region = null, string ResponsableFor = null)
        {
            RequestAddProvider requestAddProvider = new RequestAddProvider
            {
                CompName = CompName,
                ServName = ServName,
                ServAddress = ServAddress,
                PhoneNumber = PhoneNumber,
                NumberOfPeople = NumberOfPeople,
                ProviderType = ProviderType,
                Country = Country,
                Region = Region,
                ResponsableFor = ResponsableFor,
                RespobeStatus = "لم يتم الرد بعد ",
                CreatedBy = User.Identity.GetUserName(),
                CreatedDate = DateTime.Now,
                IsDeleted = false,
                IsSync = false,
                Status = "W",
                ReasonRefuse = null
            };
            db.RequestAddProviders.Add(requestAddProvider);
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = requestAddProvider.ID;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SearchRequest(long searchId)
        {
            var model = db.RequestAddProviders.Where(r => r.ID == searchId).FirstOrDefault();
            if (model == null)
            {
                return new JsonResult { Data = new { modelreturn = 0, msg = "There's no data for that Request Code : " + searchId }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            RequestAddProvider requestAddProvider = new RequestAddProvider
            {
                CompName = model.CompName,
                ServName = model.ServName,
                ServAddress = model.ServAddress,
                PhoneNumber = model.PhoneNumber,
                NumberOfPeople = model.NumberOfPeople,
                ProviderType = model.ProviderType,
                Country = model.Country,
                Region = model.Region,
                ResponsableFor = model.ResponsableFor,
                RespobeStatus = model.RespobeStatus,
                ID = model.ID,
                Status = model.Status
            };

            return new JsonResult { Data = new { modelreturn = requestAddProvider, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]

        public JsonResult EditRequest(long ReqestID, string CompName, string ServName, string ServAddress, string PhoneNumber,
        int NumberOfPeople, int? ProviderType, string Country = null, string Region = null, string ResponsableFor = null)
        {
            var model = db.RequestAddProviders.AsNoTracking().Where(r => r.ID == ReqestID).FirstOrDefault();
            if (model == null)
            {
                return new JsonResult { Data = new { respcode = 0, msg = "There's no data for that Request Code : " + ReqestID }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            RequestAddProvider requestAddProvider = new RequestAddProvider
            {
                ID = ReqestID,
                CompName = CompName,
                ServName = ServName,
                ServAddress = ServAddress,
                PhoneNumber = PhoneNumber,
                NumberOfPeople = NumberOfPeople,
                ProviderType = ProviderType,
                Country = Country,
                Region = Region,
                ResponsableFor = ResponsableFor,
                RespobeStatus = model.RespobeStatus,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                IsDeleted = model.IsDeleted,
                IsSync = model.IsSync,
                SyncBy = model.SyncBy,
                SyncDate = model.SyncDate,
                UpdatedBy = User.Identity.GetUserName(),
                UpdatedDate = DateTime.Now,
                Status = model.Status,
                ReasonRefuse = model.ReasonRefuse
            };
            db.Entry(requestAddProvider).State = EntityState.Modified;
            int saved = db.SaveChanges();
            if (saved > 0)
            {
                long rescodereturn = requestAddProvider.ID;

                return new JsonResult { Data = new { respcode = rescodereturn, msg = "ok" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { respcode = saved, msg = "Error Save Data ... " }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetClassList(string compId)
        {
            ViewBag.CompName = compId;
            int companyCode = Convert.ToInt32(compId);

            var maxcontract = db.CompContractClasses.AsNoTracking().Where(c => c.C_COMP_ID == companyCode).OrderByDescending(y => y.CONTRACT_NO).FirstOrDefault().CONTRACT_NO;

            var compclasses = (from comcont in db.CompContractClasses
                               where comcont.CONTRACT_NO == maxcontract && comcont.C_COMP_ID == companyCode
                               join insclass in db.Insurance_Class
                               on comcont.CLASS_CODE equals insclass.CLASS_CODE
                               select new
                               {
                                   ClassCode = comcont.CLASS_CODE,
                                   ClassString = comcont.CLASS_CODE + " | " + insclass.CLASS_ANAME
                               }).ToList();

            SelectList compclasseslist = new SelectList(compclasses, "ClassCode", "ClassString");
            ViewBag.compclasseslist = compclasseslist;
            return Json(compclasseslist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSpecialistList()
        {
            var Specialist = db.Basic_Data.Where(b => b.SOURCE_MOD == "DTYP").Select(c => new
            {
                Code = c.BS_CODE,
                Name = c.BS_ANAME
            }).ToList();

            SelectList SpecialistList = new SelectList(Specialist, "Code", "Name");
            //ViewBag.SpecialistList = SpecialistList;
            return Json(SpecialistList, JsonRequestBehavior.AllowGet);
        }

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

        public JsonResult GetStateList(string id)
        {
            myEntities.Configuration.ProxyCreationEnabled = false;

            //var Number = myEntities.BASIC_DATA.Where(m => m.BS_ENAME == id).Select(l => l.BS_CODE).FirstOrDefault();
            var State = myEntities.BASIC_DATA.Where(m => m.SOURCE_MOD == "M" && m.BS_CODE_UP == id).ToList();
            SelectList StateListlist = new SelectList(State, "BS_CODE", "BS_ANAME");
            ViewBag.State = StateListlist;

            return Json(StateListlist, JsonRequestBehavior.AllowGet);
        }

        public class ReturnResult
        {
            public string data { get; set; }
            public string message { get; set; }
        }

        public class ClassCodeSt
        {
            public string ClassCode { get; set; }
            public string ClassString { get; set; }
        }

        #endregion

    }
}