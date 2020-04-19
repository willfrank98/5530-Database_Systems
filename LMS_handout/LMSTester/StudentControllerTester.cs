using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

/// <summary>
/// A collection of xUnit tests for the following controllers:
/// 
/// - AdministratorController
/// - CommonController
/// - ProfessorController
/// - StudentController
/// 
/// </summary>
namespace LMSTester
{
	public class StudentControllerTester
	{
		/// <summary>
		/// Service provider setup for mockup database
		/// </summary>
		/// <returns></returns>
		private static ServiceProvider NewServiceProvider()
		{
			var serviceProvider = new ServiceCollection()
			  .AddEntityFrameworkInMemoryDatabase()
			  .BuildServiceProvider();
			return serviceProvider;
		}

		/// <summary>
		/// Miny database for courses
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeEmptyDatabase()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("empty_database").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			return db;
		}

		/// <summary>
		/// Helper for making a tiny catalog (contains one class) 
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeTinyCatalog()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Students student = new Students { UId = "u0000003", FirstName = "Tony", LastName = "Diep",
											  BirthDate = DateTime.Parse("02/02/1996"), Major = "LING"};

			Courses course = new Courses
			{
				CourseId = 1,
				CourseNumber = 1069,
				SubjectAbbr = "LING",
				Name = "Bad Words & Taboo Terms"
			};

			Classes badwordsClass = new Classes{ CourseId = 1, Semester = "Fall 2020", Location = "LNCO 1104",
			                                     Start = TimeSpan.Parse("10:45:00"), End = TimeSpan.Parse("11:35:00"),
												 Professor = "u0000002"};

			db.Classes.Add(badwordsClass);
			db.Students.Add(student);
			db.Courses.Add(course);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Verifies that a student who has not enrolled in any classes yet 
		/// should not have a list of enrolled classes
		/// </summary>
		[Fact]
		public void NoEnrolledClassesShouldDisplayNoClassesForStudent()
		{
			StudentController student = new StudentController();
			Team55LMSContext db = MakeEmptyDatabase();

			Students tony = new Students
			{
				UId = "u0000000",
				FirstName = "Tony",
				LastName = "Diep",
				BirthDate = DateTime.Parse("02/02/1996"),
				Major = "CS"
			};

			db.Students.Add(tony);
			db.SaveChanges();

			student.UseLMSContext(db);
			student.GetMyClasses(tony.UId);

			var classes = from cla in db.Classes
						  join enr in db.Enrolled on cla.ClassId equals enr.ClassId
						  into claJoinEnr
						  from enrolled in claJoinEnr.DefaultIfEmpty()
						  where enrolled.UId == tony.UId
						  select cla;

			Assert.Equal(0, classes.Count());
		}

		/// <summary>
		/// Verifies that a student is able to enroll in a class successfully
		/// </summary>
		[Fact]
		public void CanEnrollInClass()
		{
			StudentController student = new StudentController();
			Team55LMSContext db = MakeTinyCatalog();
			student.UseLMSContext(db);

			//Classes badwordsClass = new Classes
			//{
			//	CourseId = 0,
			//	Semester = "Fall 2020",
			//	Location = "LNCO 1104",
			//	Start = TimeSpan.Parse("10:45:00"),
			//	End = TimeSpan.Parse("11:35:00"),
			//	Professor = "u0000002"
			//};

			var result = student.Enroll("LING", 1069, "Fall", 2020, "u0000003") as JsonResult;
		
			Assert.Equal("{ success = True }", result.Value.ToString());
		}

		/// <summary>
		/// Verifies that a student cannot enroll in a class that they 
		/// are already enrolled in 
		/// </summary>
		[Fact]
		public void CannotEnrollInSameClass()
		{
			StudentController student = new StudentController();
			Team55LMSContext db = MakeTinyCatalog();
			student.UseLMSContext(db);

			student.Enroll("LING", 1069, "Fall", 2020, "u0000003");

			var enrolled = student.Enroll("LING", 1069, "Fall", 2020, "u0000003") as JsonResult;
			dynamic result = enrolled.Value;

			Assert.Equal("{ success = False }", result.ToString());
		}
	}
}
