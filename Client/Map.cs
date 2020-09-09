using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Client {
	public class Map {
		
		public void DisplayMap(char[,] map) {

			// Set height and width offsets
			int height = 12;
			int width = 12;

			// Set cursor to middle of screen and display first line
			Console.SetCursorPosition((Console.WindowWidth / 2) - width, (Console.WindowWidth / 2) - height);
			Console.Write("            11111111112");

			// Decrement height offset
			height--;

			// Set cursor to middle of screen and display second line
			Console.SetCursorPosition((Console.WindowWidth / 2) - width, (Console.WindowWidth / 2) - height);
			Console.Write("  123456788901234567890");

			// Decrement height offset
			height--;
			
			for(int i = 0; i < 20; i++) {
				for (int j = 0; j < 20; j++) {
					if (j == 19) {
						Console.Write($"{map[i, j]}");
						height--;
					} else if (i < 9 && j == 0) {
						Console.SetCursorPosition((Console.WindowWidth / 2) - width, (Console.WindowWidth / 2) - height);
						Console.Write($" {i + 1}{map[i, j]}");
					} else if (i >= 9 && j == 0) {
						Console.SetCursorPosition((Console.WindowWidth / 2) - width, (Console.WindowWidth / 2) - height);
						Console.Write($"{i + 1}{map[i, j]}");
					} else {
						Console.Write($"{map[i, j]}");
					}
				}
			}
		}
	}
}