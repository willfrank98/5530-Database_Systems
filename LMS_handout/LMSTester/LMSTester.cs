using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LMSTester
{
	public class LMSTester
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
		private Team55LMSContext MakeTinyCatalog()
		{
			var optionsBuilder = new DbContextOptionsBuilder<Team55LMSContext>();
			optionsBuilder.UseInMemoryDatabase("tiny_catalog").UseApplicationServiceProvider(NewServiceProvider());

			Team55LMSContext db = new Team55LMSContext(optionsBuilder.Options);

			/*INSERT DATA HERE*/
			Courses course = new Courses {Name = "Intro to OOP", SubjectAbbr = "CS", CourseNumber = 1410};

			db.Courses.Add(course);
			db.SaveChanges();

			/*END DATA HERE*/

			return db;
		}

		/// <summary>
		/// Verifies that a course has been successfully added
		/// </summary>
		[Fact]
		public void TestGetCourse()
		{
			HomeController home = new HomeController();
			CommonController controller = new CommonController();
			
			Team55LMSContext db = MakeTinyCatalog();
			controller.UseLMSContext(db);

			var query = from c in db.Courses
						select c;

			Assert.Equal(1, query.Count());
		}

		
	}
}
