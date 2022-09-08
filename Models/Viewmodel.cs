using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ChattingApp.Models
{
    public class Registermodel
    {
        public int userid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string conpassword { get; set; }
        public int jobid { get; set; }
        public int companyid { get; set; }
        public string role { get; set; }
        public string images { get; set; }
        public int avatar { get; set; }
    }
    public class Registerview
    {
        public List<Companyinfo> com { get; set; }
        public List<Jobinfo> job { get; set; }
       
    }
    public class massagemodel
    {
        public int userid { get; set; }
        public int groupid { get; set; }
        public string masssage { get; set; }
        public int mtype { get; set; }
        public IFormFile file { get; set; }

    }
    public class Chatmodel
    {
        public int chatid { get; set; }
        public string masssage { get; set; }
        public string file { get; set; }
        public string massagedate { get; set; }
        public bool isseen { get; set; }
        public int userid { get; set; }
        public string firstname { get; set; }
        public bool isgroup { get; set; } 
        public int groupid { get; set; }
        public int flag { get; set; }
        public string filename { get; set; }
        public bool isimg { get; set; }
        public string images { get; set; }
       
      
    }
    public class Usermodel
    {
        
              public int userid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool isblock { get; set; }
        public string jobname { get; set; }
        public string company { get; set; }
        public string role { get; set; }
        public string images { get; set; }
       
        
    }
    public class Notification
    {
      
        public int notid { get; set; }
        public string notification { get; set; }

        public string notificationdate { get; set; }

        public int userid { get; set; }

    }
}
