using ChattingApp.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace ChattingApp.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly DBContext db;
        private IHostingEnvironment env;
        public ChatController(DBContext _db, IHostingEnvironment _env)
        {
            db = _db;
            env = _env;          
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetChat(int groupid=0,int uid=0)
        {
          
           List<Chatmodel> ct=new List<Chatmodel>();
           List<Object> obj=new List<Object>();
            if (User.Identity.IsAuthenticated)
            {
                var userid = int.Parse(User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
                if (groupid == 0 && uid> 0)
                {
                    var chat = (from c in db.chatinfos
                                join g in db.groupinfos on c.groupid equals g.groupid
                                join u in db.userinfos.Where(w=> w.userid==uid) on c.userid equals u.userid
                                select new
                                {
                                    c.userid,
                                    c.groupid,
                                    c.isseen,
                                    c.chatid,
                                    c.file,
                                    c.massagedate,
                                    c.masssage,
                                    u.firstname,
                                    u.images,
                                    c.filename,c.isimg,
                                    g.isgroup
                                }).OrderBy(o => o.massagedate).ToList();
                    var dat = (from u in db.userinfos.Where(w=> w.userid==uid) 
                        
                    select new { groupid=0, u.userid, name = u.firstname + " " + u.lastname, status = u.status == true ? "Online" : "Offline", u.images, mytype = 0 }).ToList();
                    obj.Add(dat);
                    if (chat.Count > 0)
                    {
                        foreach (var i in chat)
                        {
                            ct.Add(new Chatmodel
                            {
                                chatid = i.chatid,
                                isgroup = i.isgroup,
                                masssage = i.masssage,
                                massagedate = convertTime(i.massagedate),
                                file = i.file,
                                groupid = i.groupid,
                                isseen = i.isseen,
                                userid = i.userid,
                                firstname = i.firstname,
                                filename=i.filename,
                                isimg = i.isimg,
                                images=i.images,
                                flag = userid == i.userid ? 0 : 1
                            });
                        }
                    }
                    obj.Add(ct);
                }
                else
                {
                    if (groupid == 0)
                    {
                        var ch = db.chatinfos.Where(w => (w.userid == userid || w.chatto == userid)|| w.groupid==1).OrderByDescending(o => o.massagedate).Select(s => s.groupid).FirstOrDefault();
                        if (ch != 0)
                        {
                            groupid = db.groupinfos.Where(w => w.groupid == ch).Select(s => s.groupid).FirstOrDefault();

                        }
                        else
                        {
                            groupid = db.groupinfos.Select(s => s.groupid).FirstOrDefault();
                        }
                    }
                    if (groupid == 1)
                    {

                        var chat = (from c in db.chatinfos.Where(w => w.groupid == groupid)
                                    join g in db.groupinfos on c.groupid equals g.groupid
                                    join u in db.userinfos on c.userid equals u.userid
                                    select new
                                    {
                                        c.userid,
                                        c.groupid,
                                        c.isseen,
                                        c.chatid,
                                        c.file,
                                        c.massagedate,
                                        c.masssage,
                                        u.firstname,
                                        c.filename,
                                        u.images,
                                        c.isimg,
                                        g.isgroup
                                    }).OrderBy(o => o.massagedate).ToList();
                        var dat = (from g in db.groupinfos
                                   where g.groupid == groupid
                                   select new { g.groupid, userid = 0, name = g.groupname, status = "", images = "images/icons/avatar1.png", mytype = 1 }).ToList();
                        obj.Add(dat);
                        if (chat.Count > 0)
                        {
                            foreach (var i in chat)
                            {
                                ct.Add(new Chatmodel
                                {
                                    chatid = i.chatid,
                                    isgroup = i.isgroup,
                                    masssage = i.masssage,
                                    massagedate = convertTime(i.massagedate),
                                    file = i.file,
                                    groupid = i.groupid,
                                    isseen = i.isseen,
                                    userid = i.userid,
                                    firstname = i.firstname,
                                    filename = i.filename,
                                    isimg = i.isimg,
                                    images = i.images,
                                    flag = userid == i.userid ? 0 : 1
                                });
                            }

                        }
                        obj.Add(ct);
                    }
                    else
                    {
                        var chat = (from c in db.chatinfos.Where(w => w.groupid == groupid)
                                    join g in db.groupinfos on c.groupid equals g.groupid
                                    join u in db.userinfos on c.userid equals u.userid
                                    select new
                                    {
                                        c.userid,
                                        c.groupid,
                                        c.isseen,
                                        c.chatid,
                                        c.file,
                                        c.massagedate,
                                        c.masssage,
                                        u.firstname,
                                        u.images,
                                        c.filename,

                                        c.isimg,
                                        g.isgroup
                                    }).OrderBy(o => o.massagedate).ToList();
                        var dat = (from g in db.groupinfos
                                   where g.groupid == groupid
                                   join gn in db.groupmembers.Where(w => w.userid != userid) on g.groupid equals gn.groupid
                                   join u in db.userinfos on gn.userid equals u.userid
                                   select new { g.groupid, u.userid, name = u.firstname + " " + u.lastname, status = u.status == true ? "Online" : "Offline", images=u.images==null? "images/icons/avatar1.png" : u.images, mytype = 0 }).ToList();
                        obj.Add(dat);
                        if (chat.Count > 0)
                        {
                            foreach (var i in chat)
                            {
                                ct.Add(new Chatmodel
                                {
                                    chatid = i.chatid,
                                    isgroup = i.isgroup,
                                    masssage = i.masssage,
                                    massagedate = convertTime(i.massagedate),
                                    file = i.file,
                                    groupid = i.groupid,
                                    isseen = i.isseen,
                                    userid = i.userid,
                                    firstname = i.firstname,
                                    filename = i.filename,
                                    isimg = i.isimg,
                                    images = i.images,
                                    flag = userid == i.userid ? 0 : 1
                                });
                            }
                        }
                        obj.Add(ct);
                    }
                }                          
            }

            else
            {
                if (uid > 0 && groupid == 0)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (groupid == 0)
                {
                    groupid = db.groupinfos.Select(s=> s.groupid).FirstOrDefault();                 
                }
                if (groupid == 1)
                {
                    
                        var chat = (from c in db.chatinfos.Where(w => w.groupid == groupid)
                                    join g in db.groupinfos on c.groupid equals g.groupid
                                    join u in db.userinfos on c.userid equals u.userid
                                    select new
                                    {
                                        c.userid,
                                        c.groupid,
                                        c.isseen,
                                        c.chatid,
                                        c.file,
                                        c.massagedate,
                                        c.masssage,
                                        u.firstname,
                                        c.filename,
                                        c.isimg,
                                        g.isgroup
                                    }).OrderBy(o => o.massagedate).ToList();
                        var dat = (from g in db.groupinfos
                                   where g.groupid == groupid
                                   select new { g.groupid, userid = 0, name = g.groupname, status = "", images = "images/icons/avatar1.png", mytype = 1 }).ToList();
                        obj.Add(dat);
                        if (chat.Count > 0)
                        {
                            foreach (var i in chat)
                            {
                                ct.Add(new Chatmodel
                                {
                                    chatid = i.chatid,
                                    isgroup = i.isgroup,
                                    masssage = i.masssage,
                                    massagedate = convertTime(i.massagedate),
                                    file = i.file,
                                    groupid = i.groupid,
                                    isseen = i.isseen,
                                    userid = i.userid,
                                    firstname = i.firstname,
                                    filename = i.filename,
                                    isimg = i.isimg,
                                    flag = 1
                                });
                            } 
                        }
                      obj.Add(ct);
                }            
            }
            return Json(obj);
        }

        public  string convertTime(DateTime dt)
        {
            string ss = "";
            var date=DateTime.Now;
            var dif = dt.Subtract(date).Days / (365.25 / 12);
            if (dif==0)
            {
                ss = dt.ToShortTimeString();
            }
            else if (dif == 1)
            {
                ss ="Yesterday"+ dt.ToShortTimeString();
            }
            else if (dif >1 && dif<30)
            {
                ss = dif + "Days ago";
            }
            else if (dif >30 && dif < 365)
            {
                ss = dif%30 + "Month ago";

            }
            else 
            {
                ss = dif % 365 + "Month ago";

            }
           return ss;
        }
        public IActionResult Notification()
        {
           
            return View();
        }
        public IActionResult GetMember(string key)
        {
            int userid=0;
            if (User.Identity.IsAuthenticated)
            {
                userid = int.Parse(User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
                var dat = (from u in db.userinfos.Where(w => w.userid != userid && w.firstname.Contains(key)&& w.role != "admin")
                           join co in db.companyinfos on u.companyid equals co.companyid
                           join j in db.jobinfos on u.jobid equals j.jobid
                           join gn in db.groupmembers on u.userid equals gn.userid into gnn
                           from gn in gnn.DefaultIfEmpty()
                           join g in db.groupinfos.Where(w => w.isgroup == false) on gn.groupid equals g.groupid into gg
                           from g in gg.DefaultIfEmpty()
                           join c in db.chatinfos on g.groupid equals c.groupid into cc
                           from c in cc.DefaultIfEmpty()
                           group new { massagedate = c == null ? DateTime.Now.AddDays(-365) : c.massagedate } by new { groupid = g == null ? 0 : g.groupid, u.userid, name = u.firstname + " " + u.lastname, u.images, u.status, j.jobname, co.company } into ss
                           select new { ss.Key.name, ss.Key.groupid, ss.Key.userid, ss.Key.status, ss.Key.images, ss.Key.jobname, ss.Key.company, massagedate = ss.Max(m => m.massagedate) }).OrderByDescending(d => d.massagedate).ToList();
                return Json(dat);
            }
            else
            {
                var dat = (from u in db.userinfos.Where(w => w.firstname.Contains(key)&& w.role != "admin")
                           join co in db.companyinfos on u.companyid equals co.companyid
                           join j in db.jobinfos on u.jobid equals j.jobid
                           join gn in db.groupmembers on u.userid equals gn.userid into gnn
                           from gn in gnn.DefaultIfEmpty()
                           join g in db.groupinfos.Where(w => w.isgroup == false) on gn.groupid equals g.groupid into gg
                           from g in gg.DefaultIfEmpty()
                           join c in db.chatinfos on g.groupid equals c.groupid into cc
                           from c in cc.DefaultIfEmpty()
                           group new { massagedate = c == null ? DateTime.Now.AddDays(-365) : c.massagedate } by new { groupid = g == null ? 0 : g.groupid, u.userid, name = u.firstname + " " + u.lastname, u.images, u.status, j.jobname, co.company } into ss
                           select new { ss.Key.name, ss.Key.groupid, ss.Key.userid, ss.Key.status, ss.Key.images, ss.Key.jobname, ss.Key.company, massagedate = ss.Max(m => m.massagedate) }).OrderByDescending(d => d.massagedate).ToList();
                return Json(dat);
            }
           
        }
        public IActionResult GetGroup()
        {
           
            var dat = db.groupinfos.Where(g=>g.isgroup == true) .FirstOrDefault();
            return Json(dat);
        }
        public IActionResult SendChat(massagemodel chat, IFormFile file)
        {
            var userid = int.Parse(User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());

            Chatinfo ch = new Chatinfo();
            if (chat.mtype==0)
            {
                int grid = 0;
              
                var gc=db.groupinfos.Where(g=>g.groupid == chat.groupid).FirstOrDefault();
                if (gc==null)
                {
                    Groupinfo gr = new Groupinfo();
                    gr.isgroup = false;
                    gr.groupname = ".";
                    db.groupinfos.Add(gr);
                    db.SaveChanges();
                    Groupmember gm = new Groupmember();
                    Groupmember gm1 = new Groupmember();
                    gm.userid = userid;  
                    gm1.groupid = gr.groupid; 
                    gm.groupid= gr.groupid;
                    gm1.userid = chat.userid;
                    db.groupmembers.Add(gm);
                    db.groupmembers.Add(gm1);
                    db.SaveChanges();
                    grid = gr.groupid;
                }
                else
                {
                    grid = gc.groupid;

                }
                ch.userid=userid;
                ch.groupid = grid;
                ch.chatto = chat.userid;
                ch.masssage = chat.masssage;
                ch.massagedate=DateTime.Now;
               
                Notificationinfo nt = new Notificationinfo();
                nt.userid = chat.userid;
                nt.notification = "You have a new massage";
                nt.notificationdate = DateTime.Now;
                db.notificationinfos.Add(nt);
                db.SaveChanges();
            }
            else
            {
                ch.groupid = chat.groupid;
                ch.masssage = chat.masssage;
                ch.massagedate = DateTime.Now;
                ch.userid = userid;
            }

            if (file != null)
            {
                string wwwPath = this.env.WebRootPath;
                string contentPath = this.env.ContentRootPath;
                string con = file.FileName;
                string path = Path.Combine(this.env.WebRootPath, "images/chat");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string ext = Path.GetExtension(file.FileName);
                string fname = con + ext;
                using (FileStream stream = new FileStream(Path.Combine(path, fname), FileMode.Create))
                {

                    file.CopyTo(stream);
                    ch.file = "images/chat/" + fname; 
                    ch.filename = fname;
                    if (ext==".jpg"|| ext == ".png"||ext == ".gif")
                    {
                        ch.isimg = true;
                    }
                }
            }
            db.Add(ch);
            db.SaveChanges();
            
            return Json(ch);
        }

        public IActionResult GetNotification()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = int.Parse(User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
                var dat = db.notificationinfos.Where(g => g.userid == userid).ToList();
                List<Notification> nt = new List<Notification>();
                if (dat.Count>0)
                {
                    foreach (var i in dat)
                    {
                        nt.Add(new Notification {
                            notid=i.notid,
                            notification=i.notification,
                            userid=i.userid,
                            notificationdate= convertTime(i.notificationdate)
                        });
                    }
                }
                
                return Json(dat);
            }
            else
            {
                return Json("");
            }
        }

        public IActionResult GetTotalUser()
        {
           
            var dat = db.userinfos.Where(g => g.role == "user").Count();
            return Json(dat);
        }
        public IActionResult GetchatMember()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = int.Parse(User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
                var dat = (from u in db.userinfos.Where(w => w.userid != userid && w.role == "User")
                           join co in db.companyinfos on u.companyid equals co.companyid
                           join j in db.jobinfos on u.jobid equals j.jobid
                           join gn in db.groupmembers on u.userid equals gn.userid
                           join g in db.groupinfos.Where(w => w.isgroup == false) on gn.groupid equals g.groupid
                           join c in db.chatinfos on g.groupid equals c.groupid
                           group new { massagedate = c == null ? DateTime.Now.AddDays(-365) : c.massagedate } by new { groupid = g == null ? 0 : g.groupid, u.userid, name = u.firstname + " " + u.lastname, u.images, u.status, j.jobname, co.company } into ss
                           select new { ss.Key.name, ss.Key.groupid, ss.Key.userid, ss.Key.status, ss.Key.images,ss.Key.jobname,ss.Key.company, massagedate = ss.Max(m => m.massagedate) }).OrderByDescending(d => d.massagedate).ToList();
                return Json(dat);
            }
            else
            {
                var dat = (from u in db.userinfos.Where(w => w.role == "User")
                           join co in db.companyinfos on u.companyid equals co.companyid
                           join j in db.jobinfos on u.jobid equals j.jobid
                           join gn in db.groupmembers on u.userid equals gn.userid
                           join g in db.groupinfos.Where(w => w.isgroup == false) on gn.groupid equals g.groupid
                           join c in db.chatinfos on g.groupid equals c.groupid
                           group new { massagedate = c == null ? DateTime.Now.AddDays(-365) : c.massagedate } by new { groupid = g == null ? 0 : g.groupid, u.userid, name = u.firstname + " " + u.lastname, u.images, u.status, j.jobname, co.company } into ss
                           select new { ss.Key.name, ss.Key.groupid, ss.Key.userid, ss.Key.status, ss.Key.images, ss.Key.jobname, ss.Key.company, massagedate = ss.Max(m => m.massagedate) }).OrderByDescending(d => d.massagedate).ToList();
                return Json(dat);
            }
        }
    }
}