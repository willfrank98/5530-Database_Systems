using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

// For referencing LMSTester
[assembly: InternalsVisibleTo("LMSTester")]

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
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


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
		try
		{
			var classes = from en in db.Enrolled
						  join cla in db.Classes on en.ClassId equals cla.ClassId
						  into enJoinCla
						  from clas in enJoinCla.DefaultIfEmpty()
						  join cour in db.Courses on clas.CourseId equals cour.CourseId
						  into courses
						  from c in courses.DefaultIfEmpty()
						  select new
						  {
							  subject = c.SubjectAbbr,
							  number = c.CourseNumber,
							  name = c.Name,
							  season = ExtractSeason(clas.Semester),
							  year = ExtractYear(clas.Semester),
							  grade = en.Grade
						  };	

			return Json(classes.ToArray());
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return Json(null);
		}
    }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {     
		try
		{
			var query = from cla in db.Classes
						join assignCat in db.AssignmentCategories on cla.ClassId equals assignCat.ClassId
						into assignCategories 
						from a in assignCategories.DefaultIfEmpty() 
						join assignm in db.Assignments on a.AssignCatId equals assignm.AssignCatId
						into assignments 
						from assi in assignments.DefaultIfEmpty()
						select 
						new
						{
							aname = "test",
							cname = "test",
							due = "test",
							score = "--" 
						};

			return Json(query.ToArray());
		}
		catch(Exception e)
		{
			return Json(e.Message);
		}
    }


    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// The score of the submission should start as 0 until a Professor grades it
    /// If a Student submits to an assignment again, it should replace the submission contents
    /// and the submission time (the score should remain the same).
	/// Does *not* automatically reject late submissions.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}.</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, 
      string category, string asgname, string uid, string contents)
    {
		try
		{
			var query = from asCat in db.AssignmentCategories
						select asCat;


			return Json(new { success = true });
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return Json(new { success = false });
		}
    }

    
    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false},
	/// false if the student is already enrolled in the Class.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {
		try
		{
			uint? classID = FindClassID(subject, num, season, year);

			var alreadyEnrolledCourse = from enr in db.Enrolled
										where enr.ClassId == classID
										&& enr.UId == uid
										select new
										{
											enr.U
										};

			var grade = from enr in db.Enrolled
						where enr.ClassId == classID
						&& enr.UId == uid
						select new
						{
							enr.Grade
						};

			if (classID != null && !alreadyEnrolledCourse.Any())
			{ 
					Enrolled enroll = new Enrolled
					{
						UId = uid,
						Grade = grade == null ? "--" : ComputeGrade(uid),
						ClassId = (uint)classID
					};

					db.Enrolled.Add(enroll);
					db.SaveChanges();

					return Json(new { success = true });	
			}
			
			return Json(new { success = false });
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return Json(new { success = false });
		}
    }



    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// Assume all classes are 4 credit hours.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// If a student does not have any grades, they have a GPA of 0.0.
    /// Otherwise, the point-value of a letter grade is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {     
		try
		{
			//TODO: Replace
			return Json(new { gpa = 4.0 });
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return Json(new { gpa = 0.0 });
		}
    }

	/// <summary>
	/// Helper for computing the grade for the class the student is in 
	/// </summary>
	/// <param name="assignments"></param>
	/// <returns></returns>
	private String ComputeGrade(string uid)
	{
		//var getGPA = GetGPA(uid) as JsonResult;
		//double gpa = double.Parse(getGPA.Value.ToString());
		//
		//if (gpa >= 3.7 && gpa <= 4.0)
		//{
		//	return "A";
		//}
		//else if (gpa >= 3.5 && gpa < 3.7)
		//{
		//	return "A-";
		//}
		//
		//if (ComputeGrade(uid) == "--")
		//{
		//	return Json(0.0);
		//}

		return "--";
	}

	/// <summary>
	/// Helper for finding classID before enrolling a student
	/// </summary>
	/// <param name="subject"></param>
	/// <param name="num"></param>
	/// <param name="season"></param>
	/// <param name="year"></param>
	/// <returns></returns>
	private uint? FindClassID(string subject, int num, string season, int year)
	{
		var query = from cla in db.Classes
					join cour in db.Courses on cla.CourseId equals cour.CourseId
					where cla.Semester == season + " " + year
					&& cour.CourseNumber == num && cour.SubjectAbbr == subject
					select new
					{
						cla.ClassId
					};

		return query.Any() ? query.ToArray()[0].ClassId : (uint?) null;
	}

	/*******End code to modify********/

  }
}