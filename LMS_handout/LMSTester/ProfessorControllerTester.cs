﻿using Xunit;
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
		/// Helper for configuring a database with no assignment categories
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeAssignmentCategoryDatabase()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("no_assignment_categories").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 2,
				CourseNumber = 3100,
				Name = "Models of Computation",
				SubjectAbbr = "CS"
			};

			Classes modelsOfComp = new Classes
			{
				CourseId = 2,
				Location = "WEB L101",
				Semester = "Fall 2020",
				Professor = "u0000009",
				Start = TimeSpan.Parse("10:45:00"),
				End = TimeSpan.Parse("12:05:00"),
			};

			Enrolled enr = new Enrolled
			{
				ClassId = modelsOfComp.ClassId,
				Grade = "--",
				UId = "u0000001",
			};

			db.Courses.Add(course);
			db.Classes.Add(modelsOfComp);
			db.Enrolled.Add(enr);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for configuring a database with no assignment categories
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeDatabaseWithOneAssignmentCategory()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("one_assignment_category").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 5,
				CourseNumber = 3200,
				Name = "Scientific Computing",
				SubjectAbbr = "CS"
			};

			Classes scientificComputing = new Classes
			{
				CourseId = 2,
				Location = "WEB L103",
				Semester = "Spring 2021",
				Professor = "u0000009",
				Start = TimeSpan.Parse("8:05:00"),
				End = TimeSpan.Parse("09:10:00"),
			};

			Enrolled enr = new Enrolled
			{
				ClassId = scientificComputing.ClassId,
				Grade = "--",
				UId = "u0000001",
			};

			AssignmentCategories category = new AssignmentCategories
			{
				ClassId = scientificComputing.ClassId,
				Name = "Assignments",
				Weight = 60, 
			};

			db.Courses.Add(course);
			db.Classes.Add(scientificComputing);
			db.Enrolled.Add(enr);
			db.AssignmentCategories.Add(category);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for configuring a database with no assignment categories
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeDatabaseWithNoAssignments()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("no_assignments").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 5,
				CourseNumber = 2210,
				Name = "Calculus III",
				SubjectAbbr = "MATH"
			};

			Classes calculusIII = new Classes
			{
				CourseId = course.CourseId,
				Location = "TBA",
				Semester = "Summer 2020",
				Professor = "u0000010",
				Start = TimeSpan.Parse("12:25:00"),
				End = TimeSpan.Parse("13:10:00"),
			};

			Enrolled enr = new Enrolled
			{
				ClassId = calculusIII.ClassId,
				Grade = "--",
				UId = "u0000002",
			};

			AssignmentCategories category = new AssignmentCategories
			{
				ClassId = calculusIII.ClassId,
				Name = "Assignments",
				Weight = 50,
			};

			db.Courses.Add(course);
			db.Classes.Add(calculusIII);
			db.Enrolled.Add(enr);
			db.AssignmentCategories.Add(category);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for configuring a database with no assignment categories
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeDatabaseWithOneAssignment()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("one_assignment").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 5,
				CourseNumber = 2210,
				Name = "Calculus III",
				SubjectAbbr = "MATH"
			};

			Classes calculusIII = new Classes
			{
				CourseId = 2,
				Location = "TBA",
				Semester = "Summer 2020",
				Professor = "u0000010",
				Start = TimeSpan.Parse("12:25:00"),
				End = TimeSpan.Parse("13:10:00"),
			};

			Enrolled enr = new Enrolled
			{
				ClassId = calculusIII.ClassId,
				Grade = "--",
				UId = "u0000002",
			};

			AssignmentCategories category = new AssignmentCategories
			{
				ClassId = calculusIII.ClassId,
				Name = "Assignments",
				Weight = 50,
			};

			Assignments assignment = new Assignments
			{
				AssignCatId = category.AssignCatId,
				Contents = "Just compute the indefinite integral on problem 2, page 303 :)",
				MaxPoints = 10,
				Name = "One Problem",
				DueDate = DateTime.Parse("12/01/2020 11:59:59"), 
			};

			db.Courses.Add(course);
			db.Classes.Add(calculusIII);
			db.Enrolled.Add(enr);
			db.AssignmentCategories.Add(category);
			db.Assignments.Add(assignment);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Helper for making a 
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeDatabaseWithNoValidAssignmentCategories()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("no_correct_assignment_categories").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			Courses course = new Courses
			{
				CourseId = 5,
				CourseNumber = 2210,
				Name = "Calculus III",
				SubjectAbbr = "MATH"
			};

			Classes calculusIII = new Classes
			{
				CourseId = 2,
				Location = "TBA",
				Semester = "Summer 2020",
				Professor = "u0000010",
				Start = TimeSpan.Parse("12:25:00"),
				End = TimeSpan.Parse("13:10:00"),
			};

			Enrolled enr = new Enrolled
			{
				ClassId = calculusIII.ClassId,
				Grade = "--",
				UId = "u0000002",
			};

			db.Courses.Add(course);
			db.Classes.Add(calculusIII);
			db.Enrolled.Add(enr);
			db.SaveChanges();

			return db;
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

		/// <summary>
		/// Verifies that a professor can create an assignment category for a class
		/// </summary>
		[Fact]
		public void CanMakeAssignmentCategory()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeAssignmentCategoryDatabase();
			prof.UseLMSContext(db);

			var create = prof.CreateAssignmentCategory("CS", 3100, "Fall", 2020, "Dreaded Quizzes", 30) as JsonResult;
			dynamic result = create.Value;

			Assert.Equal("{ success = True }", result.ToString());
		}

		/// <summary>
		/// Verifies that a professor cannot create a duplicate assignment category 
		/// for the same class
		/// </summary>
		[Fact]
		public void CannotMakeDuplicateAssignmentCategory()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeDatabaseWithOneAssignmentCategory();
			prof.UseLMSContext(db);

			var create = prof.CreateAssignmentCategory("CS", 3200, "Spring", 2021, "Assignments", 60) as JsonResult;
			dynamic result = create.Value;

			Assert.Equal("{ success = False }", result.ToString());
		}

		/// <summary>
		/// Verifies that a professor can create an assignment for one assignment category
		/// </summary>
		[Fact]
		public void CanCreateAssignment()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeDatabaseWithNoAssignments();
			prof.UseLMSContext(db);

			string aName = "Homework 1";
			string category = "Assignments";
			string contents = "Compute gradients for problems 1-20";
			DateTime due = DateTime.Parse("04/20/2020 11:59:59");

			var createAssignment = prof.CreateAssignment("MATH", 2210, "Summer", 2020, category, aName, 100, due, contents) as JsonResult;
			dynamic result = createAssignment.Value;

			Assert.Equal("{ success = True }", result.ToString());
		}

		/// <summary>
		/// Verifies that a duplicate assignment category cannot be created
		/// </summary>
		[Fact]
		public void CannotCreateDuplicateAssignment()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeDatabaseWithOneAssignment();
			prof.UseLMSContext(db);

			string category = "Assignments";
			string name = "One Problem";
			DateTime due = DateTime.Parse("12/01/2020 11:59:59");
			string contents = "Just compute the indefinite integral on problem 2, page 303 :)";

			var duplicate = prof.CreateAssignment("MATH", 2210, "Summer", 2020, category, name, 10, due, contents) as JsonResult;
			dynamic result = duplicate.Value;

			Assert.Equal("{ success = False }", result.ToString());

		}

		/// <summary>
		/// Verifes that a professor cannot create an assignment category without any 
		/// name and weight
		/// </summary>
		[Fact]
		public void CannotCreateAssignmentCategoryWithEmptyNameAndWeight()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeDatabaseWithNoValidAssignmentCategories();
			prof.UseLMSContext(db);

			var empty = prof.CreateAssignmentCategory("MATH", 2210, "Summer", 2020, null, new int()) as JsonResult;
			dynamic result = empty.Value;

			Assert.Equal("{ success = False }", result.ToString());
		}
	}
}
