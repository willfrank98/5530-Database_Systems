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
			optionsBuilder.UseInMemoryDatabase("empty_database").UseApplicationServiceProvider(NewServiceProvider());

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
			optionsBuilder.UseInMemoryDatabase("departments").UseApplicationServiceProvider(NewServiceProvider());

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
		/// Helper for configuring a database to have a few courses
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeCourses()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("courses").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses computerSystems = new Courses
			{
				CourseId = 1,
				SubjectAbbr = "CS",
				CourseNumber = 4400,
				Name = "Computer Systems"
			};

			Courses deepLearning = new Courses
			{
				CourseId = 12,
				SubjectAbbr = "CS",
				CourseNumber = 5955,
				Name = "Deep Learning"
			};

			Courses programmingLanguages = new Courses
			{
				CourseId = 24,
				SubjectAbbr = "CS",
				CourseNumber = 3520,
				Name = "Programming Languages"
			};

			Courses beginnerSpanish = new Courses
			{
				CourseId = 354,
				SubjectAbbr = "SPAN",
				CourseNumber = 1010,
				Name = "Beginner Spanish I"
			};

			Courses beginnerSpanish2 = new Courses
			{
				CourseId = 411,
				SubjectAbbr = "SPAN",
				CourseNumber = 1020,
				Name = "Beginner Spanish II"
			};

			Courses americanHistory = new Courses
			{
				CourseId = 533,
				SubjectAbbr = "HIST",
				CourseNumber = 1700,
				Name = "American History"
			};

			Courses econHistory = new Courses
			{
				CourseId = 600,
				SubjectAbbr = "HIST",
				CourseNumber = 1500,
				Name = "History of Economy"
			};

			db.Courses.Add(computerSystems);
			db.Courses.Add(deepLearning);
			db.Courses.Add(programmingLanguages);

			db.Courses.Add(beginnerSpanish);
			db.Courses.Add(beginnerSpanish2);

			db.Courses.Add(americanHistory);
			db.Courses.Add(econHistory);

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Verifies that a course has been successfully added
		/// </summary>
		[Fact]
		public void CanGetOneCourse()
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
		/// Verifies that an administrator gets all courses for one specific 
		/// department (i.e. Spanish (SPAN))
		/// </summary>
		[Fact]
		public void CanGetMultipleCoursesForSpanishDepartment()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = MakeCourses();
			admin.UseLMSContext(db);

			var courses = admin.GetCourses("SPAN") as JsonResult;
			dynamic results = courses.Value;

			var spanishCourses = from co in db.Courses
								 where co.SubjectAbbr == "SPAN"
								 select co;

			Assert.Equal(2, spanishCourses.Count());
			Assert.Equal("{ number = 1010, name = Beginner Spanish I }", results[0].ToString());
			Assert.Equal("{ number = 1020, name = Beginner Spanish II }", results[1].ToString());
		}

		/// <summary>
		/// Verifies that an administrator gets all courses for one specific 
		/// department (i.e. Computer Science (CS))
		/// </summary>
		[Fact]
		public void CanGetMultipleDepartmentsForCSDepartment()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = MakeCourses();
			admin.UseLMSContext(db);

			var courses = admin.GetCourses("CS") as JsonResult;
			dynamic results = courses.Value;

			var spanishCourses = from co in db.Courses
								 where co.SubjectAbbr == "CS"
								 select co;

			Assert.Equal(3, spanishCourses.Count());
			Assert.Equal("{ number = 4400, name = Computer Systems }", results[0].ToString());
			Assert.Equal("{ number = 5955, name = Deep Learning }", results[1].ToString());
			Assert.Equal("{ number = 3520, name = Programming Languages }", results[2].ToString());
		}

		/// <summary>
		/// Verifies that an administrator gets all courses for one specific 
		/// department (i.e. History (HIST))
		/// </summary>
		[Fact]
		public void CanGetMultipleDepartmentsForHistoryDepartment()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = MakeCourses();
			admin.UseLMSContext(db);

			var courses = admin.GetCourses("HIST") as JsonResult;
			dynamic results = courses.Value;

			var spanishCourses = from co in db.Courses
								 where co.SubjectAbbr == "HIST"
								 select co;

			Assert.Equal(2, spanishCourses.Count());
			Assert.Equal("{ number = 1700, name = American History }", results[0].ToString());
			Assert.Equal("{ number = 1500, name = History of Economy }", results[1].ToString());
		}

		/// <summary>
		/// Verifies that creating one class is successful
		/// </summary>
		[Fact]
		public void CanCreateOneCourse()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			var result = admin.CreateCourse("CHEM", 1210, "Organic Chemistry I") as JsonResult;

			Assert.Equal("{ success = True }", result.Value.ToString());
		}

		/// <summary>
		/// Verifies that an administrator is able to create multiple unique courses
		/// successfully
		/// </summary>
		[Fact]
		public void CanCreateMultipleUniqueCourses()
		{
			AdministratorController admin = new AdministratorController();
			Team55LMSContext db = ConfigureDatabaseNoData();
			admin.UseLMSContext(db);

			var databases = admin.CreateCourse("CS", 5530, "Database Systems") as JsonResult;
			var begRussian = admin.CreateCourse("RUSS", 1010, "Beginner Russian") as JsonResult;
			var interWriting = admin.CreateCourse("WRTG", 2010, "Intermediate Writing") as JsonResult;

			Assert.Equal("{ success = True }", databases.Value.ToString());
			Assert.Equal("{ success = True }", begRussian.Value.ToString());
			Assert.Equal("{ success = True }", interWriting.Value.ToString());
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

			var courses = from co in db.Courses
						  select co;

			Assert.Equal("{ success = False }", duplicateCourse.Value.ToString());
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

			var result = admin.CreateClass("LING", 1069, "Fall", 2020, start, end, "LNCO 1104", "Elena Khordova") as JsonResult;

			var query = from cl in db.Classes
						select cl;

			Assert.Equal(1, query.Count());
			Assert.Equal("{ success = True }", result.Value.ToString());
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
