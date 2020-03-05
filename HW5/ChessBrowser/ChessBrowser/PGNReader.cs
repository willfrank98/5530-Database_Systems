using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBrowser
{
	public class PGNReader
	{
		List<Player> players = new List<Player>();
		List<Event> events= new List<Event>();
		List<Game> games = new List<Game>();

		public void ReadPGN(string filepath)
		{
			string[] lines = System.IO.File.ReadAllLines(filepath);

			for (int i = 0; i < lines.Length; i++)
			{
				var currEvent = new Event();
				var currGame = new Game();
				var playerOne = new Player();
				var playerTwo = new Player();
				// read in event tags
				while (!lines[i].Equals("\n"))
				{
					// remove brackets
					var line = lines[i].Substring(1, lines[i].Length - 2);
					
					if (line.StartsWith("Event")) 
					{
						currEvent.Name = line.Substring(7, line.Length - 2);
					}
					else if (line.StartsWith("Site"))
					{
						currEvent.Site = line.Substring(7, line.Length - 2);
					}
					else if (line.StartsWith("EventDate"))
					{
						currEvent.Date = line.Substring(7, line.Length - 2);
					}
					else if (line.StartsWith("Round"))
					{

					}
					else if (line.StartsWith("White"))
					{

					}
					else if (line.StartsWith("Black"))
					{

					}
					else if (line.StartsWith("Result"))
					{

					}
					else if (line.StartsWith("WhiteElo"))
					{

					}
					else if (line.StartsWith("BlackElo"))
					{

					}
					//else if (line.StartsWith("ECO"))
					//{

					//}
					else if (line.StartsWith("Date"))
					{

					}
				}
			}
		}
	}

	class Player 
	{
		public string Name;
		public int Elo;
	}

	class Event 
	{
		public string Name;
		public string Site;
		public string Date;
	}
	class Game 
	{
		public string Round;
		public char Result;
		public string BlackPlayer;
		public string WhitePlayer;
		public string Moves;
	}

}
