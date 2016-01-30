using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class LikesController : Controller
    {

        [Route("answer/{answerId}/like")]
        public ActionResult LikeAnswer(int answerId)
        {
            throw new NotImplementedException();   
        }

        [Route("answer/{answerId}/unlike")]
        public ActionResult UnlikeAnswer(int answerId)
        {
            throw new NotImplementedException();
        }

        [Route("answer/likes/{userName}")]
        [Route("answer/likes/{userName}/{page}")]
        public ActionResult ShowAnswerLikes(string userName, int page = 1)
        {
            throw new NotImplementedException();
        }
    }
}