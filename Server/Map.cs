using System;
using System.Collections.Generic;
using System.Text;

namespace Server {
	class Map {
		public char[,] mapArray;

		public Map() {
			mapArray = new char[20, 20];
		}

		public Map(char[,] mapArray) {
			this.mapArray = mapArray;
		}

		public override string ToString() {
			StringBuilder map = new StringBuilder();

			map.Append("========================================\n");

			for (int i = 0; i < 20; i++) {
				for (int j = 0; j < 20; j++) {
					if (j == 19) {
						map.Append($" {mapArray[i, j]}\n");
					} else {
						map.Append($" {mapArray[i, j]}");
					}
				}
			}

			map.Append("========================================\n");

			return map.ToString();
		}
	}
}
