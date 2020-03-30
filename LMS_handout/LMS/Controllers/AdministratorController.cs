using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using LMS.Models.LMSModels;

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
		var allCourses = from co in db.Courses where co.SubjectAbbr == subject
						 select new
						 {
							number = co.CourseNumber, 
							name = co.Name
						 };
      
        return Json(allCourses.ToArray());
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
		var professors = from p in db.Professors
						 join d in db.Departments on p.Department equals d.SubjectAbbr
						 into subjects

						 from sub in subjects.DefaultIfEmpty() where p.Department == subject
						 select new
						 {
							 lName = p.LastName,
							 fName = p.FirstName,
							 uid = p.UId
						 };

		return Json(professors.ToArray());
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
		Courses course = new Courses {SubjectAbbr = subject, CourseNumber = (uint) number, Name = name };

		var courseDuplicates = from c in db.Courses
					  where c.SubjectAbbr == subject && c.CourseNumber == number 
					  && c.Name == name
					  select c;

		if(courseDuplicates.Any())
		{
			return Json(new { success = false });
		}

		db.Courses.Add(course);
		db.SaveChanges();

        return Json(new { success = true });
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
			Classes newClass = new Classes
			{
				CourseId = (uint)number,
				Semester = season + year,
				Location = location,
				Start = start.TimeOfDay,
				End = end.TimeOfDay,
				Professor = instructor,
			};

			var allClasses = from cla in db.Classes
							 select cla;

			if (allClasses.Count() > 0)
			{
				//Check for same location and same offering for the same course and semester
				if (ViolatesTimeRangeForSameLocationAndSemester(newClass, allClasses.ToList())
					|| IsSameOfferingForSameCourseAndSemester(newClass, allClasses.ToList()))
				{
					return Json(new { success = false });
				}
			}

			db.Classes.Add(newClass);
			db.SaveChanges();

			return Json(new { success = true });
    }

	/// <summary>
	/// Verifies if a class contains the same location as the current
	/// collection of classes offered
	/// </summary>
	/// <param name="first"></param>
	/// <param name="classes"></param>
	/// <returns></returns>
	private bool ViolatesTimeRangeForSameLocationAndSemester(Classes first, List<Classes> classes)
	{
		foreach(Classes c in classes)
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
	private bool IsSameOfferingForSameCourseAndSemester(Classes newClass, List<Classes> classes)
	{
		foreach(Classes c in classes)
		{
			bool isSameSemester = newClass.Semester == c.Semester;
			bool isSameCourse = newClass.Course == c.Course;

			if(isSameSemester && isSameCourse)
			{
				return true;
			}
		}

		return false;
	}

    /*******End code to modify********/

  }
}