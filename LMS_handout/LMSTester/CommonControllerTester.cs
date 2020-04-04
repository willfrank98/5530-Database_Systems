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
			Departments lingustics = new Departments { Name = "Lingustics", SubjectAbbr = "LING" };
			Departments cs = new Departments { Name = "Computer Science", SubjectAbbr = "CS" };

			db.Departments.Add(psychology);
			db.Departments.Add(civilEng);
			db.Departments.Add(lingustics);
			db.Departments.Add(cs);

			db.SaveChanges();

			return db;
		}

		
	}
}
