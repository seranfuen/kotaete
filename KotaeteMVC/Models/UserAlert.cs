using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class UserAlert
    {
        public enum MessageType
        {
            Success,
            Info,
            Warning,
            Danger
        }

        public const string Key = "AlertKey";

        public MessageType Type { get; set; }

        public string BootstrapClass
        {
            get
            {
                switch (Type)
                {
                    case MessageType.Info:
                        return "alert-info";
                    case MessageType.Success:
                        return "alert-success";
                    case MessageType.Warning:
                        return "alert-warning";
                    case MessageType.Danger:
                        return "alert-danger";
                    default:
                        return "";
                }
            }
        }
        public string Message { get; set; }

        public string Header { get; set; }

        public bool Dismissable { get; set; }

        public string DismissableClass
        {
            get
            {
                return Dismissable ? "alert-dismissable" : "";
            }
        }
    }
}