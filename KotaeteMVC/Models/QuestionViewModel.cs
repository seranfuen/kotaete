using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class QuestionViewModel
    {
        public virtual Question Question { get; set; }

        public virtual string AskedUserName { get; set; }
    }
}