using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KotaeteMVC.Models;

namespace KotaeteMVC.Controllers
{
    public class QuestionDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: QuestionDetails
        public ActionResult Index()
        {
            var questionDetails = db.QuestionDetails.Include(q => q.AskedBy).Include(q => q.AskedTo).Include(q => q.Question);
            return View(questionDetails.ToList());
        }

        // GET: QuestionDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionDetail questionDetail = db.QuestionDetails.Find(id);
            if (questionDetail == null)
            {
                return HttpNotFound();
            }
            return View(questionDetail);
        }

        // GET: QuestionDetails/Create
        public ActionResult Create()
        {
            ViewBag.AskedById = new SelectList(db.Users, "Id", "Avatar");
            ViewBag.AskedToId = new SelectList(db.Users, "Id", "Avatar");
            ViewBag.QuestionId = new SelectList(db.Questions, "QuestionId", "Content");
            return View();
        }

        // POST: QuestionDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuestionDetailId,QuestionId,AskedToId,AskedById,TimeStamp,Deleted,SeenByUser,Answered")] QuestionDetail questionDetail)
        {
            if (ModelState.IsValid)
            {
                db.QuestionDetails.Add(questionDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AskedById = new SelectList(db.Users, "Id", "Avatar", questionDetail.AskedById);
            ViewBag.AskedToId = new SelectList(db.Users, "Id", "Avatar", questionDetail.AskedToId);
            ViewBag.QuestionId = new SelectList(db.Questions, "QuestionId", "Content", questionDetail.QuestionId);
            return View(questionDetail);
        }

        // GET: QuestionDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionDetail questionDetail = db.QuestionDetails.Find(id);
            if (questionDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.AskedById = new SelectList(db.Users, "Id", "Avatar", questionDetail.AskedById);
            ViewBag.AskedToId = new SelectList(db.Users, "Id", "Avatar", questionDetail.AskedToId);
            ViewBag.QuestionId = new SelectList(db.Questions, "QuestionId", "Content", questionDetail.QuestionId);
            return View(questionDetail);
        }

        // POST: QuestionDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuestionDetailId,QuestionId,AskedToId,AskedById,TimeStamp,Deleted,SeenByUser,Answered")] QuestionDetail questionDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AskedById = new SelectList(db.Users, "Id", "Avatar", questionDetail.AskedById);
            ViewBag.AskedToId = new SelectList(db.Users, "Id", "Avatar", questionDetail.AskedToId);
            ViewBag.QuestionId = new SelectList(db.Questions, "QuestionId", "Content", questionDetail.QuestionId);
            return View(questionDetail);
        }

        // GET: QuestionDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionDetail questionDetail = db.QuestionDetails.Find(id);
            if (questionDetail == null)
            {
                return HttpNotFound();
            }
            return View(questionDetail);
        }

        // POST: QuestionDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionDetail questionDetail = db.QuestionDetails.Find(id);
            db.QuestionDetails.Remove(questionDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
