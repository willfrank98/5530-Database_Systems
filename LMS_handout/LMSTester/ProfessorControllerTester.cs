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
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			return db;
		}

		/// <summary>
		/// Makes a database that has some students enrolled in a class 
		/// that a professor is teaching
		/// </summary>
		/// <returns></returns>
		private Team55LMSContext MakeClassesWithStudents()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

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

			db.Classes.Add(biology1210);

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Verifies that a professor is able to get all of the students
		/// in a class in which they are teaching
		/// </summary>
		[Fact]
		public void CanGetAllStudentsInClass()
		{
			ProfessorController prof = new ProfessorController();
			Team55LMSContext db = MakeClassesWithStudents();
			prof.UseLMSContext(db);

			var students = from stu in db.Students
						   join enr in db.Enrolled on stu.UId equals enr.UId
						   into stuJoinEnr
						   from stuEnr in stuJoinEnr.DefaultIfEmpty()
						   join cla in db.Classes on stuEnr.ClassId equals cla.ClassId
						   into stuEnrJoinCla
						   from clas in stuEnrJoinCla.DefaultIfEmpty()
						   select stu;

			Assert.Equal(3, students.Count());
		}
	}
}
