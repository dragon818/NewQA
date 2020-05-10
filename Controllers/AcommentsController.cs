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
    public class AcommentsController : Controller
    {
        private QADBEntities db = new QADBEntities();

        // GET: Acomments
        public ActionResult Index()
        {
            var acomment = db.Acomment.Include(a => a.Answer).Include(a => a.AppUser);
            return View(acomment.ToList());
        }

        // GET: Acomments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acomment acomment = db.Acomment.Find(id);
            if (acomment == null)
            {
                return HttpNotFound();
            }
            return View(acomment);
        }

        // GET: Acomments/Create
        public ActionResult Create()
        {
            ViewBag.answer_id = new SelectList(db.Answer, "id", "body");
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name");
            return View();
        }

        // POST: Acomments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,answer_id,body,user_id")] Acomment acomment)
        {
            if (ModelState.IsValid)
            {
                db.Acomment.Add(acomment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.answer_id = new SelectList(db.Answer, "id", "body", acomment.answer_id);
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", acomment.user_id);
            return View(acomment);
        }

        // GET: Acomments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acomment acomment = db.Acomment.Find(id);
            if (acomment == null)
            {
                return HttpNotFound();
            }
            ViewBag.answer_id = new SelectList(db.Answer, "id", "body", acomment.answer_id);
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", acomment.user_id);
            return View(acomment);
        }

        // POST: Acomments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,answer_id,body,user_id")] Acomment acomment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acomment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.answer_id = new SelectList(db.Answer, "id", "body", acomment.answer_id);
            ViewBag.user_id = new SelectList(db.AppUser, "id", "name", acomment.user_id);
            return View(acomment);
        }

        // GET: Acomments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acomment acomment = db.Acomment.Find(id);
            if (acomment == null)
            {
                return HttpNotFound();
            }
            return View(acomment);
        }

        // POST: Acomments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Acomment acomment = db.Acomment.Find(id);
            db.Acomment.Remove(acomment);
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
