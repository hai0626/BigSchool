using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace BigSchool.Controllers
{
    public class CourseController : Controller
    {
        BigSchoolContext context = new BigSchoolContext();
        // GET: Course
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext db = new BigSchoolContext();

            ModelState.Remove("LectureID");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = db.Categories.ToList();
                return View("Create", objCourse);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LectureId = user.Id;
            db.Courses.Add(objCourse);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                           .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                       .FindById(objCourse.LectureId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                           .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = context.Courses.Where(c => c.LectureId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.LectureName = currentUser.Name;
            }
            return View(courses);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Course c = context.Courses.SingleOrDefault(p => p.Id == id);
            c.ListCategory = context.Categories.ToList();
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course c)
        {
            Course dbUpdate = context.Courses.SingleOrDefault(p => p.Id == c.Id);
            if (dbUpdate != null)
            {
                context.Courses.AddOrUpdate(c);
                context.SaveChanges();
            }
            return RedirectToAction("Mine");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Course c = context.Courses.SingleOrDefault(p => p.Id == id);
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Course c)
        {
            //Attendance a = context.Attendances.Where(p => p.CourseId == c.Id).First();
            //if (a != null)
            //{
            //    MessageBox.Show("Không thể xóa vì đã có người tham dự");
            //}
            //else
            //{
            //    Course dbDelete = context.Courses.SingleOrDefault(p => p.Id == c.Id);
            //    if (dbDelete != null)
            //    {
            //        context.Courses.Remove(dbDelete);
            //        context.SaveChanges();
            //    }
            //   else
            //    {
            //        MessageBox.Show("Không có khóa học");
            //    }

            //}
            
            Course dbDelete = context.Courses.SingleOrDefault(p => p.Id == c.Id);
            if (dbDelete != null)
            {
                context.Courses.Remove(dbDelete);
                context.SaveChanges();
            }
            return RedirectToAction("Mine");
        }
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            //danh sách giảng viên được theo dõi bởi người dùng (đăng nhập) hiện tại
            var listFollwee = context.Followings.Where(p => p.FollowerID ==

            currentUser.Id).ToList();

            //danh sách các khóa học mà người dùng đã đăng ký
            var listAttendances = context.Attendances.Where(p => p.Attendee ==

            currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)

            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeID == course.Course.LectureId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LectureId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }
}