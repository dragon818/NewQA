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
    public class AvotesController : Controller
    {
        private QADBEntities db = new QADBEntities();

        // GET: Avotes
        public ActionResult Index()
        {
            var avote = db.Avote.Include(a => a.Answer).Include(a => a.AppUser);
            return View(avote.ToList());
        }

        // GET: Avotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Avote avote = db.Avote.Find(id);
            if (avote == null)
            {
                return HttpNotFound();
            }
            return View(avote);
        }

        // GET: Avotes/Create
        public ActionResult Create()
        {
            ViewBag.answer_id = new SelectList(db.Answer, "id", "body");
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name");
            return View();
        }

        // POST: Avotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,answer_id,status,user_id")] Avote avote)
        {
            if (ModelState.IsValid)
            {
                db.Avote.Add(avote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.answer_id = new SelectList(db.Answer, "id", "body", avote.answer_id);
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", avote.user_id);
            return View(avote);
        }

        // GET: Avotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Avote avote = db.Avote.Find(id);
            if (avote == null)
            {
                return HttpNotFound();
            }
            ViewBag.answer_id = new SelectList(db.Answer, "id", "body", avote.answer_id);
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", avote.user_id);
            return View(avote);
        }

        // POST: Avotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,answer_id,status,user_id")] Avote avote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(avote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.answer_id = new SelectList(db.Answer, "id", "body", avote.answer_id);
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", avote.user_id);
            return View(avote);
        }

        // GET: Avotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Avote avote = db.Avote.Find(id);
            if (avote == null)
            {
                return HttpNotFound();
            }
            return View(avote);
        }

        // POST: Avotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Avote avote = db.Avote.Find(id);
            db.Avote.Remove(avote);
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
