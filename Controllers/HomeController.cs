using ChattingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ChattingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBContext db;
        private IHostingEnvironment env;

        public HomeController(DBContext _db, IHostingEnvironment _env)
        {
            db = _db;
            env = _env;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize (Roles ="admin")]
        public IActionResult Company()
        {
            return View(db.companyinfos.ToList());
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Company(Companyinfo com)
        {
            if (ModelState.IsValid)
            {
                db.companyinfos.Add(com);
                db.SaveChanges();   
            }
            return View(db.companyinfos.ToList());
        }

        [Authorize(Roles = "admin")]       
        public IActionResult DeleteCompany(int id)
        {
           var company = db.companyinfos.Find(id);  
               
            var user=db.userinfos.Where(u=> u.companyid==id).ToList();
            foreach (var i in user)
            { 
                var gmm=db.groupmembers.Where(u=>u.userid  ==i.userid).ToList();
                foreach (var it in gmm)
                {
                    var grp = db.groupinfos.Where(u => u.groupid == it.groupid).FirstOrDefault();
                    var grpm = db.groupmembers.Where(u => u.groupid == it.groupid).ToList();
                    foreach (var ite in grpm)
                    {
                        db.groupmembers.Remove(ite);
                        db.SaveChanges();
                    }
                    db.groupinfos.Remove(grp);
                    db.SaveChanges();
                }
                var not = db.notificationinfos.Where(u => u.userid == i.userid).ToList();
                foreach (var it in not)
                {
                    db.notificationinfos.Remove(it);
                    db.SaveChanges();
                }
                db.userinfos.Remove(i);
                db.SaveChanges();
            }
            db.companyinfos.Remove(company);
            db.SaveChanges();

            return RedirectToAction("Company");
        }

        public IActionResult Job()
        {
            return View(db.jobinfos.ToList());
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Job(Jobinfo job)
        {
            if (ModelState.IsValid)
            {
                db.jobinfos.Add(job);
                db.SaveChanges();
            }
            return View(db.jobinfos.ToList());
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteJob(int id)
        {
            var company = db.jobinfos.Find(id);
            var user = db.userinfos.Where(u => u.jobid == id).ToList();
            foreach (var i in user)
            {
                var gmm = db.groupmembers.Where(u => u.userid == i.userid).ToList();
                foreach (var it in gmm)
                {
                    var grp = db.groupinfos.Where(u => u.groupid == it.groupid).FirstOrDefault();
                    var grpm = db.groupmembers.Where(u => u.groupid == it.groupid).ToList();
                    foreach (var ite in grpm)
                    {
                        db.groupmembers.Remove(ite);
                        db.SaveChanges();
                    }
                    db.groupinfos.Remove(grp);
                    db.SaveChanges();
                }
                var not = db.notificationinfos.Where(u => u.userid == i.userid).ToList();
                foreach (var it in not)
                {
                    db.notificationinfos.Remove(it);
                    db.SaveChanges();
                }
                db.userinfos.Remove(i);
                db.SaveChanges();
            }
            db.jobinfos.Remove(company);
            db.SaveChanges();

            return RedirectToAction("Job");
        }
        public IActionResult Useropload()
        {
            @TempData["msg"] = "";
            return View();
        }
      
        [HttpPost]

        public IActionResult Useropload(IFormFile xFile)
        {

            if (xFile?.Length > 0)
            {
                var stream = xFile.OpenReadStream();
                List<Userinfo> users = new List<Userinfo>();

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;

                    for (var row = 1; row <= rowCount; row++)
                    {
                        try
                        {
                            int coid = 0;
                            int jbid = 0;

                            var firstname = worksheet.Cells[row, 1].Value?.ToString();
                            var lastname = worksheet.Cells[row, 2].Value?.ToString();
                            var email = worksheet.Cells[row, 3].Value?.ToString();
                            var phone = worksheet.Cells[row, 4].Value?.ToString();
                            var password = worksheet.Cells[row, 5].Value?.ToString();
                            var company = worksheet.Cells[row, 6].Value?.ToString();
                            var job = worksheet.Cells[row, 7].Value?.ToString();
                            var code = Convert.ToBase64String((new ASCIIEncoding()).GetBytes(password)).ToCharArray().Select(x => String.Format("{0:X}", (int)x)).Aggregate(new StringBuilder(), (x, y) => x.Append(y)).ToString();
                            var conp = db.companyinfos.Where(w => w.company == company).FirstOrDefault();
                            var jobb = db.jobinfos.Where(w => w.jobname == job).FirstOrDefault();
                            if (conp==null)
                            {
                                Companyinfo cm=new Companyinfo();
                                cm.company = company;
                                db.companyinfos.Add(cm);
                                db.SaveChanges();
                                coid = cm.companyid;
                            }
                            else
                            {
                                coid = conp.companyid;
                            }
                            if (jobb == null)
                            {
                                Jobinfo cm = new Jobinfo();
                                cm.jobname = job;
                                db.jobinfos.Add(cm);
                                db.SaveChanges();
                                jbid = cm.jobid;
                            }
                            else
                            {
                                jbid = jobb.jobid;
                            }
                            var check = db.userinfos.Where(w => w.email == email.Trim()).FirstOrDefault();
                            if (check != null) continue;
                            var user = new Userinfo()
                            {
                               

                                firstname = firstname,
                                lastname = lastname,
                                email = email,
                                phone = phone,
                                password = code,
                                role = "user",
                                isvarify = true,
                                images = "images/icons/avatar1.png",
                                companyid = coid,
                                jobid = jbid,
                                varificationcode = "0364",
                                status = false,
                                isblock = false

                            };

                            db.userinfos.Add(user);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Something went wrong");
                        }
                    }

                }
            }
            @TempData["msg"] = "File upload successfull";
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult Userlist()
        {
            List<Usermodel> us = new List<Usermodel>();
            var users =(from u in db.userinfos
                        join j in db.jobinfos on u.jobid equals j.jobid
                        join c in db.companyinfos on u.companyid equals c.companyid
                        select new {u.userid,u.firstname,u.lastname,u.email,u.phone,u.role,u.images,u.isblock,j.jobname,c.company }).ToList();
            if (users.Count>0)
            {
                foreach (var i in users)
                {
                    us.Add(new Usermodel { 
                   firstname = i.firstname,lastname = i.lastname,email = i.email,phone = i.phone,
                        jobname =i.jobname,company=i.company,images =i.images,role=i.role,isblock=i.isblock,userid=i.userid
                    });
                }
            }
            return View(us);
        }
        
        
        [Authorize(Roles = "admin")]
        public IActionResult Blockunblock(int id)
        {
            var us=db.userinfos.FirstOrDefault(u => u.userid==id);
            if (us.isblock == false) { us.isblock = true; }
            else
            {
                us.isblock = false;

            }
            db.SaveChanges();
            return RedirectToAction("Userlist");
        }
        public IActionResult ExportToExcel()
        {
            // Get the user list 
            var users = (from u in db.userinfos
                         join j in db.jobinfos on u.jobid equals j.jobid
                         join c in db.companyinfos on u.companyid equals c.companyid
                         select new { u.firstname, u.lastname, u.email, u.phone, u.role, u.images, j.jobname, c.company }).ToList();

            //var stream = new MemoryStream();
            //string wwwPath = this.env.WebRootPath;
            //string contentPath = this.env.ContentRootPath;

            //string path = Path.Combine(this.env.WebRootPath, "images/Exfile/Userlist.xlsx");
            var stream = new MemoryStream();
            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Users");

                const int startRow = 5;
                var row = startRow;

                worksheet.Cells["A4"].Value = "Firstname";
                worksheet.Cells["B4"].Value = "Lastname";
                worksheet.Cells["C4"].Value = "Email";
                worksheet.Cells["D4"].Value = "Phone";
                worksheet.Cells["E4"].Value = "Company";
                worksheet.Cells["F4"].Value = "Job";
                worksheet.Cells["G4"].Value = "Role";
                worksheet.Cells["A4:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:G4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                row = 5;
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = user.firstname;
                    worksheet.Cells[row, 2].Value = user.lastname;
                    worksheet.Cells[row, 3].Value = user.email;
                    worksheet.Cells[row, 4].Value = user.phone;
                    worksheet.Cells[row, 5].Value = user.company;
                    worksheet.Cells[row, 6].Value = user.jobname;
                    worksheet.Cells[row, 7].Value = user.role;
                    row++;
                }


                xlPackage.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
        }

        [Authorize(Roles = "admin")]
        public IActionResult Edituser(int id)
        {
            Userinfo data = db.userinfos.Where(w => w.userid == id).FirstOrDefault();

            return View(data);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Edituser(Userinfo user)
        {
           var us=db.userinfos.Where(w => w.userid == user.userid).FirstOrDefault();    
            if (us != null)
            {
                us.phone = user.phone;
                us.firstname = user.firstname;
                us.email = user.email;
                us.role = user.role;
               
                us.lastname = user.lastname;
                db.SaveChanges();    
            } 

            return RedirectToAction("Userlist");
        }
    }
}