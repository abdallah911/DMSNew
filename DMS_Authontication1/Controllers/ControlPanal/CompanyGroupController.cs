using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.ControlPanal
{
    [Authorize(Roles = "Admin,Doctor")]
    public class CompanyGroupController : Controller
    {
        DMS_TESTEntities db;
        public CompanyGroupController()
        {
            db = new DMS_TESTEntities();
        }
        // GET: CompanyGroup
        public ActionResult Index()
        {
            return View();
        }
       
        public JsonResult CompanyGroupList()
        {
            List<Contract_Comp> coms = new List<Contract_Comp>();
            coms = db.Contract_Comp.ToList();
            return new JsonResult { Data = coms, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetCompSites(int id)
        {
            try
            {
                var comp = db.S_Ent_7.Where(x=>x.C_COMP_ID==id).ToList();
                return new JsonResult { Data = comp ,JsonRequestBehavior =JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult PostCompSites(List<S_Ent_7> sites)
        {
            try
            {
                int compId =Convert.ToInt32(sites.ElementAt(0).C_COMP_ID);
                var old = db.S_Ent_7.Where(r => r.C_COMP_ID == compId).ToList();
                db.S_Ent_7.RemoveRange(old);
                db.SaveChanges();
                foreach (S_Ent_7 site in sites)
                {
                    site.S_ID =site.S_ID;
                    site.S_NAME = site.S_NAME;
                    site.S_NO = site.S_NO;
                    site.C_COMP_ID = site.C_COMP_ID;
                    //Medicien.CANCEL_ITEM = "1";
                    db.S_Ent_7.Add(site);
                }
                db.SaveChanges();
                
                return new JsonResult { Data = "saved", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}