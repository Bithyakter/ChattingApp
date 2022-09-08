using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChattingApp.Models
{
    public class Companyinfo
    {
        [Key]
        public int companyid { get; set; }
        public string company { get; set; }

    }
    public class Jobinfo
    {
        [Key]
        public int jobid { get; set; }
        public string jobname { get; set; }

    }
    public class Userinfo
    {
        [Key]
        public int userid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }  
        public string role { get; set; }
        public string images { get; set; }
        public string varificationcode { get; set; }
        public bool isvarify { get; set; }
        public bool isblock { get; set; }
        public bool status { get; set; }
        public int jobid { get; set; }
        public Jobinfo job { get; set; }
        public int companyid { get; set; }
        public Companyinfo comp { get; set; }
        
    }

    public class Groupinfo
    {
        [Key]
        public int groupid { get; set; }
        public string groupname { get; set; }
        public bool isgroup { get; set; }
      

    }
    public class Groupmember
    {
        [Key]
        public int memberid { get; set; }
        public int groupid { get; set; }
       
        public bool blockstatus { get; set; }
        public int userid { get; set; }
        public Groupinfo group { get; set; }

    }
    public class Chatinfo
    {
        [Key]
        public int chatid { get; set; }
        public string masssage { get; set; }
        public string file { get; set; }
        public string filename { get; set; }
        public bool isimg { get; set; }
        public DateTime massagedate { get; set; }
        public bool isseen { get; set; }
        public int userid { get; set; }
        public int chatto { get; set; }
        public int groupid { get; set; }
        public Groupinfo group { get; set; }

    }
    public class Notificationinfo
    {
        [Key]
        public int notid { get; set; }
        public string notification { get; set; }
  
        public DateTime notificationdate { get; set; }
       
        public int userid { get; set; }
   
        public Userinfo user { get; set; }

    }
}
