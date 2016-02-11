using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class ConfirmModalViewModel
    {
        public string Id { get; private set; }
        public string Question { get; set; }
        public string YesButton { get; set; }
        public string NoButton { get; set; }

        public ConfirmModalViewModel(string id)
        {
            Id = id;
        }
    }
}