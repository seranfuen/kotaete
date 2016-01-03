using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class AnswersController : AlertControllerBase
    {
        public ActionResult Create([Bind(Include = "QuestionDetailId, AnswerContent")] QuestionDetailAnswerViewModel answerViewModel)
        {
            var answer = new Answer()
            {
                Question = Context.QuestionDetails.First(qstDetail => qstDetail.QuestionDetailId == answerViewModel.QuestionDetailId),
                QuestionDetailId = answerViewModel.QuestionDetailId,
                TimeStamp = DateTime.Now,
                Content = answerViewModel.AnswerContent                
            };
            if (answer.Question.Answered)
            {
                AddAlertDanger("This questions is already answered", "Error", true);
            }
            else {
                var result = TryValidateModel(answer);
                if (result)
                {
                    answer.Question.Answered = true;
                    Context.Answers.Add(answer);
                    Context.SaveChanges();
                    AddAlertSuccess("Your answer was posted", "", true);
                }
                else
                {
                    AddAlertDanger("The answer cannot be empty!", "", true);
                }
            }
            return RedirectToAction("Index", "Inbox");
        }
    }
}