using DMS_Authontication1.ViewModel;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DMS_Authontication1.Controllers
{
    public class SyncController : Controller
    {
        private ApplicationRoleManager _roleManager;

        public SyncController()
        { }
        public SyncController(ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager;
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Synchronize()
        {
            return View("~/Views/Sync/SyncDB.cshtml");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ActionResult> Synchronize(SyncViewModel Sync)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ViewBag.response = "";
            if (!ModelState.IsValid)
            {

                return View("~/Views/Sync/SyncDB.cshtml", Sync);
            }
            else
            {
                string apiUrl = "http://72.52.116.106:8085/sync?operation=" + Sync.type.ToString();
                //string apiUrl = "http://localhost:62439/sync?operation=" + Sync.type.ToString();

                var client = new RestClient(apiUrl);
                var req = new RestRequest(Method.POST);
                //req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                //req.AddParameter("client_id", "48df0a67-70f0-482c-882e-07846a8610e5", ParameterType.GetOrPost);
                //req.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
                //req.AddParameter("client_secret", "#{/5{zsAk]o&omr_H0BMK$$h!?VR03g2Dw2qtu6clg1l]6$e>t!", ParameterType.GetOrPost);
                //req.AddParameter("scope", "api://48df0a67-70f0-482c-882e-07846a8610e5/.default", ParameterType.GetOrPost);
                IRestResponse response =client.Execute(req);
                if (response.IsSuccessful)
                {
                    ViewBag.response = response.Content.ToString();
                }



                //using (HttpClient client = new HttpClient())
                //{
                //    try
                //    {
                //        client.Timeout = TimeSpan.FromMinutes(120);
                //        var json = JsonConvert.SerializeObject(Sync);
                //        client.BaseAddress = new Uri(apiUrl);
                //        client.DefaultRequestHeaders.Accept.Clear();
                //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //        HttpResponseMessage response = client.PostAsync(apiUrl, new StringContent(json, UnicodeEncoding.UTF8, "application/json")).Result;
                //        if (response.IsSuccessStatusCode)
                //        {
                //            ViewBag.response = await response.Content.ReadAsStringAsync();

                //        }
                //    }
                //    catch (Exception e)
                //    {

                //        ViewBag.response = string.Format("Message :{0} ", e.Message);
                //    }


                //}
                return View("~/Views/Sync/SyncDB.cshtml");
            }
        }
    }
}