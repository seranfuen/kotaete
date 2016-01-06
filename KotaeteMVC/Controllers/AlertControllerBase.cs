using KotaeteMVC.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public abstract class AlertControllerBase : BaseController
    {

        public void AddAlertSuccess(string message, string header = "", bool dismissable = false)
        {
            AddAlert(message, header, dismissable, UserAlert.MessageType.Success);
        }

        public void AddAlertInfo(string message, string header = "", bool dismissable = false)
        {
            AddAlert(message, header, dismissable, UserAlert.MessageType.Info);
        }

        public void AddAlertWarning(string message, string header = "", bool dismissable = false)
        {
            AddAlert(message, header, dismissable, UserAlert.MessageType.Warning);
        }

        public void AddAlertDanger(string message, string header = "", bool dismissable = false)
        {
            AddAlert(message, header, dismissable, UserAlert.MessageType.Danger);
        }

        public void AddAlertDangerOverride(string message, string header = "")
        {
            if (TempData.ContainsKey(UserAlert.Key))
            {
                (TempData[UserAlert.Key] as List<UserAlert>).Clear();
            }
            AddAlertDanger(message, header);
        }

        public void AddAlertDatabaseErrror(Exception e)
        {
#if (DEBUG)
            AddAlertDanger(e.Message + " ##### " + e.StackTrace, "", true);
#else
            AddAlertDanger(MainGlobal.DatabaseError, MainGlobal.ErrorHeader, false);
#endif
            // TODO: log e, show e if debug
        }

        private void AddAlert(string message, string header, bool dismissable, UserAlert.MessageType alertType)
        {
            var alert = new UserAlert()
            {
                Message = message,
                Header = header,
                Type = alertType,
                Dismissable = dismissable
            };
            if (!TempData.ContainsKey(UserAlert.Key))
            {
                TempData[UserAlert.Key] = new List<UserAlert>();
            }
            (TempData[UserAlert.Key] as List<UserAlert>).Add(alert);
        }

    }
}