using System;
using System.Collections.Generic;

namespace Client
{
    public class MapArea
    {
        private readonly ConsoleWrapper _console;

        public List<string> Map { get; set; } = new List<string>();

        public MapArea()
        {
            for (var count = 0; count < 20; count++)
            {
                Map.Add("                    ");
            }
        }

        public MapArea(ConsoleWrapper console) : this()
        {
            this._console = console;
        }

        public void Show()
        {
            int halfWidth = Console.WindowWidth / 2;
            int halfHeight = Console.WindowHeight / 2;
            int startX = halfWidth - 11;
            int startY = halfHeight - 11;
            string line1 = "           11111111112";
            string line2 = "  12345678901234567890";
            int x = startX;
            int y = startY;
            _console.Output(x, y, line1);
            y++;
            _console.Output(x, y, line2);
            int count = 1;
            foreach (var line in Map)
            {
                y++;
                _console.Output(x, y, $"{count,2}{line}");
                count++;
            }
        }
    }
}
