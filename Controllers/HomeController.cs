using knowledgeSphere.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using NuGet.DependencyResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using NuGet.Protocol.Core.Types;

namespace knowledgeSphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly KnowledgeContext context;

        public HomeController(KnowledgeContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentLogin()
        {
            if (HttpContext.Session.GetString("StudentSession") != null)
            {
                return RedirectToAction("StDashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult StudentLogin(Student student)
        {
            var user = context.Students.Where(x => x.Email == student.Email && x.Password == student.Password).FirstOrDefault();
            if (user != null)
            {
                HttpContext.Session.SetString("StudentSession", user.Email);
                return RedirectToAction("StDashboard");
            }
            else
            {
                ViewBag.Message = "Login Failed!";
            }
            return View();
        }

        public IActionResult StDashboard()
        {
            if (HttpContext.Session.GetString("StudentSession") != null)
            {
                string studentEmail = HttpContext.Session.GetString("StudentSession");

                var enrolledCourses = context.EnrollStudents
                .Where(enrollment => enrollment.StudentEmail == studentEmail)
                .Select(enrollment => enrollment.Course)
                .ToList();

                // Pass the enrolled courses to the view
                return View(enrolledCourses);
            }
            else
            {
                return RedirectToAction("StudentLogin");
            }
            //return View();
        }

        public ActionResult StudentRegister()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentRegister(Student student)
        {
            if (ModelState.IsValid)
            {
                await context.Students.AddAsync(student);
                await context.SaveChangesAsync();
                TempData["Success"] = "Registered Succesfully!";
                return RedirectToAction("StudentLogin");
            }
            return View();
        }

        public IActionResult StudentLogout()
        {
            if (HttpContext.Session.GetString("StudentSession") != null)
            {
                HttpContext.Session.Remove("StudentSession");
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult TeacherLogin()
        {
            if (HttpContext.Session.GetString("TeacherSession") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }
        [HttpPost]
        public IActionResult TeacherLogin(Teacher teacher)
        {
            var user= context.Teachers.Where(x => x.Email==teacher.Email && x.Password==teacher.Password).FirstOrDefault();
            if(user!=null)
            {
                HttpContext.Session.SetString("TeacherSession", user.Email);
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Message = "Login Failed!";
            }    
            return View();
        }

        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetString("TeacherSession")!= null)
            {
                ViewBag.Mysession = HttpContext.Session.GetString("TeacherSession").ToString();
            }
            else
            {
                return RedirectToAction("TeacherLogin");
            }
            string teacherEmail = ViewBag.Mysession;
            List<Course> teacherCourses = context.Courses.Where(c => c.Mail == teacherEmail).ToList();

            ViewBag.TeacherCourses = teacherCourses;

            return View();
        }

        public IActionResult CourseContent(string course)
        {
            // Assuming you have a DbContext named 'context' with a 'Courses' DbSet
            //Course selectedCourse = context.Courses.FirstOrDefault(c => c.CourseNo == course);
            Course selectedCourse = context.Courses.Include(c => c.Posts).FirstOrDefault(c => c.CourseNo == course);

            // Pass the selected course to the view for rendering details
            return View(selectedCourse);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(int courseId, string title, string content, IFormFile file)
        {
            var course = await context.Courses.FindAsync(courseId);

            if (course != null)
            {
                var newPost = new CoursePost
                {
                    Title = title,
                    Content = content,
                    CreatedAt = DateTime.Now,
                    CourseId = courseId
                };

                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        newPost.FileData = memoryStream.ToArray();
                        newPost.FileName = file.FileName;
                        newPost.ContentType = file.ContentType;
                    }
                }
                

                context.CoursePosts.Add(newPost);
                await context.SaveChangesAsync();

                // Redirect back to the CourseContent page
                return RedirectToAction("CourseContent", new { course = course.CourseNo });
            }

            // Handle the case where the course is not found
            return RedirectToAction("Dashboard");
        }


        public async Task<IActionResult> TeacherLogout()
        {
            if (HttpContext.Session.GetString("TeacherSession") != null)
            {
                await HttpContext.SignOutAsync();
                HttpContext.Session.Remove("TeacherSession");

                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult TeacherRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> TeacherRegister(Teacher teacher)
        {
            if(ModelState.IsValid)
            {
                await context.Teachers.AddAsync(teacher);
                await context.SaveChangesAsync();
                TempData["Success"] = "Registered Succesfully!";
                return RedirectToAction("TeacherLogin");
            }
            return View();
        }

        public IActionResult AddCourse()
        {

            string teacherEmail = HttpContext.Session.GetString("TeacherSession");


            ViewBag.Mysession= teacherEmail;
            Course model = new Course
            {
                Mail = teacherEmail
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(Course model)
        {
            if (ModelState.IsValid)
            {
                string teacherEmail = HttpContext.Session.GetString("TeacherSession");
                model.Mail = teacherEmail;
                try
                {
                    await context.Courses.AddAsync(model);
                    await context.SaveChangesAsync();

                    // Display a success message using JavaScript
                    TempData["SuccessMessage"] = "Course added successfully";
                }
                catch (Exception ex)
                {
                    // Handle exceptions if needed
                    return BadRequest(new { success = false, message = "An error occurred while adding the course." });
                }

                return RedirectToAction("Dashboard");
            }

            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        [HttpGet]
        public IActionResult EnrollStudents()
        {
            string teacherEmail = HttpContext.Session.GetString("TeacherSession");

            ViewBag.Mysession = teacherEmail;

            var courses = context.Courses
            .Where(c => c.Mail == teacherEmail)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.CourseNo} - {c.CourseTitle}" })
            .ToList();

            
            EnrollStudents model = new EnrollStudents
            {
                TeacherMail = teacherEmail,
                Courses = courses
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnrollStudents(string studentEmail, int courseId)
        {
            //Debug.WriteLine($"ModelState Errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)))}");

            if (ModelState.IsValid)
            {
                string teacherEmail = HttpContext.Session.GetString("TeacherSession");
                var course = await context.Courses.FindAsync(courseId);

                if (course != null)
                {
                    var enrollment = new EnrollStudents
                    {
                        StudentEmail = studentEmail,
                        TeacherMail = teacherEmail,
                        CourseId = courseId
                    };

                    context.EnrollStudents.Add(enrollment);
                    await context.SaveChangesAsync();

                    return RedirectToAction("Dashboard");
                }
                return BadRequest(new { success = false, errors = "Course not found" });
            }

            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        }

        public IActionResult StResource()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> StResource1(ResourceView model)
        {
            if (ModelState.IsValid)
            {
                var file = HttpContext.Request.Form.Files.FirstOrDefault();
                
                if (file != null && file.Length > 0)
                {
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        model.FileData = memoryStream.ToArray();
                    }

                    model.FileName = file.FileName;
                    
                    await context.ResourceView.AddAsync(model);
                    await context.SaveChangesAsync();

                    //TempData["Success"] = "Resource submitted successfully!";
                    return RedirectToAction("StDashboard");
                }
                else
                {
                    ModelState.AddModelError("File", "Please upload a file.");
                }
            }

            // If the model is not valid or the file is not uploaded, return to the form with validation errors
            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        public IActionResult AllResources()
        {
            var resources = context.ResourceView.ToList(); // Retrieve all resources from the database
            return View(resources);
        }

        public IActionResult DownloadFile(int id)
        {
            // Retrieve the ResourceView from the database
            var resource = context.ResourceView.Find(id);

            // Check if the resource and file data exist
            if (resource != null && resource.FileData != null)
            {
                // Return the file as a FileResult
                return File(resource.FileData, "application/octet-stream", resource.ResourceName);
            }

            // If the resource or file data is not found, return a not found response
            return RedirectToAction("Dashboard");
        }

        public IActionResult StCourseContent(int courseId)
        {
            // Retrieve the course and its posts based on courseId
            var course = context.Courses.Include(c => c.Posts).FirstOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public IActionResult ListQ()
        {
            var qs = context.Questions.Include(q=>q.Answers).ToList(); // Retrieve all resources from the database
            //List<QnAViewModel> qnaList = context.Questions.Include(q => q.Answers).ToList();
            return View(qs);
        }


        [HttpPost]
        public async Task<IActionResult> NewAnswer(AnswerViewModel model)
        {
            if (ModelState.IsValid)
            {
                string Mail;
                if (HttpContext.Session.GetString("TeacherSession") != null)
                {
                    Mail = HttpContext.Session.GetString("TeacherSession");
                }
                else
                {
                    Mail = HttpContext.Session.GetString("StudentSession");
                }
                model.Author = Mail;

                await context.Answers.AddAsync(model);
                await context.SaveChangesAsync();
                //TempData["Success"] = "Answer added!";
                return RedirectToAction("ListQ");
            }
            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        }


        public IActionResult NewQuestionPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewQuestionPage(QnAViewModel ques)
        {
            string Mail;
            if (HttpContext.Session.GetString("TeacherSession") != null)
            {
                Mail = HttpContext.Session.GetString("TeacherSession");
            }
            else{
                Mail= HttpContext.Session.GetString("StudentSession");
            }
            ques.Author = Mail;
            if (ModelState.IsValid)
            { 
                await context.Questions.AddAsync(ques);
                await context.SaveChangesAsync();
                TempData["Success"] = "Question added!";
                return RedirectToAction("ListQ");
            }
            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        }






        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}