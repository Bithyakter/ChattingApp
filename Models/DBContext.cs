
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChattingApp.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<Companyinfo> companyinfos { get; set; }
        public virtual DbSet<Jobinfo> jobinfos { get; set; }
        public virtual DbSet<Userinfo> userinfos { get; set; }
        public virtual DbSet<Groupinfo> groupinfos { get; set; }
        public virtual DbSet<Groupmember> groupmembers { get; set; }
        public virtual DbSet<Chatinfo> chatinfos { get; set; }
        public virtual DbSet<Notificationinfo> notificationinfos { get; set; }
        

    }
}
