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
using System.Web;
using System.Web.Mvc;
using DMS_Authontication1.Models;
using DMS_Authontication1.ViewModel.HR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR,HR_Admin,User")]
    public class IndemnitiesAdminController : Controller
    {
        private DMS_TESTEntities db = new DMS_TESTEntities();
        private ApplicationUserManager _userManager;

        public IndemnitiesAdminController()
        {
            db = new DMS_TESTEntities();
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

        //[Authorize(Roles = "Admin")]

        // GET: Indemnities
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.IndemnityMasters.Where(se => se.IsDeleted == false).OrderByDescending(x => x.Id).ToList());

            }
            return View(db.IndemnityMasters.Where(x => x.CreatedBy == User.Identity.Name && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList());
        }

        // GET: Indemnities/Details/5
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

        // GET: Indemnities/Create
        public ActionResult CreateAdmin()
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.UserCompId = CurrentUser.Provider;
            ViewBag.CompName = CurrentUser.Provider;
            if (User.IsInRole("HR_Admin"))
            {
                ViewBag.CompName = null;
                var compines = db.HrAdminCompanies.Where(x => x.UserId == CurrentUser.Id).Select(c => c.CompId).ToList();
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
            ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
            ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");

            IndemnityMasterVm indemnityVMs = new IndemnityMasterVm
            {
                IndemnityCardsImages = new IndemnityVM[20],
                ServiceTest = new IndemnityServiceVm[200],
            };
            ViewBag.error = "0";
            if (User.IsInRole("User"))
            {
                var usr = User.Identity.GetUserId();
                var cardId = db.EmployeePersonalDatas.Where(e => e.UserId == usr).FirstOrDefault().CardId;
                ViewBag.cardID = cardId;
                ViewBag.CompName = cardId.Split('-')[0];
                ViewBag.UserCompId = cardId.Split('-')[0];
            }
            return View(indemnityVMs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(IndemnityMasterVm IndemnityMaster)
        {
            int compId = int.Parse(IndemnityMaster.CompanyName);
            var companyname = db.Contract_Comp.Where(c => c.C_COMP_ID == compId).FirstOrDefault().C_ENAME;
            // General Object
            IndemnityMaster model = new IndemnityMaster();
            model.NationalID = IndemnityMaster.NationalId;
            model.Name = IndemnityMaster.Name;
            model.RelatedCardId = IndemnityMaster.RelatedCardId;
            model.Type = IndemnityMaster.Type;
            model.BankName = IndemnityMaster.BankName;
            model.BankBranch = IndemnityMaster.BankBranch;
            model.BankAccount = IndemnityMaster.BankAccount;
            model.Phone = IndemnityMaster.Phone;
            model.CompanyName = companyname;
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
                        string fileName = "Service_" + item.ServiceCardId + "_" + item.SpecialistId + "_" + item.ServiceId + "_" + i + extension;
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
                    ApplicationDbContext users1 = new ApplicationDbContext();
                    var CurrentUser1 = users1.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    ViewBag.UserCompId = CurrentUser1.Provider;

                    ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
                    ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
                    ViewBag.error = "1";
                    IndemnityMasterVm indemnityVMs1 = new IndemnityMasterVm
                    {
                        IndemnityCardsImages = new IndemnityVM[20],
                        ServiceTest = new IndemnityServiceVm[200],
                    };

                    return View(indemnityVMs1);
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
                        string fileName = "NationalID_" + item.PhotoCardId + "_" + j++ + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
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
                    ApplicationDbContext users1 = new ApplicationDbContext();
                    var CurrentUser1 = users1.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    ViewBag.UserCompId = CurrentUser1.Provider;

                    ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
                    ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
                    ViewBag.error = "1";
                    IndemnityMasterVm indemnityVMs1 = new IndemnityMasterVm
                    {
                        IndemnityCardsImages = new IndemnityVM[20],
                        ServiceTest = new IndemnityServiceVm[200],
                    };
                    return View(indemnityVMs1);

                }
                //item.NationalIdPDF.CopyTo(ImageFiles, ImageFiles.Length);


            }


            db.IndemnityMasters.Add(model);
            db.SaveChanges();

            var userid = User.Identity.GetUserId();
            var Hospitalprovider = UserManager.FindById(userid);
            string sub = @"Request Indemnity From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + IndemnityMaster.CompanyName;
            string msg = @"<h3> Request Number  : </h3>" + model.Id + "<br/>";
            if (model.Type == 1)
            {
                msg += /*"<h3> Employee Name: </h3> " + model.Name + " <br/> " +*/
                "<h3> NationalId : </h3 > " + model.NationalID + " <br/> ";

            }
            else
            {
                msg += "<h3> Company Name : </h3> " + Hospitalprovider.FName + " " + Hospitalprovider.LName + " <br/> ";
            }
            msg += "<h3> Bank Name: </h3> " + model.BankName + " <h3> Bank Branch: </h3>  " + model.BankBranch +
                " <h3> Bank Account: </h3> " + model.BankAccount + " <br/> " + " <h3> Phone : </h3> " + model.Phone + " <br/> " +
                "<h3>Take the following Services : </ h3 > <br/> ";
            foreach (var item in model.IndemnityServiceMasters)
            {
                string specialist = db.Specialities1.Where(s => s.SPEC_ID == item.SpecialistId).FirstOrDefault().SPEC_ANAME;
                string service = db.Services.Where(s => s.ID == item.ServiceId).FirstOrDefault().NameAr;
                msg += "<h3> CardId : </h3> " + item.ServiceCard + " <h3> Specialist : </h3>  " + specialist +
                     "<h3> Service : </h3> " + service + " <h3> Service Value: </h3>  " + item.Value +
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
                        string NameFile = "Service_" + item.ServiceCard + "_" + item.SpecialistId + "_" + item.ServiceId + "_" + (i++);
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

                    string NameFile = "NationalID_" + item.PhotoCardId + "_" + DateTime.Now.ToString("yymmss");
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

            SendMail("indhrrequest@gmail.com", sub, msg, altView);

            if (model.Id > 0)
            {
                return RedirectToAction("Index");
            }
            ViewBag.error = "1";
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.UserCompId = CurrentUser.Provider;
            if (User.IsInRole("HR_Admin"))
            {
                ViewBag.CompName = null;
                var compines = db.HrAdminCompanies.Where(x => x.UserId == CurrentUser.Id).Select(c => c.CompId).ToList();
                if (compines[0] == "All")
                {
                    var companynam = db.Contract_Comp
                        .Select(l => new
                        {
                            Code = l.C_COMP_ID,
                            Name = l.C_ENAME + " || " + l.C_COMP_ID

                        }).ToList();
                    SelectList companylist = new SelectList(companynam, "Code", "Name", compId);
                    ViewBag.company = companylist;
                }
                else
                {
                    var companynam = (from comp in compines
                                      join contCo in db.Contract_Comp
                                      on int.Parse(comp) equals contCo.C_COMP_ID
                                      select new
                                      {
                                          Code = contCo.C_COMP_ID,
                                          Name = contCo.C_ENAME + " || " + contCo.C_COMP_ID
                                      }).ToList();
                    SelectList companylist = new SelectList(companynam, "Code", "Name", compId);
                    ViewBag.company = companylist;
                }
            }
            ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
            ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
            IndemnityMasterVm indemnityVMs = new IndemnityMasterVm
            {
                IndemnityCardsImages = new IndemnityVM[20],
                ServiceTest = new IndemnityServiceVm[200]
            };

            return View(indemnityVMs);
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

        public void SendMail(string to, string subject, string Message, AlternateView altView)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("hrindemnity@gmail.com", to, subject, Message);
            mail.AlternateViews.Add(altView);
            System.Net.NetworkCredential mailAuthenticaion = new System.Net.NetworkCredential("hrindemnity@gmail.com", "dms123456");

            System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            mailclient.EnableSsl = true;
            mailclient.UseDefaultCredentials = false;
            mailclient.Credentials = mailAuthenticaion;
            mail.IsBodyHtml = true;
            mailclient.Send(mail);
        }


        public ActionResult Create()
        {
            ApplicationDbContext users = new ApplicationDbContext();
            var CurrentUser = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.UserCompId = CurrentUser.Provider;
            ViewBag.ServiceId = new SelectList(db.Services, "ID", "NameAr");
            ViewBag.SpecialistId = new SelectList(db.Specialities1, "SPEC_ID", "SPEC_ANAME");
            return View();
        }

        // POST: Indemnities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BatchID,CardID,ServiceCardID,EmployeeName,CompanyName,NationalID,BankName,BankBranch,BankAccount,Type,ServiceID,ServiceDate,Total,CreatedDate,CreatedBy,UpdateDate,UpdateBy,DeletedBy,DeletedDate,IsDeleted,Status,AttachPDF")] Indemnity indemnity)
        {
            var services = db.Services.ToList();
            SelectList ServicesList = new SelectList(services, "ID", "NameAr");
            ViewBag.Services = ServicesList;
            if (ModelState.IsValid)
            {
                db.Indemnities.Add(indemnity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(indemnity);
        }

        // GET: Indemnities/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indemnity indemnity = db.Indemnities.Find(id);
            if (indemnity == null)
            {
                return HttpNotFound();
            }
            return View(indemnity);
        }

        // POST: Indemnities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BatchID,CardID,EmployeeName,CompanyName,NationalID,BankName,BankBranch,BankAccount,Type,ServiceID,ServiceDate,Total,CreatedDate,CreatedBy,UpdateDate,UpdateBy,DeletedBy,DeletedDate,IsDeleted,Status,AttachPDF")] Indemnity indemnity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indemnity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indemnity);
        }

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
            var userid = User.Identity.GetUserId();
            var Hospitalprovider = UserManager.FindById(userid);
            string sub = @" Delete Request Indemnity From " + Hospitalprovider.FName + " " + Hospitalprovider.LName + "  Code : " + Hospitalprovider.Provider;
            string msg = @"<h3> Request Number  : </h3>" + id + "<br/>";
            AlternateView altView = AlternateView.CreateAlternateViewFromString(msg, null, MediaTypeNames.Text.Html);
            SendMail("indhrrequest@gmail.com", sub, msg, altView);
            return RedirectToAction("Index");
        }

        public JsonResult SaveAttaches(List<HttpPostedFileBase> Files)
        {
            if (Files != null)
            {
                foreach (HttpPostedFileBase File in Files)
                {
                    var fileName = Path.GetFileName(File.FileName);
                    var extention = Path.GetExtension(File.FileName);
                    var filenamewithoutextension = Path.GetFileNameWithoutExtension(File.FileName);
                    fileName = filenamewithoutextension + DateTime.Now.ToString("yyMMddHH") + extention;
                    File.SaveAs(Server.MapPath("/Content/IndemnitiesAttachments/" + fileName));
                }
            }

            return new JsonResult { Data = "r", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
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

        public JsonResult ChickServiceDate(string id, int ServType, string servdate)
        {
            string SERV_CODE = "12";
            string D_SERV_CODE;
            string SER_SERV;
            Comp_Customized_D_D fount = new Comp_Customized_D_D();
            DateTime date = DateTime.Parse(servdate);
            var employe = db.Comp_Employees.Where(e => e.CARD_ID == id &&
                            date >= e.INS_START_DATE && date <= e.INS_END_DATE/*&&date>= e.SPECIFIC_DATE*/)
                           .OrderByDescending(e => e.CONTRACT_NO).FirstOrDefault();

            if (employe != null)
            {
                if ((employe.TERMINATE_FLAG == "Y" && date <= employe.TERMINATE_DATE) ||
                    (employe.TERMINATE_FLAG == "N" || employe.TERMINATE_FLAG == null))
                {
                    switch (ServType)
                    {
                        // عملية
                        case 1:
                            D_SERV_CODE = "121";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO).FirstOrDefault();
                            break;
                        // تحاليل 
                        case 2:
                            D_SERV_CODE = "122";
                            SER_SERV = "12202";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // علاج طبيعي
                        case 3:
                            D_SERV_CODE = "122";
                            SER_SERV = "12204";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // دكتور
                        case 4:
                            D_SERV_CODE = "122";
                            SER_SERV = "12205";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // اشعة
                        case 5:
                            D_SERV_CODE = "122";
                            SER_SERV = "12201";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // كشف نظارة
                        case 6:
                            D_SERV_CODE = "123";
                            SER_SERV = "12301";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // اسنان
                        case 7:
                            D_SERV_CODE = "124";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               ).FirstOrDefault();
                            break;
                        // نظارة
                        case 8:
                            D_SERV_CODE = "123";
                            SER_SERV = "12302";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // متابعة حمل
                        case 9:
                            D_SERV_CODE = "125";
                            SER_SERV = "12501";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // ولادة طبيعية
                        case 10:
                            D_SERV_CODE = "125";
                            SER_SERV = "12502";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // ولادة قيصرية
                        case 11:
                            D_SERV_CODE = "125";
                            SER_SERV = "12503";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               && c.SER_SERV == SER_SERV).FirstOrDefault();
                            break;
                        // ادوية
                        case 12:
                            D_SERV_CODE = "126";
                            fount = db.Comp_Customized_D_D.Where(c => c.C_COMP_ID == employe.C_COMP_ID && c.CLASS_CODE == employe.CLASS_CODE &&
                               c.SERV_CODE == SERV_CODE && c.D_SERV_CODE == D_SERV_CODE && c.CONTRACT_NO == employe.CONTRACT_NO
                               ).FirstOrDefault();
                            break;
                        default:
                            break;
                    }

                    if (fount != null)
                    {
                        var result = new { Success = "Yes" };
                        return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        var result = new { Success = "No", Message = "الخدمة غير مغطاة طبقا لبنود الوثيقة" };
                        return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                else
                {
                    var result = new { Success = "No", Message = "الكارت خارج التغطية التامينة خلال هذه الفترة" };
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                var result = new { Success = "No", Message = "الكارت خارج التغطية التامينة خلال هذه الفترة" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetRelatedCards(string CardId)
        {
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
                               && e.CONTRACT_NO == maxContract)
                    .Select(c => new
                    {
                        CardIDValue = c.CARD_ID,
                        CardIdString = c.CARD_ID
                    }).ToList();
                var result = new { Success = "Yes", subCards = subCards };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                var result = new { Success = "No" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        public JsonResult UpdateStatus(int id, string statustext)
        {
            var model = db.IndemnityMasters.Where(r => r.Id == id).FirstOrDefault();
            if (model != null)
            {
                model.Status = statustext;
                model.UpdateBy = User.Identity.Name;
                model.UpdateDate = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return new JsonResult { Data = "Data Saved Susseccfuly", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            else
            {
                return new JsonResult { Data = "Rrequest not found", JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
