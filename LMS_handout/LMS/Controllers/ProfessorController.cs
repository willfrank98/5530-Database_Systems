using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
	[Authorize(Roles = "Professor")]
	public class ProfessorController : CommonController
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Students(string subject, string num, string season, string year)
		{
			ViewData["subject"] = subject;
			ViewData["num"] = num;
			ViewData["season"] = season;
			ViewData["year"] = year;
			return View();
		}

		public IActionResult Class(string subject, string num, string season, string year)
		{
			ViewData["subject"] = subject;
			ViewData["num"] = num;
			ViewData["season"] = season;
			ViewData["year"] = year;
			return View();
		}

		public IActionResult Categories(string subject, string num, string season, string year)
		{
			ViewData["subject"] = subject;
			ViewData["num"] = num;
			ViewData["season"] = season;
			ViewData["year"] = year;
			return View();
		}

		public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
		{
			ViewData["subject"] = subject;
			ViewData["num"] = num;
			ViewData["season"] = season;
			ViewData["year"] = year;
			ViewData["cat"] = cat;
			return View();
		}

		public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
		{
			ViewData["subject"] = subject;
			ViewData["num"] = num;
			ViewData["season"] = season;
			ViewData["year"] = year;
			ViewData["cat"] = cat;
			ViewData["aname"] = aname;
			return View();
		}

		public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
		{
			ViewData["subject"] = subject;
			ViewData["num"] = num;
			ViewData["season"] = season;
			ViewData["year"] = year;
			ViewData["cat"] = cat;
			ViewData["aname"] = aname;
			return View();
		}

		public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
		{
			ViewData["subject"] = subject;
			ViewData["num"] = num;
			ViewData["season"] = season;
			ViewData["year"] = year;
			ViewData["cat"] = cat;
			ViewData["aname"] = aname;
			ViewData["uid"] = uid;
			return View();
		}

		/*******Begin code to modify********/


		/// <summary>
		/// Returns a JSON array of all the students in a class.
		/// Each object in the array should have the following fields:
		/// "fname" - first name
		/// "lname" - last name
		/// "uid" - user ID
		/// "dob" - date of birth
		/// "grade" - the student's grade in this class
		/// </summary>
		/// <param name="subject">The course subject abbreviation</param>
		/// <param name="num">The course number</param>
		/// <param name="season">The season part of the semester for the class the assignment belongs to</param>
		/// <param name="year">The year part of the semester for the class the assignment belongs to</param>
		/// <returns>The JSON array</returns>
		public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
		{
			try
			{
				var students = from stu in db.Students
							   join enr in db.Enrolled on stu.UId equals enr.UId
							   into enrolled 
							   from en in enrolled.DefaultIfEmpty()
							   join cla in db.Classes on en.ClassId equals cla.ClassId
							   into classes 
							   from c in classes.DefaultIfEmpty()
							   join cour in db.Courses on c.CourseId equals cour.CourseId
							   into courses 
							   from co in courses.DefaultIfEmpty()
							   where c.Semester == season + " " + year
							   && co.CourseNumber == num 
							   && co.SubjectAbbr == subject
							   select new
							   {
								   fname = stu.FirstName,
								   lname = stu.LastName,
								   uid = stu.UId,
								   dob = stu.BirthDate,
								   grade = en.Grade
							   };

				return Json(students.ToArray());
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return Json(null);
			}
		}


		/// <summary>
		/// Returns a JSON array with all the assignments in an assignment category for a class.
		/// If the "category" parameter is null, return all assignments in the class.
		/// Each object in the array should have the following fields:
		/// "aname" - The assignment name
		/// "cname" - The assignment category name.
		/// "due" - The due DateTime
		/// "submissions" - The number of submissions to the assignment
		/// </summary>
		/// <param name="subject">The course subject abbreviation</param>
		/// <param name="num">The course number</param>
		/// <param name="season">The season part of the semester for the class the assignment belongs to</param>
		/// <param name="year">The year part of the semester for the class the assignment belongs to</param>
		/// <param name="category">The name of the assignment category in the class, 
		/// or null to return assignments from all categories</param>
		/// <returns>The JSON array</returns>
		public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
		{
			try
			{
				if(category == null)
				{
					var allAssignments = from cour in db.Courses
										 join cla in db.Classes on cour.CourseId equals cla.CourseId
										 into classes
										 from c in classes.DefaultIfEmpty()
										 join aCat in db.AssignmentCategories on c.ClassId equals aCat.ClassId
										 into categories
										 from cat in categories.DefaultIfEmpty()
										 join assign in db.Assignments on cat.AssignCatId equals assign.AssignCatId
										 into assignments
										 from assi in assignments.DefaultIfEmpty()
										 where cour.SubjectAbbr == subject
										 where cour.CourseNumber == num
										 where c.Semester == season + " " + year
										 select new
										 {
											aname = assi.Name,
											cname = cat.Name,
											due = assi.DueDate,
											submissions = 0
										 };

				}
				else
				{
					var allAssignmentsInCategory = from cour in db.Courses
												   join cla in db.Classes on cour.CourseId equals cla.CourseId
												   into classes
												   from c in classes.DefaultIfEmpty()
												   join aCat in db.AssignmentCategories on c.ClassId equals aCat.ClassId
												   into categories
												   from cat in categories.DefaultIfEmpty()
												   join assign in db.Assignments on cat.AssignCatId equals assign.AssignCatId
												   into assignments
												   from assi in assignments.DefaultIfEmpty()
												   where cour.SubjectAbbr == subject
												   where cour.CourseNumber == num
												   where c.Semester == season + " " + year
												   where category == cat.Name
												   select new
												   {
														aname = assi.Name,
														cname = cat.Name,
														due = assi.DueDate,
														submissions = 0
												   };
				}


				return Json(null);
			}
			catch(Exception e)
			{
				return Json(e.Message);
			}
		}


		/// <summary>
		/// Returns a JSON array of the assignment categories for a certain class.
		/// Each object in the array should have the following fields:
		/// "name" - The category name
		/// "weight" - The category weight
		/// </summary>
		/// <param name="subject">The course subject abbreviation</param>
		/// <param name="num">The course number</param>
		/// <param name="season">The season part of the semester for the class the assignment belongs to</param>
		/// <param name="year">The year part of the semester for the class the assignment belongs to</param>
		/// <param name="category">The name of the assignment category in the class</param>
		/// <returns>The JSON array</returns>
		public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
		{
			try
			{
				var aCategories = from cla in db.Classes
								  join aCat in db.AssignmentCategories on cla.ClassId equals aCat.ClassId
								  into aCats
								  from a in aCats.DefaultIfEmpty()
								  select new
								  {
										name = a.Name,
										weight = a.Weight
								  };

				return Json(aCategories.ToArray());
			}
			catch(Exception e)
			{
				return Json(e.Message);
			}
		}

		/// <summary>
		/// Creates a new assignment category for the specified class.
		/// </summary>
		/// <param name="subject">The course subject abbreviation</param>
		/// <param name="num">The course number</param>
		/// <param name="season">The season part of the semester for the class the assignment belongs to</param>
		/// <param name="year">The year part of the semester for the class the assignment belongs to</param>
		/// <param name="category">The new category name</param>
		/// <param name="catweight">The new category weight</param>
		/// <returns>A JSON object containing {success = true/false},
		///	false if an assignment category with the same name already exists in the same class.</returns>
		public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
		{
			try
			{
				uint classIDForAsnCat = GetClassID(subject, num, season, year);

				AssignmentCategories aCat = new AssignmentCategories
				{
					ClassId = classIDForAsnCat,
					Name = category, 
					Weight = (uint) catweight
				};

				if(AssignmentCategoryExists(aCat))
				{
					return Json(new { success = false });
				}

				db.AssignmentCategories.Add(aCat);
				db.SaveChanges();

				return Json(new { success = true });
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return Json(new { success = false });
			}
		}

		/// <summary>
		/// Creates a new assignment for the given class and category.
		/// </summary>
		/// <param name="subject">The course subject abbreviation</param>
		/// <param name="num">The course number</param>
		/// <param name="season">The season part of the semester for the class the assignment belongs to</param>
		/// <param name="year">The year part of the semester for the class the assignment belongs to</param>
		/// <param name="category">The name of the assignment category in the class</param>
		/// <param name="asgname">The new assignment name</param>
		/// <param name="asgpoints">The max point value for the new assignment</param>
		/// <param name="asgdue">The due DateTime for the new assignment</param>
		/// <param name="asgcontents">The contents of the new assignment</param>
		/// <returns>A JSON object containing success = true/false,
		/// false if an assignment with the same name already exists in the same assignment category.</returns>
		public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
		{
			try
			{
				uint aCatID = GetAssignmentCategoryID(category);

				Assignments assignment = new Assignments
				{
					AssignCatId = aCatID,
					Contents = asgcontents,
					DueDate = asgdue,
					Name = asgname,
					MaxPoints = (uint) asgpoints
				};

				if(AssignmentAlreadyExists(assignment))
				{
					return Json(new { success = false });
				}

				db.Assignments.Add(assignment);
				db.SaveChanges();

				return Json(new { success = true });
			}
			catch(Exception e)
			{
				return Json(e.Message);
			}
		}


		/// <summary>
		/// Gets a JSON array of all the submissions to a certain assignment.
		/// Each object in the array should have the following fields:
		/// "fname" - first name
		/// "lname" - last name
		/// "uid" - user ID
		/// "time" - DateTime of the submission
		/// "score" - The score given to the submission
		/// 
		/// </summary>
		/// <param name="subject">The course subject abbreviation</param>
		/// <param name="num">The course number</param>
		/// <param name="season">The season part of the semester for the class the assignment belongs to</param>
		/// <param name="year">The year part of the semester for the class the assignment belongs to</param>
		/// <param name="category">The name of the assignment category in the class</param>
		/// <param name="asgname">The name of the assignment</param>
		/// <returns>The JSON array</returns>
		public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
		{
			try
			{
				var submissionsToAssignment = from cour in db.Courses
											  join cla in db.Classes on cour.CourseId equals cla.CourseId
											  into classes
											  from cl in classes.DefaultIfEmpty()
											  join aCat in db.AssignmentCategories on cl.ClassId equals aCat.ClassId
											  into categories
											  from ca in categories.DefaultIfEmpty()
											  join assign in db.Assignments on ca.AssignCatId equals assign.AssignCatId
											  into assignments
											  from assi in assignments.DefaultIfEmpty()
											  join sub in db.Submission on assi.AssignmentId equals sub.AssignmentId
											  into submissions
											  from submis in submissions.DefaultIfEmpty()
											  join stu in db.Students on submis.UId equals stu.UId
											  into students
											  from stu in students.DefaultIfEmpty()
											  select new
											  {
												  fname = stu.FirstName,
												  lname = stu.LastName,
												  uid = stu.UId,
												  time = submis.Time,
												  score = submis.Score
											  };

				return Json(submissionsToAssignment.ToArray());
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return Json(null);
			}
		}


		/// <summary>
		/// Set the score of an assignment submission
		/// </summary>
		/// <param name="subject">The course subject abbreviation</param>
		/// <param name="num">The course number</param>
		/// <param name="season">The season part of the semester for the class the assignment belongs to</param>
		/// <param name="year">The year part of the semester for the class the assignment belongs to</param>
		/// <param name="category">The name of the assignment category in the class</param>
		/// <param name="asgname">The name of the assignment</param>
		/// <param name="uid">The uid of the student who's submission is being graded</param>
		/// <param name="score">The new score for the submission</param>
		/// <returns>A JSON object containing success = true/false</returns>
		public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
		{
			try
			{
				return Json(new { success = true });
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return Json(new { success = false });
			}
		}


		/// <summary>
		/// Returns a JSON array of the classes taught by the specified professor
		/// Each object in the array should have the following fields:
		/// "subject" - The subject abbreviation of the class (such as "CS")
		/// "number" - The course number (such as 5530)
		/// "name" - The course name
		/// "season" - The season part of the semester in which the class is taught
		/// "year" - The year part of the semester in which the class is taught
		/// </summary>
		/// <param name="uid">The professor's uid</param>
		/// 
		/// NOTE: This method is called when you are logged in as a professor
		///		  and you click on "My Classes" 
		/// 
		/// <returns>The JSON array</returns>
		public IActionResult GetMyClasses(string uid)
		{
			try
			{
				var profClasses = from cla in db.Classes
								  join cour in db.Courses on cla.CourseId equals cour.CourseId
								  into classes
								  from c in classes.DefaultIfEmpty()
								  where cla.Professor == uid
								  select new
								  {
										subject = c.SubjectAbbr,
										number = c.CourseNumber,
										name = c.Name,
										season = ExtractSeason(cla.Semester),
										year = ExtractYear(cla.Semester)
								  };

				return Json(profClasses.ToArray());
			}
			catch(Exception e)
			{
				return Json(e.Message);
			}
		}

		/// <summary>
		/// Helper for verifying if a newly created assignment category
		/// already exists for the class
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		private bool AssignmentCategoryExists(AssignmentCategories category)
		{
			var categories = from cat in db.AssignmentCategories
							 select cat;

			return categories.Contains(category);
		}

		/// <summary>
		/// Helper for getting the class ID
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="num"></param>
		/// <param name="season"></param>
		/// <param name="year"></param>
		/// <returns></returns>
		private uint GetClassID(string subject, int num, string season, int year)
		{
			var classID = from cla in db.Classes
						  join cour in db.Courses on cla.CourseId equals cour.CourseId
						  into courseClasses
						  from c in courseClasses.DefaultIfEmpty()
						  where c.SubjectAbbr == subject
						  && c.CourseNumber == num
						  && cla.Semester == season + " " + year
						  select cla.ClassId;

			return classID.First();
		}

		/// <summary>
		/// Helper for getting the assignment category ID
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		private uint GetAssignmentCategoryID(String category)
		{
			var aCatID = from aCat in db.AssignmentCategories
						 where aCat.Name == category
						 select aCat.AssignCatId;

			return aCatID.First();
		}

		/// <summary>
		/// Helper to verify if a duplicate assignment exists
		/// </summary>
		/// <param name="assignment"></param>
		/// <returns></returns>
		private bool AssignmentAlreadyExists(Assignments assignment)
		{
			var assignments = from assign in db.Assignments
							  where assign.AssignCatId == assignment.AssignCatId
							  where assign.Name == assignment.Name
							  select assign;

			return assignments.Any();
		}

		/*******End code to modify********/

	}
}