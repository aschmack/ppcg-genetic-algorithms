﻿/**
 * Notes to people using the controller:
 *  - see the Main method for an example of how to use the controller
 *  - see the examples methods which can be passed to GameLogic.Game class
 *     - these should behave just like those found in the python examples
 *  - your method must accept a GameLogic.IView, a GameLogic.IGenome, and pass out the x offset, and y offset of your chosen movement
 * 
 * If the controller breaks, shout at VisualMelon
 */

using System;
using System.Linq;

namespace ppcggacscontroller
{
	class Program
	{
		// convienience struct
		public struct Coord
		{
			public int x, y;
			
			public Coord(int xN, int yN)
			{
				x = xN;
				y = yN;
			}
		}
		
		public static void Main(string[] args)
		{
			// create a game instance, passing it your GameLogic.PlayerDel
			GameLogic.Game g = new GameLogic.Game(ColorScorePlayer);
			// add a display
			g.displayer = Display.createAndRun(g);
			// run it
			g.runSession();
			
			Console.ReadKey(true);
		}
		
		// fgrr likes to go forward
		public static Coord[] fgrrcoords = new [] {
			new Coord(1, 0),
			new Coord(1, -1),
			new Coord(1, 1),
			new Coord(0, -1),
			new Coord(0, 1),
//			new Coord(-1, -1),
//			new Coord(-1, 1),
//			new Coord(0, 0),
//			new Coord(-1, 0),
		};
		
		public static void fgrr(GameLogic.IView v, GameLogic.IGenome g, Random rnd, out int ox, out int oy)
		{
			// defaults
			ox = 1; // into the unknown
			oy = 0;
			
			int k = 2;
			
			for (int j = 0; j < 3; j++)
			{
				foreach (Coord c in fgrrcoords.Where(c => c.x > 0))
				{
					for (int i = j * k; i < j * k + k; i++)
					{
						uint ci = g.cutOutInt(i * 4, 4);
					
						if (v[c.x, c.y] == ci)
						{
							ox = c.x;
							oy = c.y;
							return;
						}
					}
				}
			}
			
			for (int j = 0; j < 3; j++)
			{
				foreach (Coord c in fgrrcoords.Where(c => c.x == 0))
				{
					for (int i = j * k; i < j * k + k; i++)
					{
						uint ci = g.cutOutInt(i * 4, 4);
					
						if (v[c.x, c.y] == ci)
						{
							ox = c.x;
							oy = c.y;
							return;
						}
					}
				}
			}
		}
		
		// Linear Combination Player is a port from the Python one here https://github.com/mbuettner/ppcg-genetic-algorithms/tree/master/python/player.py
		public static Coord[] lcpcoords = new [] {
//			new Coord(-1, -1),
//			new Coord(0, -1),
			new Coord(1, 0),
			new Coord(1, -1),
//			new Coord(-1, 0),
//			new Coord(0, 0),
//			new Coord(-1, 1),
//			new Coord(0, 1),
			new Coord(1, 1),
		};
		
		// ported from python, no idea what it does
		public static void LinearCombinationPlayer(GameLogic.IView v, GameLogic.IGenome g, Random rnd, out int ox, out int oy)
		{
			ox = 0;
			oy = 0;
			
			var restrictedCoords = lcpcoords.Where(c => v[c.x, c.y] > -1).ToArray();
			var rcCount = restrictedCoords.Length;
			
			int s = 0;
			for (int i = 0; i < 25; i++)
				s += (int)g.cutOutInt(2 * i, 2) * v[i / 5 - 2, i % 5 - 2];
			
			Coord res = restrictedCoords[(s + rcCount * 1000/*bignumber*/) % rcCount];
			
			ox = res.x;
			oy = res.y;	
		}
		
		// Color Score Player is a port from the Python one here https://github.com/mbuettner/ppcg-genetic-algorithms/tree/master/python/player.py
		public static Coord[] cspcoords = new [] {
//			new Coord(-1, -1),
//			new Coord(0, -1),
			new Coord(1, 0),
			new Coord(1, -1),
//			new Coord(-1, 0),
//			new Coord(0, 0),
//			new Coord(-1, 1),
//			new Coord(0, 1),
			new Coord(1, 1),
		};

		// this implementation isn't inefficient atall		
		public static void ColorScorePlayer(GameLogic.IView v, GameLogic.IGenome g, Random rnd, out int ox, out int oy)
		{
			ox = 0;
			oy = 0;
			
			var max_score = cspcoords.Where(c => v[c.x, c.y] > -1).Select(c => g.cutOutInt(6 * v[c.x, c.y], 6)).Max();
			var restrictedCoords = cspcoords.Where(c => v[c.x, c.y] > -1 && g.cutOutInt(6 * v[c.x, c.y], 6) == max_score).ToArray();
			
			Coord res = restrictedCoords[rnd.Next(restrictedCoords.Length)];
			
			ox = res.x;
			oy = res.y;	
		}
		
		public static Coord[] rpcoords = new [] {
//			new Coord(-1, -1),
//			new Coord(0, -1),
			new Coord(1, 0),
			new Coord(1, -1),
//			new Coord(-1, 0),
//			new Coord(0, 0),
//			new Coord(-1, 1),
//			new Coord(0, 1),
			new Coord(1, 1),
		};

		public static void RandomPlayer(GameLogic.IView v, GameLogic.IGenome g, Random rnd, out int ox, out int oy)
		{
			Coord res = rpcoords[rnd.Next(rpcoords.Length)];
			
			ox = res.x;
			oy = res.y;	
		}
		
		// Lemming Player is a port from the Python one here https://github.com/mbuettner/ppcg-genetic-algorithms/tree/master/python/player.py
		public static Coord[] lpcoords = new [] {
			new Coord(-1, -1),
//			new Coord(0, -1),
			new Coord(1, 0),
//			new Coord(1, -1),
			new Coord(-1, 0),
//			new Coord(0, 0),
			new Coord(-1, 1),
//			new Coord(0, 1),
//			new Coord(1, 1),
		};

		public static void LemmingPlayer(GameLogic.IView v, GameLogic.IGenome g, Random rnd, out int ox, out int oy)
		{
			Coord res = lpcoords[rnd.Next(lpcoords.Length)];
			
			ox = res.x;
			oy = res.y;	
		}
	}
}