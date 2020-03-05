using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessBrowser
{
	public class PGNReader
	{
		List<Player> players = new List<Player>();
		List<Event> events= new List<Event>();
		List<Game> games = new List<Game>();

		/// <summary>
		/// Reads in a PGN file to local storage variables
		/// </summary>
		/// <param name="filepath">The path to the PGN file</param>
		public void ReadPGN(string filepath)
		{
			// regex to get text between quotes
			var rgx = new Regex("\"([^\"]*)\"");

			// read in all lines of text
			string[] lines = System.IO.File.ReadAllLines(filepath);

			for (int i = 0; i < lines.Length; i++)
			{
				var currEvent = new Event();
				var currGame = new Game();
				var whitePlayer = new Player();
				var blackPlayer = new Player();
				// read in event tags
				while (!lines[i].Equals(""))
				{
					// remove brackets
					var line = lines[i].Substring(1, lines[i].Length - 2);

					if (line.StartsWith("EventDate"))
					{
						currEvent.Date = rgx.Match(line).Groups[1].Value;
					}
					else if (line.StartsWith("Event"))
					{
						currEvent.Name = rgx.Match(line).Groups[1].Value;
					}
					else if (line.StartsWith("Site"))
					{
						currEvent.Site = rgx.Match(line).Groups[1].Value;
					}
					
					else if (line.StartsWith("Round"))
					{
						currGame.Round = rgx.Match(line).Groups[1].Value;
					}
					else if (line.StartsWith("WhiteElo"))
					{
						whitePlayer.Elo = int.Parse(rgx.Match(line).Groups[1].Value);
					}
					else if (line.StartsWith("BlackElo"))
					{
						blackPlayer.Elo = int.Parse(rgx.Match(line).Groups[1].Value);
					}
					else if (line.StartsWith("White"))
					{
						currGame.WhitePlayer = rgx.Match(line).Groups[1].Value;
						whitePlayer.Name = rgx.Match(line).Groups[1].Value;
					}
					else if (line.StartsWith("Black"))
					{
						currGame.BlackPlayer = rgx.Match(line).Groups[1].Value;
						blackPlayer.Name = rgx.Match(line).Groups[1].Value;
					}
					else if (line.StartsWith("Result"))
					{
						var tempRes = rgx.Match(line).Groups[1].Value;
						char result = ' ';
						switch (tempRes)
						{
							case "1/2-1/2":
								result = 'D';
								break;
							case "0-1":
								result = 'B';
								break;
							case "1-0":
								result = 'W';
								break;
						}

						currGame.Result = result;
					}
					i++;
				}
				i++;

				// get moves
				string moves = "";
				while (!lines[i].Equals(""))
				{
					moves += lines[i];
					i++;
				}

				currGame.Moves = moves;

				events.Add(currEvent);
				games.Add(currGame);
				players.Add(whitePlayer);
				players.Add(blackPlayer);
				// check to see if whitePlayer already exist in current list and updates ELO if necessary
				//var tempPlayer = players.First(p => p.Name.Equals(whitePlayer.Name));
				//if (tempPlayer != null && tempPlayer.Elo < whitePlayer.Elo)
				//{
				//	players.Where(p => p.Name.Equals(whitePlayer.Name)).First().Elo = whitePlayer.Elo;
				//}
				//else
				//{
				//	players.Add(blackPlayer);
				//}

				//// does the same for blackPlayer
				//tempPlayer = players.First(p => p.Name.Equals(blackPlayer.Name));
				//if (tempPlayer != null && tempPlayer.Elo < blackPlayer.Elo)
				//{
				//	players.Where(p => p.Name.Equals(blackPlayer.Name)).First().Elo = blackPlayer.Elo;
				//}
				//else
				//{
				//	players.Add(blackPlayer);
				//}
			}
		}

		/// <summary>
		/// Returns the current number of work items (items to be added to the database)
		/// </summary>
		/// <returns></returns>
		public int GetNumWorkItems()
		{
			return games.Count + events.Count + players.Count;
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
