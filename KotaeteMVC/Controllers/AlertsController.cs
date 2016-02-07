using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class AlertsController : BaseController
    {
        public const string FollowSuccessKey = "FollowSuccessKey";
        public const string UnfollowSuccessKey = "UnfollowSuccessKey";
        public const string FollowErrorKey = "FollowErrorKey";

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

        [Route("alerts/alertMessage/{key}")]
        [Route("alerts/alertMessage/{key}/{param1}")]
        [Route("alerts/alertMessage/{key}/{param1}/{param2}")]
        [Route("alerts/alertMessage/{key}/{param1}/{param2}/{param3}")]
        public ActionResult AlertMessage(string key, string param1 = "", string param2 = "", string param3 = "")
        {
            return Content(GetMessageByKey(key, new string[] { param1, param2, param3 }));
        }

        private string GetMessageByKey(string key, params string[] args)
        {
            if (key == FollowSuccessKey)
            {
                return string.Format(UsersStrings.FollowingSuccess + "{0}", args);
            }
            else if (key == UnfollowSuccessKey)
            {
                return string.Format("{0}{1}{2}", UsersStrings.UnfollowingSuccessFst, GetFirstArgOrEmpty(args), UsersStrings.UnfollowingSuccessLst);
            }
            else if (key == FollowErrorKey)
            {
                return UsersStrings.FollowingError;
            }
            else if (key == "askingSuccess")
            {
                return QuestionStrings.AskingSuccess;
            }
            else if (key == "askingFailure")
            {
                return QuestionStrings.AskingFailure;
            }
            else if (key == "modalJS")
            {
                return MainGlobal.ModalJS;
            }
            else if (key == "askSuccess")
            {
                return string.Format(QuestionStrings.AskingSuccess, GetFirstArgOrEmpty(args));
            }
            else if (key == "answerSuccess")
            {
                return string.Format(AnswerStrings.SuccessAnswer, GetFirstArgOrEmpty(args));
            }
            return "UnknownAlertKey";
        }

        private string GetFirstArgOrEmpty(params string[] args)
        {
            return args.Count() > 0 ? args[0] : "";
        }
    }
}