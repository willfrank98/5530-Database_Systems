using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

// For referencing LMSTester
[assembly: InternalsVisibleTo("AdministratorControllerTester")]

namespace LMS.Controllers
{
	[Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Department(string subject)
    {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 5530)
    /// "name" - The course name (as in "Database Systems")
    /// </summary>
    /// <param name="subject">The department subject abbreviation (as in "CS")</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetCourses(string subject)
    {
		try
		{
			var allCourses = from co in db.Courses
							 where co.SubjectAbbr == subject
							 select new
							 {
								  number = co.CourseNumber,
								  name = co.Name
							 };

			return Json(allCourses.ToArray());
		}
		catch(Exception e)
		{
			return Json(e.Message);
		}
    }


    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetProfessors(string subject)
    {
		try
		{
			var professors = from p in db.Professors
							 where p.Department == subject
							 select new
							 {
								  lname = p.LastName,
								  fname = p.FirstName,
								  uid = p.UId
							 };

			return Json(professors.ToArray());
		}
		catch(Exception e)
		{
			return Json(e.Message);
		}
    }



    /// <summary>
    /// Creates a course.
    /// A course is uniquely identified by its number + the subject to which it belongs
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false},
	/// false if the Course already exists.</returns>
    public IActionResult CreateCourse(string subject, int number, string name)
    {
		try
		{
			Courses course = new Courses
			{
				SubjectAbbr = subject,
				CourseNumber = (uint)number,
				Name = name
			};

			var courses = from co in db.Courses
						  select co;

			if(CourseAlreadyExists(subject, number, name))
			{
				return Json(new { success = false });
			}

			db.Courses.Add(course);
			db.SaveChanges();

			return Json(new { success = true });
		}
		catch(Exception)
		{
			return Json(new { success = false });
		}
    }


	/// <summary>
	/// Creates a class offering of a given course.
	/// </summary>
	/// <param name="subject">The department subject abbreviation</param>
	/// <param name="number">The course number</param>
	/// <param name="season">The season part of the semester</param>
	/// <param name="year">The year part of the semester</param>
	/// <param name="start">The start time</param>
	/// <param name="end">The end time</param>
	/// <param name="location">The location</param>
	/// <param name="instructor">The uid of the professor</param>
	/// <returns>A JSON object containing {success = true/false}. 
	/// false if another class occupies the same location during any time 
	/// within the start-end range in the same semester, or if there is already
	/// a Class offering of the same Course in the same Semester.</returns>
	public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
	{
		try
		{
			uint courseID = GetCourseIDForClass((uint)number, subject);

			Classes newClass = new Classes
			{
				CourseId = courseID,
				Semester = season + " " + year,
				Location = location,
				Start = start.TimeOfDay,
				End = end.TimeOfDay,
				Professor = instructor, 
			};

			//Check for same location and same offering for the same course and semester
			if (ViolatesTimeRangeForSameLocationAndSemester(newClass)
				|| IsSameOfferingForSameCourseAndSemester(newClass))
			{
				return Json(new { success = false });
			}

			db.Classes.Add(newClass);
			db.SaveChanges();

			return Json(new { success = true });
		}
		catch(Exception)
		{
			return Json(new { success = false });
		}
	}

	/// <summary>
	/// Verifies if a class contains the same location as the current
	/// collection of classes offered
	/// </summary>
	/// <param name="first"></param>
	/// <param name="classes"></param>
	/// <returns></returns>
	private bool ViolatesTimeRangeForSameLocationAndSemester(Classes first)
	{
		var classes = from cla in db.Classes
					  select cla;

		foreach (Classes c in classes)
		{
			bool isSameLocation = first.Location == c.Location;
			bool isSameSemester = first.Semester == c.Semester;
			bool overlapsTimeRange = (first.Start <= c.End && c.Start <= first.End);

			if ( isSameLocation && isSameSemester && overlapsTimeRange )
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Verifies if a newly added class is a duplicate of another class offering
	/// for the same semseter
	/// </summary>
	/// <param name="newClass"></param>
	/// <param name="classes"></param>
	/// <returns></returns>
	private bool IsSameOfferingForSameCourseAndSemester(Classes newClass)
	{
		var classes = from cla in db.Classes
					  select cla;

		foreach (Classes c in classes)
		{
			bool isSameSemester = newClass.Semester == c.Semester;
			bool isSameCourse = newClass.Course == c.Course;
			bool isSameTime = (newClass.Start == c.Start && newClass.End == c.End);
			bool isSameLocation = (newClass.Location == c.Location);
			bool hasSameCourse = (newClass.Course == c.Course);

			if(isSameSemester && isSameCourse && isSameTime && isSameLocation)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Helper for determining if a course is already in the catalog; this 
	/// will prevent an administrator from creating duplicate courses
	/// </summary>
	/// <param name="subject"></param>
	/// <param name="number"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	private bool CourseAlreadyExists(String subject, int number, String name)
	{
		foreach(Courses c in db.Courses)
		{
			if(c.SubjectAbbr == subject &&
				c.CourseNumber == number && c.Name == name)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Helper for getting the course ID for a new class offering
	/// </summary>
	/// <returns></returns>
	private uint GetCourseIDForClass(uint number, String subjectAbbr)
	{
		try
		{
			var courseID = from cour in db.Courses
							where cour.CourseNumber == number
							&& cour.SubjectAbbr == subjectAbbr
							select cour.CourseId;

			return courseID.First();
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
			return 0;
		}
	}

    /*******End code to modify********/

  }
}