using Microsoft.Ajax.Utilities;
using NewQA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NewQA.Controllers
{
    public class HomeController : Controller
    {
        private QADBEntities db = new QADBEntities();



      
        public ActionResult ShowQusetionComment(int qid) 
        {
            return RedirectToAction("ShowQandA", new { id = qid, ShowComment = true });
        }


        public ActionResult ShowAnswerComment(int qid, int aid)
        {
            return RedirectToAction("ShowQandA", new { id = qid, answerId = aid });
        }

        [HttpPost]
        public ActionResult CreateQuestionComment(int qid)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                var userId = int.Parse(cookieValue);
                var qcomment = new Qcomment();
                qcomment.question_id = qid;
                qcomment.body = Request.Form["qCommentBody"];
                qcomment.user_id = userId;
                db.Question.Find(qid).Qcomment.Add(qcomment);
                db.SaveChanges();
                return RedirectToAction("ShowQandA", new { id = qid, ShowComment = true });
            }
            return RedirectToAction("LogIn");
        }


        [HttpPost]
        public ActionResult CreateAnswerComment(int qid, int aid)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                var userId = int.Parse(cookieValue);
                var acomment = new Acomment();
                acomment.answer_id = aid;
                acomment.body = Request.Form["aCommentBody"];
                acomment.user_id = userId;
                db.Answer.Find(aid).Acomment.Add(acomment);
                db.SaveChanges();
                return RedirectToAction("ShowQandA", new { id = qid, ShowComment = true });
            }
            return RedirectToAction("LogIn");
        }

        //GET: ShowQandA
        public ActionResult ShowQandA(int id, int? answerId = null, bool showComment = false)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                ViewBag.CurrentUser = db.AppUser.Find(int.Parse(cookieValue));
            }
            ViewBag.UserId = new SelectList(db.AppUser, "id", "name");

            ViewBag.AnswerId = answerId;
            
            ViewBag.ShowComment = showComment ? true : false;

            var question = db.Question.FirstOrDefault(q => q.id == id);
            ViewBag.Count = question.Qvote.Count(qv => qv.status == true) - question.Qvote.Count(qv => qv.status == false);

           
            return View(question);
        }


        //Post: CreateAnswer
        [HttpPost]
        public ActionResult CreateAnswer(int qid)
        {
            
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                var userId = int.Parse(cookieValue);
                var question = db.Question.Find(qid);
                var answer = new Answer();
                answer.body = Request.Form["answerBody"] ;
                answer.user_id = userId;
                answer.question_id = qid;
                answer.time = DateTime.Now;
                question.Answer.Add(answer);
                db.SaveChanges();

                return RedirectToAction("ShowQandA", new { id = qid, ShowComment = true });
            }

            return RedirectToAction("LogIn");
        }

       


        // GET:
        public ActionResult AskQuestion()
        {
            var checkList = new List<CheckModel>();

            var tags = db.Tag.ToList();

            for (var i =0; i< tags.Count(); i++) 
            {
                checkList.Add(new CheckModel { Id = tags[i].id, Name = tags[i].name, Checked = false });
            }
         
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                ViewBag.CurrentUser = db.AppUser.Find(int.Parse(cookieValue));
                return View(checkList);
            }


            return RedirectToAction("LogIn");
        }
        // POST: askQestion
        [HttpPost]
        public ActionResult AskQuestion(string QuestionTitle, string QuestionBody, List<CheckModel> CheckList)
        {
       
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                var currentUser = db.AppUser.Find(int.Parse(cookieValue));
                ViewBag.CurrentUser = currentUser;
                Question question = new Question();
                question.time = DateTime.Now;
                question.title = QuestionTitle;
                question.body = QuestionBody;
                question.user_id = int.Parse(cookieValue);
               
                foreach (var tag in CheckList) 
                {
                    if (tag.Checked) 
                    {
                        var tq = new Tag_Question();
                        tq.tag_id = tag.Id;
                        tq.question_id = question.id;
                        question.Tag_Question.Add(tq);
                    } 
                }
                
                currentUser.Question.Add(question);

                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return RedirectToAction("LogIn");
        }
        //GET: ShowQestion 
        public ActionResult Index(int id = 1, int sortType = 0)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                ViewBag.CurrentUser = db.AppUser.Find(int.Parse(cookieValue));
            }
            
            ViewBag.PageNum = id;
            ViewBag.UserId = new SelectList(db.AppUser, "id", "name");
            int count = db.Question.Count();
            ViewBag.Total = count / 10 + 1;
            var result = db.Question.OrderByDescending(q => q.time).Take(10 * id).Skip((id - 1) * 10).ToList();
            if (sortType == 1)
            {
                result = db.Database.SqlQuery<Question>("showNewestQuestion").ToList();
               
            } else if (sortType == 2) 
            {
                result = db.Database.SqlQuery<Question>("MostAnsweredQuestion").ToList();
            }
           
            return View(result);
        }
        
        public ActionResult LogOut()
        {
            Response.Cookies["userId"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index");
        }

        //GET: logIn
        public ActionResult LogIn()
        {
            ViewBag.UserId = new SelectList(db.AppUser, "id", "name");

            return View();
        }
        //Post LogIn
        [HttpPost]
        public ActionResult LogIn(int? UserId)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser user = db.AppUser.Find(UserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            HttpCookie cookie = new HttpCookie("userId", UserId.ToString());//初使化并设置Cookie的名称
            DateTime dt = DateTime.Now;
            TimeSpan ts = new TimeSpan(0, 0, 1, 0, 0);//过期时间为1分钟
            cookie.Expires = dt.Add(ts);//设置过期时间
            Response.AppendCookie(cookie);
            ViewBag.UserId = new SelectList(db.AppUser, "id", "name");
           
            return RedirectToAction("Index");
        }


        //GET: ShowQuestionInTag 
        public ActionResult ShowQuestionInTag(int id)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
            if (cookieValue != "")
            {
                ViewBag.CurrentUser = db.AppUser.Find(int.Parse(cookieValue));
            }
            ViewBag.TagId = id;
            ViewBag.UserId = new SelectList(db.AppUser, "id", "name");
            var result = db.Question.Where(q => q.Tag_Question.Any(tq => tq.Tag.id == id)).ToList();
            int count = result.Count();
            ViewBag.Total = count / 10 + 1;
            return View(result);
        }

        //GET:
        public ActionResult ShowNewestQuestion() 
        {
            var result = db.Database.SqlQuery<Question>("showNewestQuestion").ToList();
            return RedirectToAction("Index",new { sortType = 1 });
        }

        //GET:
        public ActionResult ShowMostAnsweredQuestion()
        {
            var result = db.Database.SqlQuery<Question>("MostAnsweredQuestion").ToList();
            return RedirectToAction("Index" ,new { sortType = 2 });
        }

        //Post: VoteQuestionUp 
        [HttpPost]
        public ActionResult VoteQuestionUp(int QuestionId)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();
           
            if (cookieValue != "")
            {
                var question = db.Question.Find(QuestionId);
                var currentUserId = int.Parse(cookieValue);
                var userIdOfTheQuestion = question.user_id;
                ViewBag.CurrentUser = db.AppUser.Find(currentUserId);
                if (userIdOfTheQuestion == currentUserId)
                { 
                    return RedirectToAction("ShowQandA", new { id = QuestionId });
                }
                if (db.Qvote.Any(qv => qv.user_id == currentUserId && qv.question_id == QuestionId))
                {
                    var qvote = db.Qvote.FirstOrDefault(qv => qv.user_id == currentUserId && qv.question_id == QuestionId);
                    if (qvote.status == false)
                    {
                        qvote.status = true;
                        question.AppUser.reputation += 5;
                        db.SaveChanges();
                    }

                }
                else
                {
                    var qvote = new Qvote();
                    qvote.status = true;
                    qvote.question_id = QuestionId;
                    qvote.user_id = currentUserId;
                   
                    question.AppUser.reputation += 5;
                    question.Qvote.Add(qvote);
                    db.SaveChanges();
                }

                ViewBag.Count = question.Qvote.Count(qv => qv.status == true) - question.Qvote.Count(qv => qv.status == false);
                return RedirectToAction("ShowQandA", new { id = QuestionId });

            }
            return RedirectToAction("LogIn");
        }

        [HttpPost]
        public ActionResult VoteQuestionDown(int QuestionId)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();

            if (cookieValue != "")
            {
                var question = db.Question.Find(QuestionId);
                var currentUserId = int.Parse(cookieValue);
                var userIdOfTheQuestion = question.user_id;
                ViewBag.CurrentUser = db.AppUser.Find(currentUserId);
                if (userIdOfTheQuestion == currentUserId)
                {
                    return RedirectToAction("ShowQandA", new { id = QuestionId });
                }
                if (db.Qvote.Any(qv => qv.user_id == currentUserId && qv.question_id == QuestionId))
                {
                    var qvote = db.Qvote.FirstOrDefault(qv => qv.user_id == currentUserId && qv.question_id == QuestionId);
                    if (qvote.status == true)
                    {
                        qvote.status = false;
                        question.AppUser.reputation -= 5;
                        db.SaveChanges();
                    }

                }
                else
                {
                    var qvote = new Qvote();
                    qvote.status = false;
                    qvote.question_id = QuestionId;
                    qvote.user_id = currentUserId;

                    question.AppUser.reputation -= 5;
                    question.Qvote.Add(qvote);
                    db.SaveChanges();
                }

                ViewBag.Count = question.Qvote.Count(qv => qv.status == true) - question.Qvote.Count(qv => qv.status == false);
                return RedirectToAction("ShowQandA", new { id = QuestionId });

            }
            return RedirectToAction("LogIn");
        }
        //Post: VoteAnswerUp 
        [HttpPost]
        public ActionResult VoteAnswerUp(int QuestionId, int AnswerId)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();

            if (cookieValue != "")
            {
                var answer = db.Answer.Find(AnswerId);
                var currentUserId = int.Parse(cookieValue);
                var userIdOfTheAnswer = answer.user_id;
                ViewBag.UserOfTheAnwser = db.AppUser.Find(userIdOfTheAnswer);
                ViewBag.CurrentUser = db.AppUser.Find(currentUserId);
                if (currentUserId == userIdOfTheAnswer) 
                {
                    return RedirectToAction("ShowQandA", new { id = QuestionId });
                }
                if (db.Avote.Any(av => av.user_id == currentUserId && av.answer_id == AnswerId))
                {
                    var avote = db.Avote.FirstOrDefault(av => av.user_id == currentUserId && av.answer_id == AnswerId);
                    if (avote.status == false)
                    {
                        avote.status = true;
                        answer.AppUser.reputation += 5;
                        db.SaveChanges();
                    }
                  

                }
                else
                {
                    var avote = new Avote();
                    avote.status = true;
                    avote.answer_id = AnswerId;
                    avote.user_id = currentUserId;

                    answer.AppUser.reputation += 5;
                    answer.Avote.Add(avote);
                    db.SaveChanges();
                }

                //ViewBag.AnswerCount = answer.Avote.Count(qv => qv.status == true) - answer.Avote.Count(qv => qv.status == false);
                return RedirectToAction("ShowQandA", new { id = QuestionId });

            }
            return RedirectToAction("LogIn");
        }

        //Post: VoteAnswerDown
        [HttpPost]
        public ActionResult VoteAnswerDown(int QuestionId, int AnswerId)
        {
            var cookieValue = Request.Cookies["userId"] == null ? "" : Request.Cookies["userId"].Value.ToString();

            if (cookieValue != "")
            {
                var answer = db.Answer.Find(AnswerId);
                var currentUserId = int.Parse(cookieValue);
                var userIdOfTheAnswer = answer.user_id;
                ViewBag.UserOfTheAnwser = db.AppUser.Find(userIdOfTheAnswer);
                ViewBag.CurrentUser = db.AppUser.Find(currentUserId);
                if (currentUserId == userIdOfTheAnswer)
                {
                    return RedirectToAction("ShowQandA", new { id = QuestionId });
                }
                if (db.Avote.Any(av => av.user_id == currentUserId && av.answer_id == AnswerId))
                {
                    var avote = db.Avote.FirstOrDefault(av => av.user_id == currentUserId && av.answer_id == AnswerId);
                    if (avote.status == true)
                    {
                        avote.status = false;
                        answer.AppUser.reputation -= 5;
                        db.SaveChanges();
                    }
                }
                else
                {
                    var avote = new Avote();
                    avote.status = false;
                    avote.answer_id = AnswerId;
                    avote.user_id = currentUserId;

                    answer.AppUser.reputation -= 5;
                    answer.Avote.Add(avote);
                    db.SaveChanges();
                }
                return RedirectToAction("ShowQandA", new { id = QuestionId });

            }
            return RedirectToAction("LogIn");
        }


        
    }
}