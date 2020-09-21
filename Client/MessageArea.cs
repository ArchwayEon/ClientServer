using System;
using System.Text;

namespace Client
{
    public class MessageArea
    {
        private ConsoleWrapper _console;

        public MessageArea(ConsoleWrapper console)
        {
            this._console = console;
        }

        public int Line { get; set; }
        public char BorderCharacter { get; set; } = '*';
        public string Prompt { get; set; } = "MESSAGE";
        public void Show(string message)
        {
            var border = new StringBuilder();
            int y = Line;
            for (var x = 0; x < Console.WindowWidth; x++)
            {
                border.Append(BorderCharacter);
            }
            _console.Output(0, y, border.ToString());
            y++;
            _console.Output(1, y, $"{Prompt}: {message}");
            y++;
            _console.Output(0, y, border.ToString());
        }
    }
}
