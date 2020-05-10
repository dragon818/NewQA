using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NewQA;

namespace NewQA.Controllers
{
    public class QvotesController : Controller
    {
        private QADBEntities db = new QADBEntities();

        // GET: Qvotes
        public ActionResult Index()
        {
            var qvote = db.Qvote.Include(q => q.AppUser).Include(q => q.Question);
            return View(qvote.ToList());
        }

        // GET: Qvotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qvote qvote = db.Qvote.Find(id);
            if (qvote == null)
            {
                return HttpNotFound();
            }
            return View(qvote);
        }

        // GET: Qvotes/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name");
            ViewBag.question_id = new SelectList(db.Question, "id", "title");
            return View();
        }

        // POST: Qvotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,status,question_id,user_id")] Qvote qvote)
        {
            if (ModelState.IsValid)
            {
                db.Qvote.Add(qvote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", qvote.user_id);
            ViewBag.question_id = new SelectList(db.Question, "id", "title", qvote.question_id);
            return View(qvote);
        }

        // GET: Qvotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qvote qvote = db.Qvote.Find(id);
            if (qvote == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", qvote.user_id);
            ViewBag.question_id = new SelectList(db.Question, "id", "title", qvote.question_id);
            return View(qvote);
        }

        // POST: Qvotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,status,question_id,user_id")] Qvote qvote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qvote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", qvote.user_id);
            ViewBag.question_id = new SelectList(db.Question, "id", "title", qvote.question_id);
            return View(qvote);
        }

        // GET: Qvotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qvote qvote = db.Qvote.Find(id);
            if (qvote == null)
            {
                return HttpNotFound();
            }
            return View(qvote);
        }

        // POST: Qvotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Qvote qvote = db.Qvote.Find(id);
            db.Qvote.Remove(qvote);
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
