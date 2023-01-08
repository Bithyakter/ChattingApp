using ChattingApp.Models;
using System.Linq;

namespace ChattingApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DBContext db)
        {
            db.Database.EnsureCreated();
            if (db.jobinfos.ToList().Count == 0)
            {
                var data = new Jobinfo[]
                      {
                        new Jobinfo{jobname="Bench Sales"},
                        new Jobinfo{jobname="IT Recruiter"},
                        new Jobinfo{jobname="Consultant"}
                      };
                foreach (var c in data)
                {
                    db.jobinfos.Add(c);
                }
                db.SaveChanges();
            }
            

            if (db.userinfos.ToList().Count == 0)
            {
                var data = new Userinfo[]
                      {
                        new Userinfo{email="admin@gmail.com",firstname="Admin",role="admin",password="595752746157343D",isvarify=true,jobid=1,companyid=1}
                      };
                foreach (var c in data)
                {
                    db.userinfos.Add(c);
                }
                db.SaveChanges();
            }
            if (db.groupinfos.ToList().Count == 0)
            {
                var data = new Groupinfo[]
                      {
                        new Groupinfo{groupname="Common Room",isgroup=true}
                      };
                foreach (var c in data)
                {
                    db.groupinfos.Add(c);
                }
                db.SaveChanges();
            }
        }
    }
}
