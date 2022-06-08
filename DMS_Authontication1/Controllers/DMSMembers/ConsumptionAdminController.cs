using DMS_Authontication1.Models;
using System;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace DMS_TEST.Controllers
{

    public class ConsumptionAdminController : Controller
    {
        #region Properties
        DMS_TESTEntities db;
        ApplicationDbContext UserDB;
        #endregion

        #region CTO
        public ConsumptionAdminController()
        {
            db = new DMS_TESTEntities();
            UserDB = new ApplicationDbContext();
        }
        #endregion

        #region Actions
        //Main Roshta
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintConsumption(string From, string To, string CardId)
        {
            try
            {

                DateTime F = Convert.ToDateTime(From);
                DateTime T = Convert.ToDateTime(To).AddSeconds(86399);//to get all pervious day
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Consumption"), "ConsumptionofCard.rpt"));
                rd.SetDatabaseLogon("APP", "12369");
                rd.SetParameterValue("SRDA1", F);
                rd.SetParameterValue("SRDA2", T);
                rd.SetParameterValue("CRD", CardId);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                GC.Collect();
                return File(stream, "application/pdf", F.ToString("ddMMyyyy") + "Clams.pdf");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");
                // throw ex.Message("There are no calmes");
            }
        }

        #endregion

    }
}