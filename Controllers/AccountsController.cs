using ChattingApp.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChattingApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext db;
        private IHostingEnvironment env;

        public AccountController(DBContext _db,  IHostingEnvironment _env)
        {
            db = _db;
            env= _env;  
        }
        public IActionResult Index()
        { return View(); 
        }
        
            [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                //if (User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault() == "Admin")
                //{
                //    RedirectToAction("Index", "Admin");
                //}
                RedirectToAction("Index", "Chat");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            @TempData["msg"] = "";
              // Find and validate the user:
              Userinfo user = GetUserByUsername(email, password);
            if (user == null)
            {
                @TempData["msg"] = "Incorrect username or password.";
                ModelState.AddModelError("", "Incorrect username or password.");
                return View();
            }
            else if (user.isvarify==false)
            {
                return RedirectToAction("Confirmation", "Account", user);
            }
            else if (user.isblock == true)
            {
                @TempData["msg"] = "You are block. Contact to admin";
                return View();
            }
            else
            {
                user.status = true;
                db.SaveChanges();
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.GivenName, user.firstname));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.email));
                identity.AddClaim(new Claim(ClaimTypes.Sid, user.userid.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.role));

               
                // Sign in
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal));


                return RedirectToAction("Index", "Chat");
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var uid = int.Parse(User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
            var user=db.userinfos.Where(w=> w.userid == uid).FirstOrDefault();
            user.status = false;
            db.SaveChanges();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Chat");
        }
        private Userinfo GetUserByUsername(string username, string password)
        {

           
            var code = Convert.ToBase64String((new ASCIIEncoding()).GetBytes(password)).ToCharArray().Select(x => String.Format("{0:X}", (int)x)).Aggregate(new StringBuilder(), (x, y) => x.Append(y)).ToString();
            Userinfo us = db.userinfos.Where(w => w.email == username && w.password == code).FirstOrDefault();
            return us;
        }
        [HttpGet]
        public IActionResult Register()
        {
            Registerview registerview = new Registerview();
            registerview.job = db.jobinfos.ToList();
           
            return View(registerview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Registermodel user, IFormFile File)
        {
            @TempData["msg"] = "";
            if (user.password!=user.conpassword)
            {
                @TempData["msg"] = "Password not match.";
                
                Registerview registerview = new Registerview();
                registerview.job = db.jobinfos.ToList();
               
                return View(registerview);
            }
           
            var code = Convert.ToBase64String((new ASCIIEncoding()).GetBytes(user.password)).ToCharArray().Select(x => String.Format("{0:X}", (int)x)).Aggregate(new StringBuilder(), (x, y) => x.Append(y)).ToString();
            if (ModelState.IsValid)
            {
                var checkuser=db.userinfos.Where(w => w.email == user.email).FirstOrDefault();
                if (checkuser != null) {
                    @TempData["msg"] = "Use another email.";
                  
                    Registerview registerview = new Registerview();
                    registerview.job = db.jobinfos.ToList();
                   
                    return View(registerview);
                }
                Userinfo us=new Userinfo();
                us.firstname=user.firstname;
                us.lastname=user.lastname;
                us.email=user.email;
                us.phone=user.phone;
                us.password= code;
                us.role="user";
                us.jobid = user.jobid;
                us.companyid = user.companyid;
                if (File!=null)
                {
                    string wwwPath = this.env.WebRootPath;
                    string contentPath = this.env.ContentRootPath;
                    string con = us.firstname.Replace(" ", "").ToLower();
                    string path = Path.Combine(this.env.WebRootPath, "images/user");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                   
                        string ext = Path.GetExtension(File.FileName);
                        string fname = con  + ext;
                        using (FileStream stream = new FileStream(Path.Combine(path, fname), FileMode.Create))
                        {

                        File.CopyTo(stream);
                        us.images = "images/user/" + fname;
                    }
                    
                }
                else
                {
                    string path = Path.Combine(this.env.WebRootPath, "images/icons");
                    us.images = "images/icons/avatar" + (user.avatar + 1).ToString() + ".png";
                }
                us.varificationcode = generateCode();
                db.userinfos.Add(us);
                db.SaveChanges();
                senDmail(us.firstname,us.email,us.varificationcode);
                return RedirectToAction("Confirmation", "Account", us);
            }
            else
            {
                @TempData["msg"] = "Fill all field correctly.";
                Registerview registerview = new Registerview();
                registerview.job = db.jobinfos.ToList();
                registerview.com = db.companyinfos.ToList();
                return View(registerview);
            }
            
        }

        private void senDmail(string firstname,string email, string varificationcode)
        {
            try
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress("confirmation@code-seekers.com");
                message.To.Add(new MailAddress(email));
                message.Subject = "Hi-Chat Confirmation";
                message.IsBodyHtml = true;
                message.Body = "<div><h3>Hi " + firstname + "</h3><br><p>Your verification code is <b>"+ varificationcode + "</b> <p><br><p>Thank you</p></div>";
                using (SmtpClient smtp = new SmtpClient())
                {                     
                    smtp.Credentials = new NetworkCredential("confirmation@code-seekers.com", "bas8E4^96");
                    smtp.Port = 25;
                    smtp.Host = "code-seekers.com";
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.EnableSsl = false;
                    smtp.Send(message);
                }
            }
            catch (Exception) 
            {
            }
        }

        private string generateCode()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }
        [Authorize]
        [HttpGet]
        public IActionResult Confirmation(Userinfo user)
        {
            return View(user);
        }

        [HttpPost]
        public IActionResult Confirmation(string confarmation, string useremail)
        {
            var check = db.userinfos.Where(w => w.email == useremail && w.varificationcode == confarmation).FirstOrDefault();
            if (check != null)
            {
                check.isvarify=true;
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                TempData["msg"] = "Code not match";
                var ch = db.userinfos.Where(w => w.email == useremail).FirstOrDefault();
                return View(ch);
            }
           
        }
      
        public IActionResult Resend( string id)
        {
            var check = db.userinfos.Where(w => w.email == id).FirstOrDefault();
            if (check != null)
            {
                check.varificationcode = generateCode();                
                db.SaveChanges();
                senDmail(check.firstname, check.email, check.varificationcode);
                TempData["msg"] = "Send new code to your email.";
            }
            else
            {
                TempData["msg"] = "Try again.";
            }
            return RedirectToAction("Confirmation");
        }
        public IActionResult GetCountry(string id)
        {
            var check = db.companyinfos.Where(w => w.company.Contains(id)).ToList();
            return Json(check);
        }
        public IActionResult SaveCountry(string id)
        {
            Companyinfo com=new Companyinfo();
            com.company = id;
            db.companyinfos.Add(com);
            db.SaveChanges();   
            return Json(com);
        }
    }
}
