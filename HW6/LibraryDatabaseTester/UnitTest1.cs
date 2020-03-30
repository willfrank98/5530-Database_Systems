using System;
using Xunit;
using LibraryWebServer.Controllers;
using Microsoft.AspNetCore.Mvc;
using LibraryWebServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace LibraryDatabaseTester
{
	public class UnitTest1
	{
		/// <summary>
		/// For SampleTest1
		/// </summary>
		/// <returns></returns>
		private Team55LibraryContext MakeTinyLibrary()
		{
			/*MOCK DATABASE SETUP*/
			var optionsBuilder = new DbContextOptionsBuilder<Team55LibraryContext>();
			Team55LibraryContext db = new Team55LibraryContext(optionsBuilder.UseInMemoryDatabase("tiny_library").Options);
			/*END MOCK DATABASE SETUP*/

			/*Add in data*/
			Titles t = new Titles();
			t.Author = "Tony Diep";
			t.Title = "Tony's Life";
			t.Isbn = "978-1111111111";

			db.Titles.Add(t);
			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// For SampleTest2
		/// </summary>
		/// <returns></returns>
		private Team55LibraryContext MakeMediumLibrary()
		{
			/*MOCK DATABASE SETUP*/
			var optionsBuilder = new DbContextOptionsBuilder<Team55LibraryContext>();
			Team55LibraryContext db = new Team55LibraryContext(optionsBuilder.UseInMemoryDatabase("medium_library").Options);
			/*END MOCK DATABASE SETUP*/

			/*Add in data*/
			Titles profilesInCourage = new Titles
			{ Isbn = "978-0062278791", Title = "Profiles in Courage", Author = "Kennedy" };

			Titles theLorax = new Titles
			{ Isbn = "978-0394823379", Title = "The Lorax", Author = "Seuss"};

			Titles dune = new Titles
			{ Isbn = "978-0441172719", Title = "Dune", Author = "Herbert"};

			Patrons dan = new Patrons { Name = "Dan", CardNum = 4 };

			Inventory inv = new Inventory { Serial = 1001, Isbn = "978-0441172719"};
			Inventory inv2 = new Inventory { Serial = 1002, Isbn = "978-0441172719"};
			Inventory invPIC = new Inventory { Serial = 1006, Isbn = profilesInCourage.Isbn};

			db.Titles.Add(dune);
			db.Patrons.Add(dan);
			db.Inventory.Add(inv);
			db.Inventory.Add(inv2);
			db.Inventory.Add(invPIC);

			db.SaveChanges();

			return db;
		}

		/// <summary>
		/// Taken from Lecture 16 video
		/// </summary>
		[Fact]
		public void CanAddTitleToTitlesDatabase()
		{
			HomeController c = new HomeController();
			c.UseTeam55LibraryContext(MakeTinyLibrary());

			var allTitles = c.AllTitles() as JsonResult;
			dynamic value = allTitles.Value;

			Assert.Equal(1, value.Length);
		}

		/// <summary>
		/// Taken from Lecture 16 video
		/// </summary>
		[Fact]
		public void CanCheckoutBookSuccessfully()
		{
			HomeController c = new HomeController();

			Team55LibraryContext db = MakeMediumLibrary();

			c.UseTeam55LibraryContext(db);

			c.CheckLogin("Dan", 4);

			c.CheckOutBook(1001);
		
			//Verify if the CheckedOut table has one new row
			var query = from co in db.CheckedOut
						select co;

			Assert.Equal(1, query.Count());
			Assert.Equal((uint)4, query.ToArray()[0].CardNum);
		}

		/*END SAMPLE TESTS*/


		/*---------------------------------- BEGIN TESTS HERE ----------------------------------*/

		///// <summary>
		///// Verifies that bad credentials won't let the user log in
		///// </summary>
		//[Fact]
		//public void VerifyBadLoginCredentials()
		//{
		//	/*MOCK DATABASE SETUP*/
		//	var optionsBuilder = new DbContextOptionsBuilder<Team55LibraryContext>();
		//	Team55LibraryContext db = new Team55LibraryContext(optionsBuilder.UseInMemoryDatabase("tiny_library").Options);
		//	/*END MOCK DATABASE SETUP*/

		//	Patrons p = new Patrons { Name = "Post Malone", CardNum = 100 };

		//	HomeController c = new HomeController();
		//	c.UseTeam55LibraryContext(db);

		//	var result = c.CheckLogin(p.Name, (int) p.CardNum) as JsonResult;
		//	var expected = new {success = false};

		//	Assert.Equal(expected, result.Value);
		//} 

		///// <summary>
		///// Verifies that a checked out book was successfully returned
		///// </summary>
		//[Fact]
		//public void TestReturnBook()
		//{
		//	HomeController c = new HomeController();
		//	Team55LibraryContext db = MakeTinyLibrary();
		//	c.UseTeam55LibraryContext(db);

		//	c.CheckLogin("Joe", 1);
		//	c.CheckOutBook(1006);
		//	c.ReturnBook(1006);

		//	var query = from co in db.CheckedOut
		//				select co;

		//	Assert.Equal(0, query.Count());
		//}

		/*---------------------------------- END TESTS HERE ----------------------------------*/
	}
}
