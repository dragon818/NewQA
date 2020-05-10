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
    public class Tag_QuestionController : Controller
    {
        private QADBEntities db = new QADBEntities();

        // GET: Tag_Question
        public ActionResult Index()
        {
            var tag_Question = db.Tag_Question.Include(t => t.Question).Include(t => t.Tag);
            return View(tag_Question.ToList());
        }

        // GET: Tag_Question/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag_Question tag_Question = db.Tag_Question.Find(id);
            if (tag_Question == null)
            {
                return HttpNotFound();
            }
            return View(tag_Question);
        }

        // GET: Tag_Question/Create
        public ActionResult Create()
        {
            ViewBag.question_id = new SelectList(db.Question, "id", "title");
            ViewBag.tag_id = new SelectList(db.Tag, "id", "name");
            return View();
        }

        // POST: Tag_Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,question_id,tag_id")] Tag_Question tag_Question)
        {
            if (ModelState.IsValid)
            {
                db.Tag_Question.Add(tag_Question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.question_id = new SelectList(db.Question, "id", "title", tag_Question.question_id);
            ViewBag.tag_id = new SelectList(db.Tag, "id", "name", tag_Question.tag_id);
            return View(tag_Question);
        }

        // GET: Tag_Question/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag_Question tag_Question = db.Tag_Question.Find(id);
            if (tag_Question == null)
            {
                return HttpNotFound();
            }
            ViewBag.question_id = new SelectList(db.Question, "id", "title", tag_Question.question_id);
            ViewBag.tag_id = new SelectList(db.Tag, "id", "name", tag_Question.tag_id);
            return View(tag_Question);
        }

        // POST: Tag_Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,question_id,tag_id")] Tag_Question tag_Question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tag_Question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.question_id = new SelectList(db.Question, "id", "title", tag_Question.question_id);
            ViewBag.tag_id = new SelectList(db.Tag, "id", "name", tag_Question.tag_id);
            return View(tag_Question);
        }

        // GET: Tag_Question/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag_Question tag_Question = db.Tag_Question.Find(id);
            if (tag_Question == null)
            {
                return HttpNotFound();
            }
            return View(tag_Question);
        }

        // POST: Tag_Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tag_Question tag_Question = db.Tag_Question.Find(id);
            db.Tag_Question.Remove(tag_Question);
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
