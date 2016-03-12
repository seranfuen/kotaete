using KotaeteMVC.Service;
using System.Net;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class LikesController : BaseController
    {
        [Authorize]
        [HttpPost]
        [Route("like", Name = "LikeAnswer")]
        public ActionResult LikeAnswer(int answerId)
        {
            var likesService = new LikesService(Context, this.GetPageSize());
            var result = likesService.LikeAnswer(answerId);
            return GetLikeResult(answerId, likesService, result);
        }

        private ActionResult GetLikeResult(int answerId, LikesService likesService, bool result)
        {
            if (result)
            {
                if (Request.IsAjaxRequest())
                {
                    var likesModel = likesService.GetLikeButtonViewModel(answerId);
                    return PartialView("LikeButton", likesModel);
                }
                else
                {
                    return RedirectToPrevious();
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize]
        [HttpPost]
        [Route("unlike", Name = "UnlikeAnswer")]
        public ActionResult UnlikeAnswer(int answerId)
        {
            var likesService = new LikesService(Context, this.GetPageSize());
            var result = likesService.UnlikeAnswer(answerId);
            return GetLikeResult(answerId, likesService, result);
        }
    }
}