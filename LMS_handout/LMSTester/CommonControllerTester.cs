using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Newtonsoft.Json;

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
	public class CommonControllerTester
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
		private Team55LMSContext ConfigureDatabaseNoData()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			return db;
		}

		/// <summary>
		/// Database for holding departments
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeDepartments()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Departments psychology = new Departments { Name = "Psychology", SubjectAbbr = "PSY" };
			Departments civilEng = new Departments { Name = "Civil Engineering", SubjectAbbr = "CVEN" };

			db.Departments.Add(psychology);
			db.Departments.Add(civilEng);

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for making a database with some catalogs
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeCatalog()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Departments math = new Departments
			{
				Name = "Mathematics",
				SubjectAbbr = "MATH"
			};

			Departments physics = new Departments
			{
				Name = "Physics",
				SubjectAbbr = "PHYS"
			};

			Courses calculus = new Courses
			{
				CourseId = 10,
				Name = "Calculus I",
				SubjectAbbr = "MATH", 
				CourseNumber = 1210
			};

			Courses elementaryPhysics = new Courses
			{
				CourseId = 11,
				Name = "Elementary Physics",
				SubjectAbbr = "PHYS",
				CourseNumber = 1010
			};

			db.Departments.Add(math);
			db.Departments.Add(physics);

			db.Courses.Add(calculus);
			db.Courses.Add(elementaryPhysics);

			return db;
		}

		/// <summary>
		/// Helper for configuring a database to have some class offerings
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeClassOfferings()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_class_offerings").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 0,
				CourseNumber = 5530,
				Name = "Database Systems",
				SubjectAbbr = "CS"
			};

			Classes dbFall2020Morning = new Classes
			{
				ClassId = 0,
				CourseId = 1,
				Semester = "Fall 2020",
				Location = "WEB L104",
				Start = TimeSpan.Parse("10:45:00"),
				End = TimeSpan.Parse("11:35:00"),
				Professor = "u0000001"
			};

			Classes dbFall2020Evening = new Classes
			{
				ClassId = 0,
				CourseId = 2,
				Semester = "Fall 2020",
				Location = "WEB L104",
				Start = TimeSpan.Parse("17:00:00"),
				End = TimeSpan.Parse("18:35:00"),
				Professor = "u0000001"
			};

			db.Courses.Add(course);
			db.Classes.Add(dbFall2020Morning);
			db.Classes.Add(dbFall2020Evening);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for making a student user in the database
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeStudentUser()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("student_user").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Students tony = new Students
			{
				FirstName = "Tony",
				LastName = "Diep",
				BirthDate = DateTime.Parse("02/02/1996"),
				Major = "CS",
				UId = "u0000001"
			};

			db.Students.Add(tony);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for making a professor user in the database
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeProfessorUser()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("student_user").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Professors erinParker = new Professors
			{
				FirstName = "Erin",
				LastName = "Parker",
				BirthDate = DateTime.Parse("02/02/1996"),
				UId = "u0000010"
			};

			db.Professors.Add(erinParker);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Verifies that all of the departments are listed
		/// </summary>
		[Fact]
		public void CanGetDepartments()
		{
			CommonController common = new CommonController();
			Team55LMSContext db = MakeDepartments();
			common.UseLMSContext(db);

			var departments = common.GetDepartments() as JsonResult;
			dynamic result = departments.Value;

			var departmentsQuery = from depart in db.Departments
								   select depart;

			Assert.Equal("{ name = Psychology, subject = PSY }", result[0].ToString());
			Assert.Equal("{ name = Civil Engineering, subject = CVEN }", result[1].ToString());
			Assert.Equal(2, departmentsQuery.Count());
		} 

		/// <summary>
		/// Verifies that all of the catalogs are listed
		/// </summary>
		[Fact]
		public void CanGetCatalog()
		{
			CommonController common = new CommonController();
			Team55LMSContext db = MakeCatalog();
			common.UseLMSContext(db);

			JsonResult catalogs = (JsonResult) common.GetCatalog();

			string json = JsonConvert.SerializeObject(catalogs.Value.ToString());
		}

		/// <summary>
		/// Verifies that there should not be any class offerings 
		/// if there aren't any for a given course
		/// 
		/// TODO: Find how to compare JSON values in string
		/// 
		/// </summary>
		[Fact]
		public void NoClassOfferings()
		{
			CommonController common = new CommonController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			common.UseLMSContext(db);

			var classOfferings = common.GetClassOfferings("CS", 5530) as JsonResult;
			dynamic result = classOfferings.Value;

			//season = ExtractSeason(cla.Semester),
			//year = ExtractYear(cla.Semester),
			//location = cla.Location,
			//start = cla.Start,
			//end = cla.End,
			//fName = cla.ProfessorNavigation.FirstName,
			//lname = cla.ProfessorNavigation.LastName

			String expected = "{  }";

			Assert.Equal(expected, result.ToString());
		}

		/// <summary>
		/// Verifies that all of the class offerings for a class is returned 
		/// </summary>
		[Fact]
		public void CanGetClassOfferings()
		{
			CommonController common = new CommonController();
			Team55LMSContext db = MakeClassOfferings();
			common.UseLMSContext(db);

			var departments = common.GetClassOfferings("CS", 5530) as JsonResult;
			dynamic result = departments.Value;

			var classOfferings = from cla in db.Classes
								 where cla.Course.SubjectAbbr == "CS"
								 && cla.Course.CourseNumber == 5530
								 select cla;

			Assert.Equal(2, classOfferings.Count());
			//Assert.Equal("{ }", result);
		}

		/// <summary>
		/// Verifies information retrieved from a student user
		/// </summary>
		[Fact]
		public void CanGetStudentUser()
		{
			CommonController common = new CommonController();
			Team55LMSContext db = MakeStudentUser();
			common.UseLMSContext(db);

			var studentUser = common.GetUser("u0000001") as JsonResult;
			dynamic result = studentUser.Value;

			Assert.Equal("{ fname = Tony, lname = Diep, uid = u0000001, department = CS }", result.ToString());
		}

		/// <summary>
		/// Verifies information retrieved from a professor user
		/// </summary>
		[Fact]
		public void CanGetProfessorUser()
		{
			CommonController common = new CommonController();
			Team55LMSContext db = MakeStudentUser();
			common.UseLMSContext(db);

			var professorUser = common.GetUser("u0000010") as JsonResult;
			dynamic result = professorUser.Value;

			Assert.Equal("{ fname = Erin, lname = Parker, uid = u0000010, department = CS }", result.ToString());
		}

		///// <summary>
		///// Verifies information retrieved from a professor user
		///// </summary>
		//[Fact]
		//public void CanGetAdminUser()
		//{
		//	CommonController common = new CommonController();
		//	Team55LMSContext db = MakeStudentUser();
		//	common.UseLMSContext(db);

		//	var adminUser = common.GetUser("u0000003") as JsonResult;
		//	dynamic result = adminUser.Value;

		//	Assert.Equal("{ fname = admin, lname = admin, uid = u0000003 }", result.ToString());
		//}
	}
}
