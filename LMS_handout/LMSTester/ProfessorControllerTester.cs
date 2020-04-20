using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
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
	public class ProfessorControllerTester
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
			optionsBuilder.UseInMemoryDatabase("empty_prof_database").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			return db;
		}

		/// <summary>
		/// Makes a database that has some students enrolled in a class 
		/// that a professor is teaching; the student is in the same major
		/// as the class's subject abbreviation
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeClassesWithOneStudentSameMajor()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("class_with_one_student_same_major").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 0,
				CourseNumber =  1210,
				SubjectAbbr = "CHEM", 
				Name = "Organic Chemistry"
			};

			Classes chemistry1210 = new Classes
			{
				ClassId = 0,
				CourseId = 0,
				Semester = "Summer 2020",
				Location = "CHEM Building 101",
				Start = TimeSpan.Parse("8:25:00"),
				End = TimeSpan.Parse("9:30:00"),
				Professor = "u0000000"
			};

			Students one = new Students
			{
				UId = "u0000001",
				FirstName = "Tony",
				LastName = "Diep",
				BirthDate = DateTime.Parse("02/02/1996"),
				Major = "CHEM"
			};

			db.Students.Add(one);
			db.Courses.Add(course);
			db.Classes.Add(chemistry1210);

			StudentController studController = new StudentController();
			studController.Enroll("CHEM", 1210, "Summer", 2020, "u0000001");

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Makes a database that has some students enrolled in a class 
		/// that a professor is teaching; the student is in a different major
		/// than the class's subject abbreviation
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeClassesWithOneStudentDifferentMajor()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("class_with_one_student_diff_major").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 0,
				CourseNumber = 1210,
				Name = "Organic Chemistry",
				SubjectAbbr = "CHEM",
			};

			Classes chemistry1210 = new Classes
			{
				ClassId = 0,
				CourseId = 0,
				Semester = "Summer 2020",
				Location = "CHEM Building 101",
				Start = TimeSpan.Parse("8:25:00"),
				End = TimeSpan.Parse("9:30:00"),
				Professor = "u0000000"
			};

			Students one = new Students
			{
				UId = "u0000001",
				FirstName = "Tony",
				LastName = "Diep",
				BirthDate = DateTime.Parse("02/02/1996"),
				Major = "CS"
			};

			db.Students.Add(one);
			db.Courses.Add(course);
			db.Classes.Add(chemistry1210);

			StudentController studentController = new StudentController();
			studentController.Enroll("CHEM", 1210, "Summer", 2020, "u0000001");

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Makes a database that has some students enrolled in a class 
		/// that a professor is teaching
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeClassesWithAFewStudents()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("classes_with_few_students").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses biology = new Courses
			{
				CourseId = 0,
				CourseNumber = 1210,
				Name = "Intro to Biology",
				SubjectAbbr = "BIOL"
			};

			Classes biology1210 = new Classes
			{
				ClassId = 0,
				CourseId = 0, 
				Semester = "Fall 2020",
				Location = "WEB L102",
				Start = TimeSpan.Parse("9:10:00"),
				End = TimeSpan.Parse("10:30:00"),
				Professor = "u0000000"
			};

			Students one = new Students
			{
				UId = "u0000001",
				FirstName = "Tony",
				LastName = "Diep",
				BirthDate = DateTime.Parse("02/02/1996"),
				Major = "BIOL"
			};

			Students two = new Students
			{
				UId = "u0000002",
				FirstName = "Will",
				LastName = "Frank",
				BirthDate = DateTime.Parse("01/01/1998"),
				Major = "BIOL"
			};

			Students three = new Students
			{
				UId = "u0000003",
				FirstName = "Jon",
				LastName = "Pilling",
				BirthDate = DateTime.Parse("03/01/1995"),
				Major = "BIOL"
			};

			db.Students.Add(one);
			db.Students.Add(two);
			db.Students.Add(three);

			db.Courses.Add(biology);
			db.Classes.Add(biology1210);

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for making one class for one professor
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeClassForProfessor()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("class_for_one_professor").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses cr_1010 = new Courses
			{
				CourseId = 10, 
				CourseNumber = 1010, 
				Name = "Beginner German I",
				SubjectAbbr = "GERM"
			};

			Classes germ_1010 = new Classes
			{
				ClassId = 11, 
				CourseId = 10, 
				Location = "BUC 204",
				Start = TimeSpan.Parse("16:35:00"),
				End = TimeSpan.Parse("18:50:00"),
				Semester = "Fall 2018",
				Professor = "u0999990",
				Course = cr_1010
			};

			db.Courses.Add(cr_1010);
			db.Classes.Add(germ_1010);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for making some classes for one professor
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeClassesForProfessor()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("classes_for_one_professor").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses cr_1030 = new Courses
			{
				CourseId = 6, 
				CourseNumber = 1030,
				Name = "Intro to Computer Science", 
				SubjectAbbr = "CS"
			};

			Classes cs1030 = new Classes
			{
				ClassId = 3,
				CourseId = cr_1030.CourseId,
				Location = "WEB L104",
				Semester = "Fall 2018",
				Start = TimeSpan.Parse("10:45:00"),
				End = TimeSpan.Parse("11:35:00"),
				Professor = "u1111110",
				Course = cr_1030
			};

			Courses cr_2420 = new Courses
			{
				CourseId = 7,
				CourseNumber = 2420,
				Name = "Data Structures & Algorithms",
				SubjectAbbr = "CS"
			};

			Classes cs2420 = new Classes
			{
				ClassId = 4, 
				CourseId = cr_2420.CourseId,
				Location = "WEB L103",
				Semester = "Fall 2017",
				Start = TimeSpan.Parse("14:00:00"),
				End = TimeSpan.Parse("15:20:00"),
				Professor = "u1111110",
				Course = cr_2420
			};

			db.Classes.Add(cs1030);
			db.Classes.Add(cs2420);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Verifies that a professor is able to get all of the students
		/// in a class in which they are teaching
		/// </summary>
		[Fact]
		public void CanGetAllStudentsInClassOneStudentSameMajorAsClassSubject()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeClassesWithOneStudentSameMajor();
			prof.UseLMSContext(db);

			var allStudents = prof.GetStudentsInClass("CHEM", 1210, "Summer", 2020) as JsonResult;
			dynamic result = allStudents.Value;

			Assert.Equal("{ fname = Tony, lname = Diep, dob = 02/02/1996, grade = -- }", result[0].ToString());
		}

		/// <summary>
		/// Verifies that a professor is able to get all of the students
		/// in a class in which they are teaching
		/// </summary>
		[Fact]
		public void CanGetAllStudentsInClassOneStudentDifferentMajorThanClassSubject()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeClassesWithOneStudentDifferentMajor();
			prof.UseLMSContext(db);

			var allStudents = prof.GetStudentsInClass("CHEM", 1210, "Summer", 2020) as JsonResult;
			dynamic result = allStudents.Value;

			Assert.Equal("{ fname = Tony, lname = Diep, dob = 02/02/1996, grade = -- }", result.ToString());
		}

		/// <summary>
		/// Verifies that a professor can get a list of all of the 
		/// students that are in their class
		/// </summary>
		[Fact]
		public void CanGetAllStudentsInClass()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeClassesWithAFewStudents();
			prof.UseLMSContext(db);

			var allStudents = prof.GetStudentsInClass("BIOL", 1210, "Fall", 2020) as JsonResult;
			dynamic result = allStudents.Value;

			String firstStudent = "{ fname = Tony, lname = Diep, dob = 02/02/1996, grade = -- }";
			Assert.Equal(firstStudent, result[0].ToString());

			//Assert.Equal("{ { fname = Tony, lname = Diep, dob = 02/02/1996, grade = -- }, " +
			//	"{ fname = Will, lname = Frank, dob = 01/01/1998, grade = -- }, " +
			//  " { fname = Jon, lname = Pilling, dob = 03/01/1995, grade = -- } }", result.ToString());
		}

		/// <summary>
		/// Verifies that a professor can get the only class he is/has taught
		/// </summary>
		[Fact]
		public void CanGetMyClassOne()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeClassForProfessor();
			prof.UseLMSContext(db);

			var oneClass = prof.GetMyClasses("u0999990") as JsonResult;
			dynamic result = oneClass.Value;

			Assert.Equal("{ subject = GERM, number = 1010, name = Beginner German I, season = Fall, year = 2018 }", result[0].ToString());
		}

		/// <summary>
		/// Verifies that a professor can get a list of all of the 
		/// classes they are teaching 
		/// </summary>
		[Fact]
		public void CanGetMyClasses()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeClassesForProfessor();
			prof.UseLMSContext(db);

			var profClasses = prof.GetMyClasses("u1111110") as JsonResult;
			dynamic result = profClasses.Value;

			String firstClass = "{ subject = CS, number = 1030, name = Intro to Computer Science, season = Fall, year = 2018 }";
			String secondClass = "{ subject = CS, number = 2420, name = Data Structures & Algorithms, season = Fall, year = 2017 }";

			Assert.Equal(firstClass, result[0].ToString());
			Assert.Equal(secondClass, result[1].ToString());
		}
	}
}
