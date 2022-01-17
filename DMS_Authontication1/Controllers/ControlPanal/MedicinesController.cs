using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using System.Threading;
using System.Threading.Tasks;
using DMS_TEST.ViewModel;
using DMS_Authontication1.ViewModel;

namespace DMS_Authontication1.Controllers.ControlPanal
{
    [Authorize(Roles = "Admin,Doctor")]

    public class MedicinesController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();

        #region Medicine license Type
        [HttpGet]
        public ActionResult MedicineLicenseType()
        {
            var viewmodel = new LicTypeViewModel()
            {
                Licenses = db.LicenseTypes.ToList()
            };
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult MedicineLicenseType(LicTypeViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                viewmodel.Licenses = db.LicenseTypes.ToList();
                return View(viewmodel);

            }

            var model = new LicenseType
            { LicenseName = viewmodel.licType };
            db.LicenseTypes.Add(model);
            db.SaveChanges();
            return RedirectToAction("MedicineLicenseType");
            //var Isduplicated = db.LicenseTypes.Where(x => x.LicenseName == viewmodel.licType).Select(x => x.LicenseName).FirstOrDefault();
            //if (Isduplicated != viewmodel.licType)
            //{ }
            //else
            //{
            //    ViewBag.Error = "done";
            //}
            //return View();
        }
        public JsonResult MedicineLicenseDelete(int id)
        {
            LicenseType _LicenseType = db.LicenseTypes.Find(id);
            if (_LicenseType != null)
            {
                db.LicenseTypes.Remove(_LicenseType);
            }
            return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Medicine Dosage Form
        [HttpGet]
        public ActionResult MedicineDosageForm()
        {
            var viewmodel = new DoasgeViewModel()
            {
                DosageFormList = db.DosageForms.ToList()
            };
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult MedicineDosageForm(DoasgeViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                viewmodel.DosageFormList = db.DosageForms.ToList();
                return View(viewmodel);
            }

            var model = new DosageForm
            {
                DosageFormName = viewmodel.DosageFormName,

            };
            db.DosageForms.Add(model);
            db.SaveChanges();
            return RedirectToAction("MedicineDosageForm");

        }
        public JsonResult MedicineDosageFormDelete(int id)
        {
            DosageForm _DosageForm = db.DosageForms.Find(id);
            if (_DosageForm != null)
            {
                db.DosageForms.Remove(_DosageForm);
            }
            return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);

        }
        #endregion
        #region Medicine Type
        public ActionResult MedicineType()
        {
            var viewmodel = new MedicineTypeViewmodel()
            {
                medtypelist = db.MedicineTypes.ToList()
            };
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult MedicineType(MedicineTypeViewmodel viewmodel)
        {
            if (!ModelState.IsValid)
            {

                viewmodel.medtypelist = db.MedicineTypes.ToList();

                return View(viewmodel);
            }

            var model = new MedicineType
            { MedicineTypeName = viewmodel.MedicineTypeName };
            db.MedicineTypes.Add(model);
            db.SaveChanges();
            return RedirectToAction("MedicineType");
            //  var Isduplicated = db.MedicineTypes.Where(x => x.MedicineTypeName == viewmodel.MedicineTypeName).Select(x => x.MedicineTypeName).FirstOrDefault();
            //if (Isduplicated != viewmodel.MedicineTypeName)
            //{}
            //else
            //{
            //    ViewBag.Error = "done";
            //}
            //return View();
        }
        //public ActionResult MedTypeList()
        //{
        //    var viewmodel = new MedicineTypeViewmodel()
        //    {
        //        medtypelist = db.MedicineTypes.ToList()
        //    };
        //    return View(viewmodel);
        //}
        public JsonResult MedicineTypeDelete(int id)
        {
            MedicineType _MedicineType = db.MedicineTypes.Find(id);
            if (_MedicineType != null)
            {
                db.MedicineTypes.Remove(_MedicineType);
            }
            return Json(new { ok = true, data = db.SaveChanges(), message = "ok" }, JsonRequestBehavior.AllowGet);

        }
        #endregion
        #region Medicines
        public ActionResult Index()
        {
            return View();
        }
        // GET: Medicines
        public JsonResult MedList(int sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {

            var result = new
            {
                sEcho = sEcho,
                aaData = db.MedicineDatas.Join(db.MedicineGroups, m => m.MED_GROUP, g => g.GroupId, (m, g) => new { m, g })
                 .Where(r => sSearch != "" ? r.m.TRADE_NAME.Contains(sSearch) || r.m.M_CODE.Contains(sSearch) || r.m.DOSAGE_FORM.Contains(sSearch) || r.m.MED_GROUP.Contains(sSearch) || r.g.GroupName.Contains(sSearch) : true)

                .Select(l => new MedicienDataViewModal
                {
                    Id = l.m.Id,
                    M_CODE = l.m.M_CODE,
                    LIC_TYPE = l.m.LIC_TYPE,
                    MED_GROUP = l.g.GroupName,
                    TRADE_NAME = l.m.TRADE_NAME,
                    DOSAGE_FORM = l.m.DOSAGE_FORM,
                    PACK_SIZE = l.m.PACK_SIZE,
                    PACK_PRICE = l.m.PACK_PRICE,
                    M_TYPE = l.m.M_TYPE,
                    UNIT_NO = l.m.UNIT_NO,
                    UNIT_PRICE = l.m.UNIT_PRICE,
                    ACTIVE = l.m.ACTIVE


                }).OrderBy(m => m.M_CODE).Skip(iDisplayStart).Take(iDisplayLength).ToList(),

                iTotalRecords = db.MedicineDatas.Join(db.MedicineGroups, m => m.MED_GROUP, g => g.GroupId, (m, g) => new { m, g })
                 .Where(r => sSearch != "" ? r.m.TRADE_NAME.Contains(sSearch) || r.m.M_CODE.Contains(sSearch) || r.m.DOSAGE_FORM.Contains(sSearch) || r.m.MED_GROUP.Contains(sSearch) || r.g.GroupName.Contains(sSearch) : true)
                .Count(),
                iTotalDisplayRecords = db.MedicineDatas.Join(db.MedicineGroups, m => m.MED_GROUP, g => g.GroupId, (m, g) => new { m, g })
                 .Where(r => sSearch != "" ? r.m.TRADE_NAME.Contains(sSearch) || r.m.M_CODE.Contains(sSearch) || r.m.DOSAGE_FORM.Contains(sSearch) || r.m.MED_GROUP.Contains(sSearch) || r.g.GroupName.Contains(sSearch) : true)
                .Count()
            };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //if (sSearch != null)
            //{ }
            //else
            //{
            //    var result = new
            //    {
            //        sEcho = sEcho,
            //        aaData = db.MedicineDatas.OrderBy(m => m.M_CODE).AsEnumerable()
            //        .Select(l => new MedicienDataViewModal
            //        {
            //            Id = l.Id,

            //            M_CODE = l.M_CODE,
            //            LIC_TYPE = l.LIC_TYPE,
            //            MED_GROUP = l.MED_GROUP,
            //            TRADE_NAME = l.TRADE_NAME,
            //            DOSAGE_FORM = l.DOSAGE_FORM,
            //            PACK_SIZE = l.PACK_SIZE,
            //            PACK_PRICE = l.PACK_PRICE,
            //            M_TYPE = l.M_TYPE,
            //            UNIT_NO = l.UNIT_NO,
            //            UNIT_PRICE = l.UNIT_PRICE

            //        }).Skip(iDisplayStart).Take(iDisplayLength).ToList(),
            //        iTotalRecords = db.MedicineDatas.Count(),
            //        iTotalDisplayRecords = db.MedicineDatas.Count()
            //    };
            //    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //}


        }


        // GET: Medicines/Create
        public ActionResult Create()
        {
            ViewBag.LicenceType = new SelectList(db.LicenseTypes, "LicenseName", "LicenseName");
            ViewBag.MedicineType = new SelectList(db.MedicineTypes, "MedicineTypeName", "MedicineTypeName");
            ViewBag.DosageForm = new SelectList(db.DosageForms, "DosageFormName", "DosageFormName");
            ViewBag.MedicineGroup = new SelectList(db.MedicineGroups, "GroupId", "GroupName");
            return View();
        }

        public JsonResult getDiag()
        {
            var Diagnoises = db.Diagnosis
                .Select(l => new
                {
                    Code = l.Id,
                    Name = l.DIAG_ANAME

                })
            .ToList();
            return Json(Diagnoises, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDatas(MedicienDataViewModal data)
        {
            var mcode = db.MedicineDatas.Where(x => x.M_CODE == data.M_CODE).ToList();
            if (mcode.Count == 1)
            {
                //return new JsonResult { Data = "Medicine Code is existed", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return Json(new { ok = false, message = "Medicine Code is existed" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //int MedicineDataId = Convert.ToInt32(data.M_CODE);

                var obi = new MedicineData()
                {
                    COMP_ID = 1,
                    BRANCH_CODE = 1,
                    M_CODE = data.M_CODE,
                    LIC_TYPE = data.LIC_TYPE,
                    MED_GROUP = data.MED_GROUP,
                    TRADE_NAME = data.TRADE_NAME,
                    DOSAGE_FORM = data.DOSAGE_FORM,
                    PACK_SIZE = data.PACK_SIZE,
                    PACK_PRICE = data.PACK_PRICE,
                    M_TYPE = data.M_TYPE,
                    CON_MED = data.CON_MED,
                    UNIT_NO = data.UNIT_NO,
                    UNIT_PRICE = data.UNIT_PRICE,
                    ACTIVE = "Y",
                    CREATED_BY = User.Identity.Name,
                    CREATED_DATE = DateTime.Now,
                    GRN_CODE = data.GRN_CODE,
                    IsCovered = data.IsCovered,
                    DiagnoiseGender = data.DiagnoiseGender,
                    DiagnoiseAge = data.DiagnoiseAge,
                    MedicinesDiagnosis = data.MedicinesDiagnosis,

                };
                db.MedicineDatas.Add(obi);
                db.SaveChanges();
                string[] Name;
                int size;
                for (int i = 0; i < data.Diagnose.Length; i++)
                {
                    Name = data.Diagnose[i].Split('\n');
                    size = Name.Length;


                    for (int j = 0; j < size; j++)
                    {

                        var he = Name[j];
                        var Code = db.Diagnosis.Where(x => x.DIAG_ANAME == he).Select(x => x.Id).FirstOrDefault();
                        if (Code != 0)
                        {
                            MedicinesDiagnosi dignosi = new MedicinesDiagnosi();
                            dignosi.MedicineId = obi.Id;
                            dignosi.DiagnoiseId = Code;
                            db.MedicinesDiagnosis.Add(dignosi);
                            db.SaveChanges();

                        }
                    }
                }

                return Json(new { ok = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);

                //return new JsonResult { Data = "Row is Added", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        //public JsonResult SaveDiagnoises(List<MedicinesDiagnosi> DiagnosisList)
        //{
        //    Task.WaitAll(Task.Delay(3000));
        //    foreach (MedicinesDiagnosi item in DiagnosisList)
        //    {
        //        MedicinesDiagnosi dignosi = new MedicinesDiagnosi();
        //        dignosi.MedicineId = item.MedicineId;
        //        dignosi.DiagnoiseId = item.DiagnoiseId;
        //        db.MedicinesDiagnosis.Add(dignosi);

        //    }
        //    db.SaveChanges();
        //    return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        // GET: Medicines/Edit/5
        public ActionResult Edit(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicineData mEDICINE_DATA = db.MedicineDatas.Find(id);
            if (mEDICINE_DATA == null)
            {
                return HttpNotFound();
            }
            ViewBag.LicenceType = new SelectList(db.LicenseTypes, "LicenseName", "LicenseName", mEDICINE_DATA.LIC_TYPE);
            ViewBag.MedicineType = new SelectList(db.MedicineTypes, "MedicineTypeName", "MedicineTypeName", mEDICINE_DATA.M_TYPE);
            ViewBag.DosageForm = new SelectList(db.DosageForms, "DosageFormName", "DosageFormName", mEDICINE_DATA.DOSAGE_FORM);
            ViewBag.MedicineGroup = new SelectList(db.MedicineGroups, "GroupId", "GroupName", mEDICINE_DATA.MED_GROUP);
            return View(mEDICINE_DATA);
        }

        public JsonResult GetCurrentDiagnosis(int MedicineId)
        {
            var Diagnosis = new List<Diagnosi>();
            var DiagnosisMedicines = db.MedicinesDiagnosis.Where(m => m.MedicineId == MedicineId).ToList();
            foreach (var item in DiagnosisMedicines)
            {
                Diagnosi Diagnoise = db.Diagnosis.Where(x => x.Id == item.DiagnoiseId).FirstOrDefault();
                Diagnosis.Add(Diagnoise);
            }

            return Json(Diagnosis, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditData(MedicienDataViewModal data)
        {
            MedicineData mcode = db.MedicineDatas.Where(x => x.M_CODE == data.M_CODE).FirstOrDefault();

            mcode.COMP_ID = 1;
            mcode.BRANCH_CODE = 1;
            mcode.LIC_TYPE = data.LIC_TYPE;
            mcode.MED_GROUP = data.MED_GROUP;
            mcode.TRADE_NAME = data.TRADE_NAME;
            mcode.DOSAGE_FORM = data.DOSAGE_FORM;
            mcode.PACK_SIZE = data.PACK_SIZE;
            mcode.PACK_PRICE = data.PACK_PRICE;
            mcode.M_TYPE = data.M_TYPE;
            mcode.CON_MED = data.CON_MED;
            mcode.UNIT_NO = data.UNIT_NO;
            mcode.UNIT_PRICE = data.UNIT_PRICE;
            mcode.ACTIVE = "Y";
            mcode.UPDATE_BY = User.Identity.Name;
            mcode.UPDATE_DATE = DateTime.Now;
            mcode.GRN_CODE = data.GRN_CODE;
            mcode.IsCovered = data.IsCovered;
            mcode.DiagnoiseGender = data.DiagnoiseGender;
            mcode.DiagnoiseAge = data.DiagnoiseAge;
            mcode.SyncBy = "Updated";
            mcode.MedicinesDiagnosis = data.MedicinesDiagnosis;

            db.Entry(mcode).State = EntityState.Modified;
            //db.SaveChanges();
            List<MedicinesDiagnosi> MD = db.MedicinesDiagnosis.Where(x => x.MedicineId == mcode.Id).ToList();
            db.MedicinesDiagnosis.RemoveRange(MD);
            // db.SaveChanges();
            List<MedicinesDiagnosi> _MedicinesDiagnosi = new List<MedicinesDiagnosi>();
            string[] Name;
            int size;
            for (int i = 0; i < data.Diagnose.Length; i++)
            {
                Name = data.Diagnose[i].Split('\n');
                size = Name.Length;
                for (int j = 0; j < size; j++)
                {
                    var he = Name[j];
                    var Code = db.Diagnosis.Where(x => x.DIAG_ANAME == he).Select(x => x.Id).FirstOrDefault();
                    if (Code != 0)
                    {
                        MedicinesDiagnosi dignosi = new MedicinesDiagnosi();
                        dignosi.MedicineId = mcode.Id;
                        dignosi.DiagnoiseId = Code;
                        _MedicinesDiagnosi.Add(dignosi);

                    }
                }
            }
            db.MedicinesDiagnosis.AddRange(_MedicinesDiagnosi);

            //update med_medicine
            List<Med_Medicine> mcodeChronics = db.Med_Medicine.Where(x => x.MED_CODE == data.M_CODE).ToList();
            foreach (Med_Medicine mcodeChronic in mcodeChronics)
            {
                mcodeChronic.MED_NAME = data.TRADE_NAME;
                mcodeChronic.DOSAGE_FORM = data.DOSAGE_FORM;
                mcodeChronic.PACK_SIZE = Convert.ToInt32(data.PACK_SIZE);
                mcodeChronic.PACK_PRICE = data.PACK_PRICE;
                mcodeChronic.UNIT_NO = data.UNIT_NO;
                mcodeChronic.UNIT_PRICE = data.UNIT_PRICE;
                mcodeChronic.NO_OF_UINT = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(((mcodeChronic.DOSE * mcodeChronic.MED_DURATION) - Convert.ToDouble(mcodeChronic.EXCESS)) / (Convert.ToDouble(mcodeChronic.PACK_SIZE) / Convert.ToDouble(mcodeChronic.UNIT_NO)))));
                mcodeChronic.TOTAL_AMT = data.UNIT_PRICE * mcodeChronic.NO_OF_UINT;
                mcodeChronic.UPDATE_BY = User.Identity.Name;
                mcodeChronic.UPDATE_DATE = DateTime.Now;
                mcodeChronic.SyncBy = "Updated";
                db.Entry(mcodeChronic).State = EntityState.Modified;

                Roshita roshita = db.Roshitas.Where(x => x.CardId == mcodeChronic.CARD_NO && x.Manager == "Doctor_chronic").FirstOrDefault();
                if (roshita != null)
                {
                    RoshitaDetail roshitaDetail = db.RoshitaDetails.Where(x => x.RoshitaID == roshita.Id && x.MedicienCode == mcodeChronic.MED_CODE).FirstOrDefault();
                    if (roshitaDetail != null)
                    {
                        roshitaDetail.Amount = mcodeChronic.TOTAL_AMT.Value;
                        db.Entry(roshitaDetail).State = EntityState.Modified;
                    }
                }

            }
            db.SaveChanges();
            return Json(new { ok = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "M_CODE,LIC_TYPE,MED_GROUP,TRADE_NAME,DOSAGE_FORM,PACK_SIZE,PACK_PRICE,M_TYPE,CON_MED,UNIT_NO,UNIT_PRICE,GRN_CODE,IsCovered,DiagnoiseGender,DiagnoiseAge")] MedicineData mEDICINE_DATA)
        //{
        //    ViewBag.LicenceType = new SelectList(db.LicenseTypes, "LicenseName", "LicenseName", mEDICINE_DATA.LIC_TYPE);
        //    ViewBag.MedicineType = new SelectList(db.MedicineTypes, "MedicineTypeName", "MedicineTypeName", mEDICINE_DATA.M_TYPE);
        //    ViewBag.DosageForm = new SelectList(db.DosageForms, "DosageFormName", "DosageFormName", mEDICINE_DATA.DOSAGE_FORM);
        //    ViewBag.MedicineGroup = new SelectList(db.MedicineGroups, "GroupId", "GroupName", mEDICINE_DATA.MED_GROUP);
        //    if (ModelState.IsValid)
        //    {
        //        mEDICINE_DATA.ACTIVE = "Y";
        //        mEDICINE_DATA.UPDATE_BY = User.Identity.Name;
        //        mEDICINE_DATA.UPDATE_DATE = DateTime.Now;

        //        db.Entry(mEDICINE_DATA).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(mEDICINE_DATA);
        //}
        //public JsonResult EditDiagnoises(List<MedicinesDiagnosi> DiagnosisList)
        //{
        //    //Task.WaitAll(Task.Delay(3000));
        //    //delete
        //    int MedicineId = DiagnosisList.FirstOrDefault().MedicineId;

        //    List<MedicinesDiagnosi> MD = db.MedicinesDiagnosis.Where(x => x.MedicineId == MedicineId).ToList();
        //    db.MedicinesDiagnosis.RemoveRange(MD);
        //    db.SaveChanges();
        //    //insert
        //    foreach (MedicinesDiagnosi item in DiagnosisList)
        //    {
        //        MedicinesDiagnosi dignosi = new MedicinesDiagnosi();
        //        dignosi.MedicineId = item.MedicineId;
        //        dignosi.DiagnoiseId = item.DiagnoiseId;
        //        db.MedicinesDiagnosis.Add(dignosi);

        //    }
        //    db.SaveChanges();
        //    return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        // GET: Medicines/Delete/5
        public ActionResult MedicineActivation(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicineData mEDICINE_DATA = db.MedicineDatas.Find(id);
            if (mEDICINE_DATA == null)
            {
                return HttpNotFound();
            }
            return View(mEDICINE_DATA);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("MedicineActivation")]
        [ValidateAntiForgeryToken]
        public ActionResult MedicineActivationConfirmed(int id)
        {
            MedicineData mEDICINE_DATA = db.MedicineDatas.Find(id);
            mEDICINE_DATA.ACTIVE = mEDICINE_DATA.ACTIVE == "Y" ? "N" : "Y";
            db.Entry(mEDICINE_DATA).State = EntityState.Modified;
            db.SaveChanges(); return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
