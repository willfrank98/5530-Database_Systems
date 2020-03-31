using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

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
		private Team55LMSContext ConfigureDatabaseNoData()
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
		/// Verifies that a student is able to enroll in a class successfully
		/// </summary>
		[Fact]
		public void CanEnrollInClass()
		{
			StudentController student = new StudentController();
			Team55LMSContext db = MakeTinyCatalog();
			student.UseLMSContext(db);

			student.Enroll("LING", 1069, "Fall", 2020, "u0000003");

			var query = from en in db.Enrolled
						join cl in db.Classes on en.ClassId equals cl.ClassId
						into classes 
						from c in classes.DefaultIfEmpty()
						where c.CourseId == 1069 && c.Professor == "u0000003"
						&& c.Semester == "Fall 2020"
						select en;

			Assert.Equal(1, query.Count());
		}
	}
}
