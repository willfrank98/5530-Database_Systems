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
	public class AdministratorControllerTester
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
			Departments lingustics = new Departments { Name = "Lingustics", SubjectAbbr = "LING" };
			Departments cs = new Departments { Name = "Computer Science", SubjectAbbr = "CS" };

			db.Departments.Add(psychology);
			db.Departments.Add(civilEng);
			db.Departments.Add(lingustics);
			db.Departments.Add(cs);

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Verifies that a course has been successfully added
		/// </summary>
		[Fact]
		public void TestGetCourse()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);
			admin.CreateCourse("RUSS", 1010, "Beginner Russian I");

			var courses = admin.GetCourses("RUSS") as JsonResult;
			dynamic result = courses.Value;

			var query = from co in db.Courses
						where co.SubjectAbbr == "RUSS"
						select co;

			Assert.True(query.ToArray().Length == 1);
			Assert.Equal("{ number = 1010, name = Beginner Russian I }", result[0].ToString());
		}

		/// <summary>
		/// Verifies that creating one class is successful
		/// </summary>
		[Fact]
		public void CanCreateCourse()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			admin.CreateCourse("CHEM", 1210, "Organic Chemistry I");

			var courses = from co in db.Courses
						  select co;

			Assert.Equal(1, courses.Count());
		}

		/// <summary>
		/// Verifies that a duplicate course cannot be created
		/// </summary>
		[Fact]
		public void CannotCreateDuplicateCourse()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			admin.CreateCourse("ESSF", 1710, "Elem Hip-Hop Dance");
			var duplicateCourse = admin.CreateCourse("ESSF", 1710, "Elem Hip-Hop Dance") as JsonResult;
			dynamic result = duplicateCourse.Value;

			var courses = from co in db.Courses
						  select co;

			Assert.Equal("{ success = False }", result.ToString());
			Assert.Equal(1, courses.Count());
		}

		/// <summary>
		/// Verifies that all departments are listed after insertion
		/// </summary>
		[Fact]
		public void CanGetDepartments()
		{
			AdministratorController admin = new AdministratorController();
			admin.UseLMSContext(MakeDepartments());

			var departments = admin.GetDepartments() as JsonResult;
			dynamic result = departments.Value;

			Assert.Equal(4, result.Length);
		}

		/// <summary>
		/// Verifies that adding one class from an empty Classes database 
		/// should be successful
		/// </summary>
		[Fact]
		public void CanCreateOneClass()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			DateTime start = new DateTime(2020, 8, 24, 10, 45, 0);
			DateTime end = new DateTime(2020, 8, 24, 11, 35, 0);

			admin.CreateClass("LING", 1069, "Fall", 2020, start, end, "LNCO 1104", "Elena Khordova");

			var query = from cl in db.Classes
						select cl;

			Assert.Equal(1, query.Count());
		}

		/// <summary>
		/// Verifies that a class cannot be added if it is another instance of 
		/// another class for the same semester
		/// </summary>
		[Fact]
		public void CannotCreateClassSameClassesThatViolateTimeRange()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			DateTime start = new DateTime(2020, 8, 24, 10, 45, 0);
			DateTime secondStart = new DateTime(2020, 8, 24, 11, 0, 0);
			DateTime end = new DateTime(2020, 8, 24, 11, 35, 0);

			admin.CreateClass("CS", 2420, "Spring", 2020, start, end, "WEB L104", "Swaroop Joshi");
			var createClass = admin.CreateClass("CS", 2420, "Spring", 2020, secondStart, end, "WEB L104", "Swaroop Joshi") as JsonResult;
			dynamic result = createClass.Value;

			var classes = from cla in db.Classes
						  select cla;

			Assert.Equal("{ success = False }", result.ToString());
			Assert.Equal(1, classes.Count());
		}

		/// <summary>
		/// Verifies that two classes that are from different departments but have the same
		/// location and violates the time range from the other class shouldn't be allowed
		/// </summary>
		[Fact]
		public void CannotCreateClassDifferentClassesSameLocationThatViolateTimeRange()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			DateTime start = new DateTime(2020, 8, 24, 14, 00, 00);
			DateTime end = new DateTime(2020, 8, 24, 15, 20, 00);
			DateTime secondStart = new DateTime(2020, 8, 24, 13, 45, 0);
			DateTime secondEnd = new DateTime(2020, 8, 24, 14, 45, 0);

			admin.CreateClass("CS", 2420, "Spring", 2020, start, end, "WEB L104", "Swaroop Joshi");
			var createClass = admin.CreateClass("BIOL", 2420, "Spring", 2020, secondStart, secondEnd, "WEB L104", "SomeCool BioInstructor") as JsonResult;
			dynamic result = createClass.Value;

			var classes = from cla in db.Classes
						  select cla;

			Assert.Equal("{ success = False }", result.ToString());
			Assert.Equal(1, classes.Count());
		}

		/// <summary>
		/// Verifies that two classes that are from different departments but have the same
		/// location and violates the time range from the other class shouldn't be allowed
		/// </summary>
		[Fact]
		public void CannotCreateClassesOfSameOffering()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			DateTime start = new DateTime(2020, 8, 24, 14, 00, 00);
			DateTime end = new DateTime(2020, 8, 24, 15, 20, 00);
			DateTime secondStart = new DateTime(2020, 8, 24, 10, 45, 0);
			DateTime secondEnd = new DateTime(2020, 8, 24, 11, 35, 0);

			admin.CreateClass("CS", 2420, "Spring", 2020, start, end, "WEB L104", "Swaroop Joshi");
			var createClass = admin.CreateClass("CS", 2420, "Spring", 2020, secondStart, secondEnd, "WEB L104", "SomeCool BioInstructor") as JsonResult;
			dynamic result = createClass.Value;

			var classes = from cla in db.Classes
						  select cla;

			Assert.Equal("{ success = False }", result.ToString());
			Assert.Equal(1, classes.Count());
		}
	}
}
