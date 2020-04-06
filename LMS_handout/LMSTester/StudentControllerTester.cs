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
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

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

			Classes badwordsClass = new Classes{ CourseId = 1069, Semester = "Fall 2020", Location = "LNCO 1104",
			                                     Start = TimeSpan.Parse("10:45:00"), End = TimeSpan.Parse("11:35:00"),
												 Professor = "u0000002"};

			db.Classes.Add(badwordsClass);
			db.Students.Add(student);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for configuring a db to make a list of classes for a student to be enrolled in 
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeMyClassesForStudent()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Classes databases = new Classes
			{
				ClassId = 0,
				CourseId = 5530,
				Semester = "Spring 2021",
				Location = "WEB L104",
				Start = TimeSpan.Parse("11:50:00"),
				End = TimeSpan.Parse("01:10:00"),
				Professor = "u0000003"
			};

			Classes capstoneDesign = new Classes
			{
				ClassId = 1,
				CourseId = 4000,
				Semester = "Spring 2021",
				Location = "WEB 1230",
				Start = TimeSpan.Parse("10:45:00"),
				End = TimeSpan.Parse("11:35:00"),
				Professor = "u0000001"
			};

			Students student = new Students
			{
				UId = "u0000004",
				FirstName = "FirstName",
				LastName = "LastName",
				BirthDate = DateTime.Parse("01/01/1993"),
				Major = "LING"
			};

			db.Classes.Add(databases);
			db.Classes.Add(capstoneDesign);
			db.SaveChanges();
			db.Students.Add(student);

			return db;
		}

		/// <summary>
		/// Verifies that a student can see all of their classes they are enrolled in 
		/// the MyClassesPage
		/// </summary>
		[Fact]
		public void CanViewMyClassesAfterEnrolling()
		{
			StudentController student = new StudentController();
			Team55LMSContext db = MakeMyClassesForStudent();
			student.UseLMSContext(db);

			student.Enroll("CS", 4000, "Spring", 2021, "u0000004");
			student.Enroll("CS", 5530, "Spring", 2021, "u0000004");

			var classes = from enr in db.Enrolled
						  join cla in db.Classes on enr.ClassId equals cla.ClassId
						  into allClasses
						  from cl in allClasses.DefaultIfEmpty()
						  where enr.UId == "u0000004"
						  select cl;
		
			var result = student.GetMyClasses("u0000004") as JsonResult;

			Assert.Equal(2, classes.Count());
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

			var result = student.Enroll("LING", 1069, "Fall", 2020, "u0000003") as JsonResult;

			var query = from en in db.Enrolled
						join cl in db.Classes on en.ClassId equals cl.ClassId
						into classes
						from c in classes.DefaultIfEmpty()
						where c.CourseId == 1069 && c.Professor == "u0000003"
						&& c.Semester == "Fall 2020"
						select en;

			Assert.Equal(1, query.Count());
			Assert.Equal("{ success = False }", result.Value.ToString());
		}
	}
}
