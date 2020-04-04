using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

// For referencing LMSTester
[assembly: InternalsVisibleTo("CommonControllerTester")]

namespace LMS.Controllers
{
  public class CommonController : Controller
  {

    /*******Begin code to modify********/

    protected Team55LMSContext db;

    public CommonController()
    {
      db = new Team55LMSContext();
    }
    

    /*
     * WARNING: This is the quick and easy way to make the controller
     *          use a different LibraryContext - good enough for our purposes.
     *          The "right" way is through Dependency Injection via the constructor 
     *          (look this up if interested).
    */

    // TODO: Uncomment and change 'X' after you have scaffoled
    
    public void UseLMSContext(Team55LMSContext ctx)
    {
      db = ctx;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }


    /// <summary>
    /// Retreive a JSON array of all departments from the database.
    /// Each object in the array should have a field called "name" and "subject",
    /// where "name" is the department name and "subject" is the subject abbreviation.
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetDepartments()
    {
		try
		{
			var query = from d in db.Departments
						select new
						{
							name = d.Name,
							subject = d.SubjectAbbr
						};

			return Json(query.ToArray());
		}
		catch(Exception e)
		{
			return Json(e.Message);
		}
    }


	/// <summary>
	/// Returns a JSON array representing the course catalog.
	/// Each object in the array should have the following fields:
	/// "subject": The subject abbreviation, (e.g. "CS")
	/// "dname": The department name, as in "Computer Science"
	/// "courses": An array of JSON objects representing the courses in the department.
	///            Each field in this inner-array should have the following fields:
	///            "number": The course number (e.g. 5530)
	///            "cname": The course name (e.g. "Database Systems")
	/// </summary>
	/// 
	/// NOTE:  This method is called when you click "Catalog" 
	/// 
	/// 
	/// <returns>The JSON array</returns>
	public IActionResult GetCatalog()
	{
		try
		{
			var catalog = from depart in db.Departments
							join course in db.Courses on depart.SubjectAbbr equals course.SubjectAbbr
							into departCourse
							from dep in departCourse.DefaultIfEmpty()
							select new
							{
								depart.Name,
								depart.SubjectAbbr,
								courses = from c in db.Courses
										  where c.SubjectAbbr == depart.SubjectAbbr
										  select new
										  {
											  c.CourseNumber,
											  c.Name
										  }
							};

			return Json(catalog.ToArray());
		}
		catch(Exception e)
		{
			return Json(e.Message);
		}
	}

    /// <summary>
    /// Returns a JSON array of all class offerings of a specific course.
    /// Each object in the array should have the following fields:
    /// "season": the season part of the semester, such as "Fall"
    /// "year": the year part of the semester
    /// "location": the location of the class
    /// "start": the start time in format "hh:mm:ss"
    /// "end": the end time in format "hh:mm:ss"
    /// "fname": the first name of the professor
    /// "lname": the last name of the professor
    /// </summary>
    /// <param name="subject">The subject abbreviation, as in "CS"</param>
    /// <param name="number">The course number, as in 5530</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetClassOfferings(string subject, int number)
    {
		try
		{
			var classOfferings = from cla in db.Classes
									where cla.Course.SubjectAbbr == subject &&
									cla.Course.CourseNumber == number
									select new
									{
										season = ExtractSeason(cla.Semester),
										year = ExtractYear(cla.Semester),
										location = cla.Location,
										start = cla.Start,
										end = cla.End,
										fName = ExtractFirstName(cla.Professor),
										lname = ExtractLastName(cla.Professor)
									};

			return Json(classOfferings.ToArray());
		}
		catch(Exception e)
		{
			return Json(e.Message);
		}
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {
		try
		{
			var assignContents = from assign in db.Assignments
								 join asgCat in db.AssignmentCategories on assign.AssignCatId equals asgCat.AssignCatId
								 into assignJoinCat
								 from cat in assignJoinCat.DefaultIfEmpty()
								 join cla in db.Classes on cat.ClassId equals cla.ClassId
								 into catJoinClasses
								 from classes in catJoinClasses.DefaultIfEmpty()
								 select new
								 {
									 subject = classes.ProfessorNavigation.Department,
									 num = classes.Course.CourseNumber,
									 season = ExtractSeason(classes.Semester),
									 year = ExtractYear(classes.Semester),
									 category = cat.Name,
									 asgname = assign.Name
								 };

			return Content(assignContents.ToString());
		}
		catch(Exception e)
		{
			return Content(e.Message);
		}
    }


    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// Returns the empty string ("") if there is no submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {
		try
		{
			var submissionText = from sub in db.Submission
								 join a in db.Assignments on sub.AssignmentId equals a.AssignmentId
								 into subsJoinAssign
								 from assign in subsJoinAssign.DefaultIfEmpty()
								 join asgCat in db.AssignmentCategories on assign.AssignCatId equals asgCat.AssignCatId
								 into assignJoinCat
								 from cat in assignJoinCat.DefaultIfEmpty()
								 join cla in db.Classes on cat.ClassId equals cla.ClassId
								 into catJoinClasses
								 from classes in catJoinClasses.DefaultIfEmpty()
								 select new
								 {
									 subject = classes.Course.SubjectAbbr,
									 num = classes.Course.CourseNumber,
									 season = ExtractSeason(classes.Semester),
									 year = ExtractYear(classes.Semester),
									 category = cat.Name,
									 asgname = assign.Name,
									 uid = sub.UId
								 };

			if (submissionText != null)
			{
				return Content(submissionText.ToString());
			}

			return Content("");
		}
		catch(Exception e)
		{
			return Content(e.Message);
		}
    }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>
    /// The user JSON object 
    /// or an object containing {success: false} if the user doesn't exist
    /// </returns>
    public IActionResult GetUser(string uid)
    {
		var student = from stu in db.Students where stu.UId == uid
					  select stu.Major;

		var professor = from pro in db.Professors
						where pro.UId == uid
						select pro.Department;

		var admin = from adm in db.Administrators
					where adm.UId == uid
					select adm.UId;

		if(student.Count() > 0)
		{
			return Json(student);
		}
		else if(professor.Count() > 0)
		{
			return Json(professor);
		}
		else
		{
			return Json(new { success = false });
		}
    }

	/// <summary>
	/// Helper for extracting the season from a semester string
	/// </summary>
	/// <param name="semester"></param>
	/// <returns></returns>
	public static String ExtractSeason(String semester)
	{
		return semester.Substring(0, semester.Length - semester.IndexOf(" "));
	}

	/// <summary>
	/// Helper for extracting the year from a semester string
	/// </summary>
	/// <param name="semester"></param>
	/// <returns></returns>
	public static String ExtractYear(String semester)
	{
		return semester.Substring(semester.IndexOf(" ") + 1);
	}

	/// <summary>
	/// Helper for getting the first name of a user
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static String ExtractFirstName(String name)
	{
		return name.Substring(0, name.Length - name.IndexOf(" ") + 1);
	}

	/// <summary>
	/// Helper for getting the last name of a user
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static String ExtractLastName(String name)
	{
		return name.Substring(name.IndexOf(" ") + 1);
	}

    /*******End code to modify********/

  }
}